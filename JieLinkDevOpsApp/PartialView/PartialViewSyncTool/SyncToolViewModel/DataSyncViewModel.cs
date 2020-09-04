using JieLinkSyncTool;
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

        private bool canExecute = false;

        public DataSyncViewModel()
        {
            GetBoxConnStringCommand = new DelegateCommand();
            GetBoxConnStringCommand.ExecuteAction = GetBoxConnString;

            StartDataSyncCommand = new DelegateCommand();
            StartDataSyncCommand.ExecuteAction = Start;
            StartDataSyncCommand.CanExecuteFunc = new Func<object, bool>((object obj) => { return canExecute; });
        }

        /// <summary>
        /// 获取所有盒子的连接信息
        /// </summary>
        Dictionary<string, string> dictBoxConnStr = new Dictionary<string, string>();

        /// <summary>
        /// 连接字符串配置
        /// </summary>
        public DbConfigEntity DbConfig = new DbConfigEntity();

        private void GetBoxConnString(object parameter)
        {
            ShowMessage("正在检测盒子的数据库连接，请等待！");
            string sql = "SELECT IP from control_devices where DeviceType = 25";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string ip = dr["IP"].ToString();
                    if (dictBoxConnStr.ContainsKey(ip))
                    {
                        continue;
                    }

                    string boxConn = $"Data Source={ip};port=10080;User ID=test;Password=123456;Initial Catalog=smartbox;";

                    //读取到盒子配置文件，加载参数
                    ReadBoxConfig(ref boxConn, ip);

                    try
                    {
                        string cmd = "select * from sys_boxinformation";
                        MySqlHelper.ExecuteDataset(boxConn, cmd);
                        ShowMessage($"盒子{ip}连接成功");
                        //存储盒子连接字符串
                        SaveBoxDbConfig(DbConfig, boxConn);
                        dictBoxConnStr.Add(ip, boxConn);
                    }
                    catch (Exception)
                    {
                        (Application.Current.MainWindow as WindowX).IsMaskVisible = true;
                        DbConfig dbConfig = new DbConfig(ip);
                        if (dbConfig.ShowDialog() == true)
                        {
                            ShowMessage($"盒子{ip}连接成功");
                            //存储盒子连接字符串
                            SaveBoxDbConfig(DbConfig, dbConfig.DbConnString);
                            dictBoxConnStr.Add(ip, dbConfig.DbConnString);
                        }
                        (Application.Current.MainWindow as WindowX).IsMaskVisible = false;
                    }
                }
                //全部保存盒子字符串后保存到文件
                SaveBoxDbConfigFile(DbConfig);

                canExecute = true;

            }
        }

        private void SaveBoxDbConfigFile(DbConfigEntity dbconfig)
        {
            System.IO.File.WriteAllText("DbBoxConfig.ini", Newtonsoft.Json.JsonConvert.SerializeObject(dbconfig.BoxDbConnStrs), Encoding.UTF8);
        }

        private void SaveBoxDbConfig(DbConfigEntity dbconfig, string conbox)
        {
            //string boxConn = $"Data Source={ip};port=10080;User ID=test;Password=123456;Initial Catalog=smartbox;";

            string[] strsplit = conbox.Split(new char[2] { '=', ';' });
            DbConnEntity boxcon = new DbConnEntity();
            boxcon.Ip = strsplit[1];
            boxcon.Port = Convert.ToInt32(strsplit[3]);
            boxcon.UserName = strsplit[5];
            boxcon.Password = strsplit[7];
            boxcon.DbName = strsplit[9];
            dbconfig.BoxDbConnStrs.Add(boxcon);

        }

        private void ReadBoxConfig(ref string str, string ip)
        {
            if (File.Exists(@"DbBoxConfig.ini"))
            {
                string ReadStr = File.ReadAllText(@"DbBoxConfig.ini");
                DbConfig.BoxDbConnStrs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DbConnEntity>>(ReadStr);
                DbConnEntity find = DbConfig.BoxDbConnStrs.Find(a => a.Ip == ip);
                if (find != null)
                    str = $"Data Source={find.Ip};port={find.Port};User ID={find.UserName};Password={find.Password};Initial Catalog={find.DbName};";
                else
                    return;
            }
            return;
        }

        private void Start(object parameter)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    StartCompareData();
                    System.Threading.Thread.Sleep(1000);
                }
            });
        }

        public List<string> Cmds
        {
            get
            {
                if (!string.IsNullOrEmpty(CMD))
                {
                    return CMD.Split(';').ToList();
                }
                return new List<string>();
            }
        }


        /// <summary>
        /// 比对数据
        /// </summary>
        private void StartCompareData()
        {
            foreach (var cmd in Cmds)
            {
                List<SyncBoxEntity> centerDatas = GetCenterData(cmd, Day, Limit);
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
