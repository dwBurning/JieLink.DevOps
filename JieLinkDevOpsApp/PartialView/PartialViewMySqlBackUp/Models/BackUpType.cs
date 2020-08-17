using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewMySqlBackUp.Models
{
    public enum BackUpType
    {
        /// <summary>
        /// 全库
        /// </summary>
        DataBase,

        /// <summary>
        /// 业务表
        /// </summary>
        Tables
    }
}
