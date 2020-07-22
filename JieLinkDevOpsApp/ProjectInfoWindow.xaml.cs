using JieShun.JieLink.DevOps.App.ViewModels;
using Panuon.UI.Silver;
using PartialViewInterface;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;

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
            ViewModel = new ProjectInfoWindowViewModel();
            DataContext = ViewModel;
        }

        public ProjectInfoWindowViewModel ViewModel { get; set; }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentInfo.ProjectNo = ViewModel.ProjectNo;
            EnvironmentInfo.RemoteAccount = ViewModel.RemoteAccount;
            EnvironmentInfo.RemotePassword = ViewModel.RemotePassword;
            EnvironmentInfo.ContactName = ViewModel.ContactName;
            EnvironmentInfo.ContactPhone = ViewModel.ContactPhone;
            FileHelper.WriterAppConfig("ProjectInfo", JsonConvert.SerializeObject(ViewModel));
            Notice.Show("保存成功", "通知", 3, MessageBoxIcon.Success);
            this.Close();
        }
    }
}
