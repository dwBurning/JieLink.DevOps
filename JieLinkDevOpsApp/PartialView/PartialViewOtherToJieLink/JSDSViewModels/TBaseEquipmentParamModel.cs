using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    public class TBaseEquipmentParamModel
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 父设备ID
        /// </summary>
        public string EQUIPMENT_ID { get; set; }
        /// <summary>
        /// 参数类型：如 MAC DevID gateway MASK ip
        /// </summary>
        public string PARAM_CODE { get; set; }
        /// <summary>
        /// 参数类型对应的值
        /// </summary>
        public string PARAM_VALUE { get; set; }

        public string PARAM_NAME { get; set; }
        public string PARAM_TYPE { get; set; }
        public string PARAM_DESC { get; set; }
        public string DEF_VAL { get; set; }
        public string GROUP_ID { get; set; }
        public string REMARK { get; set; }

    }
}
