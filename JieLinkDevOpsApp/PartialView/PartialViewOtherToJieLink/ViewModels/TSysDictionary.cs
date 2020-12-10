using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJsdsOneClickUpgradeToJieLink.ViewModels
{
    public class TSysDictionary
    {
        public string ID { get; set; }
        /// <summary>
        /// 字典类型编号
        /// </summary>
        public string TYPE_CODE { get; set; }
        /// <summary>
        /// 字典类型名称
        /// </summary>
        public string TYPE_NAME { get; set; }
        /// <summary>
        /// 字典条目编号
        /// </summary>
        public string ITEM_CODE { get; set; }
        /// <summary>
        /// 字典条目名称
        /// </summary>
        public string ITEM_NAME { get; set; }
        /// <summary>
        /// 是否可修改，（系统初始化的不可改，运行过程中的可改 ）R：读  RW：读写
        /// </summary>
        public string IS_EDIT { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SEQ { get; set; }

        //ede8919065de11ea940f484d7ee96b00 PAY_TYPE    支付方式 XJ  现金
        //ede891ed65de11ea940f484d7ee96b00    PAY_TYPE 支付方式    BANKCARD 银行卡
        //ede8928365de11ea940f484d7ee96b00 PAY_TYPE    支付方式 CLOUDPAY    云支付
        //ede892f765de11ea940f484d7ee96b00    PAY_TYPE 支付方式    PREPAYCARD 预支付
    }
}
