using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// jsds高版本有收费信息表t_park_pay
    /// </summary>
    public class TParkPayModel
    {
        /// <summary>
        /// 收费记录guid
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 凭证编号
        /// </summary>
        public string PHYSICAL_NO { get; set; }
        public string IN_TIME { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public string PAY_TIME { get; set; }
        /// <summary>
        /// 刷卡时间：？
        /// </summary>
        public string BRUSHCARD_TIME { get; set; }
        /// <summary>
        /// 凭证ID
        /// </summary>
        public string VOUCHER_ID { get; set; }
        /// <summary>
        /// 停车时长：jielink报表时会自动计算
        /// </summary>
        public long WAIT_TIME_LONG { get; set; }
        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal AVAILABLE_BALANCE { get; set; }
        /// <summary>
        /// 优费金额
        /// </summary>
        public decimal ABATE_BALANCE { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal ACTUAL_BALANCE { get; set; }
        /// <summary>
        /// 退还金额
        /// </summary>
        public decimal REFUND_BALANCE { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PAY_TYPE { get; set; }
        /// <summary>
        /// 入场记录guid
        /// </summary>
        public string RECORDIN_ID { get; set; }
        /// <summary>
        /// 出场记录guid
        /// </summary>
        public string RECORDOUT_ID { get; set; }
        /// <summary>
        /// 正常收费、超时收费：忽略，对应jielink的一般正常记录
        /// </summary>
        public string OPERATE_TYPE { get; set; }
        /// <summary>
        /// 操作员guid：默认9999
        /// </summary>
        public string USER_ID { get; set; }
        public string VERIFY_DATA { get; set; }
        public string CAR_NO { get; set; }
        public string CENT_ORIG_DATA { get; set; }
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
        /// 全程费用
        /// </summary>
        public decimal TOTAL_CHARGE_AVAILABLE { get; set; }
        /// <summary>
        /// 总优惠费用
        /// </summary>
        public decimal HAS_CHARGE_ABATE { get; set; }
        /// <summary>
        /// 总减免费用
        /// </summary>
        public decimal HAS_CHARGE_ACTUAL { get; set; }
        /// <summary>
        /// 订单编号：直接赋值
        /// </summary>
        public string ORDER_NO { get; set; }


        public string SERVICESET_NAME { get; set; }
        public string ESTABLISH_NAME { get; set; }
        public string SYNC_TIME { get; set; }
        public int SYNC_FLAG { get; set; }
        public int SYNC_FAILS { get; set; }
        public string DATA_ORIGIN { get; set; }
        public string START_TIME { get; set; }
        public string OVER_TIME { get; set; }
        public string SIGNATORY_FLAG { get; set; }
        public string SIGNATORY_USER { get; set; }
        public string ESTABLISH_ID { get; set; }
        public string EQUIPMENT_ID { get; set; }
        public string USER_CODE { get; set; }
    }
}
