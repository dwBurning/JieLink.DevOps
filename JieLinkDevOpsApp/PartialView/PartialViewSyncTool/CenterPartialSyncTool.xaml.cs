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
using PartialViewInterface;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;
using System.IO;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using Panuon.UI.Silver;
using PartialViewSyncTool.SyncToolViewModel;

namespace PartialViewSyncTool
{
    /// <summary>
    /// CenterPartialSyncTool.xaml 的交互逻辑
    /// </summary>
    public partial class CenterPartialSyncTool : UserControl, IPartialView
    {
        public string MenuName
        {
            get { return "数据同步"; }
        }

        public string TagName
        {
            get { return "CenterPartialSyncTool"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        DataSyncViewModel dataSyncViewModel;
        DataCheckViewModel dataCheckViewModel;
        public CenterPartialSyncTool()
        {
            InitializeComponent();
            dataSyncViewModel = new DataSyncViewModel();
            gridSyncTool.DataContext = dataSyncViewModel;

            dataCheckViewModel = new DataCheckViewModel();
            gridDataCheck.DataContext = dataCheckViewModel;
        }

        private void chbVersion_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            { return; }
            dataSyncViewModel.CMD = "742;743;820;821;74A;811";
        }

        private void chbVersion_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            { return; }
            dataSyncViewModel.CMD = "82A";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
                this.IsEnabled = true;
            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
                this.IsEnabled = false;
            }
        }
    }
}
