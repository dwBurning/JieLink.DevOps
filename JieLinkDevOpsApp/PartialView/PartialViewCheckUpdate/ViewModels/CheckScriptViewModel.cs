using Panuon.UI.Silver;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PartialViewCheckUpdate.ViewModels
{
    public class CheckScriptViewModel : DependencyObject
    {
        #region Property

        public event Action<string> UpdateFaildNotify;

        public DelegateCommand TestConnCommand { get; set; }

        public DelegateCommand CheckDBUpdateCommand { get; set; }

        public DelegateCommand ExecuteStepByStepCommand { get; set; }


        public string CenterIp
        {
            get { return (string)GetValue(CenterIpProperty); }
            set { SetValue(CenterIpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterIp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterIpProperty =
            DependencyProperty.Register("CenterIp", typeof(string), typeof(CheckScriptViewModel));

        public string CenterDb
        {
            get { return (string)GetValue(CenterDbProperty); }
            set { SetValue(CenterDbProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterDb.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterDbProperty =
            DependencyProperty.Register("CenterDb", typeof(string), typeof(CheckScriptViewModel));


        public string CenterDbPort
        {
            get { return (string)GetValue(CenterDbPortProperty); }
            set { SetValue(CenterDbPortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterDbPort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterDbPortProperty =
            DependencyProperty.Register("CenterDbPort", typeof(string), typeof(CheckScriptViewModel));


        public string CenterDbUser
        {
            get { return (string)GetValue(CenterDbUserProperty); }
            set { SetValue(CenterDbUserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterDbUser.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterDbUserProperty =
            DependencyProperty.Register("CenterDbUser", typeof(string), typeof(CheckScriptViewModel));

        public string CenterDbPwd
        {
            get { return (string)GetValue(CenterDbPwdProperty); }
            set { SetValue(CenterDbPwdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterDbPwd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterDbPwdProperty =
            DependencyProperty.Register("CenterDbPwd", typeof(string), typeof(CheckScriptViewModel));


        public string CurrentVersion
        {
            get { return (string)GetValue(CurrentVersionProperty); }
            set { SetValue(CurrentVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVersionProperty =
            DependencyProperty.Register("CurrentVersion", typeof(string), typeof(CheckScriptViewModel));


        public string UpdateVersion
        {
            get { return (string)GetValue(UpdateVersionProperty); }
            set { SetValue(UpdateVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateVersionProperty =
            DependencyProperty.Register("UpdateVersion", typeof(string), typeof(CheckScriptViewModel));


        public bool EnableCheckUpdateResult
        {
            get { return (bool)GetValue(EnableCheckUpdateResultProperty); }
            set { SetValue(EnableCheckUpdateResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableCheckUpdateResult.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableCheckUpdateResultProperty =
            DependencyProperty.Register("EnableCheckUpdateResult", typeof(bool), typeof(CheckScriptViewModel));



        public string CheckResult
        {
            get { return (string)GetValue(CheckResultProperty); }
            set { SetValue(CheckResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckResult.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckResultProperty =
            DependencyProperty.Register("CheckResult", typeof(string), typeof(CheckScriptViewModel));


        #endregion

        public CheckScriptViewModel()
        {
            this.TestConnCommand = new DelegateCommand();
            this.TestConnCommand.ExecuteAction = new Action<object>(this.TestMySqlConn);
            this.CheckDBUpdateCommand = new DelegateCommand();
            this.CheckDBUpdateCommand.ExecuteAction = new Action<object>(this.CheckDBUpdate);
            this.ExecuteStepByStepCommand = new DelegateCommand();
            this.ExecuteStepByStepCommand.ExecuteAction = new Action<object>(this.ExecuteStepByStep);
        }

        private void TestMySqlConn(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            //PasswordBox的Password属性因为安全原因不支持直接绑定
            this.CenterDbPwd = PasswordBoxHelper.GetPassword(passwordBox);
            string mySqlConnStr = GetConnStr();
            if (string.IsNullOrEmpty(mySqlConnStr))
            {
                MessageBoxHelper.MessageBoxShowWarning("请输入完整的数据库连接信息");
                return;
            }
            TestMySql(mySqlConnStr);
        }

        private void CheckDBUpdate(object parameter)
        {
            string msg = "";
            try
            {
                string mySqlConnStr = GetConnStr();
                if (string.IsNullOrEmpty(mySqlConnStr))
                {
                    MessageBoxHelper.MessageBoxShowWarning("请输入完整的数据库连接信息");
                    return;
                }
                if (string.IsNullOrEmpty(CheckUpdateContext.SetUpPackagePath))
                {
                    MessageBoxHelper.MessageBoxShowWarning("安装包路径不能为空");
                    return;
                }
                string packagePath = CheckUpdateContext.SetUpPackagePath.Trim();

                packagePath = Path.Combine(packagePath, "DbInitScript");
                var files = from file in Directory.EnumerateFiles(packagePath, "*.json*", SearchOption.TopDirectoryOnly)
                            select file;

                if (files == null || files.Count() == 0)
                {
                    MessageBoxHelper.MessageBoxShowWarning($"未找到数据库对比文件{packagePath}\\xxxxx.json");
                    return;
                }
                string packageDbJsonFile = files.First();
                if (TestMySql(mySqlConnStr))
                {
                    if (CheckDBUpdateTool.CheckDBUpdate(packageDbJsonFile, mySqlConnStr))
                    {
                        MessageBoxHelper.MessageBoxShowSuccess("数据库升级成功！");
                        return;
                    }
                    else
                    {
                        Notice.Show("数据库升级失败", "通知", 3, MessageBoxIcon.Warning);
                        UpdateFaildNotify?.Invoke("CheckScripts");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                msg = "检查升级出错，文件没有访问权限，请设置文件夹读写权限";
            }
            catch (PathTooLongException)
            {
                msg = "检查升级出错，软件安装目录或安装包目录 指定的路径或文件名超过了系统定义的最大长度";
            }
            catch (DirectoryNotFoundException)
            {
                msg = "检查升级出错，所检查的目录不存在";

            }
            catch (Exception ex)
            {
                msg = "程序异常：" + ex.ToString();
            }

            ShowMessage(msg);
            MessageBoxHelper.MessageBoxShowError("检查升级出错！");

        }

        private void ExecuteStepByStep(object parameter)
        {
            if (string.IsNullOrEmpty(this.CurrentVersion) || string.IsNullOrEmpty(this.UpdateVersion))
            {
                MessageBoxHelper.MessageBoxShowWarning("请输入完整的版本信息");
                return;
            }
            string path = Path.Combine(CheckUpdateContext.SetUpPackagePath, "dbScript");
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        /// <summary>
        /// 获取Mysql连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnStr()
        {
            if (string.IsNullOrEmpty(this.CenterIp) || string.IsNullOrEmpty(this.CenterDb) || string.IsNullOrEmpty(this.CenterDbPort) || string.IsNullOrEmpty(this.CenterDbPwd) || string.IsNullOrEmpty(this.CenterDbUser))
            {
                return string.Empty;
            }
            return $"Data Source={this.CenterIp};port={this.CenterDbPort};Initial Catalog={this.CenterDb};Persist Security Info=True;User ID={this.CenterDbUser};Password={this.CenterDbPwd};MAX Pool Size=2000;Min Pool Size=3;Connection Timeout=300;Pooling=true;charset=utf8";
        }

        public bool TestMySql(string connStr)
        {
            string message = "数据库连接失败：";
            try
            {
                connStr = connStr.Trim();
                if (CheckDBUpdateTool.TestMySqlConn(connStr))
                {
                    ShowMessage("数据库连接成功！");
                    this.EnableCheckUpdateResult = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                message += ex.ToString();
            }
            ShowMessage(message);
            MessageBoxHelper.MessageBoxShowWarning("数据库连接失败！");
            return false;
        }

        private void ShowMessage(string message)
        {
            string append = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
            this.CheckResult += append;
        }
    }
}
