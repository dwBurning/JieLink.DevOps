using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using PartialViewMySqlBackUp.BackUp;
using PartialViewMySqlBackUp.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewMySqlBackUp.ViewModels
{
    public class MySqlBackUpViewModel : DependencyObject
    {
        private static MySqlBackUpViewModel instance;
        private static readonly object locker = new object();

        public static MySqlBackUpViewModel Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new MySqlBackUpViewModel();
                    }
                }
            }
            return instance;
        }

        public DelegateCommand TaskStartCommand { get; set; }

        public DelegateCommand TaskStopCommand { get; set; }

        public DelegateCommand ManualExecuteCommand { get; set; }

        public DelegateCommand RemovePolicyCommand { get; set; }

        public DelegateCommand AddPolicyCommand { get; set; }

        public DelegateCommand ChooseAllCommand { get; set; }

        public DelegateCommand RecoverDefaultCommand { get; set; }



        private MySqlBackUpViewModel()
        {
            TaskStartCommand = new DelegateCommand();
            TaskStopCommand = new DelegateCommand();
            ManualExecuteCommand = new DelegateCommand();
            RemovePolicyCommand = new DelegateCommand();
            AddPolicyCommand = new DelegateCommand();
            ChooseAllCommand = new DelegateCommand();
            RecoverDefaultCommand = new DelegateCommand();

            TaskStartCommand.ExecuteAction = Start;
            TaskStopCommand.ExecuteAction = Stop;
            ManualExecuteCommand.ExecuteAction = ManualExecute;
            RemovePolicyCommand.ExecuteAction = RemovePolicy;

            AddPolicyCommand.ExecuteAction = AddPolicy;
            ChooseAllCommand.ExecuteAction = ChooseAll;
            RecoverDefaultCommand.ExecuteAction = RecoverDefault;


            Policys = new ObservableCollection<BackUpPolicy>();
            CurrentPolicy = new BackUpPolicy();
            Tables = new List<BackUpTable>();

            string cron = ConfigHelper.ReadAppConfig("TablesBackUpJob");
            if (!string.IsNullOrEmpty(cron))
            {
                BackUpPolicy policy = ConvertToPolicy(cron);
                policy.IsTaskBackUpTables = true;
                policy.ItemString = policy.PolicyToString;
                Policys.Add(policy);
            }

            cron = ConfigHelper.ReadAppConfig("DataBaseBackUpJob");
            if (!string.IsNullOrEmpty(cron))
            {
                BackUpPolicy policy = ConvertToPolicy(cron);
                policy.IsTaskBackUpDataBase = true;
                policy.ItemString = policy.PolicyToString;
                Policys.Add(policy);
            }

            GetTables();

            ProcessHelper.ShowOutputMessage += ProcessHelper_ShowOutputMessage;
        }

        private void ProcessHelper_ShowOutputMessage(string message)
        {
            ShowMessage(message);
        }

        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        private void Start(object parameter)
        {
            if (string.IsNullOrEmpty(TaskBackUpPath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请配置保存文件的路径！");
                return;
            }

            if (TaskBackUpPath.ToLower().StartsWith("c:"))
            {
                if (MessageBoxHelper.MessageBoxShowQuestion("请确认是否将备份文件保存的C盘？") == MessageBoxResult.No)
                {
                    return;
                }
            }

            foreach (var policy in Policys)
            {
                string cron = ConvertToCron(policy);
                if (policy.BackUpType == BackUpType.DataBase)
                {
                    ConfigHelper.WriterAppConfig("DataBaseBackUpJob", cron);
                    if (scheduler.CheckExists(new JobKey("DataBaseBackUpJob")))
                    {
                        scheduler.UnscheduleJob(new TriggerKey("DataBaseBackUpTrigger"));
                    }
                    var job = JobBuilder.Create(typeof(DataBaseBackUpJob))
                        .WithIdentity("DataBaseBackUpJob", "BackUp").Build();
                    var trigger = TriggerBuilder.Create()
                                    .StartNow()
                                    .WithIdentity("DataBaseBackUpTrigger", "BackUp")
                                    .WithCronSchedule(cron)
                                    .Build();
                    scheduler.ScheduleJob(job, trigger);
                }
                else
                {
                    ConfigHelper.WriterAppConfig("TablesBackUpJob", cron);
                    if (scheduler.CheckExists(new JobKey("TablesBackUpJob")))
                    {
                        scheduler.UnscheduleJob(new TriggerKey("TablesBackUpTrigger", "BackUp"));
                    }
                    var job = JobBuilder.Create(typeof(TablesBackUpJob))
                        .WithIdentity("TablesBackUpJob").Build();
                    var trigger = TriggerBuilder.Create()
                                    .StartNow()
                                    .WithIdentity("TablesBackUpTrigger", "BackUp")
                                    .WithCronSchedule(cron)
                                    .Build();
                    scheduler.ScheduleJob(job, trigger);
                }
            }

            Notice.Show("定时任务已经启动....", "通知", 3, MessageBoxIcon.Success);
        }

        private void Stop(object parameter)
        {
            scheduler.PauseJobs(Quartz.Impl.Matchers.GroupMatcher<JobKey>.GroupEquals("BackUp"));
            Notice.Show("定时任务已经停止....", "通知", 3, MessageBoxIcon.Success);
        }

        private void ManualExecute(object parameter)
        {
            if (string.IsNullOrEmpty(TaskBackUpPath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请配置文件的路径！");
                return;
            }

            if (!(CurrentPolicy.IsTaskBackUpDataBase || CurrentPolicy.IsTaskBackUpTables))
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择备份类型！");
                return;
            }

            if (TaskBackUpPath.ToLower().StartsWith("c:"))
            {
                if (MessageBoxHelper.MessageBoxShowQuestion("请确认是否将备份文件保存的C盘？") == MessageBoxResult.No)
                {
                    return;
                }
            }

            ExecuteBackUp executeBackUp = new ExecuteBackUp();
            if (CurrentPolicy.BackUpType == BackUpType.DataBase)
            {
                Task.Factory.StartNew(() =>
                {
                    executeBackUp.BackUpDataBase();
                });
            }
            else
            {
                Task.Factory.StartNew(() =>
                {
                    executeBackUp.BackUpTables();
                });
            }
        }

        #region 数据转换
        private BackUpPolicy ConvertToPolicy(string cron)
        {
            BackUpPolicy policy = new BackUpPolicy();
            string[] cronArry = cron.Split(' ');
            int second = int.Parse(cronArry[0]);
            int minute = int.Parse(cronArry[1]);
            int hour = int.Parse(cronArry[2]);
            policy.SelectedTime = DateTime.Now.Date.AddHours(hour).AddMinutes(minute).AddSeconds(second);
            string dayOfWeek = cronArry[cronArry.Length - 1];
            string[] week = dayOfWeek.Split(',');
            foreach (var day in week)
            {
                switch (day)
                {
                    case "1":
                        policy.Sunday = true;
                        break;
                    case "2":
                        policy.Monday = true;
                        break;
                    case "3":
                        policy.Tuesday = true;
                        break;
                    case "4":
                        policy.Wednesday = true;
                        break;
                    case "5":
                        policy.Thursday = true;
                        break;
                    case "6":
                        policy.Friday = true;
                        break;
                    case "7":
                        policy.Saturday = true;
                        break;
                }
            }

            return policy;
        }

        private string ConvertToCron(BackUpPolicy policy)
        {
            //周日 1 周一 2...
            //0 15 10 ? * 1,5 每周日，周四 上午10:15分执行
            string second = policy.SelectedTime.ToString("ss");//秒
            string minute = policy.SelectedTime.ToString("mm");
            string hour = policy.SelectedTime.ToString("HH");
            string dayOfWeek = "";
            if (policy.Sunday)
            {
                dayOfWeek += "1,";
            }
            if (policy.Monday)
            {
                dayOfWeek += "2,";
            }
            if (policy.Tuesday)
            {
                dayOfWeek += "3,";
            }
            if (policy.Wednesday)
            {
                dayOfWeek += "4,";
            }
            if (policy.Thursday)
            {
                dayOfWeek += "5,";
            }
            if (policy.Friday)
            {
                dayOfWeek += "6,";
            }
            if (policy.Saturday)
            {
                dayOfWeek += "7";
            }

            return $"{second} {minute} {hour} ? * {dayOfWeek}".Trim(',');
        }
        #endregion

        private void RemovePolicy(object parameter)
        {
            BackUpPolicy policy = Policys.FirstOrDefault(x => x.BackUpType == CurrentPolicy.BackUpType);
            if (policy != null)
            {
                Policys.Remove(policy);
                Clear();
                Notice.Show("策略已移除成功", "通知", 3, MessageBoxIcon.Success);
            }
            else
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择你需要移除的策略");
            }

            WriteConfig();
        }

        private void AddPolicy(object parameter)
        {
            if (CheckDayOfWeek())
            { return; }

            CurrentPolicy.SelectedTime = (DateTime)parameter;//通过绑定无法获取到值 原因不明
            BackUpPolicy policy = Policys.FirstOrDefault(x => x.BackUpType == CurrentPolicy.BackUpType);

            if (policy == null)
            {
                Policys.Add(new BackUpPolicy()
                {
                    Sunday = CurrentPolicy.Sunday,
                    Monday = CurrentPolicy.Monday,
                    Tuesday = CurrentPolicy.Tuesday,
                    Wednesday = CurrentPolicy.Wednesday,
                    Thursday = CurrentPolicy.Thursday,
                    Friday = CurrentPolicy.Friday,
                    Saturday = CurrentPolicy.Saturday,

                    SelectedTime = CurrentPolicy.SelectedTime,
                    IsTaskBackUpDataBase = CurrentPolicy.IsTaskBackUpDataBase,
                    IsTaskBackUpTables = CurrentPolicy.IsTaskBackUpTables,
                    ItemString = CurrentPolicy.PolicyToString
                });

                Notice.Show("策略已添加成功", "通知");
            }
            else
            {
                policy.Sunday = CurrentPolicy.Sunday;
                policy.Monday = CurrentPolicy.Monday;
                policy.Tuesday = CurrentPolicy.Tuesday;
                policy.Wednesday = CurrentPolicy.Wednesday;
                policy.Thursday = CurrentPolicy.Thursday;
                policy.Friday = CurrentPolicy.Friday;
                policy.Saturday = CurrentPolicy.Saturday;
                policy.SelectedTime = CurrentPolicy.SelectedTime;
                policy.IsTaskBackUpDataBase = CurrentPolicy.IsTaskBackUpDataBase;
                policy.IsTaskBackUpTables = CurrentPolicy.IsTaskBackUpTables;
                policy.ItemString = CurrentPolicy.PolicyToString;

                Notice.Show("策略已更新成功", "通知", 3, MessageBoxIcon.Success);
            }

            if (CurrentPolicy.BackUpType == BackUpType.DataBase)
            {
                ConfigHelper.WriterAppConfig("DataBaseBackUpJob", ConvertToCron(CurrentPolicy));
            }
            else
            {
                ConfigHelper.WriterAppConfig("TablesBackUpJob", ConvertToCron(CurrentPolicy));
            }

            WriteConfig();
        }

        public void WriteConfig()
        {
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = BaseDirectoryPath + "plugs\\BackUpTables.json";
            BackUpConfig.Tables.Clear();
            BackUpConfig.SavePath = TaskBackUpPath;
            Tables.ForEach((x) =>
            {
                if (x.IsChecked)
                { BackUpConfig.Tables.Add(new Table() { TableName = x.TableName }); }
            });

            File.WriteAllText(path, JsonConvert.SerializeObject(BackUpConfig), Encoding.UTF8);
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="parameter"></param>
        private void ChooseAll(object parameter)
        {
            Tables.ForEach(x => x.IsChecked = IsSelectedAll);
            WriteConfig();
        }

        /// <summary>
        /// 恢复默认
        /// </summary>
        /// <param name="parameter"></param>
        private void RecoverDefault(object parameter)
        {
            Tables.ForEach((x) =>
            {
                x.IsChecked = BackUpConfig.DefaultTables.FindIndex(t => t.TableName == x.TableName) >= 0;
            });

            WriteConfig();
        }

        #region 获取配置
        public BackUpConfig BackUpConfig { get; set; }
        public void GetTables()
        {
            try
            {
                string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string path = BaseDirectoryPath + "plugs\\BackUpTables.json";
                string jsonData = File.ReadAllText(path);
                BackUpConfig = JsonConvert.DeserializeObject<BackUpConfig>(jsonData);
                TaskBackUpPath = BackUpConfig.SavePath;//文件保存路径
                if (!Directory.Exists(TaskBackUpPath))
                {
                    Directory.CreateDirectory(TaskBackUpPath);
                }

                //string cmd = "select @@basedir as mysqlpath from dual";
                //DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, cmd).Tables[0];
                //MySqlBinPath = dt.Rows[0][0].ToString() + "\\bin";//获取mysql的bin路径

                MySqlBinPath = BaseDirectoryPath;//如果不是在服务器上，那么可能无法获取到mysqldump文件

                
                string sql = $"select table_name from information_schema.`TABLES` where TABLE_SCHEMA='{EnvironmentInfo.DbConnEntity.DbName}';";
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];//获取所有的表
                foreach (DataRow dr in dt.Rows)
                {
                    BackUpTable backUpTable = new BackUpTable() { TableName = dr["table_name"].ToString(), IsChecked = false };

                    BackUpConfig.Tables.ForEach((x) =>
                    {
                        if (x.TableName == dr["table_name"].ToString())
                        {
                            backUpTable.IsChecked = true;
                        }
                    });
                    Tables.Add(backUpTable);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }
        }
        #endregion

        #region 校验数据
        private void Clear()
        {
            CurrentPolicy.Sunday = false;
            CurrentPolicy.Monday = false;
            CurrentPolicy.Tuesday = false;
            CurrentPolicy.Wednesday = false;
            CurrentPolicy.Thursday = false;
            CurrentPolicy.Friday = false;
            CurrentPolicy.Saturday = false;
            CurrentPolicy.IsTaskBackUpDataBase = false;
            CurrentPolicy.IsTaskBackUpTables = false;
        }

        private bool CheckDayOfWeek()
        {
            if (!CurrentPolicy.Sunday
               && !CurrentPolicy.Monday
               && !CurrentPolicy.Monday
               && !CurrentPolicy.Tuesday
               && !CurrentPolicy.Wednesday
               && !CurrentPolicy.Thursday
               && !CurrentPolicy.Friday
               && !CurrentPolicy.Saturday)
            {
                MessageBoxHelper.MessageBoxShowWarning("请至少选择一个周期！");
                return true;
            }

            if (!CurrentPolicy.IsTaskBackUpDataBase && !CurrentPolicy.IsTaskBackUpTables)
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择备份类型！");
                return true;
            }

            BackUpPolicy policy;
            if (CurrentPolicy.Sunday)
            {
                policy = Policys.FirstOrDefault(x => x.Sunday
                && x.BackUpType != CurrentPolicy.BackUpType);
                if (policy != null) CurrentPolicy.Sunday = false;
            }
            if (CurrentPolicy.Monday)
            {
                policy = Policys.FirstOrDefault(x => x.Monday
                && x.BackUpType != CurrentPolicy.BackUpType);
                if (policy != null) CurrentPolicy.Monday = false;
            }
            if (CurrentPolicy.Tuesday)
            {
                policy = Policys.FirstOrDefault(x => x.Tuesday
                && x.BackUpType != CurrentPolicy.BackUpType);
                if (policy != null) CurrentPolicy.Tuesday = false;
            }
            if (CurrentPolicy.Wednesday)
            {
                policy = Policys.FirstOrDefault(x => x.Wednesday
                && x.BackUpType != CurrentPolicy.BackUpType);
                if (policy != null) CurrentPolicy.Wednesday = false;
            }
            if (CurrentPolicy.Thursday)
            {
                policy = Policys.FirstOrDefault(x => x.Thursday
                && x.BackUpType != CurrentPolicy.BackUpType);
                if (policy != null) CurrentPolicy.Thursday = false;
            }
            if (CurrentPolicy.Friday)
            {
                policy = Policys.FirstOrDefault(x => x.Friday
                && x.BackUpType != CurrentPolicy.BackUpType);
                if (policy != null) CurrentPolicy.Friday = false;
            }
            if (CurrentPolicy.Saturday)
            {
                policy = Policys.FirstOrDefault(x => x.Saturday
                && x.BackUpType != CurrentPolicy.BackUpType);
                if (policy != null) CurrentPolicy.Saturday = false;
            }

            if (!CurrentPolicy.Sunday
               && !CurrentPolicy.Monday
               && !CurrentPolicy.Monday
               && !CurrentPolicy.Tuesday
               && !CurrentPolicy.Wednesday
               && !CurrentPolicy.Thursday
               && !CurrentPolicy.Friday
               && !CurrentPolicy.Saturday)
            {
                MessageBoxHelper.MessageBoxShowWarning("已有策略占用了选定周期，请至少选择一个有效的周期！");
                return true;
            }

            return false;
        }
        #endregion

        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Message != null && Message.Length > 5000)
                {
                    Message = string.Empty;
                }

                if (message.Length > 0)
                {
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }

        #region 属性


        public string MySqlBinPath { get; set; }

        public List<BackUpTable> Tables { get; set; }

        public ObservableCollection<BackUpPolicy> Policys { get; set; }

        public BackUpPolicy CurrentPolicy { get; set; }

        /// <summary>
        /// 全选
        /// </summary>

        public bool IsSelectedAll
        {
            get { return (bool)GetValue(IsSelectedAllProperty); }
            set { SetValue(IsSelectedAllProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedAllProperty =
            DependencyProperty.Register("IsSelectedAll", typeof(bool), typeof(MySqlBackUpViewModel));



        /// <summary>
        /// 文件保存路径-任务
        /// </summary>

        public string TaskBackUpPath
        {
            get { return (string)GetValue(TaskBackUpPathProperty); }
            set { SetValue(TaskBackUpPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoBackUpPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskBackUpPathProperty =
            DependencyProperty.Register("TaskBackUpPath", typeof(string), typeof(MySqlBackUpViewModel));


        /// <summary>
        /// 是否备份全库-手动
        /// </summary>
        public bool IsManualBackUpDataBase
        {
            get { return (bool)GetValue(IsManualBackUpDataBaseProperty); }
            set { SetValue(IsManualBackUpDataBaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsManualBackUpDataBase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsManualBackUpDataBaseProperty =
            DependencyProperty.Register("IsManualBackUpDataBase", typeof(bool), typeof(MySqlBackUpViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MySqlBackUpViewModel));


        #endregion
    }
}
