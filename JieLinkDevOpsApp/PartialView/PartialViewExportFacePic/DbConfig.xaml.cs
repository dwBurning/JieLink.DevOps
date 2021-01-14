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
    public partial class DbConfig : WindowX
    {
        public DbConfig()
        {
            InitializeComponent();
        }

        public string DbSQLConnString { get; private set; }
        public bool IsSqlCon = false;

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //string boxConn2 = $"Data Source={Ip};User ID={txtBoxDbUser.Text};Password={txtBoxDbPwd.Password};Initial Catalog=smartbox;";
            //设置了超时时间 防止过长等待
            DbSQLConnString = $"Data Source={txtBoxIp.Text};Initial Catalog = {txtBoxDb.Text};User ID={txtBoxDbUser.Text};Password={txtBoxDbPwd.Password};Connect Timeout = 3;" ;
            try
            {
                SQLHelper.TestConnection(DbSQLConnString, "select * from mc.setting");
                //string cmd = "select * from sys_boxinformation";
                //MySqlHelper.ExecuteDataset(boxConn, cmd);
                //DbConnString = boxConn;
                IsSqlCon = true;
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception)
            {
                IsSqlCon = false;
                MessageBoxHelper.MessageBoxShowWarning("SQL连接错误，请重新输入。如需取消请点右上角的×");
            }
        }
    }
}
