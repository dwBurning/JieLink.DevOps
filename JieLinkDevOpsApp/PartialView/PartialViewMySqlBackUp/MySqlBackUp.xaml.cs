using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewMySqlBackUp.Models;
using PartialViewMySqlBackUp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace PartialViewMySqlBackUp
{
    /// <summary>
    /// MySqlBackUp.xaml 的交互逻辑
    /// </summary>
    public partial class MySqlBackUp : UserControl, IPartialView
    {
        MySqlBackUpViewModel viewModel;
        public MySqlBackUp()
        {
            InitializeComponent();
            viewModel = MySqlBackUpViewModel.Instance();
            DataContext = viewModel;
            wpDayOfWeek.DataContext = viewModel.CurrentPolicy;
            dtpTime.DataContext = viewModel.CurrentPolicy;
            gridBackUpType.DataContext = viewModel.CurrentPolicy;
            gridManualBackUpType.DataContext = viewModel.CurrentPolicy;
        }

        public string MenuName
        {
            get { return "数据备份"; }
        }

        public string TagName
        {
            get { return "MySqlBackUp"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Common; }
        }

        public int Order
        {
            get { return 2000; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select UUID();");
                if (viewModel.Tables.Count == 0)
                {
                    viewModel.GetTables();
                    viewModel.ShowMessage("数据库连接成功");
                }
                this.IsEnabled = true;

            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
                this.IsEnabled = false;
            }
        }

        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.TaskBackUpPath = folderBrowserDialog.SelectedPath.Trim() + "\\jielink_bdbackup";

                if (!Directory.Exists(viewModel.TaskBackUpPath))
                {
                    Directory.CreateDirectory(viewModel.TaskBackUpPath);
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var selectItem = (BackUpPolicy)e.AddedItems[0];

            viewModel.CurrentPolicy.Sunday = selectItem.Sunday;
            viewModel.CurrentPolicy.Monday = selectItem.Monday;
            viewModel.CurrentPolicy.Tuesday = selectItem.Tuesday;
            viewModel.CurrentPolicy.Wednesday = selectItem.Wednesday;
            viewModel.CurrentPolicy.Thursday = selectItem.Thursday;
            viewModel.CurrentPolicy.Friday = selectItem.Friday;
            viewModel.CurrentPolicy.Saturday = selectItem.Saturday;
            viewModel.CurrentPolicy.SelectedTime = selectItem.SelectedTime;
            viewModel.CurrentPolicy.IsTaskBackUpDataBase = selectItem.IsTaskBackUpDataBase;
            viewModel.CurrentPolicy.IsTaskBackUpTables = selectItem.IsTaskBackUpTables;
            viewModel.CurrentPolicy.SelectedDatabase = selectItem.SelectedDatabase;
            viewModel.CurrentPolicyBak = viewModel.DeepCopy(viewModel.CurrentPolicy);
            viewModel.SetTables(selectItem.SelectedDatabase);
        }

        private void dgDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var backUpDatabase = dgDatabases.SelectedItem as BackUpDatabase;
            EnvironmentInfo.SelectedDatabase = backUpDatabase.DatabaseName;
            viewModel.CurrentPolicy.SelectedDatabase = EnvironmentInfo.SelectedDatabase; //避免选择数据库后直接编辑策略，不触发ListBox_SelectionChanged
            viewModel.SetTables(backUpDatabase.DatabaseName);
        }
    }
}
