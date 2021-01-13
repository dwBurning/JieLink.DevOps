using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    public class ProjectInfo
    {
        public string ProjectNo { get; set; }

        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }

        public string RemoteAccount { get; set; }

        public string RemotePassword { get; set; }

        public string ContactName { get; set; }

        public string ContactPhone { get; set; }

        public string DevopsVersion { get; set; }

        public enumProductType ProductType { get; set; }
    }
}
