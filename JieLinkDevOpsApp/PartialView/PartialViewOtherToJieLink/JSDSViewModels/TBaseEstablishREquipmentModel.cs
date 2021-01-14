using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// 通道+设备关系表
    /// </summary>
    public class TBaseEstablishREquipmentModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 通道ID
        /// </summary>
        public string ESTA_ID { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string EQUI_ID { get; set; }
        /// <summary>
        /// 状态 NORMAL
        /// </summary>
        public string STATE { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }

        public string IS_MASTER { get; set; }
        public string DIRECTION { get; set; }
        public string EVENT_TYPE { get; set; }
        public int DELAY { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
    }
}
