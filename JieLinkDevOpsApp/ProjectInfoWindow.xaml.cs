using Panuon.UI.Silver;
using System.Windows.Controls;
using System.Windows.Markup;
using JieShun.JieLink.DevOps.App.ViewModels;
using PartialViewInterface;

namespace JieShun.JieLink.DevOps.App
{
    /// <summary>
    /// ProjectInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectInfoWindow : WindowX, IComponentConnector
    {
        public ProjectInfoWindow()
        {
            InitializeComponent();
            ContentControl.Content = MainWindowViewModel.partialViewDic["SystemSetting"];
        }

        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EnvironmentInfo.ProjectNo)
                || string.IsNullOrEmpty(EnvironmentInfo.RemoteAccount)
                || string.IsNullOrEmpty(EnvironmentInfo.RemotePassword)
                || string.IsNullOrEmpty(EnvironmentInfo.ServerUrl))
            {
                Notice.Show("检测到项目信息有误，请重新确认！", "通知", 3, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(EnvironmentInfo.ConnectionString))
            {
                Notice.Show("检测到数据库连接有误，请重新确认！", "通知", 3, MessageBoxIcon.Warning);
                return;
            }

            this.Close();
        }
    }
}
