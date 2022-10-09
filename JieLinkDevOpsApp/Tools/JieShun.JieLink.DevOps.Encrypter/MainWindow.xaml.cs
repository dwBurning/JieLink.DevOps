using JieShun.JieLink.DevOps.Encrypter.ViewModels;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using ThirdParty.Json.LitJson;
using PartialViewInterface;
using System.Data;
using MySql.Data.MySqlClient;
using JieShun.JieLink.DevOps.Encrypter.Utils;
using System.Threading;
using PartialViewEncrypter.Models;
using PartialViewInterface.Utils;

namespace JieShun.JieLink.DevOps.Encrypter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        public EncrypterViewModel viewModel;

        public static string paramsFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "params.json");



        public MainWindow()
        {
            InitializeComponent();
            viewModel = new EncrypterViewModel();
            DataContext = viewModel;
        }

        

        private async void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            #region 初始化
            LogHelper.CommLogger.Info("--------------------------------------------------------------------------------");
            LogHelper.CommLogger.Info("开始初始化Encrypter");
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName("JieShun.JieLink.DevOps.Encrypter");
            if (ps.Length>1)
            {
                LogHelper.CommLogger.Info("检测到相同进程，退出");
                OnClose();
            }
            Params param = null;
            try
            {
                if (!File.Exists(paramsFilePath))
                {
                    UpdateProgress(0, "未找到参数文件！");
                    LogHelper.CommLogger.Error("未找到参数文件!");
                    return;
                }
                //param = JsonMapper.ToObject<Params>(File.ReadAllText(paramsFilePath));
                param = JsonHelper.GetFromFile<Params>(paramsFilePath);
            }
            catch (Exception ex)
            {
                UpdateProgress(0, "解析参数失败！");
                LogHelper.CommLogger.Error("解析参数失败!");
                return;
            }
            LogHelper.CommLogger.Info($"解析参数完成");
            //Dictionary<string, string> connStrs = new Dictionary<string, string>();
            //Dictionary<string, string> sqlFindColumns = new Dictionary<string, string>();
            //var dbs = param.database.Replace('；',';').Split(';');
            string path = param.path;
            int cmd = param.cmd;
            var command = (EnumCMD)cmd;
            //if (dbs.Length < 1)
            //{
            //    return;
            //}
            //for (int i = 0; i < dbs.Length; i++)
            //{
            //    dbs[i] = dbs[i].Trim();
            //    connStrs.Add(dbs[i], GetConnStr(dbs[i],param.connStr));
            //    sqlFindColumns.Add(dbs[i], GetConnStr(dbs[i], param.sqlFindColumn));
            //}

            #endregion
            LogHelper.CommLogger.Info($"开始执行命令：{command}");
            UpdateProgressSafely(0, "开始加密！");

            EncryptorHelper encrypter = new EncryptorHelper(UpdateProgressSafely, param.database, param.connStr, param.sqlFindColumn, command, path);
            try
            {
                if (cmd >= 0 && cmd <= 3)
                {
                    await encrypter.StartAsync();
                }
                else if (cmd >= 4 && cmd <= 9)
                {
                    await encrypter.StartAsync();
                }
               
            }
            catch (Exception ex)
            {
                viewModel.EncryptMessage = ex.Message + ",程序即将退出！";
                LogHelper.CommLogger.Error(ex.Message + ",程序即将退出！");
            }
            LogHelper.CommLogger.Info("执行完成，关闭Encrypter");
            Thread.Sleep(3000);
            OnClose();
        }

        private string GetConnStr(string dbName,string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
            {
                return "";
            }
            return connStr.Replace("$db$", dbName);
            //return $"Data Source=10.101.90.133;port=10080;User ID=jieLink;Password=js*168;Initial Catalog={dbName};Pooling=true;charset=utf8;";
            //return $"Data Source=localhost;port=3306;User ID=jieLink;Password=js*168;Initial Catalog={dbName};Pooling=true;charset=utf8;";

        }

        



        #region 更新progress
        void UpdateProgressSafely(int progress, string message)
        {
            Dispatcher.Invoke(() => {
                UpdateProgress(progress, message);
            });
        }
        void UpdateProgress(int progress, string message)
        {
            viewModel.EncryptProgress = progress;
            viewModel.EncryptMessage = message;
        }
        #endregion

        #region Window_Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnClose();
        }

        void OnClose()
        {
            Application.Current.Shutdown();
        }
        #endregion

    }
}
