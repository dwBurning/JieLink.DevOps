using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.JSDSViewModels
{
    /// <summary>
    /// 用户表 t_base_person
    /// </summary>
    public class TBasePersonModel
    {
        /// <summary>
        /// 用户guid
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 组织guid：为空用户直接关联到根组织节点下
        /// </summary>
        public string ORG_ID { get; set; }
        /// <summary>
        /// 转JieLink时统一为业主
        /// </summary>
        public string PERSONTYPE_ID { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string NAME { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string SEX { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string EMAIL { get; set; }
        /// <summary>
        /// 手机号：没有的话，虚构
        /// </summary>
        public string TEL { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string STATE { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CREATE_TIME { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }

        /// <summary>
        /// 区域guid：jsds的guid不是标准guid，因此区域不做迁移
        /// </summary>
        public string DISTRICT_ID { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string SYNC_TIME { get; set; }
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
        /// <summary>
        /// 可忽略
        /// </summary>
        public string facePicOne { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string facePicTwo { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string facePicThree { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string CARD_TYPE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string CARD_CODE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string NATION { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string BUILDING_CODE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string ACC_STATUS { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string NATIVE_PLACE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string REGISTERED_ADDRESS { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string POSTALCODE { get; set; }
        /// <summary>
        /// 年龄：可忽略
        /// </summary>
        public int AGE { get; set; }
        /// <summary>
        /// 生日：空值
        /// </summary>
        public string BIRTHDAY { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string IS_EMPLOYEE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string JOIN_DATE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string UPDATE_TIME { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string TYPE { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string LABOR_NO { get; set; }
        /// <summary>
        /// jsds用户表的自增id：对应JieLink的自增Id，查询时排序，无需转到JieLink
        /// </summary>
        public int PERSON_SN { get; set; }
        /// <summary>
        /// 可忽略
        /// </summary>
        public string FROM_ID { get; set; }
    }
    /// <summary>
    /// 性别
    /// </summary>
    public enum SexEnum
    {
        /// <summary>
        /// 男
        /// </summary>
        MALE = 1,
        /// <summary>
        /// 女
        /// </summary>
        FEMALE = 0,
    }
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum PersonEnum
    {
        deleted = 1,
        NORMAL = 0,
    }
}
