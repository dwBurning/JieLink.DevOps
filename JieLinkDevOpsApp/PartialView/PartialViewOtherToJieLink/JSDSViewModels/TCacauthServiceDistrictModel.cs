using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// 车场服务+区域关系表
    /// </summary>
    public class TCacauthServiceDistrictModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 服务ID
        /// </summary>
        public string AUTH_SERVICE_ID { get; set; }
        /// <summary>
        /// 车场区域ID
        /// </summary>
        public string PARK_DISTRICT_ID { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string PARK_DISTRICT_NAME { get; set; }
        /// <summary>
        /// 状态 NORMAL
        /// </summary>
        public string STATUS { get; set; }
        public string OPERATE_TIME { get; set; }
        public string MDF_TIME { get; set; }
        public string REMARK { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }
    }
}
