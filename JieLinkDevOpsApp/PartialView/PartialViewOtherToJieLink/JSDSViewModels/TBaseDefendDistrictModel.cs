using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    public class TBaseDefendDistrictModel
    {
        /// <summary>
        /// 区域ID：根节点root
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 父区域ID
        /// </summary>
        public string DISTRICT_ID { get; set; }
        public string TYPE_CODE { get; set; }
        public string MAP_ID { get; set; }
        public int LOGIC_ID { get; set; }
        public int LEVEL { get; set; }
        /// <summary>
        /// 区域编号 如 SZ_AREA0013 EXTERIOR 001
        /// </summary>
        public string DISTRICT_CODE { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string DISTRICT_NAME { get; set; }
        /// <summary>
        /// 状态 NORMAL
        /// </summary>
        public string STATE { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }
        public string REMARK { get; set; }

        public string DATA_ORIGIN { get; set; }
        public string DISTRICT_CLASS { get; set; }
        public string SPACE_TYPE { get; set; }
        public decimal COORD_1X { get; set; }
        public decimal COORD_1Y { get; set; }
        public decimal COORD_2X { get; set; }
        public decimal COORD_2Y { get; set; }
        public string IS_REFUGE { get; set; }
        public string isControlRegion { get; set; }
        public int CarNums { get; set; }
        public int PersonNums { get; set; }
        public int PARKUNIT_COUNT { get; set; }
        public string ORG_NAME { get; set; }
        public string PERSON_NAME { get; set; }
        public string CALENDAR_ID { get; set; }
        public int RESTRICT_FLUX { get; set; }
        public string WAY_RULE { get; set; }
        public string FROM_ID { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
    }
}
