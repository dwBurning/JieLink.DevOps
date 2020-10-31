using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJsdsOneClickUpgradeToJieLink.ViewModels
{
    public class TCacAuthServiceModel
    {
        /// <summary>
        /// 服务GUID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// TCacServiceSetModel套餐的GUID
        /// </summary>
        public string SERVICESET_ID { get; set; }
        /// <summary>
        /// 账号GUID：一个用户一个账号，一个账号多个凭证，一个账号多个服务
        /// </summary>
        public string ACCOUNT_ID { get; set; }
        /// <summary>
        /// 服务状态
        /// </summary>
        public string STATE { get; set; }
        /// <summary>
        /// 服务开始时间：
        /// 车场服务：获取年月日，填充00:00:00
        /// 门禁服务：获取年月日，时间段1填充00:00
        /// </summary>
        public string START_TIME { get; set; }
        /// <summary>
        /// 服务结束时间：
        /// 车场服务：获取年月日，填充23:59:59
        /// 门禁服务：获取年月日，时间段1填充23:59
        /// </summary>
        public string END_TIME { get; set; }
        /// <summary>
        /// 车位数：最小1
        /// </summary>
        public int PARK_SEAT_NUM { get; set; }
        /// <summary>
        /// 服务类型：PARK、DOOR
        /// </summary>
        public string BUSINESS_CODE { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CREATE_TIME { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UPDATE_TIME { get; set; }

        /// <summary>
        /// 忽略
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 忽略，使用CREATE_TIME
        /// </summary>
        public string AUTH_DATE { get; set; }
        /// <summary>
        /// 忽略：全0
        /// </summary>
        public decimal BALANCE { get; set; }
        /// <summary>
        /// 忽略：空
        /// </summary>
        public string LOCKED_TIME { get; set; }
        /// <summary>
        /// 忽略：空
        /// </summary>
        public string UNLOCK_TIME { get; set; }
        /// <summary>
        /// 绑定的通道GUID：忽略
        /// </summary>
        public string PARK_ESTABLISH_ID { get; set; }
        /// <summary>
        /// 绑定的区域GUID：忽略
        /// </summary>
        public string PARK_DISTRICT_ID { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string SERVICE_PWD { get; set; }
        /// <summary>
        /// 凭证号：忽略
        /// </summary>
        public string VEHICLE_CODES { get; set; }
        /// <summary>
        /// 凭证号：忽略
        /// </summary>
        public string PHYSICAL_NOS { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string PARK_SEAT_CODES { get; set; }
        /// <summary>
        /// 忽略：true false
        /// </summary>
        public string IS_AUTO_RECHARGE { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string VERIFY_DATA { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string FROM_ID { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string SYNC_TIME { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public int SYNC_FLAG { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public int SYNC_FAILS { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string DATA_ORIGIN { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string SELF_ID { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string MONTH_ORDER_ID { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public decimal DELAY_MONEY { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string DELAY_INFO { get; set; }
    }
    public enum BusinessCodeEnum
    {
        PARK,
        DOOR,
    }
    public enum ServiceStateEnum
    {
        CLOSE,
        NORMAL,
    }
}
