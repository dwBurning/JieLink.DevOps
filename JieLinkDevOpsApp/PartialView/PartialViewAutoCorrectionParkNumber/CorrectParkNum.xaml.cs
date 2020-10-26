using MySql.Data.MySqlClient;
using PartialViewInterface.Utils;
using PartialViewInterface;
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

namespace PartialViewAutoCorrectionParkNumber
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class CorrectParkNum : UserControl, IPartialView
    {
        private AutoCorrectParkNum acp;
        public CorrectParkNum()
        {
            InitializeComponent();
            acp = new AutoCorrectParkNum();
            AutoCorrectParkNum.DeleEvent += AddLogs;
        }

        public string MenuName
        {
            get { return "自动校正车位数"; }
        }

        public string TagName
        {
            get { return "AutoCorrectionParkNumber"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        /// <summary>
        /// 日志行数
        /// </summary>
        private static int LogNum = 0;

        /// <summary>
        /// 开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //检查循环时间输入有效性
                AutoCorrectParkNum.LoopTime = Convert.ToInt32(TextBox_Minute.Text);
                if (AutoCorrectParkNum.LoopTime <= 1)
                    AutoCorrectParkNum.LoopTime = 1;
                if (AutoCorrectParkNum.LoopTime >= 2880)
                    AutoCorrectParkNum.LoopTime = 2880;
                TextBox_Minute.Text = AutoCorrectParkNum.LoopTime.ToString();
            }
            catch
            {
                MessageBoxHelper.MessageBoxShowWarning("请输入正确的时间");
                return;
            }


            //调整界面
            Button_Start.IsEnabled = false;
            Button_Stop.IsEnabled = true;
            TextBox_Minute.IsEnabled = false;
            AddLogs("开始同步");

            acp.DoWork(1);
        }

        /// <summary>
        /// 停止按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button_Stop_Click(object sender, RoutedEventArgs e)
        {
            //停止同步
            acp.StopWork();

            //调整界面
            TextBox_Minute.IsEnabled = true;
            Button_Start.IsEnabled = true;
            Button_Stop.IsEnabled = false;
            AddLogs("停止同步");
        }


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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
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
