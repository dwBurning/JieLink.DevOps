using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.Models
{
    /// <summary>
    /// G3组织部门表
    /// </summary>
    public class TBaseDeptModel
    {
       /// <summary>
       /// 组织ID
       /// </summary>
        public string ID { get; set; }

        public string SID { get; set; }

        /// <summary>
        /// 组织编号
        /// </summary>
        public string NO { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string NAME { get; set; }

        public string REMARK { get; set; }

        public string LEADER { get; set; }

        public string PARENTSID { get; set; }

        /// <summary>
        /// 父组织ID
        /// </summary>
        public string PARENTID { get; set; }

        public string SERVERNO { get; set; }

        public string SERVERDOWN { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public string DELETEFLAG { get; set; }

        public string UPLOADFLAG { get; set; }



        public string ARCHIVEFLAG { get; set; }

        public string TIMESTAMP { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OPTDATE { get; set; }


        public string VISITFLAG { get; set; }

        public string AREATYPE { get; set; }


        /// <summary>
        /// 组织GUID
        /// </summary>
        public string GUID { get; set; }

    }

    /// <summary>
    /// 组织状态
    /// </summary>
    public enum EnumDeptStatus
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
