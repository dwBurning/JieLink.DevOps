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
        public DateTime ReceiveTime { get; set; }

        /// <summary>
        /// 问题研发负责人
        /// </summary>
        public string ResponsiblePerson { get; set; }

        /// <summary>
        /// 转到研发的时间
        /// </summary>
        public DateTime YanFaTime { get; set; }

        /// <summary>
        /// 最后完成时间(超时时间)
        /// </summary>
        public DateTime OverTime1 { get; set; }

        /// <summary>
        /// 解决方案
        /// </summary>
        public string SolutionInfo { get; set; }

        /// <summary>
        /// 研发完成时间
        /// </summary>
        public DateTime FinishTime { get; set; }
    }

    public class PageOrder
    {
        public int pageIndex { get; set; }

        public int pageSize { get; set; }

        public int totalCount { get; set; }

        public int pageTotal { get; set; }

        public List<Order> data { get; set; }

    }
    public class DisposePageOrder
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string problemCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string problemType { get; set; }
        /// <summary>
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string currentNode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string disposeStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userCode { get; set; }
        /// <summary>
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string disposeTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmentCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string departmentName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userMobile { get; set; }

    }
}
