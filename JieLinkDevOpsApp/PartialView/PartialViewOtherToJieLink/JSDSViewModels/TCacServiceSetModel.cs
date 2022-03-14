using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// 服务表无需关联套餐：因为JSDS的套餐与JieLink的套餐不一样，车场服务默认开通月租A+有效时间，门禁服务默认业主的门禁服务时间
    /// </summary>
    public class TCacServiceSetModel
    {
        public string ID { get; set; }
        public string BUSINESS_ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string AUTH_MODE { get; set; }
        public string EFFECTIVE_DATE { get; set; }
        public string EXPIRY_DATE { get; set; }
        public string IS_SOIID { get; set; }
        public string IS_BURSE { get; set; }
        public decimal COMMISSION_CHARGE { get; set; }
        public string STATUS { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }
        public string REMARK { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
        public string DATA_ORIGIN { get; set; }
    }
}
