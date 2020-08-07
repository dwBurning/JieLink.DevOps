using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor.Jobs
{
    /// <summary>
    /// 运维工具自动升级任务
    /// </summary>
    public class CheckUpdateJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("CheckUpdateJob...");
        }
    }
}
