using Newtonsoft.Json;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewInterface.ViewModels;

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
            DataContext = viewModel;
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
            ConfigHelper.WriterAppConfig("ProjectInfo", JsonConvert.SerializeObject(viewModel));
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
        }
    }
}
