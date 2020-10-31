using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJsdsOneClickUpgradeToJieLink.ViewModels
{
    /// <summary>
    /// 人事+凭证+服务关联关系
    /// </summary>
    public class TcacVoucherBizParamModel
    {
        /// <summary>
        /// 主键GUID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 凭证GUID
        /// </summary>
        public string VOUCHER_ID { get; set; }
        /// <summary>
        /// 用户GUID：有可能为空，为空时通过t_cac_account中的ACCOUNT_ID找到PERSON_ID
        /// </summary>
        public string PERSON_ID { get; set; }
        /// <summary>
        /// 账号GUID
        /// </summary>
        public string ACCOUNT_ID { get; set; }
        /// <summary>
        /// 服务GUID
        /// </summary>
        public string AUTH_SERVICE_ID { get; set; }        

        /// <summary>
        /// 凭证编号：忽略
        /// </summary>
        public string VOUCHER_NO { get; set; }
        /// <summary>
        /// 凭证编号：忽略
        /// </summary>
        public string PHYSICAL_NO { get; set; }
        /// <summary>
        /// 凭证编号：忽略
        /// </summary>
        public string CARD_NO { get; set; }
        /// <summary>
        /// 服务类型：TCacAuthServiceModel的BUSINESS_CODE
        /// </summary>
        public string BUSINESS_CODE { get; set; }
        public string ISSUE_TIME { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string EXPIRY_DATE { get; set; }
        public string VOUCHER_TYPE { get; set; }
        public string CAR_MODEL { get; set; }
        public string RECHARGE_WAY { get; set; }
        public string AUTH_MODE { get; set; }
        public string BALANCE { get; set; }
        public string OTHER_PARAM { get; set; }
        public string CARPORT_RIGHT { get; set; }
        /// <summary>
        /// 凭证状态：直接看凭证表，忽略
        /// </summary>
        public string VOUCHER_STATE { get; set; }
        /// <summary>
        /// 账号状态：直接看账号表，忽略
        /// </summary>
        public string ACCOUNT_STATE { get; set; }
        /// <summary>
        /// 服务状态：直接看服务表，忽略
        /// </summary>
        public string AUTH_SERVICE_STATUS { get; set; }
        public string VOUCHER_OPERATE_TYPE { get; set; }
        public string CARD_MEDIUM { get; set; }
        public string VEHICLE_CODES { get; set; }
        public string REMARK { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }
        public string SYNC_TIME { get; set; }
        public string SYNC_FLAG { get; set; }
        public string SYNC_FAILS { get; set; }
    }
}
