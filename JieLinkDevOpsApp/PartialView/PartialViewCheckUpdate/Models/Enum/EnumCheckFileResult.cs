using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewCheckUpdate.Models.Enum
{
    enum EnumCheckFileResult
    {
        /// <summary>
        /// 升级成功
        /// </summary>
        OK = 0,
        /// <summary>
        /// 升级失败
        /// </summary>
        FILE_ERROR1 = 1,
        /// <summary>
        /// 检测中断
        /// </summary>
        FILE_ERROR2 = -1,
        /// <summary>
        /// 程序异常
        /// </summary>
        OTHER_ERROR = -2
            
    }
}
