using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewSyncTool
{
    public class SyncBoxEntity
    {
        /// <summary>
        /// 同步记录ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 命令字
        /// </summary>
        public string Cmd { get; set; }

        /// <summary>
        /// 协议数据
        /// </summary>
        public string JsonData { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public int DataType { get; set; }
    }
}
