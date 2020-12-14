using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    class TParkRecordOutModel
    {
        /// <summary>
        /// 出场记录guid：32位
        /// </summary>
        public string ID { get; set; }
        public string CARD_NO { get; set; }
        /// <summary>
        /// 凭证编号
        /// </summary>
        public string PHYSICAL_NO { get; set; }
        /// <summary>
        /// 入场时间
        /// </summary>
        public string IN_TIME { get; set; }
        /// <summary>
        /// 出场时间
        /// </summary>
        public string OUT_TIME { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CAR_NO { get; set; }
        /// <summary>
        /// 凭证guid：赋值说明是月租套餐
        /// </summary>
        public string VOUCHER_ID { get; set; }
        /// <summary>
        /// 服务guid：：赋值说明是月租套餐
        /// </summary>
        public string AUTHSERVICE_ID { get; set; }
        /// <summary>
        /// 套餐编号
        /// </summary>
        public string CARD_TYPE { get; set; }
        /// <summary>
        /// 入场记录guid
        /// </summary>
        public string RECORDIN_ID { get; set; }
        /// <summary>
        /// PHYSICAL_NO
        /// </summary>
        public string VOUCHER_NO { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string RECORD_TYPE { get; set; }
        /// <summary>
        /// 入出口类型：GREAT_OUT
        /// </summary>
        public string OUT_TYPE { get; set; }
        /// <summary>
        /// 车场guid：忽略，默认00000000-0000-0000-0000-000000000000
        /// </summary>
        public string PARK_DISTRICT_ID { get; set; }
        /// <summary>
        /// 操作员guid：默认9999
        /// </summary>
        public string USER_ID { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public string CREATE_TIME { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UPDATE_TIME { get; set; }
        /// <summary>
        /// 备注：空时赋值JSDS
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 最后支付时间
        /// </summary>
        public string LAST_CHARGE_TIME { get; set; }
        /// <summary>
        /// 缴费次数：忽略
        /// </summary>
        public string TOTAL_CHARGE_QTY { get; set; }
        /// <summary>
        /// 全程费用
        /// </summary>
        public decimal TOTAL_CHARGE_AVAILABLE { get; set; }
        /// <summary>
        /// 总优惠费用
        /// </summary>
        public decimal TOTAL_CHARGE_ABATE { get; set; }
        /// <summary>
        /// 总减免费用
        /// </summary>
        public decimal TOTAL_CHARGE_REFUND { get; set; }
        /// <summary>
        /// 总实收金额
        /// </summary>
        public decimal TOTAL_CHARGE_ACTUAL { get; set; }
        /// <summary>
        /// 车身颜色
        /// </summary>
        public string CAR_BODY_COLOR { get; set; }
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public string CAR_LP_COLOR { get; set; }
        /// <summary>
        /// 车标、车型
        /// </summary>
        public string CAR_BRAND { get; set; }
        /// <summary>
        /// 图片路径：迁移过去也没有用，图片显示不出来
        /// </summary>
        public string PicPath { get; set; }


        public string CSTDATA { get; set; }
        public string CSTFLAG { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
        public string DATA_ORIGIN { get; set; }
        public string SEAT_OPEN_GATE { get; set; }
        public string MACHINE_NO { get; set; }
        public string ESTABLISH_NAME { get; set; }
        public string ESTABLISH_ID { get; set; }
        public string EQUIPMENT_ID { get; set; }
        public string SERVICESET_NAME { get; set; }
        public string SERVICESET_ID { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string USER_CODE { get; set; }

        public string MANUAL_OPEN { get; set; }
        public string OUT_ORIG_DATA { get; set; }
    }
}
