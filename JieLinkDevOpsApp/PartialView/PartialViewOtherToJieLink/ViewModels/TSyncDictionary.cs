using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class TSyncDictionary
    {
        public int ID { get; set; }
        /// <summary>
        /// 字典编号（固定）
        /// </summary>
        public string TYPE_NAME { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        public string TYPE_VALUE { get; set; }
        /// <summary>
        /// 转换后的值
        /// </summary>
        public string TRANSFER_VALUE { get; set; }
        /// <summary>
        /// 字典含义
        /// </summary>
        public string TYPE_DESC { get; set; }
        //76	022	0	ALLFREE 全免
        //77	022	1	MINUSMONEY 减免金额
        //78	022	2	MINUSHOUR 减免小时
        //79	022	3	MINPERCENT 减免百分比
        //80	022	4	BYTIME 按时间点

    }
}
