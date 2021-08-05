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

        public List<TablesConfig> TablesConfig { get; set; }
    }

    public class TablesConfig
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 数据库中的业务表集合
        /// 说明：仅在添加或修改策略后，才会更新`BackUpConfig.Tables`，且将当`BackUpConfig.DbName`赋值为当前策略的数据库
        /// </summary>
        public List<Table> Tables { get; set; }

        public List<Table> DefaultTables { get; set; }
    }

    public class Table
    {
        public string TableName { get; set; }
    }
}
