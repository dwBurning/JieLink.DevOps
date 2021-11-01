using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewImportInfoVoucher.Models
{
    public class CredentialChannelRel
    {
        public string Id { get; set; }

        /// <summary>
        /// 凭证ID
        /// </summary>
        public string CredentialId { get; set; }

        /// <summary>
        /// 凭证编号
        /// </summary>
        public string CredentialNo { get; set; }

        /// <summary>
        /// 凭证类型
        /// </summary>
        public int CredentialType { get; set; }

        /// <summary>
        /// 凭证状态
        /// </summary>
        public int CredentialStatus { get; set; }

        /// <summary>
        /// 通道Id
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 服务ID
        /// </summary>
        public string LeaseStallId { get; set; }
        


    }
}
