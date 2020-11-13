using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    public class AutoStartSyncEntity
    {
        /// <summary>
        /// 自动开启标志
        /// </summary>
        public bool AutoStartFlag { get; set; }

        /// <summary>
        /// 循环时间
        /// </summary>
        public int LoopTime { get; set; }

        /// <summary>
        /// 查询天数
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// 限制条数
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 是否选择版本控件
        /// false为2.0以上版本
        /// true为2.0以下版本    
        /// </summary>
        public bool VersionCheck { get; set; }

    }
}
