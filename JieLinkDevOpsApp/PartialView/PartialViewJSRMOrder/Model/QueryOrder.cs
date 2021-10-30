using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJSRMOrder.Model
{
    public class QueryOrder
    {
        public int pageIndex { get; set; }

        public int pageSize { get; set; }

        public string currentNode { get; set; }

        public string currentUserCode { get; set; }
    }
}
