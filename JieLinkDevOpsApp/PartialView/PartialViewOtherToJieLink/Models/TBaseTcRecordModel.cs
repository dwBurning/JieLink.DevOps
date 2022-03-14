using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.Models
{
    public class TBaseTcRecordModel
    {
        public string CarNo { get; set; }

        public string GUID { get; set; }

        public DateTime Intime { get; set; }

        public int CardTypeId { get; set; }

        public string CardTypeName { get; set; }

        public int InId { get; set; }

        public int InDeviceId { get; set; }

        public string InDeviceName { get; set; }

        public int OutId { get; set; }

        public int PersonId { get; set; }

        public string PersonName { get; set; }

        public string Remark { get; set; }
    }
}
