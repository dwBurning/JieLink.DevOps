using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.TaskDataMonitor
{
    /// <summary>
    /// 外挂定时任务用于修订jielink自身的一些问题
    /// </summary>
    public class TaskDataMonitorJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IMonitor)))).ToList();
            foreach (var t in types)
            {
                IMonitor monitor = (IMonitor)Activator.CreateInstance(t);
                monitor.Monitor();
            }
        }
    }
}
