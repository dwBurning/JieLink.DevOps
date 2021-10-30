using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJSRMOrder.Model
{
    public class ReturnMsg<T> where T : class
    {
        public int status { get; set; }

        public string respCode { get; set; }

        public string respMsg { get; set; }

        public bool success { get; set; }

        public T respData { get; set; }
    }
}
