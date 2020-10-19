using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewMySqlBackUp.ViewModels;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewMySqlBackUp.BackUp
{
    /// <summary>
    /// 全库备份
    /// </summary>
    public class DataBaseBackUpJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ExecuteBackUp executeBackUp = new ExecuteBackUp();
            executeBackUp.BackUpDataBase();
        }
    }
}
