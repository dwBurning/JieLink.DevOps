using PartialViewHealthMonitor.CheckUpdate;
using PartialViewHealthMonitor.Models;
using PartialViewHealthMonitor.ViewModels;
using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using System;
using System.Windows;
using System.Windows.Controls;


namespace PartialViewHealthMonitor
{
    /// <summary>
    /// VersionUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class VersionUpdate : UserControl, IPartialView
    {
        VersionUpdateViewModel viewModel;

        public VersionUpdate()
        {
            InitializeComponent();
            viewModel = new VersionUpdateViewModel();
            this.DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "版本信息"; }
        }

        public string TagName
        {
            get { return "VersionUpdate"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.None; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DevOpsProduct product = DevOpsAPI.ReportVersion();
                if (product != null)
                {
                    viewModel.LastVersion = product.ProductVersion;

                    if (CheckUpdateHelper.CompareVersion(EnvironmentInfo.CurrentVersion, product.ProductVersion) < 0)
                    {
                        viewModel.canExecute = true;
                    }
                    else
                    {
                        viewModel.BtnContent = "当前已是最新版本";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
