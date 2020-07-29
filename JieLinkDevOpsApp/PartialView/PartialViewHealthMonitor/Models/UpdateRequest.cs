using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor
{
    public class UpdateRequest
    {
        public string Product { get; set; }
        public string Guid { get; set; }
        public string RootPath { get; set; }
        public string PackagePath { get; set; }
    }
}
