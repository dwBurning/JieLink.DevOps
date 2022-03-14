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
    /// 修改：增加数据库名参数
    /// </summary>
    public class DataBaseBackUpJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            string databaseName = data.GetString("DataBaseName");
            ExecuteBackUp executeBackUp = new ExecuteBackUp();
            executeBackUp.BackUpDataBase(databaseName);
        }
    }
}
