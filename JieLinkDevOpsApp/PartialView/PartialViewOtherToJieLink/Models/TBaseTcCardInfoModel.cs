using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.Models
{
    /// <summary>
    /// 车场卡数据
    /// </summary>
    public class TBaseTcCardInfoModel
    {
        public int ID { get; set; }

        public int SID { get; set; }

        public int IssueID { get; set; }

        public int CardTypeID { get; set; }

        public string CardTypeName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal IssueMoney { get; set; }

        public decimal CardBalance { get; set; }

        public DateTime CardBalanceDate { get; set; }

        public string PWD { get; set; }

        public string CarNO { get; set; }

        public string CarStatus { get; set; }

        public string CarColor { get; set; }

        public string CarPosition { get; set; }

        public string CarPicture { get; set; }

        public string MacNOEx { get; set; }

        public string MacNo { get; set; }

        public string Down { get; set; }

        public DateTime OptDate { get; set; }

        public int FixMonth { get; set; }

        public int DeleteFlag { get; set; }
    }

    public enum EnumTcCardInfoStatus
    {
        /// <summary>
        /// 删除
        /// </summary>
        deleted = 1,
        /// <summary>
        /// 正常
        /// </summary>
        normal = 0
    }
}
