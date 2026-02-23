using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using CoreUtils;

namespace CoreORM
{
    /**
    *   Model to pass to razor view
    */
    public class ViewsModel
    {
        public DBDatabase Schema { get; set; }
        public ORMView CurrentView { get; set; }
        public IDatabase DB { get; set; }
    }

    public interface IDBMapper
    {
        Task<DBDatabase> GetMapping(string dbName, string codeNameSpace, string connectionstring, List<DBORMMappings> mappingsList);
    }

    public class DBDatabase
    {
        public string Name = string.Empty;
        public string CodeNameSpace = string.Empty;
        public List<DBTable> Tables = new List<DBTable>();
        public List<DBTable> Views = new List<DBTable>();
        public List<DBProcedure> Procedures = new List<DBProcedure>();
    }

    public class DBTable
    {
        public string Name = string.Empty;
        public string MappedName = string.Empty;
        public DBColumn Id { get; set; }
        public List<DBColumn> Columns = new List<DBColumn>();
        public List<DBColumn> PrimaryKeys = new List<DBColumn>();
        public string SQL_ScopeId = string.Empty;
        public string SQL_Insert = string.Empty;
        public string SQL_Update = string.Empty;
        public string SQL_Select = string.Empty;
        public string SQL_Delete = string.Empty;
        public string Schema = "dbo";
        public List<DBForeignKey> Foreign_Keys = new List<DBForeignKey>();
        public DBDatabase Database { get; set; }

        /// <summary>
        /// one model mapped to multiple models through a OneToMany table
        /// example: AuthUsersAndAuthRoles
        /// </summary>
        public List<DBOneToMany> OneToManyList = new List<DBOneToMany>();

        public List<DBManyForOne> ManyForOneList = new List<DBManyForOne>();


        public DBORMTableMap Mapping { get; set; }

        //when a table is used in a OneToMany
        public DBORMColumnMap OneToManyMapping { get; set; }

        public DBORMColumnMap ManyForOneMapping { get; set; }

        public DBColumn GetColumn(string name)
        {
            return Columns.Where(i => i.Name == name).FirstOrDefault();
        }

        public DBColumn GetColumnPK(string name)
        {
            return PrimaryKeys.Where(i => i.Name == name).FirstOrDefault();
        }

        public void AddColumn(DBColumn col)
        {
            var colCheck = GetColumn(col.Name);
            if (colCheck == null)
            {
                this.Columns.Add(col);
            }
            else
            {
                throw new Exception("Duplicate Column " + col.Name);
            }
        }

        public void AddColumnPK(DBColumn col)
        {
            var colCheck = GetColumnPK(col.Name);
            if (colCheck == null)
            {
                this.PrimaryKeys.Add(col);
            }
            else
            {
                throw new Exception("Duplicate Column " + col.Name);
            }
        }

    }

    public class DBColumn
    {
        public string Name = string.Empty;
        public string MappedName = string.Empty;
        public string DBType = string.Empty;
        public DBTypeMap MappedDataType = null;
        public bool IsNullable = false;
        public string DefaultValue = string.Empty;
        public long MaxLength = 0;
        public int Precision = 0;
        public bool IsPrimaryKey = false;
        public bool IsIdentity = false;
        public int Ordinal = 0;
        public int InputTypeRows = 3;

        public DBORMColumnMap Mapping { get; set; }

        public bool Mapping_IsVisible
        {
            get
            {
                if (Mapping != null)
                {
                    return Mapping.Visible;
                }
                return true;
            }
        }

        public bool Mapping_Required
        {
            get
            {
                if (Mapping != null)
                {
                    return Mapping.Required;
                }
                return false;
            }
        }


        public string Mapping_Query
        {
            get
            {
                if (Mapping != null)
                {
                    return Mapping.Query;
                }
                return "";
            }
        }

        public string Mapping_InputType
        {
            get
            {
                if (Mapping != null)
                {
                    return Mapping.InputType;
                }
                return "input";
            }
        }

    }

    public class DBProcedure
    {
        public string Name = string.Empty;
        public string Text = string.Empty;
        public string Type = string.Empty;
        public List<DBParamater> Paramaters = new List<DBParamater>();

        /**
        * return type for the stored proc
        */
        public DBTypeMap MappedDataType = null;
        public List<DBColumn> ReturnFields = new List<DBColumn>();

        public int Oid = 0;

        public string Schema = "dbo";

        public bool IsScalar { get; set; }

        public string DefParams = string.Empty;

    }

    public class DBParamater
    {
        public string Name = string.Empty;
        public string DBType = string.Empty;
        public DBTypeMap MappedDataType = null;
        public bool IsNullable = false;
        public bool HasDefaultValue = false;
        public string DefaultValue = string.Empty;
        public bool IsOutPut = false;
        public int MaxLength = 0;
        public int Precision = 0;
        public int Index = 0;

        public int DefPosStart = 0;
        public int DefPosEnd = 0;
        public string DefParam = string.Empty;
    }

    public class DBTypeMappings : List<DBTypeMap>
    {

    }
    public class DBTypeMap
    {
        public string DatabaseType = string.Empty;
        public string CodeType = string.Empty;
        public string CodeDBType = string.Empty;
        public bool CodeDBTypeIsNullable = false;
        public string CodeTypeDefaultValue = string.Empty;
        public string JSCodeType = string.Empty;
        public string JSCodeTypeDefaultValue = string.Empty;
    }

    public class DBForeignKey
    {
        public string Name { get; set; }
        public DBTable Table { get; set; }

        public DBTable ForeignTable { get; set; }
        public DBColumn ForeignColumn { get; set; }
        public DBColumn Column { get; set; }

        public DBORMColumnMap Mapping { get; set; }
    }

    /// <summary>
    /// BuildsForStoreAssets
    /// </summary>
    public class DBManyForOne
    {

        public DBForeignKey OneFK { get; set; }
        public DBForeignKey ManyFK { get; set; }

        public DBTable OneTable { get; set; }


        public DBTable ManyTable { get; set; }

        /*SELECT * from BuildsForStoreAssets where StoreAsetId = @id*/
        public string SQL_Many = "";
    }

    public class DBOneToMany
    {
        /// <summary>
        /// Linker Table: AuthUsersAndAuthRoles
        /// </summary>
        public DBTable LinkerTable { get; set; }

        /// <summary>
        /// EnumsLists
        /// </summary>
        public DBTable OneTable { get; set; }

        /// <summary>
        /// Many Table: AuthRoles
        /// </summary>
        public DBTable ManyTable { get; set; }

        /*SELECT AuthRoles.*
from AuthRoles AuthRoles
join AuthUsersAndAuthRoles AuthUsersAndAuthRoles
    on AuthUsersAndAuthRoles.RoleId = AuthRoles.Id
where AuthUsersAndAuthRoles.UserId= @userid*/
        public string SQL_Many = "";
        public string SQL_Linker = "";
    }

    public class DBORMMappings
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string FormMeta { get; set; }
        public string CodeMappedName { get; set; }
        public string MapType { get; set; }
    }

    public class DBORMDataValue
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
    }

    public class DBORMTableMap
    {
        DBORMTableMap()
        {
            DisplayColumns = new string[] { };
            DisplayNameSingle = string.Empty;
            DisplayNameMany = string.Empty;
            DisplayColumnText = string.Empty;
            DisplayColumnValue = string.Empty;
        }

        public string DisplayNameSingle { get; set; }
        public string DisplayNameMany { get; set; }
        public string DisplayColumnText { get; set; }
        public string DisplayColumnValue { get; set; }
        public string QueryForeignKey { get; set; }
        public string QueryList { get; set; }
        public string[] DisplayColumns { get; set; }

    }

    public class DBORMColumnMap
    {
        //{ Required: true, DisplayText: "Time Zone",  Query : "select Id=Id, Text=Name from TimeZones order by Name" }

        public DBORMColumnMap()
        {
            this.Required = false;
            this.DisplayName = "";
            this.Query = "";
            this.Visible = true;
            this.InputType = "input";
            this.ReadOnly = false;
            this.InputTypeRows = 3;
        }

        public bool Required { get; set; }
        public string DisplayName { get; set; }
        public string Query { get; set; }
        public bool Visible { get; set; }
        public bool ReadOnly { get; set; }

        public string InputType { get; set; }
        public int InputTypeRows { get; set; }

        public int InputTypeColumns { get; set; }

        public string DefaultValue { get; set; }

        public List<DBORMDataValue> DataValues { get; set; }

        public string ForTableName { get; set; }
        public DBTable ForDBTable { get; set; }
    }

}
