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
using Quartz.Impl.Matchers;
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

        public DelegateCommand EditPolicyCommand { get; set; }


        private MySqlBackUpViewModel()
        {
            TaskStartCommand = new DelegateCommand();
            TaskStopCommand = new DelegateCommand();
            ManualExecuteCommand = new DelegateCommand();
            RemovePolicyCommand = new DelegateCommand();
            AddPolicyCommand = new DelegateCommand();
            ChooseAllCommand = new DelegateCommand();
            RecoverDefaultCommand = new DelegateCommand();
            EditPolicyCommand = new DelegateCommand();

            TaskStartCommand.ExecuteAction = Start;
            TaskStopCommand.ExecuteAction = Stop;
            ManualExecuteCommand.ExecuteAction = ManualExecute;
            RemovePolicyCommand.ExecuteAction = RemovePolicy;

            AddPolicyCommand.ExecuteAction = AddPolicy;
            ChooseAllCommand.ExecuteAction = ChooseAll;
            RecoverDefaultCommand.ExecuteAction = RecoverDefault;
            EditPolicyCommand.ExecuteAction = EditPolicy;


            Policys = new ObservableCollection<BackUpPolicy>();
            Tables = new ObservableCollection<BackUpTable>();
            Databases = new ObservableCollection<BackUpDatabase>();
            CurrentPolicy = new BackUpPolicy();
            CurrentPolicy.SelectedDatabase = EnvironmentInfo.SelectedDatabase;
            CurrentPolicyBak = DeepCopy(CurrentPolicy);

            ReadPolicyConfig();
            GetTables();

            ProcessHelper.ShowOutputMessage += ProcessHelper_ShowOutputMessage;

            Message = "说明：\r\n1)全库备份，是完整备份。基础业务备份，是只备份核心业务数据。\r\n2)哪些表数据属于核心业务，系统已经默认配置了，也可以自行勾选，但是建议只多不少。\r\n3)如果要全部勾选，建议就不要配置为基础业务备份，直接使用全库备份即可。\r\n4)基础业务的备份频率一定要高于全库备份，因为全库备份包含了一些历史表，会很大。\r\n5)配置策略之后，重新启动不需要再去点执行任务，系统会自动执行。\r\n";
        }

        private void ProcessHelper_ShowOutputMessage(string message)
        {
            ShowMessage(message);
        }

        IScheduler scheduler = EnvironmentInfo.scheduler;
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
                string cronNoDB = ConvertToCronNoDbName(policy);
                if (policy.BackUpType == BackUpType.DataBase)
                {
                    var jobIdentity = $"DataBaseBackUpJob-{cron}";
                    var triggerKey = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals("scheduler")).Where(x => x.Name == jobIdentity).FirstOrDefault();
                    if (triggerKey != null)
                    {
                        scheduler.UnscheduleJob(triggerKey);
                    }
                    var job = JobBuilder.Create(typeof(DataBaseBackUpJob))
                                    .WithIdentity(jobIdentity, "scheduler")
                                    .UsingJobData("DatabaseName", policy.SelectedDatabase)
                                    .Build();
                    var trigger = TriggerBuilder.Create()
                                    .StartNow()
                                    .WithIdentity(jobIdentity, "scheduler")
                                    .WithCronSchedule(cronNoDB)
                                    .Build();
                    scheduler.ScheduleJob(job, trigger);
                }
                else
                {
                    var jobIdentity = $"TablesBackUpJob-{cron}";
                    var triggerKey = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals("scheduler")).Where(x => x.Name == jobIdentity).FirstOrDefault();
                    if (triggerKey != null)
                    {
                        scheduler.UnscheduleJob(triggerKey);
                    }

                    var job = JobBuilder.Create(typeof(TablesBackUpJob))
                                    .WithIdentity(jobIdentity, "scheduler")
                                    .UsingJobData("DatabaseName", policy.SelectedDatabase)
                                    .Build();
                    var trigger = TriggerBuilder.Create()
                                    .StartNow()
                                    .WithIdentity(jobIdentity, "scheduler")
                                    .WithCronSchedule(cronNoDB)
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

            if (CurrentPolicy.IsTaskBackUpDataBase)
            {
                var policies = Policys.Where(x => x.BackUpType == BackUpType.DataBase);
                if (policies.Any())
                {
                    foreach (var policy in Policys)
                    {
                        var databaseName = policy.SelectedDatabase;
                        ExecuteBackUp executeBackUp = new ExecuteBackUp();
                        Task.Factory.StartNew(() =>
                        {
                            executeBackUp.BackUpDataBase(databaseName);
                        });
                    }
                }
                else
                {
                    MessageBoxHelper.MessageBoxShowWarning("未配置全库备份类型的策略，请确认！");
                    return;
                }
            }

            if (CurrentPolicy.IsTaskBackUpTables)
            {
                var policies = Policys.Where(x => x.BackUpType == BackUpType.Tables);
                if (policies.Any())
                {
                    foreach (var policy in policies)
                    {
                        var databaseName = policy.SelectedDatabase;
                        ExecuteBackUp executeBackUp = new ExecuteBackUp();
                        Task.Factory.StartNew(() =>
                        {
                            executeBackUp.BackUpTables(databaseName);
                        });
                    }
                }
                else
                {
                    MessageBoxHelper.MessageBoxShowWarning("未配置基础业务备份类型的策略，请确认！");
                    return;
                }
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
            string dayOfWeek = cronArry[5];
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

            var databaseName = cronArry[6];
            policy.SelectedDatabase = databaseName;
            return policy;
        }

        private string ConvertToCronNoDbName(BackUpPolicy policy)
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

            return $"{second} {minute} {hour} ? * {dayOfWeek.Trim(',')}";
        }

        private string ConvertToCron(BackUpPolicy policy)
        {
            var cron = ConvertToCronNoDbName(policy);
            return $"{cron} {policy.SelectedDatabase}";
        }
        #endregion

        private void RemovePolicy(object parameter)
        {
            BackUpPolicy policy = Policys.FirstOrDefault(x => x.PolicyToString == CurrentPolicyBak.PolicyToString);
            if (policy != null)
            {
                Policys.Remove(policy);
                WritePolicyConfig();
                Clear();
                Notice.Show("策略已移除成功", "通知", 3, MessageBoxIcon.Success);
            }
            else
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择你需要移除的策略");
            }
        }

        private void AddPolicy(object parameter)
        {
            if (CheckDayOfWeek())
            { return; }

            CurrentPolicy.SelectedTime = (DateTime)parameter;//通过绑定无法获取到值 原因不明
            BackUpPolicy policy = Policys.FirstOrDefault(x => x.BackUpType == CurrentPolicy.BackUpType //备份类型
                                                           && x.SelectedDatabase == CurrentPolicy.SelectedDatabase); //选中的数据库

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
                    ItemString = CurrentPolicy.PolicyToString,
                    SelectedDatabase = CurrentPolicy.SelectedDatabase
                });
                Notice.Show("策略已添加成功", "通知", 3);
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
                policy.SelectedDatabase = CurrentPolicy.SelectedDatabase;
                Notice.Show("策略已更新成功", "通知", 3, MessageBoxIcon.Success);
            }

            WritePolicyConfig();
            WriteTablesConfig(CurrentPolicy.SelectedDatabase);
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <param name="currentPolicy"></param>
        /// <returns></returns>
        public BackUpPolicy DeepCopy(BackUpPolicy currentPolicy)
        {
            var model = new BackUpPolicy()
            {
                Sunday = currentPolicy.Sunday,
                Monday = currentPolicy.Monday,
                Tuesday = currentPolicy.Tuesday,
                Wednesday = currentPolicy.Wednesday,
                Thursday = currentPolicy.Thursday,
                Friday = currentPolicy.Friday,
                Saturday = currentPolicy.Saturday,
                SelectedTime = currentPolicy.SelectedTime,
                IsTaskBackUpDataBase = currentPolicy.IsTaskBackUpDataBase,
                IsTaskBackUpTables = currentPolicy.IsTaskBackUpTables,
                ItemString = currentPolicy.PolicyToString,
                SelectedDatabase = currentPolicy.SelectedDatabase
            };
            return model;
        }

        /// <summary>
        /// 编辑策略
        /// </summary>
        /// <param name="parameter"></param>
        private void EditPolicy(object parameter)
        {
            if (CurrentPolicy.BackUpType != CurrentPolicyBak.BackUpType)
            {
                MessageBoxHelper.MessageBoxShowWarning("不允许修改备份类型，如需要该类型的策略可选择新增。");
                return;
            }

            AddPolicy(parameter);
        }

        /// <summary>
        /// 从配置文件读取策略
        /// </summary>
        /// <returns></returns>
        public void ReadPolicyConfig()
        {
            Policys.Clear();
            string config = ConfigHelper.ReadAppConfig("TablesBackUpJob");
            if (!string.IsNullOrEmpty(config))
            {
                var policyStrs = config.Split('|');
                foreach (var policyStr in policyStrs)
                {
                    BackUpPolicy policy = ConvertToPolicy(policyStr);
                    policy.IsTaskBackUpTables = true;
                    policy.ItemString = policy.PolicyToString;
                    Policys.Add(policy);
                }
            }

            config = ConfigHelper.ReadAppConfig("DataBaseBackUpJob");
            if (!string.IsNullOrEmpty(config))
            {
                var policyStrs = config.Split('|');
                foreach (var policyStr in policyStrs)
                {
                    BackUpPolicy policy = ConvertToPolicy(policyStr);
                    policy.IsTaskBackUpDataBase = true;
                    policy.ItemString = policy.PolicyToString;
                    Policys.Add(policy);
                }
            }
        }

        /// <summary>
        /// 更新策略到配置文件
        /// </summary>
        /// <returns></returns>
        public void WritePolicyConfig()
        {
            if (Policys == null)
                return;

            string dataBaseBackUpJobConfig = string.Empty;
            string tablesBackUpJobConfig = string.Empty;
            foreach (var policy in Policys)
            {
                var currentPolicyConfig = ConvertToCron(policy);
                if (policy.BackUpType == BackUpType.DataBase)
                {
                    dataBaseBackUpJobConfig = $"{dataBaseBackUpJobConfig}|{currentPolicyConfig}";
                }
                else if (policy.BackUpType == BackUpType.Tables)
                {
                    tablesBackUpJobConfig = $"{tablesBackUpJobConfig}|{currentPolicyConfig}";
                }
            }

            dataBaseBackUpJobConfig = dataBaseBackUpJobConfig.TrimStart('|');
            tablesBackUpJobConfig = tablesBackUpJobConfig.TrimStart('|');

            ConfigHelper.WriterAppConfig("DataBaseBackUpJob", dataBaseBackUpJobConfig);
            ConfigHelper.WriterAppConfig("TablesBackUpJob", tablesBackUpJobConfig);
        }

        /// <summary>
        /// 根据选择的表更新“业务表”配置
        /// </summary>
        /// <param name="dbName"></param>
        public void WriteTablesConfig(string dbName)
        {
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = BaseDirectoryPath + "plugs\\BackUpTables.json";
            BackUpConfig.SavePath = TaskBackUpPath;
            var tablesConfig = BackUpConfig.TablesConfig.FirstOrDefault(x => x.DbName == dbName);
            if (tablesConfig == null)
            {
                tablesConfig = new TablesConfig { DbName = dbName };
            }
            tablesConfig.Tables.Clear();
            Tables.ToList().ForEach((x) =>
            {
                if (x.IsChecked)
                {
                    tablesConfig.Tables.Add(new Table() { TableName = x.TableName });
                }
            });

            File.WriteAllText(path, JsonConvert.SerializeObject(BackUpConfig), Encoding.UTF8);
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="parameter"></param>
        private void ChooseAll(object parameter)
        {
            Tables.ToList().ForEach(x => x.IsChecked = IsSelectedAll);
        }

        /// <summary>
        /// 恢复默认
        /// </summary>
        /// <param name="parameter"></param>
        private void RecoverDefault(object parameter)
        {
            var tablesConfig = BackUpConfig.TablesConfig.FirstOrDefault(x => x.DbName == CurrentPolicy.SelectedDatabase);
            if (tablesConfig != null)
            {
                Tables.ToList().ForEach((x) =>
                {
                    x.IsChecked = tablesConfig.DefaultTables.FindIndex(t => t.TableName == x.TableName) >= 0;
                });
            }
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
                if (BackUpConfig.TablesConfig == null)
                {
                    BackUpConfig.TablesConfig = new List<TablesConfig>() { };
                }
                TaskBackUpPath = BackUpConfig.SavePath;//文件保存路径
                EnvironmentInfo.TaskBackUpPath = BackUpConfig.SavePath;
                if (!Directory.Exists(TaskBackUpPath))
                {
                    Directory.CreateDirectory(TaskBackUpPath);
                }

                //string cmd = "select @@basedir as mysqlpath from dual";
                //DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, cmd).Tables[0];
                //MySqlBinPath = dt.Rows[0][0].ToString() + "\\bin";//获取mysql的bin路径

                MySqlBinPath = BaseDirectoryPath;//如果不是在服务器上，那么可能无法获取到mysqldump文件

                #region 获取当前数据库服务器下的所有非系统数据库
                string sql = $"SHOW DATABASES WHERE `Database` NOT IN ('mysql', 'performance_schema', 'information_schema', 'sys');";
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    BackUpDatabase database = new BackUpDatabase() { DatabaseName = dr["DATABASE"].ToString() };
                    Databases.Add(database);

                    if (dr["DATABASE"].ToString() == EnvironmentInfo.DatabaseName2x) //默认选中2.x数据库，触发选中事件，展示左侧数据库表
                    {
                        EnvironmentInfo.SelectedDatabase = EnvironmentInfo.DatabaseName2x;
                        SetTables(EnvironmentInfo.DatabaseName2x);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }
        }
        #endregion

        /// <summary>
        /// 获取指定库中的表，设置左侧表集合
        /// </summary>
        /// <param name="dbName">数据库名</param>
        public void SetTables(string dbName)
        {
            if (dbName.IsNullOrEmpty())
            {
                Notice.Show("未配置备份数据库，请确认！", "通知", 3, MessageBoxIcon.Warning);
                return;
            }

            var tablesConfig = BackUpConfig.TablesConfig.FirstOrDefault(x => x.DbName == dbName);
            if (tablesConfig == null)
            {
                tablesConfig = new TablesConfig
                {
                    DbName = dbName,
                    Tables = new List<Table>()
                };
            }

            Tables.Clear();
            var sql = $"select table_name from information_schema.`TABLES` where TABLE_SCHEMA='{dbName}';";
            var dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];//获取所有的表

            foreach (DataRow dr in dt.Rows)
            {
                BackUpTable backUpTable = new BackUpTable() { TableName = dr["table_name"].ToString(), IsChecked = false };

                tablesConfig.Tables.ForEach((x) =>
                {
                    if (x.TableName == dr["table_name"].ToString())
                    {
                        backUpTable.IsChecked = true;
                    }
                });
                Tables.Add(backUpTable);
            }
        }

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

        /// <summary>
        /// 校验周期中的星期几是否被占用
        /// 原则：避免同一天（只校验星期几，不校验时分秒）有多个策略，减少数据库压力。
        /// 修改前：由于最多只有两个策略（全库/业务表），所以只需判断周期中的某天是否存在两个一个类型的策略即可
        /// 修改后：由于可能存在多个库的多个策略，所以如果周期中的某天存在其他策略，即为占用。否则为未占用。
        /// </summary>
        /// <returns></returns>
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
                policy = Policys.FirstOrDefault(x => x.Sunday && x.SelectedDatabase != CurrentPolicy.SelectedDatabase);
                if (policy != null)
                {
                    CurrentPolicy.Sunday = false;
                }
            }
            if (CurrentPolicy.Monday)
            {
                policy = Policys.FirstOrDefault(x => x.Monday && x.SelectedDatabase != CurrentPolicy.SelectedDatabase);
                if (policy != null)
                {
                    CurrentPolicy.Monday = false;
                }
            }
            if (CurrentPolicy.Tuesday)
            {
                policy = Policys.FirstOrDefault(x => x.Tuesday && x.SelectedDatabase != CurrentPolicy.SelectedDatabase);
                if (policy != null)
                {
                    CurrentPolicy.Tuesday = false;
                }
            }
            if (CurrentPolicy.Wednesday)
            {
                policy = Policys.FirstOrDefault(x => x.Wednesday && x.SelectedDatabase != CurrentPolicy.SelectedDatabase);
                if (policy != null)
                {
                    CurrentPolicy.Wednesday = false;
                }
            }
            if (CurrentPolicy.Thursday)
            {
                policy = Policys.FirstOrDefault(x => x.Thursday && x.SelectedDatabase != CurrentPolicy.SelectedDatabase);
                if (policy != null)
                {
                    CurrentPolicy.Thursday = false;
                }
            }
            if (CurrentPolicy.Friday)
            {
                policy = Policys.FirstOrDefault(x => x.Friday && x.SelectedDatabase != CurrentPolicy.SelectedDatabase);
                if (policy != null)
                {
                    CurrentPolicy.Friday = false;
                }
            }
            if (CurrentPolicy.Saturday)
            {
                policy = Policys.FirstOrDefault(x => x.Saturday && x.SelectedDatabase != CurrentPolicy.SelectedDatabase);
                if (policy != null)
                {
                    CurrentPolicy.Saturday = false;
                }
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

        public ObservableCollection<BackUpTable> Tables { get; set; }

        /// <summary>
        /// 需备份的数据库的集合
        /// </summary>
        public ObservableCollection<BackUpDatabase> Databases { get; set; }

        public ObservableCollection<BackUpPolicy> Policys { get; set; }

        /// <summary>
        /// 当前选中的策略
        /// </summary>
        public BackUpPolicy CurrentPolicy { get; set; }

        /// <summary>
        /// 当前选中的策略的备份，用于记录修改前的值
        /// </summary>
        public BackUpPolicy CurrentPolicyBak { get; set; }

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
