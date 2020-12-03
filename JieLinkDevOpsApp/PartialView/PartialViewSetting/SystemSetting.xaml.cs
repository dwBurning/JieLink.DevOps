using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using PartialViewInterface.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;


namespace PartialViewSetting
{
    /// <summary>
    /// SystemSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting : UserControl, IPartialView
    {
        private ProjectInfoWindowViewModel viewModel;
        public SystemSetting()
        {
            InitializeComponent();
            viewModel = new ProjectInfoWindowViewModel();
            gridProjectConfig.DataContext = viewModel;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string url = txtServerUrl.Text;
            EnvironmentInfo.ServerUrl = url.Trim();
            ConfigHelper.WriterAppConfig("ServerUrl", url);
            EnvironmentInfo.ProjectNo = viewModel.ProjectNo;
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
            projectInfo.RemoteAccount = viewModel.RemoteAccount;
            projectInfo.RemotePassword = viewModel.RemotePassword;
            projectInfo.ContactName = viewModel.ContactName;
            projectInfo.ContactPhone = viewModel.ContactPhone;

            ConfigHelper.WriterAppConfig("ProjectInfo", JsonConvert.SerializeObject(projectInfo));
            Notice.Show("保存成功", "通知", 3, MessageBoxIcon.Success);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            string url = ConfigHelper.ReadAppConfig("ServerUrl");
            txtServerUrl.Text = url;
            EnvironmentInfo.ServerUrl = url;
            viewModel.ProjectNo = EnvironmentInfo.ProjectNo;
            viewModel.RemoteAccount = EnvironmentInfo.RemoteAccount;
            viewModel.RemotePassword = EnvironmentInfo.RemotePassword;
            viewModel.ContactName = EnvironmentInfo.ContactName;
            viewModel.ContactPhone = EnvironmentInfo.ContactPhone;

            txtCenterIp.Text = EnvironmentInfo.DbConnEntity.Ip;
            txtCenterDbPort.Text = EnvironmentInfo.DbConnEntity.Port.ToString();
            txtCenterDbUser.Text = EnvironmentInfo.DbConnEntity.UserName;
            txtCenterDbPwd.Password = EnvironmentInfo.DbConnEntity.Password;
            txtCenterDb.Text = EnvironmentInfo.DbConnEntity.DbName;
        }

        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            string connStr = $"Data Source={txtCenterIp.Text};port={txtCenterDbPort.Text};User ID={txtCenterDbUser.Text};Password={txtCenterDbPwd.Password};Initial Catalog={txtCenterDb.Text};";

            try
            {
                MySqlHelper.ExecuteDataset(connStr, "select * from sys_user limit 1");
                Notice.Show("中心数据库连接成功,已自动保存!", "通知", 3, MessageBoxIcon.Success);
                //存储中心连接字符串

                EnvironmentInfo.DbConnEntity.Ip = txtCenterIp.Text;
                EnvironmentInfo.DbConnEntity.Port = Convert.ToInt32(txtCenterDbPort.Text);
                EnvironmentInfo.DbConnEntity.UserName = txtCenterDbUser.Text;
                EnvironmentInfo.DbConnEntity.Password = txtCenterDbPwd.Password;
                EnvironmentInfo.DbConnEntity.DbName = txtCenterDb.Text;
                ConfigHelper.WriterAppConfig("ConnectionString", JsonHelper.SerializeObject(EnvironmentInfo.DbConnEntity));

            }
            catch (Exception)
            {
                Notice.Show("数据库连接失败!", "通知", 3, MessageBoxIcon.Warning);
            }
        }
    }
}
