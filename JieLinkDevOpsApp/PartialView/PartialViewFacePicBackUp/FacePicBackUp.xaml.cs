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
        FacePicBackUpOptViewModel viewModel = new FacePicBackUpOptViewModel();
        public FacePicBackUp()
        {
            FacePicBackUpOptViewModel.DeleEvent += AddLogs;
            FacePicBackUpOptViewModel.DeleEventShowWarn += MessageBoxShowWarning;
            InitializeComponent();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "人脸图片备份"; }
        }

        public string TagName
        {
            get { return "FacePicBackUp"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.DoorServer; }
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
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
                this.IsEnabled = false;
            }
        }

        /// <summary>
        /// 日志行数
        /// </summary>
        private static int LogNum = 0;
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="str"></param>
        public void AddLogs(string str)
        {
            LogNum++;
            if (LogNum > 200)
            {
                RichTextBox_Text.Document.Blocks.Clear();
                LogNum = 0;
            }
            RichTextBox_Text.AppendText(DateTime.Now.ToString() + ":" + str + "\r");
            RichTextBox_Text.ScrollToEnd();
        }

        public void MessageBoxShowWarning(string str)
        {
            MessageBoxHelper.MessageBoxShowWarning(str);
        }
    }
}
