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
using PartialViewInterface.Utils;
using MySql.Data.MySqlClient;
using PartialViewCheckUpload.ViewModels;
using Panuon.UI.Silver;

namespace PartialViewCheckUpload
{
    public partial class CheckUpload : UserControl, IPartialView
    {
        CheckUploadViewModels viewModel;
        public CheckUpload()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "上传检测"; }
        }

        public string TagName
        {
            get { return "CheckUpload"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
            //    //if (viewModel.Tables.Count == 0)
            //    //{
            //    //    viewModel.GetTables();
            //    //    viewModel.ShowMessage("数据库连接成功");
            //    //}
            //    this.IsEnabled = true;

            //}
            //catch (Exception)
            //{
            //    MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
            //    this.IsEnabled = false;
            //}
        }
    }
}
