using MySql.Data.MySqlClient;
using PartialViewDataArchiving.ViewModels;
using PartialViewInterface;
using PartialViewInterface.Utils;
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

namespace PartialViewDataArchiving
{
    /// <summary>
    /// DataArchiving.xaml 的交互逻辑
    /// </summary>
    public partial class DataArchiving : UserControl, IPartialView
    {
        DataArchivingViewModel viewModel;
        public DataArchiving()
        {
            InitializeComponent();
            viewModel = DataArchivingViewModel.Instance();
            DataContext = viewModel;

            archiveMonths.Add(3, "归档3个月前的记录");
            archiveMonths.Add(4, "归档4个月前的记录");
            archiveMonths.Add(5, "归档5个月前的记录");
            archiveMonths.Add(6, "归档6个月前的记录");
            archiveMonths.Add(7, "归档7个月前的记录");
            archiveMonths.Add(8, "归档8个月前的记录");
            archiveMonths.Add(9, "归档9个月前的记录");
            archiveMonths.Add(10, "归档10个月前的记录");
            archiveMonths.Add(11, "归档11个月前的记录");
            archiveMonths.Add(12, "归档12个月前的记录");
        }

        public string MenuName
        {
            get { return "数据归档"; }
        }

        public string TagName
        {
            get { return "DataArchiving"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        Dictionary<int, string> archiveMonths = new Dictionary<int, string>();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.DataSource.Clear();
                foreach (var month in archiveMonths)
                {
                    viewModel.DataSource.Add(month.Key, month.Value);
                }
                viewModel.SelectIndex = EnvironmentInfo.AutoArchiveMonth;
                string sql = "select ValueText from sys_key_value_setting where KeyID='AutoArchiveDaysBefore'";
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    viewModel.DataSource.Clear();
                    var archiveMonth = archiveMonths.Where(x => x.Key >= (int.Parse(dt.Rows[0][0].ToString()) / 30));
                    foreach (var month in archiveMonth)
                    {
                        viewModel.DataSource.Add(month.Key, month.Value);

                    }

                    int index = archiveMonth.ToList().FindIndex(x => x.Key == EnvironmentInfo.AutoArchiveMonth);
                    viewModel.SelectIndex = index;
                }

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
