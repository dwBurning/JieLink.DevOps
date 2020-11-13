using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    public class AutoStartCorectEntity
    {
        /// <summary>
        /// 自动开启标志
        /// </summary>
        public bool AutoStartFlag { get; set; }

        /// <summary>
        /// 循环时间
        /// </summary>
        public int LoopTime { get; set; }
    }
}
