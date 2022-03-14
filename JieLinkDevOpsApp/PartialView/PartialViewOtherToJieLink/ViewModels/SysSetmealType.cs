using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    /// <summary>
    /// 套餐表
    /// </summary>
    public class SysSetmealType
    {
        /// <summary>
        /// 套餐ID
        /// </summary>
        public long SGUID { get; set; }
        /// <summary>
        /// 套餐GUID
        /// </summary>
        public string SetGUID { get; set; }
        public int UserType { get; set; }
        /// <summary>
        /// 套餐编号
        /// </summary>
        public int SetNO { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string SetName { get; set; }
        /// <summary>
        /// 套餐别名
        /// </summary>
        public string SetOtherName { get; set; }
        public int ParentSetNo { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Week { get; set; }
        public int SealTempId { get; set; }
        public int Status { get; set; }
        public string ExternPriod { get; set; }
        public int PlaceType { get; set; }
    }
}
