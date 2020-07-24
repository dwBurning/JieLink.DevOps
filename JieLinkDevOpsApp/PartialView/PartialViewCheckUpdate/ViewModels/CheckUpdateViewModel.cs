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

namespace PartialViewCheckUpdate.ViewModels
{
    class CheckUpdateViewModel : NotificationObject  //PropertyChangedBase
    {
        public CheckUpdateViewModel()
        {
            this.ChoseSourceCommand = new DelegateCommand();
            this.ChoseSourceCommand.ExecuteAction = new Action<object>(this.GetSourceFile);
            this.ChosePackageCommand = new DelegateCommand();
            this.ChosePackageCommand.ExecuteAction = new Action<object>(this.GetPackageFile);
            this.CheckUpdateCommand = new DelegateCommand();
            this.CheckUpdateCommand.ExecuteAction = new Action<object>(this.CheckUpdate);
            this.TestConnCommand = new DelegateCommand();
            this.TestConnCommand.ExecuteAction = new Action<object>(this.TestMySqlConn);
            //this.TestConnCommand.CanExecuteFunc = new Func<object, bool>(this.TestMySqlConn);
            this.CheckDBUpdateCommand = new DelegateCommand();
            this.CheckDBUpdateCommand.ExecuteAction = new Action<object>(this.CheckDBUpdate);


            this.BtnUpdateEnabled = "False";
            this.BtnCheckDBUpdateEnabled = "True";
        }

        private string sourceFilePath;

        public string SourceFilePath
        {
            get { return sourceFilePath; }
            set
            {
                sourceFilePath = value;
                this.RaisePropertyChanged("SourceFilePath");
            }
        }

        private string packageFilePath;

        public string PackageFilePath
        {
            get { return packageFilePath; }
            set
            {
                packageFilePath = value;
                this.RaisePropertyChanged("PackageFilePath");
            }
        }

        private string result;

        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                this.RaisePropertyChanged("Result");
            }
        }

        private string btnUpdateEnabled;

        public string BtnUpdateEnabled
        {
            get { return btnUpdateEnabled; }
            set
            {
                btnUpdateEnabled = value;
                this.RaisePropertyChanged("BtnUpdateEnabled");
            }
        }

        private string btnCheckDBUpdateEnabled;

        public string BtnCheckDBUpdateEnabled
        {
            get { return btnCheckDBUpdateEnabled; }
            set
            {
                btnCheckDBUpdateEnabled = value;
                this.RaisePropertyChanged("BtnCheckDBUpdateEnabled");
            }
        }


        private string mySqlConnStr;

        public string MySqlConnStr
        {
            get { return mySqlConnStr; }
            set
            {
                mySqlConnStr = value;
                this.RaisePropertyChanged("MySqlConnStr");
            }
        }

        private string dbIP;

        public string DBIP
        {
            get { return dbIP; }
            set
            {
                dbIP = value;
                this.RaisePropertyChanged("DBIP");
            }
        }

        private string dbName;

        public string DBName
        {
            get { return dbName; }
            set
            {
                dbName = value;
                this.RaisePropertyChanged("DBName");
            }
        }

        private string dbPort;

        public string DBPort
        {
            get { return dbPort; }
            set
            {
                dbPort = value;
                this.RaisePropertyChanged("DBPort");
            }
        }

        private string dbUser;

        public string DBUser
        {
            get { return dbUser; }
            set
            {
                dbUser = value;
                this.RaisePropertyChanged("DBUser");
            }
        }

        private string dbPassword;

        public string DBPassword
        {
            get { return dbPassword; }
            set
            {
                dbPassword = value;
                this.RaisePropertyChanged("DBPassword");
            }
        }





        public DelegateCommand ChoseSourceCommand { get; set; }
        private void GetSourceFile(object parameter)
        {

            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            this.SourceFilePath = m_Dialog.SelectedPath.Trim();
        }

        public DelegateCommand ChosePackageCommand { get; set; }
        private void GetPackageFile(object parameter)
        {

            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            this.PackageFilePath = m_Dialog.SelectedPath.Trim();
        }

        public DelegateCommand TestConnCommand { get; set; }
        private void TestMySqlConn(object parameter)
        {
            string mySqlConnStr = GetConnStr();
            if (string.IsNullOrEmpty(mySqlConnStr))
            {
                MessageBoxX.Show("请输入完整的数据库连接信息", "错误");
                return;
            }
            TestMySql(mySqlConnStr);
        }

        public DelegateCommand CheckDBUpdateCommand { get; set; }
        private void CheckDBUpdate(object parameter)
        {
            try
            {
                string mySqlConnStr = GetConnStr();
                if (string.IsNullOrEmpty(mySqlConnStr))
                {
                    MessageBoxX.Show("请输入完整的数据库连接信息", "错误");
                    return;
                }
                if (string.IsNullOrEmpty(this.PackageFilePath))
                {
                    MessageBoxX.Show("安装包路径不能为空", "失败");
                    return;
                }
                string packagePath = this.PackageFilePath.Trim();

                packagePath = Path.Combine(packagePath, "sys\\DbInitScript");
                var files = from file in Directory.EnumerateFiles(packagePath, "*.json*", SearchOption.TopDirectoryOnly)
                            select file;

                if (files == null || files.Count() == 0)
                {
                    MessageBoxX.Show($"未找到数据库对比文件{packagePath}\\xxxxx.json", "失败");
                    return;
                }
                string packageDbJsonFile = files.First();
                if (TestMySql(mySqlConnStr))
                {
                    if (CheckDBUpdateTool.CheckDBUpdate(packageDbJsonFile, mySqlConnStr))
                    {
                        MessageBoxX.Show($"数据库升级成功！", "成功", null, MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBoxX.Show($"数据库升级失败！\n请按照下方步骤升级。", "失败", null, MessageBoxButton.OK);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                string msg = "检查升级出错，文件没有访问权限，请设置文件夹读写权限\n";
                MessageBoxX.Show(msg, "异常");
                return;
            }
            catch (PathTooLongException)
            {
                string msg = "检查升级出错，软件安装目录或安装包目录 指定的路径或文件名超过了系统定义的最大长度\n";
                MessageBoxX.Show(msg, "异常");
                return;
            }
            catch (DirectoryNotFoundException)
            {
                string msg = "检查升级出错，所检查的目录不存在\n";
                MessageBoxX.Show(msg, "异常");
                return;
            }
            catch (Exception)
            {
                string msg = "程序异常";
                MessageBoxX.Show(msg, "异常");
                return;
            }

        }

        public DelegateCommand CheckUpdateCommand { get; set; }

        private void CheckUpdate(object parameter)
        {
            string msg = string.Empty;
            EnumCheckFileResult r = EnumCheckFileResult.Ok;
            if (string.IsNullOrEmpty(this.SourceFilePath) || string.IsNullOrEmpty(this.PackageFilePath))
            {
                MessageBoxX.Show("软件安装目录和安装包路径不能为空", "失败");
                return;
            }
            string sourcePath = this.SourceFilePath.Trim();
            string packagePath = this.PackageFilePath.Trim();
            this.Result = string.Empty;
            //foreach (var dir in dirs)
            //{
            //    msg = $"开始检查目录{dir}……\n";
            //    this.Result += msg;
            //    sourcePath = Path.Combine(this.SourceFilePath.Trim(), dir);
            //    r = CheckFileUpdate(sourcePath, packagePath);
            //    msg = $"完成检查目录{dir}……\n";
            //    this.Result += msg;
            //    if (r == EnumCheckFileResult.FILE_ERROR1 || r == EnumCheckFileResult.OTHER_ERROR)
            //    {
            //        break;
            //    }
            //}
            r = CheckFileUpdate(sourcePath, packagePath);
            if (r == EnumCheckFileResult.Error)
            {
                MessageBoxX.Show("JieLink软件升级失败！\n请按照下方步骤升级。", "失败", null, MessageBoxButton.OK);
            }
            else if (r == EnumCheckFileResult.Ok)
            {
                MessageBoxX.Show("JieLink软件升级成功！", "成功", null, MessageBoxButton.OK);
            }

        }



        public List<string> dirs = new List<string>()
        {
            "SmartCenter",
            "SmartApi",
            "SmartFile",
            "SmartWeb",

        };

        /// <summary>
        /// 检测当前目录是否升级成功
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public EnumCheckFileResult CheckFileUpdate(string sourcePath, string packagePath)
        {
            string msg = string.Empty;
            try
            {
                int failCount = 0;

                //只根据安装包SmartCenter.Host.exe的修改时间来确定安装包的时间
                DateTime packageTime = File.GetLastWriteTime(packagePath + "\\sys\\programfiles\\SmartCenter\\SmartCenter.Host.exe");

                var files = from file in Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories)
                            where (file.EndsWith(".dll") || file.EndsWith(".exe"))
                            select file;
                if (files.Count() <= 0)
                {
                    msg = $"未检测到文件，请检查软件安装目录和安装包路径是否正确！\n";
                    this.Result += msg;
                    MessageBoxX.Show(msg, "异常");
                    return EnumCheckFileResult.Error;
                }
                foreach (var f in files)
                {
                    if (packageTime.CompareTo(File.GetLastWriteTime(f)) > 0)
                    {
                        failCount++;
                    }
                    //else if (packageTime.CompareTo(File.GetLastWriteTime(f)) < 0)
                    //{
                    //    failCount++;
                    //}
                }
                msg = $"[WARN] 检测到文件升级失败文件数量{failCount}个\n";
                this.Result += msg;
                if (failCount > 5)
                {
                    msg = $"[WARN] JieLink软件升级失败\n";
                    this.Result += msg;
                    return EnumCheckFileResult.Error;
                }
                msg = $"升级成功！\n";
                this.Result += msg;
                return EnumCheckFileResult.Ok;
            }
            catch (UnauthorizedAccessException)
            {
                msg = "检查升级出错，文件没有访问权限，请设置文件夹读写权限\n";
                this.Result += msg;
                return EnumCheckFileResult.Error;
            }
            catch (PathTooLongException)
            {
                msg = "检查升级出错，软件安装目录或安装包目录 指定的路径或文件名超过了系统定义的最大长度\n";
                this.Result += msg;
                return EnumCheckFileResult.Error;
            }
            catch (DirectoryNotFoundException)
            {
                msg = "检查升级出错，所检查的目录不存在\n";
                this.Result += msg;
                return EnumCheckFileResult.Error;
            }
            catch (Exception)
            {
                msg = "程序异常";
                this.Result += msg;
                return EnumCheckFileResult.Error;
            }
        }

        /// <summary>
        /// 获取Mysql连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnStr()
        {
            if (string.IsNullOrEmpty(this.DBIP) || string.IsNullOrEmpty(this.DBName) || string.IsNullOrEmpty(this.DBPort) || string.IsNullOrEmpty(this.DBName) || string.IsNullOrEmpty(this.DBPassword))
            {
                return string.Empty;
            }
            return $"Data Source={this.DBIP};port={this.DBPort};Initial Catalog={this.DBName};Persist Security Info=True;User ID={this.DBUser};Password={this.DBPassword};MAX Pool Size=2000;Min Pool Size=3;Connection Timeout=300;Pooling=true;charset=utf8";
        }

        public bool TestMySql(string connStr)
        {
            try
            {
                connStr = connStr.Trim();
                if (CheckDBUpdateTool.TestMySqlConn(connStr))
                {
                    MessageBoxX.Show("数据库连接成功！", "成功");
                    this.BtnCheckDBUpdateEnabled = "True";
                    return true;
                }
                MessageBoxX.Show("数据库连接失败！", "失败");
                return false;
            }
            catch (Exception)
            {
                MessageBoxX.Show("数据库连接失败！", "失败");
                return false;
            }

        }


    }
}
