using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    /// <summary>
    /// 人脸凭证：暂时不考虑，后期有人脸版本再优化
    /// </summary>
    public class TCacVoucherDataModel
    {
        /// <summary>
        /// 凭证GUID
        /// </summary>
        public string VOUCHER_PHYSICAL_NO { get; set; }
        public string VOUCHER_DATA { get; set; }
        public string DATA_VERSION { get; set; }
        public string STATUS { get; set; }
        public string FACE_PIC_PATH { get; set; }
        public string FEATURE_PIC_PATH { get; set; }
    }
}
