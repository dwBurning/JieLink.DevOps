using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.Models
{
    /// <summary>
    /// G3设备表
    /// </summary>
    public class TBaseMcDeviceModel
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Mac
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// 掩码
        /// </summary>
        public string Mask { get; set; }

        /// <summary>
        /// 网关
        /// </summary>
        public string GateWay { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public int DeviceTypeID { get; set; }
    }
}
