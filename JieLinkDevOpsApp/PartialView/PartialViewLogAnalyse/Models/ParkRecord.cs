using System;
using System.Collections.Generic;
using System.Text;

namespace PartialViewLogAnalyse.Models
{
    public class ParkRecord
    {
        public ParkRecord()
        {
            HistoryCredentialNo = new List<string>();
        }
        public string RecordId { get; set; }
        public string TransId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string CredentialNo { get; set; }
        public DateTime EventTime { get; set; }
        public int IoType { get; set; }
        public bool Online { get; set; }
        public List<LogNode> LogNodes { get; set; }
        public bool IsEnd { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> HistoryCredentialNo { get; set; }
    }
}
