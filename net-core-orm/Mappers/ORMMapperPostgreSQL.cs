using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreORM
{
    public class ORMMapperPostgreSQL : IDBMapper
    {
        public DBTypeMappings DataTypeMaps = new DBTypeMappings();

        public ORMMapperPostgreSQL()
        {
            // PostgreSQL specific type mappings
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "int8", CodeType = "long", CodeDBType = "System.Data.DbType.Int64", CodeTypeDefaultValue = "0L", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bigint", CodeType = "long", CodeDBType = "System.Data.DbType.Int64", CodeTypeDefaultValue = "0L", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "integer", CodeType = "int", CodeDBType = "System.Data.DbType.Int32", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "int4", CodeType = "int", CodeDBType = "System.Data.DbType.Int32", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "smallint", CodeType = "short", CodeDBType = "System.Data.DbType.Int16", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bool", CodeType = "bool", CodeDBType = "System.Data.DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = false, JSCodeType = "boolean", JSCodeTypeDefaultValue = "false" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "boolean", CodeType = "bool", CodeDBType = "System.Data.DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = false, JSCodeType = "boolean", JSCodeTypeDefaultValue = "false" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "int2", CodeType = "bool", CodeDBType = "System.Data.DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = false, JSCodeType = "boolean", JSCodeTypeDefaultValue = "false" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varchar", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "character varying", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "text", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "timestamp", CodeType = "DateTime", CodeDBType = "System.Data.DbType.DateTime", CodeTypeDefaultValue = "DateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "timestamptz", CodeType = "DateTimeOffset", CodeDBType = "System.Data.DbType.DateTimeOffset", CodeTypeDefaultValue = "DateTimeOffset.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "date", CodeType = "DateOnly", CodeDBType = "System.Data.DbType.Date", CodeTypeDefaultValue = "DateOnly.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "numeric", CodeType = "decimal", CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0M", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "double precision", CodeType = "double", CodeDBType = "System.Data.DbType.Double", CodeTypeDefaultValue = "0D", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bytea", CodeType = "byte[]", CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "Uint8Array", JSCodeTypeDefaultValue = "new Uint8Array()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "uuid", CodeType = "Guid", CodeDBType = "System.Data.DbType.Guid", CodeTypeDefaultValue = "Guid.Empty", CodeDBTypeIsNullable = false, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "json", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "jsonb", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "interval", CodeType = "TimeSpan", CodeDBType = "System.Data.DbType.Time", CodeTypeDefaultValue = "TimeSpan.Zero", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "real", CodeType = "float", CodeDBType = "System.Data.DbType.Single", CodeTypeDefaultValue = "0F", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "time", CodeType = "TimeOnly", CodeDBType = "System.Data.DbType.Time", CodeTypeDefaultValue = "TimeOnly.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "money", CodeType = "decimal", CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0M", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "character", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "xml", CodeType = "string", CodeDBType = "System.Data.DbType.Xml", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bit", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "record", CodeType = "DataTable", CodeDBType = "", CodeTypeDefaultValue = "new DataTable()", CodeDBTypeIsNullable = true, JSCodeType = "any[]", JSCodeTypeDefaultValue = "[]" });
        }

        public DBTypeMap GetDBTypeMap_ByDataBaseType(string databaseType)
        {
            var t = DataTypeMaps.FirstOrDefault(i => i.DatabaseType.Equals(databaseType, StringComparison.OrdinalIgnoreCase));
            if (t == null)
            {
                // Fallback for types with lengths like varchar(255)
                t = DataTypeMaps.FirstOrDefault(i => databaseType.ToLower().StartsWith(i.DatabaseType));
                if (t == null) throw new Exception("PostgreSQL Database Type not found: " + databaseType);
            }
            return t;
        }

        public async Task<DBDatabase> GetMapping(string dbName, string codeNameSpace, string connectionstring, List<DBORMMappings> mappingsList)
        {
            CoreUtils.ConsoleLogger.Info("Loading PostgreSQL Database Schema for " + dbName);

            DBDatabase database = new DBDatabase();
            database.Name = dbName;
            database.CodeNameSpace = codeNameSpace;

            var db = new CoreUtils.PostgresDatabase(connectionstring);

            #region Assign DataTables
            // Get Tables
            string sqlTables = @"
                SELECT table_name, table_schema, table_type 
                FROM information_schema.tables 
                WHERE table_schema NOT IN ('information_schema', 'pg_catalog') 
                AND table_type = 'BASE TABLE'
                ORDER BY table_name;";
                
            DataTable dtTables = await db.GetDataTable(sqlTables, null, CommandType.Text);

            // Get Columns with Primary Key and Identity info
            string sqlColumns = $@"
                SELECT 
                    cols.table_name, cols.column_name, cols.udt_name, cols.character_maximum_length, 
                    cols.is_nullable, cols.column_default, cols.numeric_precision, cols.ordinal_position,
                    CASE WHEN pk.column_name IS NOT NULL THEN 'PRI' ELSE '' END as column_key,
                    CASE WHEN cols.is_identity = 'YES' OR cols.column_default LIKE 'nextval%' THEN 'auto_increment' ELSE '' END as extra
                FROM information_schema.columns cols
                LEFT JOIN (
                    SELECT ku.table_name, ku.column_name, ku.table_schema
                    FROM information_schema.table_constraints tc
                    INNER JOIN information_schema.key_column_usage ku ON tc.constraint_name = ku.constraint_name
                    WHERE tc.constraint_type = 'PRIMARY KEY'
                ) pk ON cols.table_name = pk.table_name AND cols.column_name = pk.column_name AND cols.table_schema = pk.table_schema
                WHERE cols.table_schema NOT IN ('information_schema', 'pg_catalog');";
            DataTable dtColumns = await db.GetDataTable(sqlColumns, null, CommandType.Text);

            // Get Procedures/Functions
            string sqlRoutines = @"
                SELECT
                    r.routine_name, r.specific_name, r.routine_schema, r.routine_type, r.data_type,
                    p.oid as routine_oid,
                    pg_get_function_arguments(p.oid) as function_arguments
                FROM information_schema.routines r
                JOIN pg_catalog.pg_proc p ON p.proname = r.routine_name
                WHERE r.routine_schema NOT IN ('information_schema', 'pg_catalog')
                ORDER BY r.routine_name; ";
            DataTable dtProcedures = await db.GetDataTable(sqlRoutines, null, CommandType.Text);

            // Get Procedure/Function Parameters
            string sqlParameters = @"
                SELECT
                    p.specific_name, p.parameter_name, p.data_type, p.parameter_mode, p.ordinal_position
                FROM information_schema.parameters p
                WHERE p.specific_schema NOT IN ('information_schema', 'pg_catalog') AND p.parameter_mode IN ('IN', 'INOUT')
                ORDER BY p.specific_name, p.ordinal_position;";
            DataTable dtParameters = await db.GetDataTable(sqlParameters, null, CommandType.Text);

            // Get Function return fields for functions that return tables (composite types)
            string sqlReturnFields = @"
               SELECT
                   p.oid as routine_oid,
                   p.proname AS function_name,
                   n.nspname AS schema_name,
                   -- Get the name of the return column
                   unnest(p.proargnames[p.pronargdefaults+1 : array_length(p.proargnames, 1)]) AS column_name,
                   -- Get the data type of the return column
                   format_type(unnest(p.proallargtypes[p.pronargdefaults+1 : array_length(p.proallargtypes, 1)]), NULL) AS column_type
               FROM pg_proc p
               JOIN pg_namespace n ON n.oid = p.pronamespace
               WHERE n.nspname NOT IN ('information_schema', 'pg_catalog')
                 AND 't' = ANY(p.proargmodes); -- Only functions returning a Table (arg_mode 't')
            ";
            DataTable dtReturnFields = await db.GetDataTable(sqlReturnFields, null, CommandType.Text);
            #endregion

            foreach (DataRow row in dtTables.Rows)
            {
                DBTable table = new DBTable
                {
                    Database = database,
                    Name = row["table_name"].ToString(),
                    MappedName = CoreUtils.Data.ToPascalCase(row["table_name"].ToString()),
                    Schema = row["table_schema"].ToString()
                };

                var columns = dtColumns.AsEnumerable().Where(c => c.Field<string>("table_name") == table.Name);

                foreach (DataRow rowCol in columns)
                {
                    DBColumn col = new DBColumn();
                    col.Name = rowCol["column_name"].ToString();
                    col.MappedName = CoreUtils.Data.ToPascalCase(col.Name);
                    col.DBType = rowCol["udt_name"].ToString();
                    col.MappedDataType = GetDBTypeMap_ByDataBaseType(col.DBType);
                    col.MaxLength = CoreUtils.Data.ParseIt<long>(rowCol["character_maximum_length"]);
                    col.IsNullable = rowCol["is_nullable"].ToString() == "YES";
                    col.DefaultValue = rowCol["column_default"].ToString();
                    col.IsPrimaryKey = rowCol["column_key"].ToString() == "PRI";
                    col.IsIdentity = rowCol["extra"].ToString() == "auto_increment";

                    if (col.IsPrimaryKey) table.PrimaryKeys.Add(col);
                    if (col.Name.ToLower() == "id") table.Id = col;

                    // Manual Mapping Logic (Ported from your MySQL code)
                    ApplyCustomMappings(table, col, mappingsList);

                    table.Columns.Add(col);
                }
                database.Tables.Add(table);
            }

            // Load procedures and functions
            foreach (DataRow row in dtProcedures.Rows)
            {
                DBProcedure proc = new DBProcedure();
                proc.Name = row["routine_name"].ToString();
                proc.Schema = row["routine_schema"].ToString();
                proc.Type = row["routine_type"].ToString();
                proc.Oid = CoreUtils.Data.ParseIt<int>(row["routine_oid"]);

                // Get the full argument signature string which includes default values
                string functionArgs = row["function_arguments"]?.ToString() ?? "";
                var argParts = functionArgs.Split(',').Select(a => a.Trim()).ToList();

                string specificName = row["specific_name"].ToString();
                string returnType = row["data_type"]?.ToString();

                if (!string.IsNullOrEmpty(returnType) && !returnType.Equals("void", StringComparison.OrdinalIgnoreCase))
                {
                    proc.MappedDataType = GetDBTypeMap_ByDataBaseType(returnType);
                    proc.IsScalar = proc.MappedDataType.CodeType != "DataTable";

                    if (!proc.IsScalar)
                    {
                        var returnFields = dtReturnFields.AsEnumerable().Where(rf => CoreUtils.Data.ParseIt<int>(rf["routine_oid"]) == proc.Oid);
                        foreach(DataRow returnFieldRow in returnFields) {
                            var col = new DBColumn();
                            col.Name = returnFieldRow["column_name"].ToString();
                            col.MappedName = CoreUtils.Data.ToPascalCase(col.Name);
                            string colType = returnFieldRow["column_type"].ToString();
                            col.DBType = colType;
                            col.MappedDataType = GetDBTypeMap_ByDataBaseType(colType);
                            proc.ReturnFields.Add(col);
                        }
                    }
                }

                var parameters = dtParameters.AsEnumerable().Where(p => p.Field<string>("specific_name") == specificName);

                foreach (DataRow rowParam in parameters)
                {
                    DBParamater param = new DBParamater();
                    param.Name = rowParam["parameter_name"]?.ToString() ?? "";
                    param.DBType = rowParam["data_type"].ToString();
                    param.MappedDataType = GetDBTypeMap_ByDataBaseType(param.DBType);
                    param.Index = CoreUtils.Data.ParseIt<int>(rowParam["ordinal_position"]);

                    string paramMode = rowParam["parameter_mode"]?.ToString();
                    param.IsOutPut = "OUT".Equals(paramMode, StringComparison.OrdinalIgnoreCase) || "INOUT".Equals(paramMode, StringComparison.OrdinalIgnoreCase);

                    // Find the corresponding argument part to check for a default value.
                    // Example: "p_name character varying DEFAULT NULL"
                    var argDef = argParts.FirstOrDefault(a => a.StartsWith(param.Name + " ") || a.StartsWith("IN " + param.Name + " "));

                    param.HasDefaultValue = (argDef?.Contains("DEFAULT") ?? false);
                    param.IsNullable = param.HasDefaultValue; // If it has a default, it's nullable.


                    proc.Paramaters.Add(param);
                }

                database.Procedures.Add(proc);
            }

            // Generate SQL Statements (PostgreSQL Syntax)
            foreach (DBTable table in database.Tables)
            {
                string idCol = table.Id?.Name ?? "id";

                // PostgreSQL uses RETURNING for identity
                table.SQL_Insert = "insert into {tablename} ({insert_columns_list}) VALUES ({insert_params_list}) RETURNING " + idCol + ";";
                table.SQL_Select = "select {select_fields} from {tablename} where {params_where};";
                table.SQL_Update = "update {tablename} set {update_columns_params} where {params_where};";
                table.SQL_Delete = "delete from {tablename} where {params_where};";

                GenerateSqlStrings(table);
            }

            return database;
        }

        private void GenerateSqlStrings(DBTable table)
        {
            List<string> selectFields = new();
            List<string> insertCols = new();
            List<string> insertParams = new();
            List<string> updatePairs = new();
            List<string> wherePairs = new();

            foreach (var col in table.Columns)
            {
                selectFields.Add(col.Name);

                if (!col.IsIdentity)
                {
                    insertCols.Add(col.Name);
                    insertParams.Add(":" + col.Name);

                    if (!new[] { "UserIdCreated", "DateCreated", "DateTimeCreated" }.Contains(col.Name))
                    {
                        updatePairs.Add($"{col.Name}=:{col.Name}");
                    }
                }

                if (col.IsPrimaryKey)
                {
                    wherePairs.Add($"{col.Name}=:{col.Name}");
                }
            }

            table.SQL_Select = table.SQL_Select.Replace("{tablename}", table.Name)
                .Replace("{select_fields}", string.Join(",", selectFields))
                .Replace("{params_where}", string.Join(" AND ", wherePairs));

            table.SQL_Insert = table.SQL_Insert.Replace("{tablename}", table.Name)
                .Replace("{insert_columns_list}", string.Join(",", insertCols))
                .Replace("{insert_params_list}", string.Join(",", insertParams));

            table.SQL_Update = table.SQL_Update.Replace("{tablename}", table.Name)
                .Replace("{update_columns_params}", string.Join(",", updatePairs))
                .Replace("{params_where}", string.Join(" AND ", wherePairs));

            table.SQL_Delete = table.SQL_Delete.Replace("{tablename}", table.Name)
                .Replace("{params_where}", string.Join(" AND ", wherePairs));
        }

        private void ApplyCustomMappings(DBTable table, DBColumn col, List<DBORMMappings> mappingsList)
        {
            // Ported your existing logic for DBORMMappings and default date labels...
            var colMapping = mappingsList?.FirstOrDefault(i => i.TableName == table.Name && i.ColumnName == col.Name && i.MapType == "Column");
            if (colMapping != null && !string.IsNullOrEmpty(colMapping.FormMeta))
            {
                col.Mapping = JsonSerializer.Deserialize<DBORMColumnMap>(colMapping.FormMeta);
            }
            else if (col.Name.Contains("DateCreated") || col.Name.Contains("DateModified"))
            {
                col.Mapping = new DBORMColumnMap { InputType = "label", ReadOnly = true, DisplayName = col.Name.Contains("Created") ? "Created" : "Modified" };
            }
        }

        public void ProcessParam(DBProcedure proc) { /* Similar logic to MySQL for parsing definition */ }
    }
}