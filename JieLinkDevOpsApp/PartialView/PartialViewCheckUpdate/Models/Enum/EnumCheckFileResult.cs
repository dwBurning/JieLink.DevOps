using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewCheckUpdate.Models.Enum
{
    public enum EnumCheckFileResult
    {
        /// <summary>
        /// 升级成功
        /// </summary>
        Ok = 0,

        /// <summary>
        /// 升级失败
        /// </summary>
        Faild = 1,

        /// <summary>
        /// 检测中断
        /// </summary>
        Error = 2,

        /// <summary>
        /// 提醒
        /// </summary>
        Warning = 3
    }
}
