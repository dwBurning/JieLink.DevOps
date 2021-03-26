using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewCheckUpload.Info;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace PartialViewCheckUpload.ViewModels
{
    class CheckUploadViewModels : DependencyObject
    {
        /// <summary>
        /// 从sys_taskinfo表里获取所有任务
        /// </summary>
        public List<TaskInfo> TaskInfos { get; set; }

        // 研究了一天才发现WPF绑定不支持List，要使用动态数据集合ObservableCollection
        //public List<SyncInfo> SyncInfos { get; set; }
        public ObservableCollection<SyncInfo> SyncInfos { get; set; }

        // 车场基础数据分类(哪些在sync_park_base_history)
        public List<string> List_ParkBase = new List<string>();
        
        // 界面上选择的任务列表
        public List<string> List_AllChoose = new List<string>();

        // 界面上选择的sync任务列表
        public List<string> List_SyncChoose = new List<string>();

        // 界面上选择的parkbase任务列表
        public List<string> List_ParkBaseChoose = new List<string>();

        public DelegateCommand SelectSyncCommand { get; set; }
        public DelegateCommand ChooseAllCommand { get; set; }

        public CheckUploadViewModels()
        {
            //初始化界面
            IsSelectedAll = true;
            limit10 = true;

            StartTime = DateTime.Now.AddHours(-1);
            EndTime = DateTime.Now.AddHours(1);

            TaskInfos = new List<TaskInfo>();
            SyncInfos = new ObservableCollection<SyncInfo>();

            SelectSyncCommand = new DelegateCommand();
            ChooseAllCommand = new DelegateCommand();
            SelectSyncCommand.ExecuteAction = SelectSync;
            ChooseAllCommand.ExecuteAction = ChooseAll;

            ////初始化数据 在这里初始化一启动程序就会加载
            //GetTaskInfos();
            //GetControlBaseData();
        }

        public void GetControlBaseData()
        {
            try
            {
                string sql = $"select valuetext from sys_key_value_setting where KeyID='ControlBaseData' limit 1;";
                using (DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0])
                {
                    foreach (var item in dt.Rows[0]["valuetext"].ToString().Split(';'))
                    {
                        List_ParkBase.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                throw;
            }
        }

        //查询sync表
        private void SelectSync(object parameter)
        {
            try
            {
                SyncInfos.Clear();

                // 获取已选择的任务
                TaskInfos.Clear();
                TaskInfos.ForEach((x) =>
                {
                    if (x.IsChecked)
                    { List_AllChoose.Add(x.ServiceId); }
                });

                // 已选择的任务和park_Base的表取交集
                List_ParkBaseChoose = List_ParkBase.Intersect(List_AllChoose).ToList<string>();

                string temtttp = "";
                List_AllChoose.ForEach((x)=>
                {
                    temtttp += x + ",";
                });
                LogHelper.CommLogger.Info("List_AllChoose:" + temtttp);
                temtttp = "";
                List_ParkBaseChoose.ForEach((x) =>
                {
                    temtttp += x + ",";
                });
                LogHelper.CommLogger.Info("List_ParkBaseChoose:" + temtttp);
                temtttp = "";
                List_ParkBase.ForEach((x) =>
                {
                    temtttp += x + ",";
                });
                LogHelper.CommLogger.Info("List_ParkBase:" + temtttp);

                // 总的已选择的和选择的park_Base取差集
                List_SyncChoose = List_AllChoose.Except(List_ParkBaseChoose).ToList<string>();

                var chayige = List_AllChoose.Except(List_ParkBaseChoose).ToList<string>();
                chayige = chayige.Except(List_SyncChoose).ToList<string>();
                string sql = $"SELECT AddTime,protocoldata,status,remark,failmessage,updatetime from sync_xmpp_history ";

                //根据protocol数据筛选查询 不允许为空
                if (!string.IsNullOrEmpty(SelectLike))
                {
                    if (SelectLike.Trim() == "")
                    {
                        Notice.Show("筛选数据条件不允许为空", "错误", 3, MessageBoxIcon.Error);
                        return;
                    }
                    sql += "where protocoldata like '%" + SelectLike + "%' ";
                }
                else
                {
                    Notice.Show("筛选数据条件不允许为空", "错误", 3, MessageBoxIcon.Error);
                    return;
                }
                //根据时间筛选查询 颠倒顺序也行
                sql += "and addtime between '" + StartTime.ToString() + "' and '" + EndTime.ToString() + "' ";

                //根据任务筛选in

                //根据ID排序
                sql += "ORDER BY id desc ";

                //根据limit限制
                if (limit10)
                    sql += "limit 10 ";
                else if (limit20)
                    sql += "limit 20 ";
                else if (limit50)
                    sql += "limit 50";
                else if (limit100)
                    sql += "limit 100 ";
                else
                    return;



                //sync_xmpp_history表里查询
                LogHelper.CommLogger.Info("查询上传执行的SQL语句(sync_xmpp_history):" + sql);
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];//获取所有的表
                foreach (DataRow dr in dt.Rows)
                {
                    SyncInfo syncinfo = new SyncInfo { AddTime = dr["AddTime"].ToString(), ProtocolData = dr["protocoldata"].ToString(), status = dr["status"].ToString(),
                    remark = dr["remark"].ToString(),failmessage = dr["failmessage"].ToString(),updatetime = dr["updatetime"].ToString()
                    };
                    SyncInfos.Add(syncinfo);
                }
                //sync_xmpp表里查询
                string sql2 = sql.Replace("sync_xmpp_history", "sync_xmpp");
                LogHelper.CommLogger.Info("查询上传执行的SQL语句(sync_xmpp):" + sql2);
                DataTable dt2 = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql2).Tables[0];//获取所有的表
                foreach (DataRow dr in dt2.Rows)
                {
                    SyncInfo syncinfo = new SyncInfo
                    {
                        AddTime = dr["AddTime"].ToString(),
                        ProtocolData = dr["protocoldata"].ToString(),
                        status = dr["status"].ToString(),
                        remark = dr["remark"].ToString(),
                        failmessage = dr["failmessage"].ToString(),
                        updatetime = dr["updatetime"].ToString()
                    };
                    SyncInfos.Add(syncinfo);
                }

                // 在sync_park_base_history表里查询
                // 怎么sync_park_base_history结构还和sync_xmpp不一样的
                string sql3 = sql.Replace("sync_xmpp_history", "sync_park_base_history");
                sql3.Replace(",failmessage,updatetime", "");
                LogHelper.CommLogger.Info("查询上传执行的SQL语句(sync_xmpp):" + sql3);
                DataTable dt3 = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql3).Tables[0];//获取所有的表
                foreach (DataRow dr in dt2.Rows)
                {
                    SyncInfo syncinfo = new SyncInfo
                    {
                        AddTime = dr["AddTime"].ToString(),
                        ProtocolData = dr["protocoldata"].ToString(),
                        status = dr["status"].ToString(),
                        remark = dr["remark"].ToString(),
                        failmessage = "",
                        updatetime = ""
                    };
                    SyncInfos.Add(syncinfo);
                }

            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                throw;
            }
        }

        //获取任务表
        public void GetTaskInfos()
        {
            try
            {
                
                string sql = $"SELECT ServiceName,ServiceID from sys_taskinfo WHERE PlatType=0 AND IsSyncData=1 AND Enabled=1 ORDER BY SendPriority asc;";
                //获取所有的表
                using (DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        TaskInfo taskinfo = new TaskInfo { ServiceName = dr["ServiceName"].ToString(), ServiceId = dr["ServiceID"].ToString(), IsChecked = true };
                        TaskInfos.Add(taskinfo);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ChooseChange(int row)
        {
            if (row > SyncInfos.Count || row < 0)
            {
                ShowProtocol = "";
                return;
            }
            SyncInfo info = SyncInfos[row];
            string show = "详细上传记录：\r\n";
            show += "任务名：" + info.remark + "\r\n";
            show += "任务添加时间：" + info.AddTime + "  ";
            show += "任务上传时间：" + info.updatetime + "\r\n";
            show += "上传状态：" + info.status + "  ";
            show += (info.failmessage == "" )? "上传无报错信息\r\n" : "☆有报错：" + info.failmessage + "\r\n";
            show += "具体上传数据：" + info.ProtocolData + "\r\n";

            ShowProtocol = show;
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="parameter"></param>
        private void ChooseAll(object parameter)
        {
            TaskInfos.ForEach(x => x.IsChecked = IsSelectedAll);
            //WriteConfig();
        }

        #region 属性定义

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
            DependencyProperty.Register("IsSelectedAll", typeof(bool), typeof(CheckUploadViewModels));

        public string ShowProtocol
        {
            get { return (string)GetValue(ShowProtocolProperty); }
            set { SetValue(ShowProtocolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProtocolProperty =
            DependencyProperty.Register("ShowProtocol", typeof(string), typeof(CheckUploadViewModels));

        public string SelectLike
        {
            get { return (string)GetValue(SelectLikeProperty); }
            set { SetValue(SelectLikeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectLikeProperty =
            DependencyProperty.Register("SelectLike", typeof(string), typeof(CheckUploadViewModels));

        public bool limit10
        {
            get { return (bool)GetValue(limit10Property); }
            set { SetValue(limit10Property, value); }
        }
        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty limit10Property =
            DependencyProperty.Register("limit10", typeof(bool), typeof(CheckUploadViewModels));

        public bool limit20
        {
            get { return (bool)GetValue(limit20Property); }
            set { SetValue(limit20Property, value); }
        }
        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty limit20Property =
            DependencyProperty.Register("limit20", typeof(bool), typeof(CheckUploadViewModels));

        public bool limit50
        {
            get { return (bool)GetValue(limit50Property); }
            set { SetValue(limit50Property, value); }
        }
        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty limit50Property =
            DependencyProperty.Register("limit50", typeof(bool), typeof(CheckUploadViewModels));

        public bool limit100
        {
            get { return (bool)GetValue(limit100Property); }
            set { SetValue(limit100Property, value); }
        }
        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty limit100Property =
            DependencyProperty.Register("limit100", typeof(bool), typeof(CheckUploadViewModels));

        public DateTime StartTime
        {
            get {
                return (DateTime)GetValue(StartTimeProperty); 
            }
            set { 
                SetValue(StartTimeProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(CheckUploadViewModels));

        public void StartDateChange(DateTime dt)
        {
            StartTime = dt;
        }

        public DateTime EndTime
        {
            get
            {
                return (DateTime)GetValue(EndTimeProperty);
            }
            set
            {
                SetValue(EndTimeProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsSelectedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(CheckUploadViewModels));
        public void EndDateChange(DateTime dt)
        {
            EndTime = dt;
        }
        #endregion
    }
}
