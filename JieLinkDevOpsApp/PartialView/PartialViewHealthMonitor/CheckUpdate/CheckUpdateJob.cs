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
            //TODO:从服务端获取升级信息

            //测试
            //UpdateRequest updateRequest = new UpdateRequest();
            //updateRequest.Guid = Guid.NewGuid().ToString();
            //updateRequest.Product = "JSOCT2016";
            //updateRequest.RootPath = @"D:\Program Files (x86)\Jielink";
            //updateRequest.PackagePath = @"D:\迅雷下载\JSOCT2016 V2.6.2 Jielink+智能终端操作平台安装包\obj\JSOCT2016-V2.6.2.zip";
            //ExecuteUpdate(updateRequest);
        }
        private void ExecuteUpdate(UpdateRequest request)
        {
            //1.升级请求写到update文件夹下
            WriteRequestFile(request);
            //2.启动升级程序
            string executePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\Updater.exe");
            ProcessHelper.StartProcessDotNet(executePath,null);
        }
        private void WriteRequestFile(UpdateRequest request)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\UpdateRequest.json");
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            string json = JsonHelper.SerializeObject(request);
            using (FileStream fs = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
        }
        private void Download(string url, string savePath)
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(url, savePath);
            }
            if (!File.Exists(savePath))
            {
                throw new Exception("下载文件失败！");
            }
        }
    }
}
