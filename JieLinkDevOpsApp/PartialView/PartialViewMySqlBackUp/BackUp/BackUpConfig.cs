using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewMySqlBackUp.BackUp
{
    public class BackUpConfig
    {
        public string SavePath { get; set; }

        public List<Table> Tables { get; set; }

        public List<Table> DefaultTables { get; set; }
    }

    public class Table
    {
        public string TableName { get; set; }
    }
}
