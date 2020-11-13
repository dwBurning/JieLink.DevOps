using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartialViewCheckUpdate;
using System.Windows;
using PartialViewCheckUpdate.Models.Enum;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using PartialViewInterface.Models;
using PartialViewInterface;

namespace PartialViewCheckUpdate.ViewModels
{
    class CheckUpdateViewModel : DependencyObject
    {
        public DelegateCommand RepairCommand { get; set; }

        public CheckUpdateViewModel()
        {
            this.RepairCommand = new DelegateCommand();
            this.RepairCommand.ExecuteAction = this.Repair;
            ProcessHelper.ShowOutputMessageEx += ProcessHelper_ShowOutputMessageEx;
        }

        private void ProcessHelper_ShowOutputMessageEx(string message)
        {
            ShowMessage(message);
        }

        public string StartVersion
        {
            get { return (string)GetValue(StartVersionProperty); }
            set { SetValue(StartVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartVersionProperty =
            DependencyProperty.Register("StartVersion", typeof(string), typeof(CheckUpdateViewModel));




        public string EndVersion
        {
            get { return (string)GetValue(EndVersionProperty); }
            set { SetValue(EndVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndVersionProperty =
            DependencyProperty.Register("EndVersion", typeof(string), typeof(CheckUpdateViewModel));




        public string InstallPath
        {
            get { return (string)GetValue(InstallPathProperty); }
            set { SetValue(InstallPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InstallPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstallPathProperty =
            DependencyProperty.Register("InstallPath", typeof(string), typeof(CheckUpdateViewModel));




        public string PackagePath
        {
            get { return (string)GetValue(PackagePathProperty); }
            set { SetValue(PackagePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PackagePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PackagePathProperty =
            DependencyProperty.Register("PackagePath", typeof(string), typeof(CheckUpdateViewModel));




        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(CheckUpdateViewModel));





        private void Repair(object parameter)
        {
            if (string.IsNullOrEmpty(this.PackagePath) || string.IsNullOrEmpty(this.InstallPath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择正确的路径！");
                return;
            }

            DirectoryInfo packageDir = new DirectoryInfo(this.PackagePath);

            if (packageDir.Name.Equals("sys") || packageDir.Name.Equals("obj"))
                packageDir = packageDir.Parent;
            var zipPath = Directory.GetFiles(Path.Combine(packageDir.FullName, "obj"), "*.zip").FirstOrDefault();
            if (string.IsNullOrEmpty(zipPath) || !zipPath.Contains("JSOCT"))//加上这个判断，防止选成盒子的包
            {
                MessageBoxHelper.MessageBoxShowWarning("升级包不存在！");
                return;
            }

            if (MessageBoxHelper.MessageBoxShowQuestion($"请确认当前版本为{StartVersion}？") == MessageBoxResult.No)
            {
                return;
            }

            DirectoryInfo installDir = new DirectoryInfo(this.InstallPath);
            if (installDir.Name.Equals("SmartCenter", StringComparison.OrdinalIgnoreCase))
            {
                installDir = installDir.Parent;
            }
            string rootPath = installDir.FullName;
            //检测是否是一个有效的中心按照目录
            if (!File.Exists(Path.Combine(rootPath, "NewG3Uninstall.exe")))
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择正确的中心安装目录！");
                return;
            }

            UpdateRequest updateRequest = new UpdateRequest();
            updateRequest.Guid = Guid.NewGuid().ToString();
            updateRequest.Product = "JSOCT2016";
            updateRequest.RootPath = rootPath;
            updateRequest.PackagePath = zipPath;
            ExecuteUpdate(updateRequest);
            ExecuteScript();

        }
        private void ExecuteUpdate(UpdateRequest request)
        {
            //1.升级请求写到update文件夹下
            WriteRequestFile(request);
            //2.启动升级程序
            string executePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\Updater.exe");
            ProcessHelper.StartProcessDotNet(executePath, "-file=UpdateRequest_2016.json");
        }

        private void WriteRequestFile(UpdateRequest request)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\UpdateRequest_2016.json");
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


        private void ExecuteScript()
        {
            string scriptPath = Path.Combine(this.PackagePath, "dbscript");
            List<FileInfo> scripts = FileHelper.GetAllFileInfo(scriptPath, "*.sql").OrderBy(x => x.Name).ToList();
            int index = scripts.FindIndex(x => x.Name.Contains(this.StartVersion));

            List<FileInfo> fileInfos = new List<FileInfo>();
            for (int i = index; i < scripts.Count; i++)
            {
                fileInfos.Add(scripts[i]);
            }

            Task.Factory.StartNew(() =>
            {
                foreach (var file in fileInfos)
                {
                    string mysqlcmd = $"mysql --default-character-set=utf8 -h{EnvironmentInfo.DbConnEntity.Ip} -u{EnvironmentInfo.DbConnEntity.UserName} -p{EnvironmentInfo.DbConnEntity.Password} -P{EnvironmentInfo.DbConnEntity.Port} {EnvironmentInfo.DbConnEntity.DbName} < \"{file.FullName}\"";

                    ShowMessage(mysqlcmd);
                    List<string> cmds = new List<string>();
                    string mysqlBin = AppDomain.CurrentDomain.BaseDirectory;
                    cmds.Add(mysqlBin.Substring(0, 2));
                    cmds.Add("cd " + mysqlBin);
                    cmds.Add(mysqlcmd);
                    ProcessHelper.ExecuteCommand(cmds, enumToolType.OneKeyUpdate);
                }
            });
        }

        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Message != null && Message.Length > 5000)
                {
                    Message = string.Empty;
                }

                if (message.Length > 0)
                {
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }

    }
}
