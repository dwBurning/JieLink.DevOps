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

namespace PartialViewSyncTool
{
    /// <summary>
    /// DbConfig.xaml 的交互逻辑
    /// </summary>
    public partial class DbConfig : WindowX
    {
        public DbConfig(string Ip)
        {
            InitializeComponent();
            this.Ip = Ip;
            txtBoxIp.Text = Ip;
        }
        public string Ip { get; set; }

        public string DbConnString { get; private set; }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            string boxConn = $"Data Source={Ip};port={txtBoxDbPort.Text};User ID={txtBoxDbUser.Text};Password={txtBoxDbPwd.Password};Initial Catalog=smartbox;";

            try
            {
                string cmd = "select * from sys_boxinformation";
                MySqlHelper.ExecuteDataset(boxConn, cmd);
                DbConnString = boxConn;
                //20200714 存疑
                //this.DialogResult = DialogResult.OK;
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("连接错误，请重新输入");
            }
        }
    }
}
