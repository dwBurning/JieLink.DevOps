using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    public class BackUpJobConfig
    {
        public string Id { get; set; }

        public string DataBaseName { get; set; }

        public string Cron { get; set; }

        /// <summary>
        /// 0 全库 1 业务表
        /// </summary>
        public int BackUpType { get; set; }
    }
}
