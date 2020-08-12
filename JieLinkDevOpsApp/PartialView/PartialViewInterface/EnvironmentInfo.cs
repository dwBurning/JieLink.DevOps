using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public static class EnvironmentInfo
    {
        static EnvironmentInfo()
        {
            if (DbConnEntity == null)
            {
                DbConnEntity = new DbConnEntity();
            }
        }

        public static string ProjectNo { get; set; }

        public static string RemoteAccount { get; set; }

        public static string RemotePassword { get; set; }

        public static string ContactName { get; set; }

        public static string ContactPhone { get; set; }

        /// <summary>
        /// 服务端URL
        /// </summary>
        public static string ServerUrl = GetValue("ServerUrl", "");

        /// <summary>
        /// 当前版本
        /// </summary>
        public static string CurrentVersion { get; set; }

        /// <summary>
        /// 中心数据库连接对象
        /// </summary>
        public static DbConnEntity DbConnEntity = JsonHelper.DeserializeObject<DbConnEntity>(GetValue("ConnectionString", ""));


        public static string ConnectionString
        {
            get
            {
                if (DbConnEntity == null) return "";
                return $"Data Source={DbConnEntity.Ip};port={DbConnEntity.Port};User ID={DbConnEntity.UserName};Password={DbConnEntity.Password};Initial Catalog={DbConnEntity.DbName};";
            }
        }

        public static string GetValue(string key, string value = "")
        {
            return ConfigurationManager.AppSettings[key] ?? value;
        }
    }
}
