using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// 车场套餐：辅助信息表
    /// 如储值卡、业主临时卡，可通过RECHARGE_WAY判断   RECHARGE_WAY="MONEY"按金额计费，转为JieLink的储值卡
    /// </summary>
    public class TCacServiceSetParkModel
    {
        /// <summary>
        /// 与TCacServiceSetModel的主键ID对应
        /// </summary>
        public string SERVICESET_ID { get; set; }
        public string CAR_MODEL { get; set; }
        public string CARPORT_RIGHT { get; set; }
        public string CARPORT_TYPE { get; set; }
        public string AUTH_OBJECT { get; set; }
        /// <summary>
        /// "MONEY"按金额计费，转为JieLink的储值卡：用于区分月租车和储值卡
        /// </summary>
        public string RECHARGE_WAY { get; set; }
        public string RECHARGE_UNIT { get; set; }
        public decimal RECHARGE_NUM { get; set; }
        public decimal RECHARGE_UNIT_PRICE { get; set; }
        public string REFUND_WAY { get; set; }
        public string REFUND_UNIT { get; set; }
        public decimal REFUND_NUM { get; set; }
        public decimal REFUND_UNIT_PRICE { get; set; }
        public string CUT_OFF { get; set; }
        public string IS_CARPORT_OR_DISTRICT { get; set; }
        public string IS_MONTH_TEMP_CARD { get; set; }
        public string CARD_WITH_CARRECONIZER { get; set; }
        public string REVISE_CARNO { get; set; }
        public string IS_SHARE { get; set; }
        public string SHARE_TIME_BEGIN { get; set; }
        public string SHARE_TIME_END { get; set; }
        public string WEEK_DAYS { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
    }
}
