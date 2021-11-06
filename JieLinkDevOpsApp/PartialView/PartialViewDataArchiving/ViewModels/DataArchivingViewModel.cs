using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewDataArchiving.DataArchive;
using PartialViewDataArchiving.DB;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.DB;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public DelegateCommand AddTableCommand { get; set; }

        public DelegateCommand RemoveTableCommand { get; set; }

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


        ArchiveTableManager archiveTableManager;
        KeyValueSettingManager keyValueSettingManager;
        private DataArchivingViewModel()
        {
            archiveTableManager = new ArchiveTableManager();
            keyValueSettingManager = new KeyValueSettingManager();
            ArchiveTables = new ObservableCollection<ArchiveTable>();

            archiveTableManager.ArchiveTables().ForEach(x =>
            {
                ArchiveTables.Add(x);
            });

            ExecuteDataArchiveCommand = new DelegateCommand();
            ExecuteDataArchiveCommand.ExecuteAction = DataArchive;


            AddTableCommand = new DelegateCommand();
            AddTableCommand.ExecuteAction = AddTable;

            RemoveTableCommand = new DelegateCommand();
            RemoveTableCommand.ExecuteAction = RemoveTable;


            AutoArchive = EnvironmentInfo.IsAutoArchive;
            ManulArchive = !EnvironmentInfo.IsAutoArchive;
            DataSource = new Dictionary<int, string>();

            Progress = 0;
            IsIndeterminate = false;
            IsPercentVisible = true;
        }

        private void AddTable(object parameter)
        {
            if (string.IsNullOrEmpty(TableName + DateField))
            {
                MessageBoxHelper.MessageBoxShowWarning("表名称和时间字段都为必填项！");
                return;
            }

            ArchiveTable archiveTable = ArchiveTables.FirstOrDefault(x => x.TableName.ToLower() == TableName.ToLower());
            if (archiveTable != null)
            {
                MessageBoxHelper.MessageBoxShowWarning("添加的表名称已存在！");
                return;
            }

            try
            {
                MySqlHelper.ExecuteScalar(EnvironmentInfo.ConnectionString, $"select {DateField} from {TableName} limit 1");
            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("添加的表名称或者时间字段在数据库中不存在！");
                return;
            }


            var table = new ArchiveTable() { TableName = TableName, DateField = DateField, Where = Where };
            archiveTableManager.AddArchiveTable(table);
            ArchiveTables.Add(table);
            this.TableName = "";
            this.DateField = "";
            this.Where = "";
        }

        private void RemoveTable(object parameter)
        {
            if (SelectedTable == null)
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择你要移除的表！");
                return;
            }

            archiveTableManager.RemoveArchiveTable(SelectedTable);
            ArchiveTables.Remove(SelectedTable);
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
                if (MessageBoxHelper.MessageBoxShowQuestion("当前版本已经支持自动归档，是否继续配置？") == MessageBoxResult.No)
                {
                    this.AutoArchive = false;
                    this.ManulArchive = true;
                    return;
                }
            }

            if (this.AutoArchive)
            {
                //ConfigHelper.WriterAppConfig("AutoArchive", "1");
                keyValueSettingManager.WriteSetting(new KeyValueSetting() { KeyId = "AutoArchive", ValueText = "1" });
                EnvironmentInfo.IsAutoArchive = true;
                Notice.Show("数据归档任务将于每晚2点执行...", "通知", 3, MessageBoxIcon.Success);
            }
            else
            {
                //ConfigHelper.WriterAppConfig("AutoArchive", "0");
                keyValueSettingManager.WriteSetting(new KeyValueSetting() { KeyId = "AutoArchive", ValueText = "0" });
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


        public bool IsPercentVisible
        {
            get { return (bool)GetValue(IsPercentVisibleProperty); }
            set { SetValue(IsPercentVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPercentVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPercentVisibleProperty =
            DependencyProperty.Register("IsPercentVisible", typeof(bool), typeof(DataArchivingViewModel));



        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(DataArchivingViewModel));



        public string TableName
        {
            get { return (string)GetValue(TableNameProperty); }
            set { SetValue(TableNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TableName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TableNameProperty =
            DependencyProperty.Register("TableName", typeof(string), typeof(DataArchivingViewModel));




        public string DateField
        {
            get { return (string)GetValue(DateFieldProperty); }
            set { SetValue(DateFieldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateField.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateFieldProperty =
            DependencyProperty.Register("DateField", typeof(string), typeof(DataArchivingViewModel));


        public ObservableCollection<ArchiveTable> ArchiveTables { get; set; }



        public ArchiveTable SelectedTable
        {
            get { return (ArchiveTable)GetValue(SelectedTableProperty); }
            set { SetValue(SelectedTableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTableProperty =
            DependencyProperty.Register("SelectedTable", typeof(ArchiveTable), typeof(DataArchivingViewModel));



        public string Where
        {
            get { return (string)GetValue(WhereProperty); }
            set { SetValue(WhereProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Where.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WhereProperty =
            DependencyProperty.Register("Where", typeof(string), typeof(DataArchivingViewModel));



        //public List<string> Tables { get; set; }


        public void ShowMessage(string message, int progress = 0)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}";
                Progress = progress;
                if (progress == 0)
                {
                    IsIndeterminate = true;
                    IsPercentVisible = true;
                }
                else if (progress == 100)
                {
                    IsIndeterminate = false;
                    IsPercentVisible = true;
                }
            }));
        }

    }
}
