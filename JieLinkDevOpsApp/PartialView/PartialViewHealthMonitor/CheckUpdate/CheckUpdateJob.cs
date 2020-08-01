using PartialViewHealthMonitor.Models;
using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor.CheckUpdate
{
    /// <summary>
    /// 运维工具自动升级任务
    /// </summary>
    public class CheckUpdateJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {

            Console.WriteLine("CheckUpdateJob...");
            UpdateRequest updateRequest = CheckUpdateHelper.GetUploadRequest();
            if (updateRequest != null)
            {
                CheckUpdateHelper.ExecuteUpdate(updateRequest);
            }

            //测试
            //UpdateRequest updateRequest = new UpdateRequest();
            //updateRequest.Guid = Guid.NewGuid().ToString();
            //updateRequest.Product = "JSOCT2016";
            //updateRequest.RootPath = @"D:\Program Files (x86)\Jielink";
            //updateRequest.PackagePath = @"D:\迅雷下载\JSOCT2016 V2.6.2 Jielink+智能终端操作平台安装包\obj\JSOCT2016-V2.6.2.zip";
            //ExecuteUpdate(updateRequest);
        }

    }
}
