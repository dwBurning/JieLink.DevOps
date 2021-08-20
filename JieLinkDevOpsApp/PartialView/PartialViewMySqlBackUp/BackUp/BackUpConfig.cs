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
        /// 说明：
        /// 1、默认值为空，在添加按业务表备份的策略时，给出提示，一定需要手动配置一次，避免jielink后续新加的需要备份的表被漏选
        /// 2、仅在添加或修改策略后，才会更新`BackUpConfig.Tables`，且将当`BackUpConfig.DbName`赋值为当前策略的数据库
        /// </summary>
        public List<Table> Tables { get; set; }

        /// <summary>
        /// 无需备份的表
        /// 说明：不在此集合中的才需备份
        /// </summary>
        public List<Table> IgnoreTables { get; set; }
    }

    public class Table
    {
        public string TableName { get; set; }
    }
}
