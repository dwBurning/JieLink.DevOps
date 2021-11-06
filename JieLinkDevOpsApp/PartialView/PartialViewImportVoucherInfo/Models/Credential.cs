using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewImportInfoVoucher.Models
{
    public class Credential
    {
        public string Id { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string PersonNo { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 凭证编号
        /// </summary>
        public string CredentialNo { get; set; }

        /// <summary>
        /// 类型枚举
        /// </summary>
        public string CredentialType { get; set; }   
        



        public DateTime CreateTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public string Remark { get; set; }

        public string Plate { get; set; }
    }
}
