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
    public class TBaseJSRJCardInfoModel
    {
        public int ID { get; set; }

        public int SID { get; set; }

        public int Status { get; set; }

        public int PersonSID { get; set; }
        public int PersonId { get; set; }
        public int UserId { get; set; }

        public int CardTypeID { get; set; }

        public string CardTypeName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime LastRecordTime { get; set; }
        public DateTime OptDate { get; set; }
        public DateTime IssueDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal IssueMoney { get; set; }

        public string CarNO { get; set; }

        public string MacNo { get; set; }

        public string Down { get; set; }

        public string GUID { get; set; }

        public int FixMonth { get; set; }

        public int DeleteFlag { get; set; }
    }

    public enum EnumJSRJCardInfoStatus
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
