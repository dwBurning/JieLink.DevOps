using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJSRMOrder.Model
{
    public class Order
    {
        public string problemCode { get; set; }

        public string projectName { get; set; }

        public string problemInfo { get; set; }

        public string userName { get; set; }

        public string problemTime { get; set; }

        public string remoteAccount { get; set; }

        public string softVersion { get; set; }
    }
}
