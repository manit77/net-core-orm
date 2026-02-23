using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreORM
{    
    public class ApiMetadata
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public List<ApiNamespace> Namespaces { get; set; } = new();
        public List<ApiModel> Models { get; set; } = new();
    }

    public class ApiNamespace
    {
        public string Name { get; set; } // e.g., "Accounts"
        public List<ApiMethod> Methods { get; set; } = new();
    }

    public class ApiMethod
    {
        public string Name { get; set; } // OperationId
        public string Path { get; set; }
        public string HttpVerb { get; set; }
        public string RequestType { get; set; } // The name of the TS Interface for body
        public string ResponseType { get; set; } // The name of the TS Interface for response
        public bool IsArrayResponse { get; set; }
    }

    public class ApiModel
    {
        public string Name { get; set; } // e.g., "AccountsDataItem"
        public List<ApiProperty> Properties { get; set; } = new();
    }

    public class ApiProperty
    {
        public string Name { get; set; }
        public string Type { get; set; } // C# will map this to TS types
        public bool IsNullable { get; set; }
        public bool IsArray { get; set; }
        public string RefModel { get; set; } // If it points to another model
    }

    #region Raw OpenAPI Deserialization Models

    /// <summary>
    /// Root object for deserializing a raw OpenAPI 3.x JSON file.
    /// </summary>
    public class OpenApiDoc
    {
        [JsonProperty("info")]
        public OpenApiInfo Info { get; set; }

        [JsonProperty("paths")]
        public Dictionary<string, OpenApiPath> Paths { get; set; }

        [JsonProperty("components")]
        public OpenApiComponents Components { get; set; }
    }

    public class OpenApiInfo
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class OpenApiPath
    {
        [JsonProperty("get")]
        public OpenApiOperation Get { get; set; }

        [JsonProperty("post")]
        public OpenApiOperation Post { get; set; }
    }

    public class OpenApiOperation
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("requestBody")]
        public OpenApiBodyContent RequestBody { get; set; }

        [JsonProperty("responses")]
        public Dictionary<string, OpenApiBodyContent> Responses { get; set; }
    }

    public class OpenApiBodyContent
    {
        [JsonProperty("content")]
        public Dictionary<string, OpenApiContent> Content { get; set; }
    }

    public class OpenApiContent
    {
        [JsonProperty("schema")]
        public OpenApiSchema Schema { get; set; }
    }

    public class OpenApiSchema
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }
        [JsonProperty("type")]
        public JToken Type { get; set; } // Use JToken to handle string or array of strings
        [JsonProperty("properties")]
        public Dictionary<string, OpenApiSchema> Properties { get; set; }
        [JsonProperty("items")]
        public OpenApiSchema Items { get; set; }
        [JsonProperty("oneOf")]
        public List<OpenApiSchema> OneOf { get; set; }
    }

    public class OpenApiComponents
    {
        [JsonProperty("schemas")]
        public Dictionary<string, OpenApiSchema> Schemas { get; set; }
    }

    #endregion
}