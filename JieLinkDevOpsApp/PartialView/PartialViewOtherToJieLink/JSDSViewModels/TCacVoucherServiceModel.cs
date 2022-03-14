using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// 凭证服务关系表：服务表中带用户账号，再加上服务于凭证关系，可获取到账号与凭证关系
    /// 不能通过t_cac_voucher_biz_param这个表查询，这个表是操作记录表
    /// </summary>
    public class TCacVoucherServiceModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 凭证guid
        /// </summary>
        public string VOUCHER_ID { get; set; }
        /// <summary>
        /// 服务guid
        /// </summary>
        public string AUTHSERVICE_ID { get; set; }
        /// <summary>
        /// 状态 NORMAL
        /// </summary>
        public string STATE { get; set; }
        public string IS_PRIMARY { get; set; }
        public string REMARK { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
    }
}
