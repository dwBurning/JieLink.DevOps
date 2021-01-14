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
            if (files.Count < 7) return;//文件小于7个 不删除
            for (int i = 0; i < files.Count - 7; i++)//永远保留7个最新的备份文件
            {
                var file = files[i];
                DateTime lastWriteTime = file.LastWriteTime;
                if ((DateTime.Now - lastWriteTime).TotalDays > 7)//删掉30天之前的文件
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
