using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewMySqlBackUp.Models;
using PartialViewMySqlBackUp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
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
            viewModel = new MySqlBackUpViewModel();
            DataContext = viewModel;
            wpDayOfWeek.DataContext = viewModel.CurrentPolicy;
            dtpTime.DataContext = viewModel.CurrentPolicy;
            gridBackUpType.DataContext = viewModel.CurrentPolicy;
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
            get { return MenuType.Center; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EnvironmentInfo.ConnectionString))
            {
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;
            }
        }

        private void btnAddPolicy_Click(object sender, RoutedEventArgs e)
        {
            BackUpPolicy policy;
            viewModel.CurrentPolicy.SelectedTime = dtpTime.SelectedDateTime;//通过绑定无法获取到值 原因不明
            if (viewModel.CurrentPolicy.IsTaskBackUpDataBase)
            {
                policy = viewModel.Policys.Where(x => x.IsTaskBackUpDataBase).FirstOrDefault();
            }
            else
            {
                policy = viewModel.Policys.Where(x => x.IsTaskBackUpTables).FirstOrDefault();
            }

            if (policy == null)
            {
                viewModel.Policys.Add(new BackUpPolicy()
                {
                    Sunday = viewModel.CurrentPolicy.Sunday,
                    Monday = viewModel.CurrentPolicy.Monday,
                    Tuesday = viewModel.CurrentPolicy.Tuesday,
                    Wednesday = viewModel.CurrentPolicy.Wednesday,
                    Thursday = viewModel.CurrentPolicy.Thursday,
                    Friday = viewModel.CurrentPolicy.Friday,
                    Saturday = viewModel.CurrentPolicy.Saturday,
                    
                    SelectedTime = viewModel.CurrentPolicy.SelectedTime,
                    IsTaskBackUpDataBase = viewModel.CurrentPolicy.IsTaskBackUpDataBase,
                    IsTaskBackUpTables = viewModel.CurrentPolicy.IsTaskBackUpTables,
                    ItemString = viewModel.CurrentPolicy.PolicyToString
                });
            }
            else
            {
                policy.Sunday = viewModel.CurrentPolicy.Sunday;
                policy.Monday = viewModel.CurrentPolicy.Monday;
                policy.Tuesday = viewModel.CurrentPolicy.Tuesday;
                policy.Wednesday = viewModel.CurrentPolicy.Wednesday;
                policy.Thursday = viewModel.CurrentPolicy.Thursday;
                policy.Friday = viewModel.CurrentPolicy.Friday;
                policy.Saturday = viewModel.CurrentPolicy.Saturday;
                policy.SelectedTime = viewModel.CurrentPolicy.SelectedTime;
                policy.IsTaskBackUpDataBase = viewModel.CurrentPolicy.IsTaskBackUpDataBase;
                policy.IsTaskBackUpTables = viewModel.CurrentPolicy.IsTaskBackUpTables;
                policy.ItemString = viewModel.CurrentPolicy.PolicyToString;
            }

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbChooseAll_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Tables.ForEach(x => x.IsChecked = viewModel.IsSelectedAll);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
        }
    }
}
