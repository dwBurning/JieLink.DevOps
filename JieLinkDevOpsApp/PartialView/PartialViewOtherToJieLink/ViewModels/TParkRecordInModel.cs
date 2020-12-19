using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    class TParkRecordInModel
    {
        /// <summary>
        /// 入场记录Guid：32位
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
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
        /// 车牌号
        /// </summary>
        public string CAR_NO { get; set; }
        /// <summary>
        /// 用户guid
        /// </summary>
        public string PERSON_ID { get; set; }
        /// <summary>
        /// 入场通道名称
        /// </summary>
        public string ESTABLISH_NAME { get; set; }
        /// <summary>
        /// 入场通道guid
        /// </summary>
        public string ESTABLISH_ID { get; set; }
        /// <summary>
        /// 入场设备guid
        /// </summary>
        public string EQUIPMENT_ID { get; set; }
        /// <summary>
        /// 套餐名称：与jielink的套餐不是一个概念
        /// </summary>
        public string SERVICESET_NAME { get; set; }
        /// <summary>
        /// 套餐guid：与jielink的套餐不是一个概念
        /// 转换到jielink的时候，找凭证看是否在服务有效期，若在赋值对应套餐
        /// </summary>
        public string SERVICESET_ID { get; set; }
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
        /// 是否出场标识：NO_OUT   OUT
        /// </summary>
        public string OUT_FLAG { get; set; }
        /// <summary>
        /// 入出口类型：GREAT_ENTER
        /// </summary>
        public string IN_TYPE { get; set; }
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

        /// <summary>
        /// 代扣标志：不要
        /// </summary>
        public string SIGNATORY_FLAG { get; set; }
        public string SIGNATORY_USER { get; set; }
        public string CARPLACESHARE_MSG { get; set; }
        /// <summary>
        /// 代扣模式：不要
        /// </summary>
        public string SIGNATORY_WITHHOLDMODE { get; set; }
        public string SIGNATORY_WITHHOLDLIMIT { get; set; }
        public string SIGNATORY_BIZCODE { get; set; }
        public string SIGNATORY_PLAYSOUNDMSG { get; set; }
        

        public string LOCK_FALG { get; set; }
        public string LOCK_RECORDID { get; set; }
        public string LOCKED_TIME { get; set; }
        public string UNLOCK_TIME { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
        public string DATA_ORIGIN { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string MACHINE_NO { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public string USER_CODE { get; set; }
        /// <summary>
        /// PHYSICAL_NO
        /// </summary>
        public string VOUCHER_NO { get; set; }
        public string RECORD_TYPE { get; set; }
        public string PARK_DISTRICT_ID { get; set; }
        public string MANUAL_OPEN { get; set; }
        public string IS_THIRD_PAY { get; set; }
        public string MANUAL_CAUSE { get; set; }
        public string IN_ORIG_DATA { get; set; }
        /// <summary>
        /// 缴费次数：忽略
        /// </summary>
        public string TOTAL_CHARGE_QTY { get; set; }
        public string StopChargeTime { get; set; }
        public string ServiceSetIDBAK { get; set; }
        public string ServiceSetNameBAK { get; set; }
        public string CardTypeBAK { get; set; }
        public string REVISE_FLAG { get; set; }
        public string SELF_ID { get; set; }
        public string CHARGERECORD_ID { get; set; }
        public string SEAT_OPEN_GATE { get; set; }
    }
}
