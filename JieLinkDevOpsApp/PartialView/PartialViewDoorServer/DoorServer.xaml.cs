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
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 900; }
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

                #region 检测卡不能自动下载问题
                if (viewModel.CheckSyncDoorNum())
                {
                    MessageBoxHelper.MessageBoxShowSuccess("检测到sync_doornum数值错误，会导致卡数据无法自动下载！已自动修复！");
                }
                #endregion

                #region 检测门禁服务配置文件中MAC地址
                if (viewModel.CheckDoorServerMac())
                {
                    string str = string.Empty;
                    if (viewModel.IsConfigMac == "true")
                    {
                        str = string.Format("检测到门禁服务配置MAC地址错误，CONFIG文件中MAC地址配置为{0}，数据库中门禁服务MAC地址为{1}，是否自动修复？", viewModel.ConfigMAC, viewModel.sqlMac);
                    }
                    else
                    {
                        str = string.Format("检测到门禁服务配置文件未启用MAC地址配置，可自动配置MAC地址为{0}，是否自动配置？", viewModel.sqlMac);
                    }

                    if (MessageBoxHelper.MessageBoxShowQuestion(str) == MessageBoxResult.Yes)
                    {
                        if(viewModel.FixDoorServerMac())
                            MessageBoxHelper.MessageBoxShowSuccess("配置门禁服务MAC地址完成！已结束门禁进程等待进程自动重启！");
                        else
                            MessageBoxHelper.MessageBoxShowError("配置门禁服务MAC地址失败！");
                    }

                }


                #endregion
            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
                this.IsEnabled = false;
            }
        }
    }
}
