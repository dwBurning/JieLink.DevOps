using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewDataArchiving.DataArchive;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewDataArchiving.ViewModels
{
    public class DataArchivingViewModel : DependencyObject
    {
        private static DataArchivingViewModel instance;
        private static readonly object locker = new object();


        public DelegateCommand ExecuteDataArchiveCommand { get; set; }

        public static DataArchivingViewModel Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new DataArchivingViewModel();
                    }
                }
            }
            return instance;
        }



        private DataArchivingViewModel()
        {
            Tables = new List<string>();
            Tables.Add("box_enter_record");
            Tables.Add("box_out_record");
            Tables.Add("box_bill");
            Tables.Add("boxdoor_door_record");
            Tables.Add("business_discount");

            ExecuteDataArchiveCommand = new DelegateCommand();
            ExecuteDataArchiveCommand.ExecuteAction = DataArchive;

            AutoArchive = EnvironmentInfo.IsAutoArchive;
            ManulArchive = !EnvironmentInfo.IsAutoArchive;
            DataSource = new Dictionary<int, string>();
        }

        private void DataArchive(object parameter)
        {
            if (SelectMonth == 0)
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择归档的月份！");
                return;
            }

            string sql = "select * from sys_key_value_setting where KeyID='AutoArchiveDaysBefore'";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            if (dt.Rows.Count > 0 && this.AutoArchive)
            {
                MessageBoxHelper.MessageBoxShowWarning("当前版本已经支持自动归档！");
                this.AutoArchive = false;
                this.ManulArchive = true;
                return;
            }

            if (this.AutoArchive)
            {
                ConfigHelper.WriterAppConfig("AutoArchive", "1");
                EnvironmentInfo.IsAutoArchive = true;
                Notice.Show("数据归档任务将于每晚12点执行...", "通知", 3, MessageBoxIcon.Success);
            }
            else
            {
                ConfigHelper.WriterAppConfig("AutoArchive", "0");
                EnvironmentInfo.IsAutoArchive = false;

                ConfigHelper.WriterAppConfig("AutoArchiveMonth", SelectMonth.ToString());
                EnvironmentInfo.AutoArchiveMonth = SelectMonth;
                if (dt.Rows.Count == 0 && MessageBoxHelper.MessageBoxShowQuestion("使用工具归档后，数据在报表中不能查询，请确认是否继续归档？") == MessageBoxResult.Yes)
                {
                    Task.Factory.StartNew(() =>
                    {
                        ExecuteDataArchive dataArchive = new ExecuteDataArchive();
                        dataArchive.Execute();
                    });
                }
                else if (dt.Rows.Count > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        ExecuteDataArchive dataArchive = new ExecuteDataArchive();
                        dataArchive.Execute();
                    });
                }
            }
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(DataArchivingViewModel));




        public bool AutoArchive
        {
            get { return (bool)GetValue(AutoArchiveProperty); }
            set { SetValue(AutoArchiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoArchive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoArchiveProperty =
            DependencyProperty.Register("AutoArchive", typeof(bool), typeof(DataArchivingViewModel));




        public bool ManulArchive
        {
            get { return (bool)GetValue(ManulArchiveProperty); }
            set { SetValue(ManulArchiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManulArchive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManulArchiveProperty =
            DependencyProperty.Register("ManulArchive", typeof(bool), typeof(DataArchivingViewModel));




        public int SelectMonth
        {
            get { return (int)GetValue(SelectMonthProperty); }
            set { SetValue(SelectMonthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectMonth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectMonthProperty =
            DependencyProperty.Register("SelectMonth", typeof(int), typeof(DataArchivingViewModel));




        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(int), typeof(DataArchivingViewModel));




        public int SelectIndex
        {
            get { return (int)GetValue(SelectIndexProperty); }
            set { SetValue(SelectIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectIndexProperty =
            DependencyProperty.Register("SelectIndex", typeof(int), typeof(DataArchivingViewModel));





        public Dictionary<int, string> DataSource
        {
            get { return (Dictionary<int, string>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(Dictionary<int, string>), typeof(DataArchivingViewModel));







        public List<string> Tables { get; set; }


        public void ShowMessage(string message, int progress = 0)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}";
                Progress = progress;
            }));
        }

    }
}
