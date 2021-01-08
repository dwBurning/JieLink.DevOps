using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// 服务+通道关系表
    /// </summary>
    public class TCacauthEstablishModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 通道ID
        /// </summary>
        public string ESTABLISH_ID { get; set; }
        /// <summary>
        /// 服务ID：车场、门禁
        /// </summary>
        public string AUTHSERVICE_ID { get; set; }
        /// <summary>
        /// 状态 NORMAL
        /// </summary>
        public string STATUS { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }

        public string IS_SPECIAL_ESTA { get; set; }
        public int IS_COMPUTE { get; set; }
        public string REMARK { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
    }
}
