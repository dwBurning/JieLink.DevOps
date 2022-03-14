using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor.CheckUpdate
{
    public class ReportProjectInfoJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            Thread.Sleep(random.Next(1, 3600) * 1000);//1小时内，暂停随机时间

            DevOpsAPI.ReportVersion();
        }
    }
}
