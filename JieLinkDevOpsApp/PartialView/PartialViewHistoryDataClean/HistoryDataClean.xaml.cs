using MySql.Data.MySqlClient;
using PartialViewHistoryDataClean.ViewModels;
using PartialViewInterface;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PartialViewHistoryDataClean
{
    /// <summary>
    /// HistoryDataClean.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryDataClean : UserControl, IPartialView
    {
        HistoryDataCleanViewModel viewModel;

        public HistoryDataClean()
        {
            InitializeComponent();
            viewModel = new HistoryDataCleanViewModel();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "数据清理"; }
        }

        public string TagName
        {
            get { return "HistoryDataClean"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 800; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Global.ValidV2(new Action<string, bool>((message, result) =>
            {
                if (!result)
                {
                    MessageBoxHelper.MessageBoxShowWarning(message);
                }

                this.IsEnabled = result;
            }));
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string xmppidb = System.IO.Path.Combine(folderBrowserDialog.SelectedPath.Trim(), "sync_xmpp.ibd");
                if (File.Exists(xmppidb))
                {
                    viewModel.DBJKDataPath = folderBrowserDialog.SelectedPath.Trim();
                    FileInfo fileInfo = new FileInfo(xmppidb);
                    viewModel.ConvertToSizeString(fileInfo.Length);
                }
                else
                {
                    MessageBoxHelper.MessageBoxShowWarning("sync_xmpp.ibd不存在！");
                }
            }
        }
    }
}
