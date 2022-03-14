using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewImportInfoVoucher.Models
{
    public class Person
    {
        public string Id { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public int NumberType { get; set; }

        /// <summary>
        /// 证件编号
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string PersonNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 组织Id
        /// </summary>
        public string GroupId { get; set; }
        


    }
}
