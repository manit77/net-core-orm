using CoreORM;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public static class OpenApiParser
{
    public static DBDatabase MapToDBDatabase(string openApiJson, string codeNameSpace)
    {
        var root = JObject.Parse(openApiJson);
        var database = new DBDatabase
        {
            Name = root["info"]?["title"]?.ToString() ?? "OpenApiImport",
            CodeNameSpace = codeNameSpace
        };

        // 1. Map Schemas (Tables/DTOs)
        var schemas = root["components"]?["schemas"] as JObject;
        if (schemas != null)
        {
            foreach (var schema in schemas)
            {
                var table = new DBTable
                {
                    Name = schema.Key,
                    MappedName = CoreUtils.Data.ToPascalCase(schema.Key),
                    Database = database
                };

                var properties = schema.Value["properties"] as JObject;
                if (properties != null)
                {
                    foreach (var prop in properties)
                    {
                        var propSchema = prop.Value;
                        // Handle 3.1.1 Type Arrays and oneOf arrays
                        string rawType = GetTypeFromSchema(propSchema);
                        bool isNullable = IsNullable(propSchema);

                        table.Columns.Add(new DBColumn
                        {
                            Name = prop.Key,
                            MappedName = CoreUtils.Data.ToPascalCase(prop.Key),
                            IsNullable = isNullable,
                            MappedDataType = MapToDbTypeMap(rawType, propSchema["format"]?.ToString())
                        });
                    }
                }
                database.Tables.Add(table);
            }
        }

        // 2. Map Paths (Procedures/API Endpoints)
        var paths = root["paths"] as JObject;
        if (paths != null)
        {
            foreach (var path in paths)
            {
                foreach (var methodProperty in (path.Value as JObject))
                {
                    string verb = methodProperty.Key.ToUpper();
                    var op = methodProperty.Value;

                    var proc = new DBProcedure
                    {
                        Name = op["operationId"]?.ToString() ?? $"{verb}_{path.Key.Replace("/", "_")}",
                        Schema = op["tags"]?.FirstOrDefault()?.ToString() ?? "Default",
                        Type = verb
                    };

                    // 1. Extract Parameters (Query, Path, Header)
                    var parameters = op["parameters"] as JArray;
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            var pSchema = p["schema"];
                            proc.Paramaters.Add(new DBParamater
                            {
                                Name = p["name"]?.ToString(),
                                MappedDataType = MapToDbTypeMap(GetTypeFromSchema(pSchema), pSchema?["format"]?.ToString()),
                                IsNullable = IsNullable(pSchema)
                            });
                        }
                    }

                    // 2. Extract Request Body (Typical for POST/PUT)
                    var requestBodySchema = op["requestBody"]?["content"]?["application/json"]?["schema"];
                    if (requestBodySchema != null)
                    {
                        string refModel = GetTypeFromSchema(requestBodySchema);

                        if (!string.IsNullOrEmpty(refModel) && refModel != "any")
                        {
                            proc.Paramaters.Add(new DBParamater
                            {
                                Name = "input", 
                                MappedDataType = new DBTypeMap { CodeType = refModel, DatabaseType = "record" }
                            });
                        }
                        else
                        {
                            proc.Paramaters.Add(new DBParamater
                            {
                                Name = "payload",
                                MappedDataType = new DBTypeMap { CodeType = "object", DatabaseType = "jsonb" }
                            });
                        }
                    }

                    // 3. Extract Response
                    var responseSchema = op["responses"]?["200"]?["content"]?["application/json"]?["schema"];
                    if (responseSchema != null)
                    {
                        // Check if response is an array of items
                        if (responseSchema["type"]?.ToString() == "array" && responseSchema["items"] != null)
                        {
                            string itemType = GetTypeFromSchema(responseSchema["items"]);
                            proc.MappedDataType = new DBTypeMap
                            {
                                CodeType = itemType,
                                DatabaseType = "record"
                            };
                            proc.IsScalar = false;
                        }
                        else
                        {
                            string type = GetTypeFromSchema(responseSchema);
                            proc.MappedDataType = MapToDbTypeMap(type, responseSchema["format"]?.ToString());
                            proc.IsScalar = true; // Though if it's a wrapper like GenericResult, you might handle it differently in views
                        }
                    }

                    database.Procedures.Add(proc);
                }
            }
        }

        return database;
    }

    private static string GetTypeFromSchema(JToken schema)
    {
        if (schema == null) return "any";

        // 1. Check for direct Reference
        var refToken = schema["$ref"];
        if (refToken != null)
        {
            return refToken.ToString().Split('/').Last();
        }

        // 2. Check for oneOf/anyOf (OpenAPI 3.1 Nullable Wrappers)
        var oneOfToken = schema["oneOf"] ?? schema["anyOf"];
        if (oneOfToken is JArray oneOfArray)
        {
            foreach (var item in oneOfArray)
            {
                var innerRef = item["$ref"];
                if (innerRef != null)
                {
                    return innerRef.ToString().Split('/').Last();
                }

                var typeVal = item["type"]?.ToString();
                if (!string.IsNullOrEmpty(typeVal) && typeVal != "null")
                {
                    // If it's an array inside a oneOf, get the item type
                    if (typeVal == "array" && item["items"] != null)
                    {
                         string arrayItemType = GetTypeFromSchema(item["items"]);
                         return $"Array<{arrayItemType}>";
                    }
                    return typeVal;
                }
            }
        }

        // 3. Check for Array Items Reference
        if (schema["type"]?.ToString() == "array" && schema["items"] != null)
        {
             // We can just call GetTypeFromSchema recursively to handle $ref or primitives inside the items
             string arrayItemType = GetTypeFromSchema(schema["items"]);
             return $"Array<{arrayItemType}>";
        }

        // 4. Handle primitive arrays ["string", "null"]
        var typeToken = schema["type"];
        if (typeToken == null) return "any";

        if (typeToken.Type == JTokenType.Array)
        {
            return typeToken.FirstOrDefault(t => t.ToString() != "null")?.ToString() ?? "any";
        }

        return typeToken.ToString();
    }

    private static bool IsNullable(JToken schema)
    {
        if (schema == null) return false;
        if (schema["nullable"]?.Value<bool>() == true) return true;

        var typeToken = schema["type"];
        if (typeToken?.Type == JTokenType.Array && typeToken.Any(t => t.ToString() == "null"))
        {
            return true;
        }

        var oneOfToken = schema["oneOf"] ?? schema["anyOf"];
        if (oneOfToken is JArray oneOfArray)
        {
            if (oneOfArray.Any(item => item["type"]?.ToString() == "null"))
            {
                return true;
            }
        }

        return false;
    }

    private static DBTypeMap MapToDbTypeMap(string apiType, string format)
    {
        // IMPORTANT FIX: If the type doesn't match a standard primitive, 
        // we must return it as the CodeType (e.g. "AddressesDataItem") 
        // instead of falling back to "object".
        return apiType.ToLower() switch
        {
            "integer" => new DBTypeMap { CodeType = "int", DatabaseType = "int4" },
            "number" => new DBTypeMap { CodeType = "decimal", DatabaseType = "numeric" },
            "boolean" => new DBTypeMap { CodeType = "bool", DatabaseType = "bool" },
            "string" => format == "date-time"
                ? new DBTypeMap { CodeType = "DateTime", DatabaseType = "timestamp" }
                : new DBTypeMap { CodeType = "string", DatabaseType = "text" },
            "object" => new DBTypeMap { CodeType = "object", DatabaseType = "jsonb" },
            "any" => new DBTypeMap { CodeType = "object", DatabaseType = "jsonb" },
            _ => new DBTypeMap { CodeType = apiType, DatabaseType = "record" } // <- This preserves AddressesDataItem
        };
    }
}