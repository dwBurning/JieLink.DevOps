using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor.CheckUpdate
{
    class ReportProjectInfoJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DevOpsAPI.ReportVersion();
        }
    }
}
