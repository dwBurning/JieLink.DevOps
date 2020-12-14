using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    /// <summary>
    /// 组织表t_base_organize
    /// </summary>
    public class TBaseOrganizeModel
    {
        /// <summary>
        /// 当前组织guid
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 父节点组织guid：为空表示根节点
        /// </summary>
        public string ORG_ID { get; set; }
        /// <summary>
        /// 组织code：汉字数组等均可，不能对应JieLink组织表的RGCode
        /// </summary>
        public string ORG_CODE { get; set; }
        /// <summary>
        /// 组织名称
        /// </summary>
        public string ORG_NAME { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string ORG_TYPE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string PRINCIPAL { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string STATE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string FROM_ID { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public DateTime SYNC_TIME { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public int SYNC_FLAG { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public int SYNC_FAILS { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string DATA_ORIGIN { get; set; }

    }
    /// <summary>
    /// 组织状态
    /// </summary>
    public enum OrganizeStatus
    {
        /// <summary>
        /// 删除
        /// </summary>
        deleted = 1,
        /// <summary>
        /// 正常
        /// </summary>
        normal = 0
    }
}
