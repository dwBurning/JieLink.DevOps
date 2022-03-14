using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.Models
{
    /// <summary>
    /// G3主表卡数据
    /// </summary>
    public class TBaseMcCardInfoModel
    {
        public int ID { get; set; }

        public int SID { get; set; }

        public string IDNO { get; set; }

        public string ICNO { get; set; }

        public int Status { get; set; }

        public int PersonSID { get; set; }

        public int PersonID { get; set; }

        public decimal Deposit { get; set; }

        /// <summary>
        /// 操作员ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OptDate { get; set; }

        /// <summary>
        /// 发行时间
        /// </summary>
        public DateTime IssueDate { get; set; }

        public string IssueOptons { get; set; }

        public string Remark { get; set; }

        public string ServerNo { get; set; }

        public string ServerDown { get; set; }

        public int Ver { get; set; }

        public int Isplan { get; set; }

        public int MediaType { get; set; }

        public int PhysicalCardType { get; set; }

        public string VMAutoQuitDeviceName { get; set; }

        public string FaceCardNO { get; set; }

        public string GUID { get; set; }
    }

    public enum EnumMcCardInfoStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        normal = 1,

        /// <summary>
        /// 挂失
        /// </summary>
        reportLoss = 2,

        /// <summary>
        /// 注销
        /// </summary>
        cancellation = 3
    }

}
