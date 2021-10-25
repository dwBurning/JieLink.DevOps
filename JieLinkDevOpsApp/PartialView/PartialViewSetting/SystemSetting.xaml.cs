using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using PartialViewInterface.ViewModels;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.IO;
using MySql.Data;
using PartialViewSetting.ViewModel;
using PartialViewInterface.DB;

namespace PartialViewSetting
{
    /// <summary>
    /// SystemSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting : UserControl, IPartialView
    {
        private ProjectInfoWindowViewModel viewModel;
        private DBConnViewModel dbConnViewModel;
        private KeyValueSettingManager manager;
        public SystemSetting()
        {
            InitializeComponent();
            viewModel = new ProjectInfoWindowViewModel();
            gridProjectConfig.DataContext = viewModel;

            dbConnViewModel = new DBConnViewModel();
            gridDBConfig.DataContext = dbConnViewModel;

            manager = new KeyValueSettingManager();
        }

        public string MenuName
        {
            get { return "设置"; }
        }

        public string TagName
        {
            get { return "SystemSetting"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.None; }
        }

        public int Order
        {
            get { return 800; }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string url = txtServerUrl.Text;
            EnvironmentInfo.ServerUrl = url.Trim();
            //ConfigHelper.WriterAppConfig("ServerUrl", url);

            manager.WriteSetting(new KeyValueSetting()
            {
                KeyId = "ServerUrl",
                ValueText = url
            });

            EnvironmentInfo.ProjectNo = viewModel.ProjectNo;
            EnvironmentInfo.ProjectName = viewModel.ProjectName;
            EnvironmentInfo.ProjectVersion = viewModel.ProjectVersion;
            EnvironmentInfo.RemoteAccount = viewModel.RemoteAccount;
            EnvironmentInfo.RemotePassword = viewModel.RemotePassword;
            EnvironmentInfo.ContactName = viewModel.ContactName;
            EnvironmentInfo.ContactPhone = viewModel.ContactPhone;

            if (string.IsNullOrEmpty(EnvironmentInfo.ProjectNo)
                || string.IsNullOrEmpty(EnvironmentInfo.RemoteAccount)
                || string.IsNullOrEmpty(EnvironmentInfo.RemotePassword)
                || string.IsNullOrEmpty(EnvironmentInfo.ServerUrl))
            {
                Notice.Show("检测到配置信息有误，请重新确认！", "通知", 3, MessageBoxIcon.Warning);
                return;
            }

            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.ProjectNo = viewModel.ProjectNo;
            projectInfo.ProjectName = viewModel.ProjectName;
            projectInfo.ProjectVersion = viewModel.ProjectVersion;
            projectInfo.RemoteAccount = viewModel.RemoteAccount;
            projectInfo.RemotePassword = viewModel.RemotePassword;
            projectInfo.ContactName = viewModel.ContactName;
            projectInfo.ContactPhone = viewModel.ContactPhone;

            //ConfigHelper.WriterAppConfig("ProjectInfo", JsonConvert.SerializeObject(projectInfo));

            manager.WriteSetting(new KeyValueSetting()
            {
                KeyId = "ProjectInfo",
                ValueText = JsonConvert.SerializeObject(projectInfo)
            });

            Notice.Show("保存成功", "通知", 3, MessageBoxIcon.Success);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            
            txtServerUrl.Text = EnvironmentInfo.ServerUrl;
            if (string.IsNullOrEmpty(EnvironmentInfo.ProjectVersion))
            {
                #region 未配置的时候，尝试自动获取
                try
                {
                    var process = Process.GetProcessesByName("SmartCenter.Host").FirstOrDefault();
                    if (process != null)
                    {
                        string settingPath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, "Config", "Settings.ini");
                        string connectionString = DESEncrypt.Decrypt(IniSetting.Read(settingPath, "Release", "DataConnectionString", ""));
                        MySqlConnectionStringBuilder mysqlsb = new MySqlConnectionStringBuilder(connectionString);
                        EnvironmentInfo.DbConnEntity = new DbConnEntity();
                        EnvironmentInfo.DbConnEntity.Ip = mysqlsb.Server;
                        EnvironmentInfo.DbConnEntity.Port = (int)mysqlsb.Port;
                        EnvironmentInfo.DbConnEntity.UserName = mysqlsb.UserID;
                        EnvironmentInfo.DbConnEntity.Password = mysqlsb.Password;
                        EnvironmentInfo.DbConnEntity.DbName = mysqlsb.Database;

                        EnvironmentInfo.ProjectNo = MySqlHelper.ExecuteScalar(connectionString, "select ValueText from sys_key_value_setting where KeyID='ProjectCode'").ToString();

                        EnvironmentInfo.ProjectName = MySqlHelper.ExecuteScalar(connectionString, "select ValueText from sys_key_value_setting where KeyID='ProjectName'").ToString();
                        //EnvironmentInfo.ContactName = MySqlHelper.ExecuteScalar(connectionString, "select ValueText from sys_key_value_setting where KeyID='ProjectName'").ToString();

                        EnvironmentInfo.ProjectVersion = MySqlHelper.ExecuteScalar(connectionString, "select ValueText from sys_key_value_setting where KeyID='ProjectVersion'").ToString();



                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                #endregion
            }
            viewModel.ProjectNo = EnvironmentInfo.ProjectNo;
            viewModel.ProjectName = EnvironmentInfo.ProjectName;
            viewModel.ProjectVersion = EnvironmentInfo.ProjectVersion;
            viewModel.RemoteAccount = EnvironmentInfo.RemoteAccount;
            viewModel.RemotePassword = EnvironmentInfo.RemotePassword;
            viewModel.ContactName = EnvironmentInfo.ContactName;
            viewModel.ContactPhone = EnvironmentInfo.ContactPhone;

            dbConnViewModel.Ip = EnvironmentInfo.DbConnEntity.Ip;
            dbConnViewModel.Port = EnvironmentInfo.DbConnEntity.Port;
            dbConnViewModel.UserName = EnvironmentInfo.DbConnEntity.UserName;
            txtCenterDbPwd.Password = EnvironmentInfo.DbConnEntity.Password;
            dbConnViewModel.Password = EnvironmentInfo.DbConnEntity.Password;
            dbConnViewModel.DbName = EnvironmentInfo.DbConnEntity.DbName;
            dbConnViewModel.SelectIndex = EnvironmentInfo.IsJieLink3x ? 1 : 0;


        }

        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            //发现127开头的IP可以随便填写，都能连接成功，但是最终命令行执行脚本的时候会连接失败
            if (txtCenterIp.Text.StartsWith("127") && !txtCenterIp.Text.Equals("127.0.0.1"))
            {
                txtCenterIp.Text = "127.0.0.1";
            }

            dbConnViewModel.Password = txtCenterDbPwd.Password;

            string connStr = $"Data Source={dbConnViewModel.Ip};port={dbConnViewModel.Port};User ID={dbConnViewModel.UserName};Password={dbConnViewModel.Password};Initial Catalog=mysql;";

            try
            {
                MySqlHelper.ExecuteDataset(connStr, "select * from `user` limit 1");

                Notice.Show("中心数据库连接成功,已自动保存!", "通知", 3, MessageBoxIcon.Success);
                //存储中心连接字符串

                EnvironmentInfo.DbConnEntity.Ip = dbConnViewModel.Ip;
                EnvironmentInfo.DbConnEntity.Port = dbConnViewModel.Port;
                EnvironmentInfo.DbConnEntity.UserName = dbConnViewModel.UserName;
                EnvironmentInfo.DbConnEntity.Password = dbConnViewModel.Password;
                EnvironmentInfo.DbConnEntity.DbName = dbConnViewModel.DbName;
                //ConfigHelper.WriterAppConfig("ConnectionString", JsonHelper.SerializeObject(EnvironmentInfo.DbConnEntity));
                manager.WriteSetting(new KeyValueSetting()
                {
                    KeyId = "ConnectionString",
                    ValueText = JsonHelper.SerializeObject(EnvironmentInfo.DbConnEntity)
                });

                if (dbConnViewModel.SelectIndex == 0)
                {
                    EnvironmentInfo.IsJieLink3x = false;

                    manager.WriteSetting(new KeyValueSetting()
                    {
                        KeyId = "IsJieLink3x",
                        ValueText = "0"
                    });
                }
                else
                {
                    EnvironmentInfo.IsJieLink3x = true;
                    manager.WriteSetting(new KeyValueSetting()
                    {
                        KeyId = "IsJieLink3x",
                        ValueText = "1"
                    });
                }
            }
            catch (Exception)
            {
                Notice.Show("数据库连接失败!", "通知", 3, MessageBoxIcon.Warning);
            }
        }
    }
}
