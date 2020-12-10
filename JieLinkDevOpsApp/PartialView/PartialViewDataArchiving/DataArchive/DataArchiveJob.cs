using MySql.Data.MySqlClient;
using PartialViewDataArchiving.Models;
using PartialViewDataArchiving.ViewModels;
using PartialViewInterface;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.DataArchive
{
    public class DataArchiveJob : IJob
    {
        /// <summary>
        /// 执行归档
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            ExecuteDataArchive dataArchive = new ExecuteDataArchive();
            if (!EnvironmentInfo.IsAutoArchive)
            {
                dataArchive.ExecuteEx();
                return;
            }

            DataArchivingViewModel.Instance().ShowMessage("定时任务执行数据归档...", 0);
            dataArchive.Execute();
        }
    }
}
