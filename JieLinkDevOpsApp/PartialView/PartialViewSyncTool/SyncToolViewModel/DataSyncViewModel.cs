using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewSyncTool.SyncToolViewModel
{
    public class DataSyncViewModel : DependencyObject
    {
        public DelegateCommand GetBoxConnStringCommand { get; set; }

        public DelegateCommand StartDataSyncCommand { get; set; }

        public DelegateCommand StopDataSyncCommand { get; set; }

        private bool canExecute = false;

        BoxConnConfig boxConnConfig;

        Dictionary<string, string> dictBoxConnStr;

        public DataSyncViewModel()
        {
            CMD = "82A";
            LoopSecond = 5;
            Day = 1;
            Limit = 100;
            boxConnConfig = new BoxConnConfig();
            boxConnConfig.ShowMessage += BoxConnConfig_ShowMessage;

            dictBoxConnStr = new Dictionary<string, string>();

            GetBoxConnStringCommand = new DelegateCommand();
            GetBoxConnStringCommand.ExecuteAction = GetBoxConnString;

            StartDataSyncCommand = new DelegateCommand();
            StartDataSyncCommand.ExecuteAction = Start;
            StartDataSyncCommand.CanExecuteFunc = new Func<object, bool>((object obj) => { return canExecute; });

            StopDataSyncCommand = new DelegateCommand();
            StopDataSyncCommand.ExecuteAction = Stop;
            StopDataSyncCommand.CanExecuteFunc = new Func<object, bool>((object obj) => { return canExecute; });
        }

        private void BoxConnConfig_ShowMessage(string message)
        {
            ShowMessage(message);
        }

        private void GetBoxConnString(object parameter)
        {
            dictBoxConnStr= boxConnConfig.GetBoxConnString();
            if (dictBoxConnStr.Count > 0)
            {
                canExecute = true;
            }
        }

        private void Start(object parameter)
        {
            cmds = new List<string>();
            if (!string.IsNullOrEmpty(CMD))
            {
                cmds = CMD.Split(';').ToList();
            }

            day = Day;
            limit = Limit;
            loopTime = LoopSecond;
            running = true;

            Task.Factory.StartNew(() =>
            {
                while (running)
                {
                    StartCompareData();
                    System.Threading.Thread.Sleep(loopTime * 1000);
                }
            });
        }

        private void Stop(object parameter)
        {
            running = false;
            ShowMessage("已停止检测");
        }

        private List<string> cmds;
        private int day;
        private int limit;
        private int loopTime;
        private bool running = true;

        /// <summary>
        /// 比对数据
        /// </summary>
        private void StartCompareData()
        {
            foreach (var cmd in cmds)
            {
                List<SyncBoxEntity> centerDatas = GetCenterData(cmd, day, limit);
                if (centerDatas.Count <= 0)
                {
                    ShowMessage("未获取到满足条件的数据");
                    continue;
                }
                int minDownloadId = centerDatas.Min(x => x.Id);
                foreach (var ip in dictBoxConnStr.Keys)
                {
                    ShowMessage($"比对盒子{ip}的数据");
                    List<SyncBoxEntity> boxDatas = GetBoxData(dictBoxConnStr[ip], cmd, minDownloadId);

                    var boxNotExists = centerDatas.Where(a => !boxDatas.Exists(t => a.Id == t.Id)).ToList();
                    if (boxNotExists.Count <= 0)
                    {
                        ShowMessage($"盒子{ip}的数据与中心一致");
                    }
                    foreach (var entity in boxNotExists)
                    {
                        InsertData(dictBoxConnStr[ip], entity);
                    }
                }
            }
        }

        /// <summary>
        /// 获取中心的数据
        /// </summary>
        /// <returns></returns>
        private List<SyncBoxEntity> GetCenterData(string cmd, int day, int limit)
        {
            ShowMessage("查询sync_box_http表");
            List<SyncBoxEntity> syncBoxEntities = new List<SyncBoxEntity>();
            string sql = $"select id,protocoldata,datatype from sync_box_http where ObjId like '%{cmd}' and DATE_ADD(AddTime, INTERVAL {day} DAY)> NOW() limit {limit}";

            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    syncBoxEntities.Add(new SyncBoxEntity() { Id = int.Parse(dr["id"].ToString()), Cmd = cmd, JsonData = dr["protocoldata"].ToString(), DataType = int.Parse(dr["datatype"].ToString()) });
                }
            }
            return syncBoxEntities;
        }

        /// <summary>
        /// 获取盒子数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="minId"></param>
        /// <returns></returns>
        private List<SyncBoxEntity> GetBoxData(string ConnString, string cmd, int minId)
        {
            ShowMessage("查询sync_center_downloadprocess表");
            List<SyncBoxEntity> syncBoxEntities = new List<SyncBoxEntity>();
            string sql = $"select downloadid,jsondata,datatype from sync_center_downloadprocess where serviceid = '{cmd}' and downloadid>={minId}";
            DataTable dt = MySqlHelper.ExecuteDataset(ConnString, sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    syncBoxEntities.Add(new SyncBoxEntity() { Id = int.Parse(dr["downloadid"].ToString()), Cmd = cmd, JsonData = dr["jsondata"].ToString(), DataType = int.Parse(dr["datatype"].ToString()) });
                }
            }
            return syncBoxEntities;
        }

        /// <summary>
        /// 往盒子插入遗漏数据
        /// </summary>
        /// <param name="syncBoxEntity"></param>
        private void InsertData(string ConnString, SyncBoxEntity syncBoxEntity)
        {
            syncBoxEntity.JsonData = syncBoxEntity.JsonData.TrimStart('[').TrimEnd(']');
            string sql = $"INSERT INTO `sync_center_downloadprocess`(downloadid, serviceid, jsondata, status, processtime, datatype, remark) VALUES ({syncBoxEntity.Id},'{syncBoxEntity.Cmd}','{syncBoxEntity.JsonData}','0',NOW(),{syncBoxEntity.DataType},'add by auto tool')";
            MySqlHelper.ExecuteNonQuery(ConnString, sql);
            ShowMessage($"插入遗漏数据Id={syncBoxEntity.Id} 命令字={syncBoxEntity.Cmd}");
        }


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

        /// <summary>
        /// 所有的命令字
        /// </summary>
        public string CMD
        {
            get { return (string)GetValue(CMDProperty); }
            set { SetValue(CMDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CMD.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CMDProperty =
            DependencyProperty.Register("CMD", typeof(string), typeof(DataSyncViewModel));



        /// <summary>
        /// 查询天数
        /// </summary>

        public int Day
        {
            get { return (int)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Day.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(int), typeof(DataSyncViewModel));


        public int Limit
        {
            get { return (int)GetValue(LimitProperty); }
            set { SetValue(LimitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Limit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LimitProperty =
            DependencyProperty.Register("Limit", typeof(int), typeof(DataSyncViewModel));

        /// <summary>
        /// 循环频率
        /// </summary>

        public int LoopSecond
        {
            get { return (int)GetValue(LoopSecondProperty); }
            set { SetValue(LoopSecondProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoopSecond.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoopSecondProperty =
            DependencyProperty.Register("LoopSecond", typeof(int), typeof(DataSyncViewModel));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(DataSyncViewModel));

    }
}
