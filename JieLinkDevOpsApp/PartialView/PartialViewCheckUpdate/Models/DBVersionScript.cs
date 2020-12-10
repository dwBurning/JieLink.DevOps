using MySql.Data.MySqlClient;
using PartialViewInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewCheckUpdate.ViewModels
{
    public class DBVersionScript
    {
        public List<Table> TableList { get; set; }

        public List<Procedure> ProcedureList { get; set; }
    }


    public class Table
    {
        public int Type { get; set; }

        public string TableName { get; set; }

        public List<Column> ColumnList { get; set; }

        public List<Index> IndexList { get; set; }
    }

    public class Procedure
    {
        public string Name { get; set; }

        public string CreateString { get; set; }

    }

    public class Index
    {
        public int NonUnique { get; set; }

        public string KeyName { get; set; }

        public int SeqInIndex { get; set; }

        public string IndexType { get; set; }

        public string ColumnName { get; set; }

    }

    public class Column
    {
        public string Field { get; set; }

        public string Type { get; set; }

        public bool IsNull { get; set; }

        public bool IsKey { get; set; }

        public string Default { get; set; }

        public string Extra { get; set; }
    }
}
