using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJsdsOneClickUpgradeToJieLink.ViewModels
{
    /// <summary>
    /// 用户密钥
    /// </summary>
    public class TBasePersonKeyModel
    {
        /// <summary>
        /// 用户GUID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 密钥：对应JieLink的CurKey,LastKey
        /// </summary>
        public string DYNAMIC_KEY { get; set; }

        public string EFFECTIVE_DATE { get; set; }
        public string INVALID_DATE { get; set; }
        public string LAST_DYNAMIC_KEY { get; set; }
        public string LAST_EFFECTIVE_DATE { get; set; }
        public string LAST_INVALID_DATE { get; set; }
        public string REMARK { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }
        public string SYNC_TIME { get; set; }
        public string SYNC_FLAG { get; set; }
        public string SYNC_FAILS { get; set; }

    }
}
