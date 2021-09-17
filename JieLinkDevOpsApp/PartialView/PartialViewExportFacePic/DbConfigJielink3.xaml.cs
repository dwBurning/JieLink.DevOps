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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface.Utils;
using DBUtility;

namespace PartialViewExportFacePic
{
    /// <summary>
    /// DbConfig.xaml 的交互逻辑
    /// </summary>
    public partial class DbConfigJielink3 : WindowX
    {
        public DbConfigJielink3()
        {
            InitializeComponent();
        }

        public string DbSQLConnString { get; private set; }
        public bool IsSqlCon = false;

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            DbSQLConnString = $"Data Source={txtBoxIp.Text};port={txtBoxPort.Text};User ID={txtBoxDbUser.Text};Password={txtBoxDbPwd.Password};Initial Catalog={txtBoxDb.Text};Pooling=true;charset=utf8;";
            
            //测试用
            //DbSQLConnString  = $"Data Source = 10.101.90.133; port = 10080; User ID = jieLink; Password = js*168; Initial Catalog = jielink; Pooling = true; charset = utf8;";
            try
            {
                string sql = "select status from sc_person limit 1";

                if (DBUtility.MySqlHelper.TestConnection(DbSQLConnString, sql))
                {
                    IsSqlCon = true;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    IsSqlCon = false;
                    MessageBoxHelper.MessageBoxShowWarning("SQL连接错误，请重新输入。如需取消请点右上角的×");
                }
            }
            catch (Exception)
            {
                IsSqlCon = false;
                MessageBoxHelper.MessageBoxShowWarning("SQL连接错误，请重新输入。如需取消请点右上角的×");
            }
        }
    }
}
