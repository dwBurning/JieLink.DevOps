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
    /// 清理过期的备份文件
    /// </summary>
    public class DeleteOverTimePackageJob : IJob
    {

        MySqlBackUpViewModel viewModel = MySqlBackUpViewModel.Instance();
        public void Execute(IJobExecutionContext context)
        {
            string filePath = "";
            viewModel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                filePath = viewModel.TaskBackUpPath;
            }));

            List<FileInfo> files = FileHelper.GetAllFileInfo(filePath, "*.zip");
            //有存在压缩失败 的情况
            List<FileInfo> fileInfos = FileHelper.GetAllFileInfo(filePath, "*.sql");
            files.AddRange(fileInfos);
            string countStr = EnvironmentInfo.Settings.FirstOrDefault(x => x.KeyId == "SaveFileCount")?.ValueText;
            int count = 7;
            int.TryParse(countStr, out count);
            if (files.Count < count) return;//文件小于7个 不删除

            long length = 0;
            files.ForEach(x =>
            {
                length += x.Length;
            });
            long m = length / (1024 * 1024);//M
            if (m < 5 * 1024) return;//文件小于5G 不删除

            for (int i = 0; i < files.Count - count; i++)//永远保留7个最新的备份文件
            {
                var file = files[i];
                DateTime lastWriteTime = file.LastWriteTime;
                if ((DateTime.Now - lastWriteTime).TotalDays > count)//删掉30天之前的文件
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
