using MySql.Data.MySqlClient;
using PartialViewInterface;
using PartialViewInterface.Utils;
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
using PartialViewFacePicBackUp.ViewModels;

namespace PartialViewFacePicBackUp
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class FacePicBackUp : UserControl, IPartialView
    {
        FacePicBackUpOptViewModel viewModel;
        public FacePicBackUp()
        {
            InitializeComponent();
            //FacePicBackUpOptViewModel.DeleEvent += AddLogs;
            viewModel = new FacePicBackUpOptViewModel();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "人脸检测/备份"; }
        }

        public string TagName
        {
            get { return "FacePicBackUp"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 902; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
                //viewModel.GetDoorServerInfo();
                this.IsEnabled = true;

            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("未查询到jielink2.x的数据库信息，请确认数据库配置信息是否正确？");
                this.IsEnabled = false;
            }
        }

        /// <summary>
        /// 右击菜单清屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Message = "";
            //RichTextBox_Text.Document.Blocks.Clear();
        }

        private void RichTextBox_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            RichTextBox_Text.ScrollToEnd();
        }
    }
}
