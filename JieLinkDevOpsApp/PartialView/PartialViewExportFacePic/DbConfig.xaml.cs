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
            //this.Ip = Ip;
            //txtBoxIp.Text = Ip;
        }
        public string Ip { get; set; }

        public string DbConnString { get; private set; }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //string boxConn2 = $"Data Source={Ip};User ID={txtBoxDbUser.Text};Password={txtBoxDbPwd.Password};Initial Catalog=smartbox;";
            string boxConn = $"Data Source={txtBoxIp.Text};Initial Catalog = {txtBoxDb.Text};User ID={txtBoxDbUser.Text};Password={txtBoxDbPwd.Password};" ;
            try
            {
                SQLHelper.TestConnection(boxConn,"select * from mc.setting");
                //string cmd = "select * from sys_boxinformation";
                //MySqlHelper.ExecuteDataset(boxConn, cmd);
                //DbConnString = boxConn;

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
