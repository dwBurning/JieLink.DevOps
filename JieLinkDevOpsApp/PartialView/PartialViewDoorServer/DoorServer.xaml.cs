using MySql.Data.MySqlClient;
using PartialViewDoorServer.ViewModels;
using PartialViewInterface;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PartialViewDoorServer
{
    /// <summary>
    /// DoorServer.xaml 的交互逻辑
    /// </summary>
    public partial class DoorServer : UserControl, IPartialView
    {
        DoorServerOptViewModel viewModel = new DoorServerOptViewModel();

        public DoorServer()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "门禁服务"; }
        }

        public string TagName
        {
            get { return "DoorServer"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.DoorServer; }
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var selectItem = (DoorServerInfo)e.AddedItems[0];
            DoorServerOptViewModel.curDoorServer = selectItem;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.GetDoorServerInfo();
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
