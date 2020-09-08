using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models
{
    public class JspayOrderRecord
    {
        public string SeqId { get; set; }
        public string MiddleValue { get; set; }
        public string CredentialNo { get; set; }
        public DateTime ReceiveTime { get; set; }
        public DateTime NotifyTime { get; set; }
        public DateTime PayTime { get; set; }
        public string ThreadName { get; set; }
        //public bool IsOutScanCode { get; set; }
        public string EnterRecordId { get; set; }
        public bool HaveEnterRecord { get; set; }
        public string OrderNo { get; set; }
        public double TotalFee { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        //public string DeviceId { get; set; }//如果有

        public bool IsMatchOutRecord { get; set; }
    }
}
