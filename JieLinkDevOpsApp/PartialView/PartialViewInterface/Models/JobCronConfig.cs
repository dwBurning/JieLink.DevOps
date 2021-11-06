using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    /// <summary>
    /// 任务cron配置
    /// 说明：为了处理数据库备份的cron配置，需要增加DatabaseName字段
    /// </summary>
    public class JobCronConfig
    {
        /// <summary>
        /// 备份类型
        /// </summary>
        public string JobTypeName { get; set; }

        /// <summary>
        /// cron表达式
        /// </summary>
        public string Cron { get; set; }

        /// <summary>
        /// 当前选中的需要备份的数据库
        /// </summary>
        public string DataBaseName { get; set; }

        public string JobIdentity { get; set; }

        public string GroupName { get; set; }
    }
}
