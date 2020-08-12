using MySql.Data.MySqlClient;
using PartialViewInterface;
using PartialViewMySqlBackUp.Models;
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
        public List<BackUpTable> tables { get; set; }
        public MySqlBackUp()
        {
            InitializeComponent();
            DataContext = this;
            GetTables();
        }

        public void GetTables()
        {
            tables = new List<BackUpTable>();
            string sql = $"select table_name from information_schema.`TABLES` where TABLE_SCHEMA='{EnvironmentInfo.DbConnEntity.DbName}';";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                tables.Add(new BackUpTable() { TableName = dr["table_name"].ToString(), IsChecked = false });
            }

            //dgTables.ItemsSource = tables;
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
            //GetTables();
        }

        private void btnAddPolicy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
