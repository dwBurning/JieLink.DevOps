using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class TCacVoucherModel
    {
        /// <summary>
        /// 凭证GUID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 账号GUID：一个用户一个账号，一个账号多个凭证，一个账号多个服务
        /// </summary>
        public string ACCOUNT_ID { get; set; }
        /// <summary>
        /// 凭证类型 
        /// ECARD：MF1-A卡  MF1_A_CARD     55
        /// VIRTUAL：车牌   LICENSE_PLATE  163
        /// </summary>
        public string VOUCHER_TYPE { get; set; }
        /// <summary>
        /// 凭证信息
        /// 当车牌凭证时：对接JieLink的VoucherNo,CardNum
        /// 当卡凭证时：对接JieLink的VoucherNo,CardNum,PhysicsNum
        /// </summary>
        public string PHYSICAL_NO { get; set; }
        /// <summary>
        /// 凭证状态
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// 发行时间
        /// </summary>
        public string CREATE_TIME { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UPDATE_TIME { get; set; }

        /// <summary>
        /// 忽略
        /// </summary>
        public string VOUCHER_NO { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string CARD_NO { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string VOUCHER_PWD { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string VOUCHER_DATA { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string DEPOSIT { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string CARD_MEDIUM { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string FROM_ID { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string SEAIAL_NO { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string SYNC_TIME { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public int SYNC_FLAG { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public int SYNC_FAILS { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string DATA_ORIGIN { get; set; }
        /// <summary>
        /// 忽略，使用CREATE_TIME
        /// </summary>
        public string ISSUE_TIME { get; set; }
    }
    /// <summary>
    /// 凭证状态
    /// </summary>
    public enum VoucherStatus
    {
        /// <summary>
        /// 白卡
        /// </summary>
        BLANK = 0,
        /// <summary>
        /// 正常
        /// </summary>
        NORMAL = 1,
        /// <summary>
        /// 注销
        /// </summary>
        RETREAT = 4,
        /// <summary>
        /// 报废：对应注销4
        /// </summary>
        SCRAP = 3,
    }
}
