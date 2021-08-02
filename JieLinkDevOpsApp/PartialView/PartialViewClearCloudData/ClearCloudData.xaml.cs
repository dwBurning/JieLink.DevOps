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
using MySql.Data.MySqlClient;
using PartialViewInterface;
using PartialViewInterface.Utils;

namespace PartialViewClearCloudData
{
    /// <summary>
    /// ClearCloudData.xaml 的交互逻辑
    /// </summary>
    public partial class ClearCloudData : UserControl, IPartialView
    {
        public string MenuName
        {
            get { return "平台设备清理"; }
        }

        public string TagName
        {
            get { return "PartialViewClearCloudData"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 1100; }
        }

        ClearCloudDeviceDataViewModel vmodel;

        public ClearCloudData()
        {
            InitializeComponent();
            vmodel = new ClearCloudDeviceDataViewModel();
            this.gridDevice.DataContext = vmodel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
            }
        }
    }
}
