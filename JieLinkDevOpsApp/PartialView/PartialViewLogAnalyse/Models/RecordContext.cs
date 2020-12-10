using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models
{
    public class RecordContext
    {
        public List<JspayOrderRecord> OrderRecords { get; set; }
        public List<ParkRecord> ParkRecords { get; set; } 

        public List<DeviceInfo> DeviceCache { get; set; }

    }
}
