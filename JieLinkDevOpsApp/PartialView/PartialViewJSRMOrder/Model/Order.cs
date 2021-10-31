using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJSRMOrder.Model
{
    public class Order
    {
        /// <summary>
        /// 工单号
        /// </summary>
        public string problemCode { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string problemInfo { get; set; }

        /// <summary>
        /// 问题提交人
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public string problemTime { get; set; }

        /// <summary>
        /// 远程账号
        /// </summary>
        public string remoteAccount { get; set; }

        /// <summary>
        /// 软件版本
        /// </summary>
        public string softVersion { get; set; }

        /// <summary>
        /// 是否已派送
        /// </summary>
        public int Dispatched { get; set; }

        /// <summary>
        /// 接收问题的时间
        /// </summary>
        public DateTime ReciveTime { get; set; }

        /// <summary>
        /// 问题研发负责人
        /// </summary>
        public string ResponsiblePerson { get; set; }
    }

    public class PageOrder
    {
        public int pageIndex { get; set; }

        public int pageSize { get; set; }

        public int totalCount { get; set; }

        public int pageTotal { get; set; }

        public List<Order> data { get; set; }

    }
}
