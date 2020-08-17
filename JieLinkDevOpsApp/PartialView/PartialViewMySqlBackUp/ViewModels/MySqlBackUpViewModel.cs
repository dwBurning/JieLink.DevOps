using MySql.Data.MySqlClient;
using Panuon.UI.Silver.Core;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using PartialViewMySqlBackUp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewMySqlBackUp.ViewModels
{
    public class MySqlBackUpViewModel : DependencyObject
    {
        public DelegateCommand TaskStartCommand { get; set; }

        public DelegateCommand TaskStopCommand { get; set; }

        public DelegateCommand ManualExecuteCommand { get; set; }

        public DelegateCommand RemovePolicyCommand { get; set; }

        public DelegateCommand AddPolicyCommand { get; set; }

        public DelegateCommand ChooseAllCommand { get; set; }



        public MySqlBackUpViewModel()
        {
            TaskStartCommand = new DelegateCommand();
            TaskStopCommand = new DelegateCommand();
            ManualExecuteCommand = new DelegateCommand();
            RemovePolicyCommand = new DelegateCommand();
            AddPolicyCommand = new DelegateCommand();
            ChooseAllCommand = new DelegateCommand();

            TaskStartCommand.ExecuteAction = Start;
            TaskStopCommand.ExecuteAction = Stop;
            ManualExecuteCommand.ExecuteAction = ManualExecute;
            RemovePolicyCommand.ExecuteAction = RemovePolicy;

            AddPolicyCommand.ExecuteAction = AddPolicy;
            ChooseAllCommand.ExecuteAction = ChooseAll;

            GetTables();
            Policys = new ObservableCollection<BackUpPolicy>();
            CurrentPolicy = new BackUpPolicy();
        }

        private void Start(object parameter)
        {

        }

        private void Stop(object parameter)
        {

        }

        private void ManualExecute(object parameter)
        {

        }

        private void RemovePolicy(object parameter)
        {
            BackUpPolicy policy = Policys.FirstOrDefault(x => x.BackUpType == CurrentPolicy.BackUpType);
            if (policy != null)
            {
                Policys.Remove(policy);
                Clear();
            }
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
            }
        }

        private void ChooseAll(object parameter)
        {
            Tables.ForEach(x => x.IsChecked = IsSelectedAll);
        }



        public void GetTables()
        {
            Tables = new List<BackUpTable>();
            string sql = $"select table_name from information_schema.`TABLES` where TABLE_SCHEMA='{EnvironmentInfo.DbConnEntity.DbName}';";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                Tables.Add(new BackUpTable() { TableName = dr["table_name"].ToString(), IsChecked = false });
            }
        }

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
                MessageBoxHelper.MessageBoxShowWarning("已有策略占用所有周期，如果需要添加新的策略，请至少预留一个周期！");
                return true;
            }

            return false;
        }

        #region 属性

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

        /// <summary>
        /// 文件保存路径-手动
        /// </summary>

        public string ManualBackUpPath
        {
            get { return (string)GetValue(ManualBackUpPathProperty); }
            set { SetValue(ManualBackUpPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManualBackUpPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManualBackUpPathProperty =
            DependencyProperty.Register("ManualBackUpPath", typeof(string), typeof(MySqlBackUpViewModel));


        #endregion



    }
}
