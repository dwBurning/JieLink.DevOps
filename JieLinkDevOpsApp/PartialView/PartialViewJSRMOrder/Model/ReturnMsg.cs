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
    public class DisposeReturnMsg
    {
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string respCode { get; set; }
        /// <summary>
        /// 成功
        /// </summary>
        public string respMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DisposePageOrder> respData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string map { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string queryBean { get; set; }
    }
}
