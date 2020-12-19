using PartialViewHealthMonitor.Models;
using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor.CheckUpdate
{
    public class CheckUpdateHelper
    {

        public static void ExecuteUpdate(UpdateRequest request)
        {
            //1.升级请求写到update文件夹下
            WriteRequestFile(request);
            //2.启动升级程序
            string executePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\Updater.exe");
            ProcessHelper.StartProcessDotNet(executePath, null);
        }



        public static UpdateRequest GetUploadRequest()
        {
            try
            {
                UpdateRequest updateRequest = null;

                string url = string.Format("{0}/devops/getTheLastVersion?productType={1}", EnvironmentInfo.ServerUrl, (int)enumProductType.DevOps);
                DevOpsProduct product = HttpHelper.Get<DevOpsProduct>(url, 3000);
                if (product == null)
                {
                    LogHelper.CommLogger.Error("获取最新版本失败");
                    return null;
                }
                if (CompareVersion(EnvironmentInfo.CurrentVersion, product.ProductVersion) >= 0)
                {
                    LogHelper.CommLogger.Error("已经是最新版，不执行升级");
                    return null;
                }
                LogHelper.CommLogger.Info("下载安装包开始");
                //拼接下载url
                string downloadUrl = product.DownloadUrl;
                if (!downloadUrl.StartsWith("http"))
                {
                    if (downloadUrl.StartsWith("/"))
                        downloadUrl = EnvironmentInfo.ServerUrl + downloadUrl;
                    else
                        downloadUrl = EnvironmentInfo.ServerUrl + "/" + downloadUrl;
                }
                //解析文件名
                string fileName = downloadUrl.Substring(downloadUrl.LastIndexOf('/') + 1);
                string guid = Guid.NewGuid().ToString();
                //下载路径
                string downloadDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\Temp");
                if (!Directory.Exists(downloadDir))
                {
                    Directory.CreateDirectory(downloadDir);
                }
                string downloadFile = Path.Combine(downloadDir, fileName);
                Download(downloadUrl, downloadFile);

                updateRequest = new UpdateRequest();
                updateRequest.Guid = Guid.NewGuid().ToString();
                updateRequest.Version = product.ProductVersion;
                updateRequest.Product = enumProductType.DevOps.ToString();
                updateRequest.RootPath = AppDomain.CurrentDomain.BaseDirectory;
                updateRequest.PackagePath = downloadFile;
                LogHelper.CommLogger.Info("下载安装包完成");
                return updateRequest;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                LogHelper.CommLogger.Error(ex, "GetUploadRequest失败");
                return null;
            }

        }
        private static void WriteRequestFile(UpdateRequest request)
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
        private static void Download(string url, string savePath)
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
        public static int CompareVersion(string v1, string v2)
        {
            if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
            {
                //1代表v2>v1，不执行升级操作
                return 1;
            }
            if (v1.Equals(v2, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }
            v1 = v1.Replace("V", "").Replace("v", "").Split('-')[0];
            v2 = v2.Replace("V", "").Replace("v", "").Split('-')[0];
            var v1Nums = v1.Split('.').ToList();
            var v2Nums = v2.Split('.').ToList();
            while (v1Nums.Count < v2Nums.Count)
            {
                v1Nums.Add("");
            }
            while (v2Nums.Count < v1Nums.Count)
            {
                v2Nums.Add("");
            }
            string v1Strings = v1Nums.Aggregate((a, b) => a.PadLeft(10, '0') + b.PadLeft(10, '0'));
            string v2Strings = v2Nums.Aggregate((a, b) => a.PadLeft(10, '0') + b.PadLeft(10, '0'));
            return string.Compare(v1Strings, v2Strings);
        }
    }
}
