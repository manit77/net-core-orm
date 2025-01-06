using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.Json;


namespace CoreORM {

    public class ORMMapperMySQL: IDBMapper
    {
        
        public DBTypeMappings DataTypeMaps = new DBTypeMappings();
        public ORMMapperMySQL() {
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bigint", CodeType = "Int64", CodeDBType = "System.Data.DbType.Int64", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bigint", CodeType = "Int64", CodeDBType = "System.Data.DbType.Int64", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "binary", CodeType = "byte[]", CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bit", CodeType = "bool", CodeDBType = "System.Data.DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = false, JSCodeType = "boolean", JSCodeTypeDefaultValue = "false" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bool", CodeType = "bool", CodeDBType = "System.Data.DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = false, JSCodeType = "boolean", JSCodeTypeDefaultValue = "false" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "char", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "date", CodeType = "DateTime", CodeDBType = "System.Data.DbType.DateTime", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "datetime", CodeType = "DateTime", CodeDBType = "System.Data.DbType.DateTime", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "datetimeoffset", CodeType = "DateTimeOffset", CodeDBType = "System.Data.DbType.DateTimeOffset", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });            
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "decimal", CodeType = "decimal", CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "double", CodeType = "double", CodeDBType = "System.Data.DbType.Double", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "float", CodeType = "double", CodeDBType = "System.Data.DbType.Double", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "geography", CodeType = "object", CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "geometry", CodeType = "object", CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "hierarchyid", CodeType = "object", CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "image", CodeType = "byte[]", CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "int", CodeType = "Int32", CodeDBType = "System.Data.DbType.Int32", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "longblob", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" }); 

            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "money", CodeType = "decimal", CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "nchar", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "ntext", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "nvarchar", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "nvarchar", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "real", CodeType = "Single", CodeDBType = "System.Data.DbType.Double", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "smalldatetime", CodeType = "DateTime", CodeDBType = "System.Data.DbType.DateTime", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date"     , JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "smallint"    , CodeType = "Int16"    , CodeDBType = "System.Data.DbType.Int16"   , CodeTypeDefaultValue = "0"                                        , CodeDBTypeIsNullable = false, JSCodeType = "Date"     , JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "smallmoney"  , CodeType = "decimal"  , CodeDBType = "System.Data.DbType.Decimal" , CodeTypeDefaultValue = "0"                                        , CodeDBTypeIsNullable = false, JSCodeType = "number"   , JSCodeTypeDefaultValue = "0" });

            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "sql_variant", CodeType = "object", CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "text", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "longtext", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "mediumtext", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "time", CodeType = "TimeSpan", CodeDBType = "System.Data.DbType.Time", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "timestamp", CodeType = "byte[]", CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "tinyint", CodeType = "bool", CodeDBType = "System.Data.DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = true, JSCodeType = "boolean", JSCodeTypeDefaultValue = "false" });

            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "uniqueidentifier", CodeType = "Guid", CodeDBType = "System.Data.DbType.Guid", CodeTypeDefaultValue = "new Guid()", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varbinary", CodeType = "byte[]", CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varbinary", CodeType = "byte[]", CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varchar", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varchar", CodeType = "string", CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "xml", CodeType = "string", CodeDBType = "System.Data.DbType.Xml", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });

            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "table", CodeType = "DataTable", CodeDBType = "", CodeTypeDefaultValue = "", CodeDBTypeIsNullable = false, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "null", CodeType = "null", CodeDBType = "", CodeTypeDefaultValue = "", CodeDBTypeIsNullable = false, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "numeric", CodeType = "decimal", CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
                       
        }

        public DBTypeMap GetDBTypeMap_ByDataBaseType (string databaseType) {
            var t = (from i in DataTypeMaps where i.DatabaseType == databaseType.ToLower() select i).FirstOrDefault();
            if (t == null) {
                throw new Exception("Database Type not found " + databaseType);
            }
            return t;
        }

        public DBDatabase GetMapping (string dbName, string codeNameSpace, string connectionstring, List<DBORMMappings> mappingsList) {
            DBDatabase database = new DBDatabase();
            database.Name = dbName;
            database.CodeNameSpace = codeNameSpace;

            var db = new CoreUtils.MySQLDatabase(connectionstring);
            var conn = db.GetConnection();
            database.DB = db;

            string sql = "";
            
            #region assign datatables
            sql = $@"
SELECT * 
FROM information_schema.tables
WHERE table_type = 'base table' AND table_schema = '{dbName}'; ";

            System.Data.DataTable dtTables = db.GetDataTable(sql, null, CommandType.Text);
            dtTables.TableName = "TABLES";

            sql = $@"
SELECT * 
FROM information_schema.columns
WHERE table_schema = '{dbName}'; ";

            System.Data.DataTable dtColumns = db.GetDataTable(sql, null, CommandType.Text);
            dtColumns.TableName = "COLUMNS";

            sql = $@"
SELECT * 
FROM information_schema.views
WHERE table_schema = '{dbName}'; ";

            System.Data.DataTable dtViews = db.GetDataTable(sql, null, CommandType.Text);
            dtViews.TableName = "VIEWS";

            sql = $@"
SELECT *
FROM information_schema.routines
WHERE routine_schema = '{dbName}'; ";

            System.Data.DataTable dtProcedures = db.GetDataTable(sql, null, CommandType.Text);
            dtProcedures.TableName = "PROCEDURES";

            sql = $@"SELECT *   
FROM information_schema.parameters
where specific_schema='{dbName}';";

            System.Data.DataTable dtParamaters = db.GetDataTable(sql, null, CommandType.Text);
            dtParamaters.TableName = "PARAMATERS";

            //System.Data.DataTable dtProceduresText = db.GetDataTable(sql, null, CommandType.Text);
            //dtProceduresText.TableName = "PROCEDURES_TEXT";
            sql = $@"SELECT  *  
FROM   INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE REFERENCED_TABLE_SCHEMA = '{dbName}';";

            sql = $@"SELECT * FROM information_schema.TABLE_CONSTRAINTS 
WHERE information_schema.TABLE_CONSTRAINTS.CONSTRAINT_TYPE = 'FOREIGN KEY' 
AND information_schema.TABLE_CONSTRAINTS.TABLE_SCHEMA = '{dbName}';";

            //System.Data.DataTable dtForeignKeys = db.GetDataTable(sql, null, CommandType.Text);
            //dtForeignKeys.TableName = "Foreign_Keys";

            #endregion

            foreach (System.Data.DataRow row in dtTables.Rows) {
                if ((string)row["table_type"] != "BASE TABLE") {
                    continue;
                }

                DBTable table = new DBTable();
                table.Database = database;
                table.Name = (string)row["table_name"];
                table.Schema = (string)row["table_schema"];

                //get all columns
                var columns = (from i in dtColumns.AsEnumerable()
                               where i.Field<string>("table_name") == table.Name
                               select i);

                //CoreUtils.Data.ParseIt<int>(1);

                if (table.Name == "EnumsList") {
                    Console.Write("");
                }

                int ordcounter = 0;
                
                foreach (System.Data.DataRow rowCol in columns) {
                    ordcounter++;
                    DBColumn col = new DBColumn();
                    col.Name = CoreUtils.Data.ParseIt<string>(rowCol["column_name"]);
                    col.DBType = CoreUtils.Data.ParseIt<string>(rowCol["data_type"]);
                    col.MappedDataType = GetDBTypeMap_ByDataBaseType(col.DBType);
                    col.MaxLength = CoreUtils.Data.ParseIt<int>(rowCol["CHARACTER_MAXIMUM_LENGTH"]);
                    col.IsNullable = (CoreUtils.Data.ParseIt<string>(rowCol["IS_NULLABLE"]) == "YES") ? true : false;
                    col.DefaultValue = CoreUtils.Data.ParseIt<string>(rowCol["COLUMN_DEFAULT"]);
                    col.Precision = CoreUtils.Data.ParseIt<int>(rowCol["NUMERIC_PRECISION"]);                    
                    col.IsPrimaryKey = (CoreUtils.Data.ParseIt<string>(rowCol["COLUMN_KEY"]) == "PRI") ? true : false;
                    col.IsIdentity = (CoreUtils.Data.ParseIt<string>(rowCol["EXTRA"]) == "auto_increment") ? true : false;
                    col.Ordinal = ordcounter;

                    if (col.Name.ToLower() == "id") {
                        table.Id = col;
                    }

                    if (col.Name.ToLower() == "isactive")
                    {
                        Console.WriteLine($"{col.Name}={col.MappedDataType.JSCodeType}");
                    }

                    if (col.IsPrimaryKey) {
                        table.PrimaryKeys.Add(col);
                    }

                    var colMapping = (from i in mappingsList
                                      where i.TableName == table.Name
                                        && i.ColumnName == col.Name
                                        && i.MapType == "Column"
                                      select i).FirstOrDefault();

                    if (colMapping != null && colMapping.FormMeta.Length > 0) {
                        try {
                            col.Mapping = JsonSerializer.Deserialize<DBORMColumnMap>(colMapping.FormMeta);
                        } catch (Exception exp) {
                            Console.WriteLine(exp.ToString());
                            throw new Exception("Unable to parse column mapping for " + table.Name + "." + col.Name);
                        }
                    } else {
                        //create default column mapping
                        if(col.Name == "DateCreated" || col.Name == "DateTimeCreated") {
                            //for datecreated fields mark as readonly
                            col.Mapping = new DBORMColumnMap();
                            col.Mapping.ForDBTable = table;
                            col.Mapping.DisplayName = "Created";
                            col.Mapping.ForTableName = table.Name;
                            col.Mapping.InputType = "label";
                            col.Mapping.ReadOnly = true;                            
                        } else if (col.Name == "DateModified") {
                            col.Mapping = new DBORMColumnMap();
                            col.Mapping.ForDBTable = table;
                            col.Mapping.DisplayName = "Modified";
                            col.Mapping.ForTableName = table.Name;
                            col.Mapping.InputType = "label";
                            col.Mapping.ReadOnly = true;
                        }
                    }

                    table.Columns.Add(col);
                }

                //validate table
                if (table.PrimaryKeys.Count == 0) {
                    throw new Exception("table " + table.Name + " missing primary keys.");
                }

                var tableMapping = (from i in mappingsList
                                    where i.TableName == table.Name
                                    && i.MapType == "Table"
                                    select i).FirstOrDefault();

                if (tableMapping != null && tableMapping.FormMeta.Length > 0) {
                    try {
                        table.Mapping = JsonSerializer.Deserialize<DBORMTableMap>(tableMapping.FormMeta);
                    } catch (Exception exp) {
                        Console.WriteLine(exp.ToString());
                        throw new Exception("Unable to parse table mapping for " + table.Name);
                    }
                }

                database.Tables.Add(table);
            }


            ////map foreign keys
            //foreach (System.Data.DataRow row in dtForeignKeys.Rows) {
            //    string foreign_key_name = (string)row["foreign_key_name"];
            //    string table_name = (string)row["table_name"];
            //    string column_name = (string)row["column_name"];
            //    string foreign_table_name = (string)row["foreign_table_name"];
            //    string foreign_column_name = (string)row["foreign_column_name"];

            //    //find the table
            //    var table = database.Tables.Where(i => i.Name == table_name).FirstOrDefault();
            //    if (table != null) {
            //        var column = table.Columns.Where(i => i.Name == column_name).FirstOrDefault();
            //        if (column != null) {
            //            //find the foreign table
            //            var ftable = database.Tables.Where(i => i.Name == foreign_table_name).FirstOrDefault();
            //            if (ftable != null) {
            //                var fcolumn = ftable.Columns.Where(i => i.Name == foreign_column_name).FirstOrDefault();
            //                if (fcolumn != null) {

            //                    //add foreign key to table
            //                    var fk = new DBForeignKey() {
            //                        Table = table,
            //                        Column = column,
            //                        ForeignColumn = fcolumn,
            //                        Name = foreign_key_name,
            //                        ForeignTable = ftable
            //                    };

            //                    table.Foreign_Keys.Add(fk);

            //                    if (fk.ForeignTable.Name == "EnumsList") {
            //                        fk.Column.Mapping = new DBORMColumnMap() {
            //                            DisplayName = ORMCoreUtils.ColumnNameToLabel(fk.Column.Name),
            //                            Query = "SELECT Value=Id, Text=DataText FROM EnumsList where DataGroup='" + fk.Column.Name + "' order by DataText",
            //                            InputType = "select",
            //                            Required = false,
            //                            Visible = true,
            //                        };
            //                    } else {
            //                        //if set the default query for the mapping is there is one
            //                        if (fk.Column.Mapping == null && fk.ForeignTable.Mapping != null) {
            //                            fk.Column.Mapping = new DBORMColumnMap() {
            //                                DisplayName = ORMCoreUtils.ColumnNameToLabel(fk.Column.Name),
            //                                Query = fk.ForeignTable.Mapping.QueryForeignKey,
            //                                InputType = "select",
            //                                Required = false,
            //                                Visible = true,
            //                            };
            //                        }
            //                    }

            //                }
            //            }
            //        }
            //    }
            //}

            //discovery one to many mappings, Rules:
            //TableName + And + TableName
            //2 foreign keys, belongs to first TableName
            foreach (var linkerTable in database.Tables) {
                if (linkerTable.Name.Contains("And")) {
                    if (linkerTable.Foreign_Keys.Count == 2) {
                        string[] tableNames = linkerTable.Name.Split("And");
                        string oneTableName = tableNames[0];
                        string manyTableName = tableNames[1];

                        var oneTable = database.Tables.Where(i => i.Name == oneTableName).FirstOrDefault();
                        var manyTable = database.Tables.Where(i => i.Name == manyTableName).FirstOrDefault();

                        if (oneTable != null && manyTable != null && oneTable != manyTable) {
                            if (oneTable.PrimaryKeys.Count == 1 && manyTable.PrimaryKeys.Count == 1) {

                                var linkerMap = (from i in mappingsList
                                                 where i.TableName == linkerTable.Name
                                                 && i.MapType == "OneToMany"
                                                 select i).FirstOrDefault();

                                if (linkerMap != null && linkerMap.FormMeta.Length > 0) {
                                    try {
                                        linkerTable.OneToManyMapping = JsonSerializer.Deserialize<DBORMColumnMap>(linkerMap.FormMeta);
                                    } catch (Exception exp) {
                                        Console.WriteLine(exp.ToString());
                                        throw new Exception("Unable to parse ForeignKey mapping for " + linkerTable.Name);
                                    }
                                }

                                //add to one to many list
                                oneTable.OneToManyList.Add(new DBOneToMany() {
                                    LinkerTable = linkerTable,
                                    OneTable = oneTable,
                                    ManyTable = manyTable
                                });
                            }
                        }
                    }
                }
            }

            //discover ManyForOne
            //Rules: TableName + For + TableName
            foreach (var table in database.Tables) {
                if (table.Name.Contains("For")) {

                    var manyTable = table;
                    string[] tableNames = table.Name.Split("For");

                    string oneTableName = tableNames[1];
                    var oneTable = database.Tables.Where(i => i.Name == oneTableName).FirstOrDefault();
                    if (oneTable != null) {
                        DBForeignKey oneFK = null;
                        DBForeignKey manyFK = null;

                        if (oneTable.Foreign_Keys != null) {
                            foreach (var fk in oneTable.Foreign_Keys) {
                                if (fk.ForeignTable == manyTable) {
                                    oneFK = fk;                                    
                                }
                            }
                        }

                        if (manyTable.Foreign_Keys != null) {
                            foreach (var fk in manyTable.Foreign_Keys) {
                                if (fk.ForeignTable == oneTable) {
                                    manyFK = fk;
                                }
                            }
                        }

                        if (oneTable != null && manyTable != null && oneTable != manyTable) {
                            if (oneTable.PrimaryKeys.Count == 1 && manyTable.PrimaryKeys.Count == 1) {

                                var manyMap = (from i in mappingsList
                                               where i.TableName == manyTable.Name
                                               && i.MapType == "ManyForOne"
                                               select i).FirstOrDefault();

                                if (manyMap != null && manyMap.FormMeta.Length > 0) {
                                    try {
                                        manyTable.ManyForOneMapping = JsonSerializer.Deserialize<DBORMColumnMap>(manyMap.FormMeta);
                                    } catch {
                                        throw new Exception("Unable to parse ForeignKey mapping for " + manyTable.Name);
                                    }
                                }

                                //add to one to many list
                                oneTable.ManyForOneList.Add(new DBManyForOne() {
                                    OneTable = oneTable,
                                    ManyTable = manyTable,
                                    OneFK = oneFK,
                                    ManyFK = manyFK
                                });
                            }
                        }
                    }
                }
            }

            //generate sql for (ManyForOne)
            foreach (var table in database.Tables) {
                if(table.ManyForOneList != null) {
                    foreach (var one in table.ManyForOneList) {
                        //string sql = $"select * from BuildsForStoreAssets where StoreAssetId={dbmodel.Id}";
                        if(one.ManyFK == null)
                        {
                            continue;
                        }
                        string valueField = "Id";
                        string textField = "Id";
                        var nameCol = one.ManyTable.GetColumn("Name");                        
                        
                        if (nameCol != null) {
                            textField = nameCol.Name;                            
                        } else {
                            var descCol = one.ManyTable.GetColumn("Description");
                            if (descCol != null) {
                                textField = descCol.Name;
                            }
                        }

                        one.SQL_Many = $"select Value={valueField}, Text={textField} from " + one.ManyTable.Name + " where " + one.ManyFK.Column.Name + " = {dbmodel.Id} order by " + $"{textField}";
                        
                        //if a column mapping does not exists create a default one
                        if (one.OneFK != null && one.OneFK.Column.Mapping == null) {
                            one.OneFK.Column.Mapping = new DBORMColumnMap() {                                
                                Visible = true,
                                Required = false,
                                InputType = "select"
                            };
                        }
                    }
                }
            }

            //generate sql for LinkerTable (OneToMany)
            foreach (var table in database.Tables) {
                foreach (var oneToMany in table.OneToManyList) {
                    string paramsList = "";
                    DBForeignKey fkOneColumn = null;
                    DBForeignKey fkManyColumn = null;
                    //there are only two forieng keys
                    foreach (var fk in oneToMany.LinkerTable.Foreign_Keys) {
                        if (oneToMany.OneTable.Name == fk.ForeignTable.Name) {
                            fkOneColumn = fk;
                            //get the column name of the oneToMany.Table
                            if (paramsList.Length > 0) {
                                paramsList = paramsList + ",";
                            }
                            paramsList = fk.Column.MappedDataType.CodeType + " " + fk.Column.Name;
                        } else {
                            fkManyColumn = fk;
                        }
                    }

                    oneToMany.SQL_Many = @"
SELECT {ManyTable.Name}.*
from {ManyTable.Name} {ManyTable.Name}
join {LinkerTable.Name} {LinkerTable.Name}
    on {LinkerTable.Name}.{ManyColumn.Name} = {ManyTable.Name}.{ManyTable.PrimaryKey.Name}
where {LinkerTable.Name}.{OneColumn.Name}= @{OneColumn.Name}";

                    oneToMany.SQL_Linker = @"
SELECT {LinkerTable.Name}.*
from {ManyTable.Name} {ManyTable.Name}
join {LinkerTable.Name} {LinkerTable.Name}
    on {LinkerTable.Name}.{ManyColumn.Name} = {ManyTable.Name}.{ManyTable.PrimaryKey.Name}
where {LinkerTable.Name}.{OneColumn.Name}= @{OneColumn.Name}";

                    //set the sql statement
                    oneToMany.SQL_Many = oneToMany.SQL_Many.Replace("{ManyTable.Name}", oneToMany.ManyTable.Name);
                    oneToMany.SQL_Many = oneToMany.SQL_Many.Replace("{LinkerTable.Name}", oneToMany.LinkerTable.Name);
                    oneToMany.SQL_Many = oneToMany.SQL_Many.Replace("{ManyColumn.Name}", fkManyColumn.Column.Name);
                    oneToMany.SQL_Many = oneToMany.SQL_Many.Replace("{OneColumn.Name}", fkOneColumn.Column.Name);
                    oneToMany.SQL_Many = oneToMany.SQL_Many.Replace("{OneTable.Name}", oneToMany.OneTable.Name);
                    oneToMany.SQL_Many = oneToMany.SQL_Many.Replace("{ManyTable.PrimaryKey.Name}", oneToMany.ManyTable.PrimaryKeys[0].Name);

                    oneToMany.SQL_Linker = oneToMany.SQL_Linker.Replace("{ManyTable.Name}", oneToMany.ManyTable.Name);
                    oneToMany.SQL_Linker = oneToMany.SQL_Linker.Replace("{LinkerTable.Name}", oneToMany.LinkerTable.Name);
                    oneToMany.SQL_Linker = oneToMany.SQL_Linker.Replace("{ManyColumn.Name}", fkManyColumn.Column.Name);
                    oneToMany.SQL_Linker = oneToMany.SQL_Linker.Replace("{OneColumn.Name}", fkOneColumn.Column.Name);
                    oneToMany.SQL_Linker = oneToMany.SQL_Linker.Replace("{OneTable.Name}", oneToMany.OneTable.Name);
                    oneToMany.SQL_Linker = oneToMany.SQL_Linker.Replace("{ManyTable.PrimaryKey.Name}", oneToMany.ManyTable.PrimaryKeys[0].Name);

                }
            }

            //load views
            foreach (System.Data.DataRow row in dtViews.Rows) {
                DBTable table = new DBTable();
                table.Name = (string)row["table_name"];
                table.Schema = (string)row["table_schema"];
                //get all columns
                var columns = (from i in dtColumns.AsEnumerable()
                               where i.Field<string>("table_name") == table.Name
                               select i);
                foreach (System.Data.DataRow rowCol in columns) {
                    DBColumn col = new DBColumn();
                    col.Name = CoreUtils.Data.ParseIt<string>(rowCol["column_name"]);
                    col.DBType = CoreUtils.Data.ParseIt<string>(rowCol["data_type"]);
                    col.MappedDataType = GetDBTypeMap_ByDataBaseType(col.DBType);
                    col.MaxLength = CoreUtils.Data.ParseIt<int>(rowCol["CHARACTER_MAXIMUM_LENGTH"]);
                    col.IsNullable = (CoreUtils.Data.ParseIt<string>(rowCol["IS_NULLABLE"]) == "YES") ? true : false;
                    col.DefaultValue = CoreUtils.Data.ParseIt<string>(rowCol["COLUMN_DEFAULT"]);
                    col.Precision = CoreUtils.Data.ParseIt<int>(rowCol["NUMERIC_PRECISION"]);

                    table.Columns.Add(col);
                }

                database.Views.Add(table);
            }

            //load procedures
            foreach (System.Data.DataRow row in dtProcedures.Rows) {
                DBProcedure proc = new DBProcedure();
                proc.Name = (string)row["SPECIFIC_NAME"];
                proc.Schema = (string)row["ROUTINE_SCHEMA"];

                string dbtype = CoreUtils.Data.ParseIt<string>(row["DATA_TYPE"]);
                if (dbtype.Length > 0) {
                    proc.MappedDataType = GetDBTypeMap_ByDataBaseType(dbtype);
                }

                proc.Type = CoreUtils.Data.ParseIt<string>(row["ROUTINE_TYPE"]);

                if (proc.Name != "GetSchema") {
                    //get all columns
                    var columns = (from i in dtParamaters.AsEnumerable()
                                   where i.Field<string>("SPECIFIC_NAME") == proc.Name
                                   orderby i.Field<int>("ORDINAL_POSITION")
                                   select i);
                    int paramIndex = 0;
                    foreach (System.Data.DataRow rowParam in columns) {
                        paramIndex++;
                        DBParamater paramater = new DBParamater();
                        paramater.Name = CoreUtils.Data.ParseIt<string>(rowParam["PARAMETER_NAME"]);
                        paramater.DBType = CoreUtils.Data.ParseIt<string>(rowParam["DATA_TYPE"]);
                        paramater.MappedDataType = GetDBTypeMap_ByDataBaseType(paramater.DBType);
                        paramater.MaxLength = CoreUtils.Data.ParseIt<int>(rowParam["CHARACTER_MAXIMUM_LENGTH"]);
                        paramater.Precision = CoreUtils.Data.ParseIt<int>(rowParam["NUMERIC_PRECISION"]);
                        paramater.IsOutPut = (CoreUtils.Data.ParseIt<string>(rowParam["PARAMETER_MODE"]) == "IN" ? false : true);
                        paramater.Index = CoreUtils.Data.ParseIt<int>(rowParam["ORDINAL_POSITION"]);

                        //IsNullable, DefaultValue does not pull back correctly in sql server
                        paramater.DefaultValue = "";
                        paramater.IsNullable = true; // all params in mysql is nullable
                        proc.Paramaters.Add(paramater);

                        //skip procedure text parsing
                        //proc.Text = (from i in dtProceduresText.AsEnumerable() where i.Field<string>("name") == proc.Name select i.Field<string>("definition")).FirstOrDefault();
                        //proc.Text = CoreUtils.Data.ParseIt<string>(proc.Text);
                    }
                    database.Procedures.Add(proc);
                }
            }

            //skip this for mysql
            //this gets the stored procedure text and parses the paramaters
            //foreach (DBProcedure proc in database.Procedures) {
            //    ProcessParam(proc);
            //}

            //generate SQL Statements for each table
            foreach (DBTable table in database.Tables) {
                table.SQL_ScopeId = " select LAST_INSERT_ID() as {param_identity};";
                table.SQL_Select = " select {select_fields} from {tablename} where {params_where};";
                table.SQL_Insert = " insert into {tablename} ({insert_columns_list})VALUES({insert_params_list});";
                table.SQL_Update = " update {tablename} set {update_columns_params} where {params_where};";
                table.SQL_Delete = " delete from {tablename} where {params_where};";

                string tablename = string.Empty;
                string param_identity = string.Empty;
                string insert_columns_list = string.Empty;
                string insert_params_list = string.Empty;
                string update_columns_params = string.Empty;
                string params_where = string.Empty;
                string select_fields = string.Empty;

                tablename = table.Name;
                if (table.Id != null) {
                    param_identity = table.Id.Name;
                }

                foreach (var col in table.Columns) {
                    if (select_fields.Length > 0) {
                        select_fields = select_fields + ",";
                    }
                    select_fields += col.Name;

                    if (col.MappedDataType.DatabaseType == "timestamp") {
                        //skip timestamp
                        continue;
                    }

                    if (!col.IsIdentity)
                    {
                        if (insert_columns_list.Length > 0)
                        {
                            insert_columns_list = insert_columns_list + ",";
                        }
                        insert_columns_list += col.Name;

                        if (insert_params_list.Length > 0)
                        {
                            insert_params_list = insert_params_list + ",";
                        }
                        insert_params_list += ":" + col.Name;

                        //skip UserIdCreated, DateCreated, DateTimeCreated
                        if (col.Name != "UserIdCreated" && col.Name != "DateCreated" && col.Name != "DateTimeCreated")
                        {
                            if (update_columns_params.Length > 0)
                            {
                                update_columns_params = update_columns_params + ",";
                            }
                            update_columns_params += col.Name + "=:" + col.Name;
                        }

                    }

                    if (col.IsPrimaryKey) {
                        if (params_where.Length > 0) {
                            params_where = params_where + ",";
                        }
                        params_where += col.Name + "=:" + col.Name;
                    }
                }

                table.SQL_Insert = table.SQL_Insert.Replace("{tablename}", tablename);
                table.SQL_Update = table.SQL_Update.Replace("{tablename}", tablename);
                table.SQL_Delete = table.SQL_Delete.Replace("{tablename}", tablename);
                table.SQL_Select = table.SQL_Select.Replace("{tablename}", tablename);

                table.SQL_ScopeId = table.SQL_ScopeId.Replace("{param_identity}", param_identity);
                table.SQL_Select = table.SQL_Select.Replace("{select_fields}", select_fields);
                table.SQL_Select = table.SQL_Select.Replace("{params_where}", params_where);
                table.SQL_Insert = table.SQL_Insert.Replace("{insert_columns_list}", insert_columns_list);
                table.SQL_Insert = table.SQL_Insert.Replace("{insert_params_list}", insert_params_list);

                table.SQL_Update = table.SQL_Update.Replace("{update_columns_params}", update_columns_params);
                table.SQL_Update = table.SQL_Update.Replace("{params_where}", params_where);

                table.SQL_Delete = table.SQL_Delete.Replace("{params_where}", params_where);
            }

            return database;
        }

        /// <summary>
        /// populate the output and isnullable flag for params
        /// </summary>
        /// <param name="proc"></param>
        public void ProcessParam (DBProcedure proc) {
            if (proc.Name == "GetAppsConfigs") {
                Console.WriteLine("DEBUG:" + proc.Name);
            }

            string definition = proc.Text.ToLower();
            definition = definition.Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
            definition = CoreUtils.Data.ReplaceAllInstances(definition, "  ", " "); //remove all double spaces

            int poscreate = definition.IndexOf("("); //find first @
            int posAs = 0;

            if (proc.Type.ToLower() == "procedure") {
                posAs = definition.IndexOf(" as");
            } else {
                posAs = definition.IndexOf(" returns");
            }

            if (posAs > poscreate && posAs > 0 && poscreate > 0) {
                //(@skip int = 0,@take int = 1000,@query varchar(100) = '',@sortfield varchar(100) = '',@sortdir varchar(100) = '')
                string paramdefinition = definition.Substring(poscreate, posAs - poscreate);

                if (!paramdefinition.StartsWith("(") && !paramdefinition.EndsWith(")")) {
                    throw new Exception("Params defined in stored procedure must start with ( and end with )");
                }

                paramdefinition = CoreUtils.Data.RemoveFirstOf(paramdefinition, "(");
                paramdefinition = CoreUtils.Data.RemoveLastOf(paramdefinition, ")");

                proc.DefParams = paramdefinition;

                List<dynamic> positions = new List<dynamic>();
                Action<DBParamater> getDef = (DBParamater x) => {
                    try {
                        x.DefParam = proc.DefParams.Substring(x.DefPosStart, x.DefPosEnd - x.DefPosStart);
                        x.DefParam = x.DefParam.Trim();
                        x.DefParam = CoreUtils.Data.RemoveLastOf(x.DefParam, ",").Trim();
                    } catch {
                        throw new Exception("Failed to parse paramter " + x.Name + " for procedure " + proc.Name);
                    }
                };

                for (int j = 0; j < proc.Paramaters.Count; j++) {
                    DBParamater param = proc.Paramaters[j];
                    DBParamater paramNext = null;

                    if (j + 1 < proc.Paramaters.Count) {
                        paramNext = proc.Paramaters[j + 1];
                    }
                    param.DefPosStart = paramdefinition.IndexOf("@" + param.Name.ToLower());
                    if (paramNext != null) {
                        param.DefPosEnd = paramdefinition.IndexOf("@" + paramNext.Name.ToLower());
                    } else {
                        param.DefPosEnd = proc.DefParams.Length;
                    }
                    getDef.Invoke(param);
                }

                for (int j = 0; j < proc.Paramaters.Count; j++) {
                    #region

                    var param = proc.Paramaters[j];
                    param.IsNullable = false;

                    //@userid int = null output)
                    if (param.DefParam.Contains("=")) {
                        string[] paradefs = param.DefParam.Trim().Split('=');
                        string paramNamePart = paradefs[0].Trim(); //@userid int
                        string paramValuePart = paradefs[1].Trim(); //null output

                        param.DefaultValue = paramValuePart;
                        param.IsNullable = true;
                        param.HasDefaultValue = true;

                        if (paramValuePart.EndsWith(" output")) {
                            param.IsOutPut = true;
                            paramValuePart = CoreUtils.Data.RemoveLastOf(paramValuePart, " output");
                        }

                        if (paramValuePart.EndsWith("null")) {
                            param.DefaultValue = "null";
                        } else {
                            if (paramValuePart.StartsWith("'") && paramValuePart.EndsWith("'")) {
                                param.DefaultValue = paramValuePart;
                                param.DefaultValue = CoreUtils.Data.RemoveLastOf(param.DefaultValue, "'");
                                param.DefaultValue = CoreUtils.Data.RemoveFirstOf(param.DefaultValue, "'");
                                param.DefaultValue = "\"" + param.DefaultValue + "\"";
                            }
                        }
                    }
                    if (param.DefParam.EndsWith(" output")) {
                        param.IsOutPut = true;
                    }

                    #endregion
                }
            }
        }
    }
}
