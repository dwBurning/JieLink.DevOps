using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class TCacAccountModel
    {
        /// <summary>
        /// 账号GUID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 用户GUID：一个用户一个账号
        /// </summary>
        public string PERSON_ID { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string ACCOUNT_NAME { get; set; }
        /// <summary>
        /// 账号状态
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CREATE_TIME { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UPDATE_TIME { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string BALANCE { get; set; }
        public string TYPE { get; set; }
        public string UNLOCK_TIME { get; set; }
        public string ACCOUNT_PWD { get; set; }
        public string CREATE_DATE { get; set; }
        public string LOCKED_TIME { get; set; }
        public string VERIFY_DATA { get; set; }
        public string FROM_ID { get; set; }
        public string REMARK { get; set; }
        public string SYNC_TIME { get; set; }
        public string SYNC_FLAG { get; set; }
        public string SYNC_FAILS { get; set; }
        public string DATA_ORIGIN { get; set; }

    }
    /// <summary>
    /// 账号状态
    /// </summary>
    public enum AccountStatusEnum
    {
        CLOSE,
        NORMAL,
    }
}
