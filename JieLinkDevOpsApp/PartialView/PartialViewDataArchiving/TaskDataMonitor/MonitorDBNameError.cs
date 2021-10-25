using MySql.Data.MySqlClient;
using PartialViewInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.TaskDataMonitor
{
    /// <summary>
    /// 数据库名称错误导致无法归档的问题
    /// </summary>
    public class MonitorDBNameError : IMonitor
    {
        public void Monitor()
        {
            string sql = $"update sys_key_value_setting set ValueText='{EnvironmentInfo.DbConnEntity.DbName}' where KeyID='DataBaseSchema';";
            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
        }
    }
}
