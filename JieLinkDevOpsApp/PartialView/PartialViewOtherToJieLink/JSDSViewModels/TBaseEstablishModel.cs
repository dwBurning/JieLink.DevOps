using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    public class TBaseEstablishModel
    {
        /// <summary>
        /// 通道ID/设施ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 父通道ID
        /// </summary>
        public string ESTABLISH_ID { get; set; }
        /// <summary>
        /// 通道编号
        /// </summary>
        public string ESTAB_CODE { get; set; }
        /// <summary>
        /// 通道名称
        /// </summary>
        public string ESTAB_NAME { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string DISTRICT_ID { get; set; }
        /// <summary>
        /// 通道状态 NORMAL
        /// </summary>
        public string STATE { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }

        public string ESTATYPE_ID { get; set; }
        public string BIZSTATE_ID { get; set; }
        public string CTRLSTATE_ID { get; set; }
        public int LOGIC_ID { get; set; }
        public string ESTAB_PLACE { get; set; }
        public decimal COORD_1X { get; set; }
        public decimal COORD_1Y { get; set; }
        public string INOUT_MODE { get; set; }
        public string DIRECTION { get; set; }
        public string WAY_DISTRICT_ID { get; set; }
        public string WAY_TYPE { get; set; }
        public string ISCHARGE { get; set; }
        public long GATE_DELAY { get; set; }
        public long GATE_TIMEOUT { get; set; }
        public string GATE_PASSWORD { get; set; }
        public string THREAT_PASSWORD { get; set; }
        public string GATE_INTERVAL { get; set; }
        public string IS_EMERGY_TUNNEL { get; set; }
        public string IS_MONITOR { get; set; }
        public string REMARK { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
        public string DATA_ORIGIN { get; set; }
        public string OPEN_GATE { get; set; }
        public string CALENDAR_ID { get; set; }
    }
}
