using MySql.Data.MySqlClient;
using Panuon.UI.Silver.Core;
using PartialViewInterface;
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
        public MySqlBackUpViewModel()
        {
            GetTables();
            Policys = new ObservableCollection<BackUpPolicy>();
            CurrentPolicy = new BackUpPolicy();
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

            //dgTables.ItemsSource = tables;
        }

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





    }
}
