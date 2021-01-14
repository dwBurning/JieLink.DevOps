using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    public class TBaseEquipmentModel
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
        /// 设备名称
        /// </summary>
        public string EQUIP_NAME { get; set; }
        /// <summary>
        /// 设备类型：通过这个筛选出jielink可迁移的设备
        /// </summary>
        public string PRODUCT_MODEL { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string EQUIP_STATE { get; set; }
        /// <summary>
        /// EQUIP_STATE ？
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string REG_DATE { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CREATE_TIME { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UPDATE_TIME { get; set; }
        /// <summary>
        /// 数据来源：备注
        /// </summary>
        public string DATA_ORIGIN { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string EQUIP_TYPE { get; set; }
        public string EQUIP_PRODUCT { get; set; }
        public string PROVIDER { get; set; }
        public string EQUIP_DESC { get; set; }
        public string MACHINE_NO { get; set; }
        public string EQUIP_VERSION { get; set; }
        public string EQUIPMENT_CODE { get; set; }
        public string CLIENT_IP { get; set; }
        public string SUPER_VOUCHER_TYPE { get; set; }
        public string PROTOCOL { get; set; }
        public string FROM_ID { get; set; }
        public string REMARK { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
    }
}
