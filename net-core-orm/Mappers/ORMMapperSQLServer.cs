using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreORM
{
    public class ORMMapperSQLServer : IDBMapper
    {
        public string SQLGetSchema = @"
    SELECT DISTINCT t.*
	FROM INFORMATION_SCHEMA.TABLES t
	where t.TABLE_NAME not in('sysdiagrams')
	order by t.TABLE_NAME
	
	select DISTINCT c.*
		, is_primarykey = ISNULL(x.is_primary_key, 0)
		, is_identity= COLUMNPROPERTY(object_id(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') 
	from INFORMATION_SCHEMA.COLUMNS c
	left join (select  
				Table_Name = OBJECT_NAME(ic.OBJECT_ID), 
				Column_Name = COL_NAME(ic.OBJECT_ID,ic.column_id)
				, i.is_primary_key
			from sys.indexes i 			
			left join sys.index_columns AS ic 
			ON  
				i.OBJECT_ID = ic.OBJECT_ID
				AND i.index_id = ic.index_id
				AND i.index_id = ic.index_id
                AND i.is_primary_key = 1) x
	on	c.TABLE_NAME = x.Table_Name
	and c.COLUMN_NAME = x.Column_Name	
	where c.TABLE_NAME not in('sysdiagrams')
	order by c.TABLE_NAME, c.ORDINAL_POSITION

	SELECT DISTINCT v.* 
	FROM INFORMATION_SCHEMA.VIEWS v
	order by v.TABLE_NAME

	SELECT DISTINCT PROCEDURE_NAME=r.ROUTINE_NAME, r.*
	FROM information_schema.routines r
	where r.ROUTINE_NAME not in ('fn_diagramobjects', 'sp_upgraddiagrams', 'sp_helpdiagrams', 'sp_helpdiagramdefinition', 'sp_creatediagram', 'sp_alterdiagram', 'sp_dropdiagram', 'sp_renamediagram', 'sysdiagrams')
	order by r.ROUTINE_NAME
	
	select 		
		PROCEDURE_NAME = OBJECT_NAME(par.object_id)
		, object_id = par.object_id
		, PARAMATER_NAME = par.name
		, Data_Type   = type_name(par.user_type_id)
		, CHARACTER_MAXIMUM_LENGTH   = par.max_length
		, NUMERIC_PRECISION   = case when type_name(par.system_type_id) = 'uniqueidentifier' 
								then par.precision  
								else OdbcPrec(par.system_type_id, par.max_length, par.precision) end
		,  Scale   = OdbcScale(par.system_type_id, par.scale)
		, Param_order  = parameter_id
		, Collation   = convert(sysname, 
					case when par.system_type_id in (35, 99, 167, 175, 231, 239)  
					then ServerProperty('collation') end)  
		, Module_Type = CASE
				WHEN OBJECTPROPERTY(par.OBJECT_ID, 'IsProcedure') = 1 THEN 'IsProcedure'
				WHEN OBJECTPROPERTY(par.OBJECT_ID, 'IsScalarFunction') = 1 THEN 'IsScalarFunction'
				WHEN OBJECTPROPERTY(par.OBJECT_ID, 'IsTableFunction') = 1 THEN 'IsTableFunction'
			END
		, par.has_default_value --does not populate correctly in 2016
		, par.default_value --does not populate correctly in 2016
		, par.is_nullable --does not populate correctly in 2016
		, par.is_output
		, p.type, p.type_desc
		, s.type
		, s.status
		--par.*, p.*, t.* 
	from sys.parameters par	
		left join sys.procedures p 
			on par.object_id = p.object_id 
		left  join dbo.sysobjects s
			on s.id = p.object_id 
		left join sys.types t 
			on par.system_type_id = t.system_type_id 
			AND par.user_type_id = t.user_type_id
	where len(par.name) > 0 
		and  OBJECT_NAME(par.object_id) not in ('sp_helpdiagrams', 'sp_helpdiagramdefinition', 'sp_creatediagram', 'sp_alterdiagram', 'sp_dropdiagram', 'sp_renamediagram', 'sysdiagrams')
	order by p.name, par.name
	
	-- get sql server text
	SELECT DISTINCT object_id=m.object_id
		, Name=OBJECT_NAME(m.object_id)
		, Definition = m.definition  
	FROM sys.sql_modules  m
	where  OBJECT_NAME(m.object_id) not in ('sp_helpdiagrams', 'sp_helpdiagramdefinition', 'sp_creatediagram', 'sp_alterdiagram', 'sp_dropdiagram', 'sp_renamediagram', 'sp_upgraddiagrams', 'fn_diagramobjects', 'sysdiagrams')
	order by OBJECT_NAME(m.object_id)
	
	SELECT DISTINCT
		Foreign_Key_Name = f.name,
		Table_Name = OBJECT_NAME(f.parent_object_id),
		Column_Name = COL_NAME(fc.parent_object_id, fc.parent_column_id) ,
		Foreign_Table_Name = t.name,
		Foreign_Column_Name = COL_NAME(fc.referenced_object_id, fc.referenced_column_id),
		f.type,
		f.type_desc   
	 FROM 
	   sys.foreign_keys AS f
	left JOIN 
	   sys.foreign_key_columns AS fc 
		  ON f.OBJECT_ID = fc.constraint_object_id
	left  JOIN 
	   sys.tables t 
		  ON t.OBJECT_ID = fc.referenced_object_id	
	order by OBJECT_NAME(f.parent_object_id)
";
        public DBTypeMappings DataTypeMaps = new DBTypeMappings();
        public ORMMapperSQLServer()
        {
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bigint"          , CodeType = "Int64"            , CodeDBType = "System.Data.DbType.Int64", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bigint"          , CodeType = "Int64"            , CodeDBType = "System.Data.DbType.Int64", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "binary"          , CodeType = "byte[]"           , CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bit"             , CodeType = "bool"             , CodeDBType = "System.Data.DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = false, JSCodeType = "boolean", JSCodeTypeDefaultValue = "false" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "char"            , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "date"            , CodeType = "DateTime"         , CodeDBType = "System.Data.DbType.DateTime", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "datetime"        , CodeType = "DateTime"         , CodeDBType = "System.Data.DbType.DateTime", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "datetimeoffset"  , CodeType = "DateTimeOffset"   , CodeDBType = "System.Data.DbType.DateTimeOffset", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "decimal"         , CodeType = "decimal"          , CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "float"           , CodeType = "double"           , CodeDBType = "System.Data.DbType.Double", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "geography"       , CodeType = "object"           , CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "geometry"        , CodeType = "object"           , CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "hierarchyid"     , CodeType = "object"           , CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "image"           , CodeType = "byte[]"           , CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "int"             , CodeType = "Int32"            , CodeDBType = "System.Data.DbType.Int32", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "money"           , CodeType = "decimal"          , CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "nchar"           , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "ntext"           , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "nvarchar"        , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "nvarchar"        , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "real"            , CodeType = "Single"           , CodeDBType = "System.Data.DbType.Double", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });            
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "smalldatetime"   , CodeType = "DateTime"         , CodeDBType = "System.Data.DbType.DateTime", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = false, JSCodeType = "Date", JSCodeTypeDefaultValue = "new Date()" });            
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "smallint"        , CodeType = "Int16"            , CodeDBType = "System.Data.DbType.Int16", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "smallmoney"      , CodeType = "decimal"          , CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "sql_variant"     , CodeType = "object"           , CodeDBType = "System.Data.DbType.Object", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "text"            , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "time"            , CodeType = "TimeSpan"         , CodeDBType = "System.Data.DbType.Time", CodeTypeDefaultValue = "new object()", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "timestamp"       , CodeType = "byte[]"           , CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "tinyint"         , CodeType = "byte"             , CodeDBType = "System.Data.DbType.Byte", CodeTypeDefaultValue = "new byte()", CodeDBTypeIsNullable = true, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "uniqueidentifier", CodeType = "Guid"             , CodeDBType = "System.Data.DbType.Guid", CodeTypeDefaultValue = "new Guid()", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varbinary"       , CodeType = "byte[]"           , CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varbinary"       , CodeType = "byte[]"           , CodeDBType = "System.Data.DbType.Binary", CodeTypeDefaultValue = "Array.Empty<byte>()", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varchar"         , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varchar"         , CodeType = "string"           , CodeDBType = "System.Data.DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "string", JSCodeTypeDefaultValue = "\"\"" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "xml"             , CodeType = "string"           , CodeDBType = "System.Data.DbType.Xml", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = true, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });

            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "table"           , CodeType = "DataTable"        , CodeDBType = "", CodeTypeDefaultValue = "", CodeDBTypeIsNullable = false, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "null"            , CodeType = "null"             , CodeDBType = "", CodeTypeDefaultValue = "", CodeDBTypeIsNullable = false, JSCodeType = "any", JSCodeTypeDefaultValue = "null" });
            DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "numeric"         , CodeType = "decimal"          , CodeDBType = "System.Data.DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = false, JSCodeType = "number", JSCodeTypeDefaultValue = "0" });

            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "varchar", CodeType = "string", CodeDBType = "DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = false });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "char", CodeType = "string", CodeDBType = "DbType.String", CodeTypeDefaultValue = "string.Empty", CodeDBTypeIsNullable = false });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "int", CodeType = "int", CodeDBType = "DbType.Int32", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "bit", CodeType = "bool", CodeDBType = "DbType.Boolean", CodeTypeDefaultValue = "false", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "money", CodeType = "decimal", CodeDBType = "DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "decimal", CodeType = "decimal", CodeDBType = "DbType.Decimal", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "float", CodeType = "double", CodeDBType = "DbType.Double", CodeTypeDefaultValue = "0", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "datetime", CodeType = "DateTime", CodeDBType = "DbType.DateTime", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "datetimeoffset", CodeType = "DateTimeOffset", CodeDBType = "DbType.DateTimeOffset", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "date", CodeType = "DateTime", CodeDBType = "DbType.Time", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "time", CodeType = "TimeSpan", CodeDBType = "DbType.Time", CodeTypeDefaultValue = "System.Data.SqlTypes.SqlDateTime.MinValue", CodeDBTypeIsNullable = true });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "table", CodeType = "DataTable", CodeDBType = "", CodeTypeDefaultValue = "", CodeDBTypeIsNullable = false });
            //DataTypeMaps.Add(new DBTypeMap() { DatabaseType = "null", CodeType = "null", CodeDBType = "", CodeTypeDefaultValue = "", CodeDBTypeIsNullable = false });


        }

        public DBTypeMap GetDBTypeMap_ByDataBaseType(string databaseType)
        {
            var t = (from i in this.DataTypeMaps where i.DatabaseType == databaseType.ToLower() select i).FirstOrDefault();
            if (t == null)
            {
                throw new Exception("Database Type not found " + databaseType);
            }
            return t;
        }

        public async Task<DBDatabase> GetMapping(string dbName, string codeNameSpace, string connectionstring, List<DBORMMappings> mappingsList)
        {
            DBDatabase database = new DBDatabase();
            database.Name = dbName;
            database.CodeNameSpace = codeNameSpace;

            var db = new CoreUtils.SQLServerDatabase(connectionstring);
            var conn = db.GetConnection();
            //var mappingsList = db.Query<DBORMMappings>("select * from ORMMappings order by TableName, ColumnName");

            System.Data.DataSet ds = await db.GetDataSet(this.SQLGetSchema, null, CommandType.Text);

            #region assign datatables

            System.Data.DataTable dtTables = ds.Tables[0];
            dtTables.TableName = "TABLES";

            System.Data.DataTable dtColumns = ds.Tables[1];
            dtColumns.TableName = "COLUMNS";

            System.Data.DataTable dtViews = ds.Tables[2];
            dtViews.TableName = "VIEWS";

            System.Data.DataTable dtProcedures = ds.Tables[3];
            dtProcedures.TableName = "PROCEDURES";

            System.Data.DataTable dtParamaters = ds.Tables[4];
            dtParamaters.TableName = "PARAMATERS";

            System.Data.DataTable dtProceduresText = ds.Tables[5];
            dtProceduresText.TableName = "PROCEDURES_TEXT";

            System.Data.DataTable dtForeignKeys = ds.Tables[6];
            dtForeignKeys.TableName = "Foreign_Keys";

            #endregion

            foreach (System.Data.DataRow row in dtTables.Rows)
            {
                if ((string)row["table_type"] != "BASE TABLE")
                {
                    continue;
                }

                DBTable table = new DBTable();
                table.Database = database;
                table.Name = (string)row["table_name"];
                table.Schema = (string)row["table_schema"];

                //get all columns
                var columns = (from i in dtColumns.AsEnumerable()
                               where i.Field<string>("table_name") == table.Name
                               select i).Distinct();

                CoreUtils.Data.ParseIt<int>(1);

                if (table.Name == "EnumsList")
                {
                    Console.Write("");
                }

                foreach (System.Data.DataRow rowCol in columns)
                {
                    DBColumn col = new DBColumn();
                    col.Name = CoreUtils.Data.ParseIt<string>(rowCol["column_name"]);
                    col.DBType = CoreUtils.Data.ParseIt<string>(rowCol["data_type"]);
                    col.MappedDataType = GetDBTypeMap_ByDataBaseType(col.DBType);
                    col.MaxLength = CoreUtils.Data.ParseIt<int>(rowCol["CHARACTER_MAXIMUM_LENGTH"]);
                    col.IsNullable = (CoreUtils.Data.ParseIt<string>(rowCol["IS_NULLABLE"]) == "YES") ? true : false;
                    col.DefaultValue = CoreUtils.Data.ParseIt<string>(rowCol["COLUMN_DEFAULT"]);
                    col.Precision = CoreUtils.Data.ParseIt<int>(rowCol["NUMERIC_PRECISION"]);
                    col.IsPrimaryKey = CoreUtils.Data.ParseIt<bool>(rowCol["Is_PrimaryKey"]);
                    col.IsIdentity = CoreUtils.Data.ParseIt<bool>(rowCol["Is_Identity"]);
                    col.Ordinal = CoreUtils.Data.ParseIt<int>(rowCol["Ordinal_Position"]);

                    if (col.Name == "Id")
                    {
                        table.Id = col;
                    }

                    if (col.IsPrimaryKey)
                    {
                        table.AddColumnPK(col);
                    }

                    var colMapping = (from i in mappingsList
                                      where i.TableName == table.Name
                                        && i.ColumnName == col.Name
                                        && i.MapType == "Column"
                                      select i).FirstOrDefault();
                    if (colMapping != null && colMapping.FormMeta.Length > 0)
                    {
                        try
                        {
                            col.Mapping = JsonSerializer.Deserialize<DBORMColumnMap>(colMapping.FormMeta);
                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine(exp.ToString());
                            throw new Exception("Unable to parse column mapping for " + table.Name + "." + col.Name);
                        }
                    }
                    else
                    {
                        //create default column mapping
                        if (col.Name == "DateCreated")
                        {
                            //for datecreated fields mark as readonly
                            col.Mapping = new DBORMColumnMap();
                            col.Mapping.ForDBTable = table;
                            col.Mapping.DisplayName = "Created";
                            col.Mapping.ForTableName = table.Name;
                            col.Mapping.InputType = "label";
                            col.Mapping.ReadOnly = true;
                        }
                        else if (col.Name == "DateModified")
                        {
                            col.Mapping = new DBORMColumnMap();
                            col.Mapping.ForDBTable = table;
                            col.Mapping.DisplayName = "Modified";
                            col.Mapping.ForTableName = table.Name;
                            col.Mapping.InputType = "label";
                            col.Mapping.ReadOnly = true;
                        }
                    }

                    table.AddColumn(col);
                }

                //validate table
                if (table.PrimaryKeys.Count == 0)
                {
                    throw new Exception("table " + table.Name + " missing primary keys.");
                }

                var tableMapping = (from i in mappingsList
                                    where i.TableName == table.Name
                                    && i.MapType == "Table"
                                    select i).FirstOrDefault();

                if (tableMapping != null && tableMapping.FormMeta.Length > 0)
                {
                    try
                    {
                        table.Mapping = JsonSerializer.Deserialize<DBORMTableMap>(tableMapping.FormMeta);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine(exp.ToString());
                        throw new Exception("Unable to parse table mapping for " + table.Name);
                    }
                }

                database.Tables.Add(table);
            }

            //map foreign keys
            foreach (System.Data.DataRow row in dtForeignKeys.Rows)
            {
                string foreign_key_name = (string)row["foreign_key_name"];
                string table_name = (string)row["table_name"];
                string column_name = (string)row["column_name"];
                string foreign_table_name = (string)row["foreign_table_name"];
                string foreign_column_name = (string)row["foreign_column_name"];

                //find the table
                var table = database.Tables.Where(i => i.Name == table_name).FirstOrDefault();
                if (table != null)
                {
                    var column = table.Columns.Where(i => i.Name == column_name).FirstOrDefault();
                    if (column != null)
                    {
                        //find the foreign table
                        var ftable = database.Tables.Where(i => i.Name == foreign_table_name).FirstOrDefault();
                        if (ftable != null)
                        {
                            var fcolumn = ftable.Columns.Where(i => i.Name == foreign_column_name).FirstOrDefault();
                            if (fcolumn != null)
                            {

                                //add foreign key to table
                                var fk = new DBForeignKey()
                                {
                                    Table = table,
                                    Column = column,
                                    ForeignColumn = fcolumn,
                                    Name = foreign_key_name,
                                    ForeignTable = ftable
                                };

                                table.Foreign_Keys.Add(fk);

                                if (fk.ForeignTable.Name == "EnumsList")
                                {
                                    fk.Column.Mapping = new DBORMColumnMap()
                                    {
                                        DisplayName = ORMFunctions.ColumnNameToLabel(fk.Column.Name),
                                        Query = "SELECT Value=Id, Text=DataText FROM EnumsList where DataGroup='" + fk.Column.Name + "' order by DataText",
                                        InputType = "select",
                                        Required = false,
                                        Visible = true,
                                    };
                                }
                                else
                                {
                                    //if set the default query for the mapping is there is one
                                    if (fk.Column.Mapping == null && fk.ForeignTable.Mapping != null)
                                    {
                                        fk.Column.Mapping = new DBORMColumnMap()
                                        {
                                            DisplayName = ORMFunctions.ColumnNameToLabel(fk.Column.Name),
                                            Query = fk.ForeignTable.Mapping.QueryForeignKey,
                                            InputType = "select",
                                            Required = false,
                                            Visible = true,
                                        };
                                    }
                                }

                            }
                        }
                    }
                }
            }

            //discovery one to many mappings, Rules:
            //TableName + And + TableName
            //2 foreign keys, belongs to first TableName
            foreach (var linkerTable in database.Tables)
            {
                if (linkerTable.Name.Contains("And"))
                {
                    if (linkerTable.Foreign_Keys.Count == 2)
                    {
                        string[] tableNames = linkerTable.Name.Split("And");
                        string oneTableName = tableNames[0];
                        string manyTableName = tableNames[1];

                        var oneTable = database.Tables.Where(i => i.Name == oneTableName).FirstOrDefault();
                        var manyTable = database.Tables.Where(i => i.Name == manyTableName).FirstOrDefault();

                        if (oneTable != null && manyTable != null && oneTable != manyTable)
                        {
                            if (oneTable.PrimaryKeys.Count == 1 && manyTable.PrimaryKeys.Count == 1)
                            {

                                var linkerMap = (from i in mappingsList
                                                 where i.TableName == linkerTable.Name
                                                 && i.MapType == "OneToMany"
                                                 select i).FirstOrDefault();

                                if (linkerMap != null && linkerMap.FormMeta.Length > 0)
                                {
                                    try
                                    {
                                        linkerTable.OneToManyMapping = JsonSerializer.Deserialize<DBORMColumnMap>(linkerMap.FormMeta);
                                    }
                                    catch (Exception exp)
                                    {
                                        Console.WriteLine(exp.ToString());
                                        throw new Exception("Unable to parse ForeignKey mapping for " + linkerTable.Name);
                                    }
                                }

                                //add to one to many list
                                oneTable.OneToManyList.Add(new DBOneToMany()
                                {
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
            foreach (var table in database.Tables)
            {
                if (table.Name.Contains("For"))
                {

                    var manyTable = table;
                    string[] tableNames = table.Name.Split("For");

                    string oneTableName = tableNames[1];
                    var oneTable = database.Tables.Where(i => i.Name == oneTableName).FirstOrDefault();
                    if (oneTable != null)
                    {
                        DBForeignKey oneFK = null;
                        DBForeignKey manyFK = null;

                        if (oneTable.Foreign_Keys != null)
                        {
                            foreach (var fk in oneTable.Foreign_Keys)
                            {
                                if (fk.ForeignTable == manyTable)
                                {
                                    oneFK = fk;
                                }
                            }
                        }

                        if (manyTable.Foreign_Keys != null)
                        {
                            foreach (var fk in manyTable.Foreign_Keys)
                            {
                                if (fk.ForeignTable == oneTable)
                                {
                                    manyFK = fk;
                                }
                            }
                        }

                        if (oneTable != null && manyTable != null && oneTable != manyTable)
                        {
                            if (oneTable.PrimaryKeys.Count == 1 && manyTable.PrimaryKeys.Count == 1)
                            {

                                var manyMap = (from i in mappingsList
                                               where i.TableName == manyTable.Name
                                               && i.MapType == "ManyForOne"
                                               select i).FirstOrDefault();

                                if (manyMap != null && manyMap.FormMeta.Length > 0)
                                {
                                    try
                                    {
                                        manyTable.ManyForOneMapping = JsonSerializer.Deserialize<DBORMColumnMap>(manyMap.FormMeta);
                                    }
                                    catch
                                    {
                                        throw new Exception("Unable to parse ForeignKey mapping for " + manyTable.Name);
                                    }
                                }

                                //add to one to many list
                                oneTable.ManyForOneList.Add(new DBManyForOne()
                                {
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
            foreach (var table in database.Tables)
            {
                if (table.ManyForOneList != null)
                {
                    foreach (var one in table.ManyForOneList)
                    {
                        //string sql = $"select * from BuildsForStoreAssets where StoreAssetId={dbmodel.Id}";

                        string valueField = "Id";
                        string textField = "Id";
                        var nameCol = one.ManyTable.GetColumn("Name");

                        if (nameCol != null)
                        {
                            textField = nameCol.Name;
                        }
                        else
                        {
                            var descCol = one.ManyTable.GetColumn("Description");
                            if (descCol != null)
                            {
                                textField = descCol.Name;
                            }
                        }

                        one.SQL_Many = $"select Value={valueField}, Text={textField} from " + one.ManyTable.Name + " where " + one.ManyFK.Column.Name + " = {dbmodel.Id} order by " + $"{textField}";

                        //if a column mapping does not exists create a default one
                        if (one.OneFK != null && one.OneFK.Column.Mapping == null)
                        {
                            one.OneFK.Column.Mapping = new DBORMColumnMap()
                            {
                                Visible = true,
                                Required = false,
                                InputType = "select"
                            };
                        }
                    }
                }
            }

            //generate sql for LinkerTable (OneToMany)
            foreach (var table in database.Tables)
            {
                foreach (var oneToMany in table.OneToManyList)
                {
                    string paramsList = "";
                    DBForeignKey fkOneColumn = null;
                    DBForeignKey fkManyColumn = null;
                    //there are only two forieng keys
                    foreach (var fk in oneToMany.LinkerTable.Foreign_Keys)
                    {
                        if (oneToMany.OneTable.Name == fk.ForeignTable.Name)
                        {
                            fkOneColumn = fk;
                            //get the column name of the oneToMany.Table
                            if (paramsList.Length > 0)
                            {
                                paramsList = paramsList + ",";
                            }
                            paramsList = fk.Column.MappedDataType.CodeType + " " + fk.Column.Name;
                        }
                        else
                        {
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
            foreach (System.Data.DataRow row in dtViews.Rows)
            {
                DBTable table = new DBTable();
                table.Name = (string)row["table_name"];
                table.Schema = (string)row["table_schema"];
                //get all columns
                var columns = (from i in dtColumns.AsEnumerable()
                               where i.Field<string>("table_name") == table.Name
                               select i);
                foreach (System.Data.DataRow rowCol in columns)
                {
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
            foreach (System.Data.DataRow row in dtProcedures.Rows)
            {
                DBProcedure proc = new DBProcedure();
                proc.Name = (string)row["procedure_name"];
                proc.Schema = (string)row["specific_schema"];

                string dbtype = CoreUtils.Data.ParseIt<string>(row["data_type"]);
                if (dbtype.Length > 0)
                {
                    proc.MappedDataType = GetDBTypeMap_ByDataBaseType(dbtype);
                }

                proc.Type = CoreUtils.Data.ParseIt<string>(row["routine_type"]);

                if (proc.Name != "GetSchema")
                {
                    //get all columns
                    var columns = (from i in dtParamaters.AsEnumerable()
                                   where i.Field<string>("procedure_name") == proc.Name
                                   orderby i.Field<int>("Param_Order")
                                   select i);
                    int paramIndex = 0;
                    foreach (System.Data.DataRow rowParam in columns)
                    {
                        paramIndex++;
                        DBParamater paramater = new DBParamater();
                        paramater.Name = CoreUtils.Data.ParseIt<string>(rowParam["paramater_name"]).Replace("@", "");
                        paramater.DBType = CoreUtils.Data.ParseIt<string>(rowParam["data_type"]);
                        paramater.MappedDataType = GetDBTypeMap_ByDataBaseType(paramater.DBType);
                        paramater.MaxLength = CoreUtils.Data.ParseIt<int>(rowParam["CHARACTER_MAXIMUM_LENGTH"]);
                        paramater.Precision = CoreUtils.Data.ParseIt<int>(rowParam["NUMERIC_PRECISION"]);
                        paramater.IsOutPut = CoreUtils.Data.ParseIt<bool>(rowParam["Is_OutPut"]);
                        paramater.Index = CoreUtils.Data.ParseIt<int>(rowParam["Param_Order"]);

                        //IsNullable, DefaultValue does not pull back correctly in sql server
                        paramater.DefaultValue = CoreUtils.Data.ParseIt<string>(rowParam["default_value"]);
                        paramater.IsNullable = CoreUtils.Data.ParseIt<bool>(rowParam["is_nullable"]);
                        proc.Paramaters.Add(paramater);
                        proc.Text = (from i in dtProceduresText.AsEnumerable() where i.Field<string>("name") == proc.Name select i.Field<string>("definition")).FirstOrDefault();
                        proc.Text = CoreUtils.Data.ParseIt<string>(proc.Text);
                    }
                    database.Procedures.Add(proc);
                }
            }

            //this gets the stored procedure text and parses the paramaters
            foreach (DBProcedure proc in database.Procedures)
            {
                ProcessParam(proc);
            }

            //generate SQL Statements for each table
            foreach (DBTable table in database.Tables)
            {
                table.SQL_ScopeId = " select {param_identity}=SCOPE_IDENTITY();";
                table.SQL_Select = " select {select_fields} from {tablename} where {params_where}";
                table.SQL_Insert = " insert into {tablename} ({insert_columns_list})VALUES({insert_params_list})";
                table.SQL_Update = " update {tablename} set {update_columns_params} where {params_where}";
                table.SQL_Delete = " delete from {tablename} where {params_where}";

                string tablename = string.Empty;
                string param_identity = string.Empty;
                string insert_columns_list = string.Empty;
                string insert_params_list = string.Empty;
                string update_columns_params = string.Empty;
                string params_where = string.Empty;
                string select_fields = string.Empty;

                tablename = table.Name;
                if (table.Id != null)
                {
                    param_identity = "@" + table.Id.Name;
                }

                foreach (var col in table.Columns)
                {
                    if (select_fields.Length > 0)
                    {
                        select_fields = select_fields + ",";
                    }
                    select_fields += "[" + col.Name + "]";

                    if (col.MappedDataType.DatabaseType == "timestamp")
                    {
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
                        insert_params_list += "@" + col.Name;
                        if (update_columns_params.Length > 0)
                        {
                            update_columns_params = update_columns_params + ",";
                        }
                        update_columns_params += col.Name + "=@" + col.Name;
                    }

                    if (col.IsPrimaryKey)
                    {
                        if (params_where.Length > 0)
                        {
                            params_where = params_where + ",";
                        }
                        params_where += col.Name + "=@" + col.Name;
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
        public void ProcessParam(DBProcedure proc)
        {
            if (proc.Name == "GetAppsConfigs")
            {
                Console.WriteLine("DEBUG:" + proc.Name);
            }

            string definition = proc.Text.ToLower();
            definition = definition.Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
            definition = CoreUtils.Data.ReplaceAll(definition, "  ", " "); //remove all double spaces

            int poscreate = definition.IndexOf("("); //find first @
            int posAs = 0;

            if (proc.Type.ToLower() == "procedure")
            {
                posAs = definition.IndexOf(" as");
            }
            else
            {
                posAs = definition.IndexOf(" returns");
            }

            if (posAs > poscreate && posAs > 0 && poscreate > 0)
            {
                //(@skip int = 0,@take int = 1000,@query varchar(100) = '',@sortfield varchar(100) = '',@sortdir varchar(100) = '')
                string paramdefinition = definition.Substring(poscreate, posAs - poscreate);

                if (!paramdefinition.StartsWith("(") && !paramdefinition.EndsWith(")"))
                {
                    throw new Exception("Params defined in stored procedure must start with ( and end with )");
                }

                paramdefinition = CoreUtils.Data.RemoveFirstOf(paramdefinition, "(");
                paramdefinition = CoreUtils.Data.RemoveLastOf(paramdefinition, ")");

                proc.DefParams = paramdefinition;

                List<dynamic> positions = new List<dynamic>();
                Action<DBParamater> getDef = (DBParamater x) => {
                    try
                    {
                        x.DefParam = proc.DefParams.Substring(x.DefPosStart, x.DefPosEnd - x.DefPosStart);
                        x.DefParam = x.DefParam.Trim();
                        x.DefParam = CoreUtils.Data.RemoveLastOf(x.DefParam, ",").Trim();
                    }
                    catch
                    {
                        throw new Exception("Failed to parse paramter " + x.Name + " for procedure " + proc.Name);
                    }
                };

                for (int j = 0; j < proc.Paramaters.Count; j++)
                {
                    DBParamater param = proc.Paramaters[j];
                    DBParamater paramNext = null;

                    if (j + 1 < proc.Paramaters.Count)
                    {
                        paramNext = proc.Paramaters[j + 1];
                    }
                    param.DefPosStart = paramdefinition.IndexOf("@" + param.Name.ToLower());
                    if (paramNext != null)
                    {
                        param.DefPosEnd = paramdefinition.IndexOf("@" + paramNext.Name.ToLower());
                    }
                    else
                    {
                        param.DefPosEnd = proc.DefParams.Length;
                    }
                    getDef.Invoke(param);
                }

                for (int j = 0; j < proc.Paramaters.Count; j++)
                {
                    #region

                    var param = proc.Paramaters[j];
                    param.IsNullable = false;

                    //@userid int = null output)
                    if (param.DefParam.Contains("="))
                    {
                        string[] paradefs = param.DefParam.Trim().Split('=');
                        string paramNamePart = paradefs[0].Trim(); //@userid int
                        string paramValuePart = paradefs[1].Trim(); //null output

                        param.DefaultValue = paramValuePart;
                        param.IsNullable = true;
                        param.HasDefaultValue = true;

                        if (paramValuePart.EndsWith(" output"))
                        {
                            param.IsOutPut = true;
                            paramValuePart = CoreUtils.Data.RemoveLastOf(paramValuePart, " output");
                        }

                        if (paramValuePart.EndsWith("null"))
                        {
                            param.DefaultValue = "null";
                        }
                        else
                        {
                            if (paramValuePart.StartsWith("'") && paramValuePart.EndsWith("'"))
                            {
                                param.DefaultValue = paramValuePart;
                                param.DefaultValue = CoreUtils.Data.RemoveLastOf(param.DefaultValue, "'");
                                param.DefaultValue = CoreUtils.Data.RemoveFirstOf(param.DefaultValue, "'");
                                param.DefaultValue = "\"" + param.DefaultValue + "\"";
                            }
                        }
                    }
                    if (param.DefParam.EndsWith(" output"))
                    {
                        param.IsOutPut = true;
                    }

                    #endregion
                }
            }
        }
    }
}
