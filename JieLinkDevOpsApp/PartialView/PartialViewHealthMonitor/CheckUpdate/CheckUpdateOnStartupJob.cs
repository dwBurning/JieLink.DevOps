using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewHealthMonitor.CheckUpdate
{
    public class CheckUpdateOnStartupJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            context.Scheduler.UnscheduleJob(context.Trigger.Key);
            UpdateRequest updateRequest = CheckUpdateHelper.GetUploadRequest();
            if (updateRequest != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (MessageBoxHelper.MessageBoxShowQuestion($"检测到新版本{updateRequest.Version}[当前版本{EnvironmentInfo.CurrentVersion}]，是否立即升级？") == MessageBoxResult.Yes)
                    {
                        CheckUpdateHelper.ExecuteUpdate(updateRequest);
                    }
                }));
                
            }
            
        }
    }
}
