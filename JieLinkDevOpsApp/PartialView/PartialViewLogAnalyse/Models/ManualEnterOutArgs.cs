using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models
{
    public class ManualEnterOutArgs
    {
        public string RecordId { get; set; }


        public int VoucherType { get; set; }


        public string VoucherNo { get; set; }


        public string PicturePath { get; set; }


        public string Plate { get; set; }


        public int Reason { get; set; }


        public string DeviceID { get; set; }


        public string SourceDeviceID { get; set; }


        public int SetMealNo { get; set; }


        public string Remark { get; set; }


        public bool IsVistor { get; set; }


        public string VistorNo { get; set; }


        public int ClientType { get; set; }

        public string OperatorNo { get; set; }


        public string OperatorName { get; set; }


        public int PlateColor { get; set; }


        public int GateCommand { get; set; }


        public Dictionary<string, string> Extensions;
    }
}
