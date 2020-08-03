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
using PartialViewInterface;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using JieLinkSyncTool;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using UIBrowser.Helpers;

namespace PartialViewSyncTool
{
    /// <summary>
    /// CenterPartialSyncTool.xaml 的交互逻辑
    /// </summary>
    public partial class CenterPartialSyncTool : UserControl, IPartialView
    {
        public string MenuName
        {
            get { return "数据同步"; }
        }

        public string TagName
        {
            get { return "CenterPartialSyncTool"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public CenterPartialSyncTool()
        {
            InitializeComponent();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;

            //读取到配置文件，加载参数
            DbConfig.CenterDbConnStr = new DbConnEntity();
            DbConfig.BoxDbConnStrs = new List<DbConnEntity>();
            if (File.Exists(@"DbCenterConfig.ini"))
            {
                string ReadStr = File.ReadAllText(@"DbCenterConfig.ini");
                DbConfig.CenterDbConnStr = Newtonsoft.Json.JsonConvert.DeserializeObject<DbConnEntity>(ReadStr);
                txtCenterIp.Text = DbConfig.CenterDbConnStr.Ip;
                txtCenterDbPort.Text = DbConfig.CenterDbConnStr.Port.ToString();
                txtCenterDbUser.Text = DbConfig.CenterDbConnStr.UserName;
                txtCenterDbPwd.Password = DbConfig.CenterDbConnStr.Password;
                txtCenterDb.Text = DbConfig.CenterDbConnStr.DbName;
            }
        }

        /// <summary>
        /// 所有的命令字
        /// </summary>
        public List<string> Cmds { get; set; }

        /// <summary>
        /// 查询天数
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// 查询条数
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 循环频率
        /// </summary>
        public int LoopSecond { get; set; }

        bool isRuning = true;
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string DbConnectString { get; set; }

        /// <summary>
        /// 连接字符串配置
        /// </summary>
        public DbConfigEntity DbConfig = new DbConfigEntity();


        BackgroundWorker backgroundWorker = new BackgroundWorker();
        private void btnStartTask_Click(object sender, RoutedEventArgs e)
        {
            //1.获取所有的界面配置
            Cmds = txtCmd.Text.Split(';').ToList();
            Day = int.Parse(txtCheckDay.Text);
            Limit = int.Parse(txtCheckRow.Text);
            LoopSecond = int.Parse(txtLoopSecond.Text);

            //2.开启任务
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }

            //3.禁用按钮
            this.btnStartTask.IsEnabled = false;
        }

        /// <summary>
        /// 测试连接中心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            DbConnectString = $"Data Source={txtCenterIp.Text};port={txtCenterDbPort.Text};User ID={txtCenterDbUser.Text};Password={txtCenterDbPwd.Password};Initial Catalog={txtCenterDb.Text};";

            try
            {
                MySqlHelper.ExecuteDataset(DbConnectString, "select * from sys_user limit 1");
                ShowMessage("中心数据库连接成功！");
                //存储中心连接字符串
                SaveCenterDbConfig(DbConfig);
                CheckBoxConnStr();
                if (dictBoxConnStr.Count > 0)
                {
                    this.btnTestConn.IsEnabled = false;
                    this.btnStartTask.IsEnabled = true;
                }
                else
                { ShowMessage("没有获取到任何盒子的连接信息！"); }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
                MessageBox.Show("数据库连接失败！");
            }
        }

        /// <summary>
        /// 获取所有盒子的连接信息
        /// </summary>
        Dictionary<string, string> dictBoxConnStr = new Dictionary<string, string>();
        private void CheckBoxConnStr()
        {
            ShowMessage("正在检测盒子的数据库连接，请等待！");
            string sql = "SELECT IP from control_devices where DeviceType = 25";
            DataTable dt = MySqlHelper.ExecuteDataset(DbConnectString, sql).Tables[0];
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
                        DbConfig dbConfig = new DbConfig(ip);
                        if (dbConfig.ShowDialog() == true)
                        {
                            ShowMessage($"盒子{ip}连接成功");
                            //存储盒子连接字符串
                            SaveBoxDbConfig(DbConfig, dbConfig.DbConnString);
                            dictBoxConnStr.Add(ip, dbConfig.DbConnString);
                        }
                    }
                }
                //全部保存盒子字符串后保存到文件
                SaveBoxDbConfigFile(DbConfig);

            }

        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                ShowMessage(e.UserState.ToString());
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isRuning)
            {
                StartCompareData();
                System.Threading.Thread.Sleep(LoopSecond * 1000);
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
                    backgroundWorker.ReportProgress(1, $"未获取到满足条件的数据");
                    continue;
                }
                int minDownloadId = centerDatas.Min(x => x.Id);
                foreach (var ip in dictBoxConnStr.Keys)
                {
                    backgroundWorker.ReportProgress(1, $"比对盒子{ip}的数据");
                    List<SyncBoxEntity> boxDatas = GetBoxData(dictBoxConnStr[ip], cmd, minDownloadId);

                    var boxNotExists = centerDatas.Where(a => !boxDatas.Exists(t => a.Id == t.Id)).ToList();
                    if (boxNotExists.Count <= 0)
                    {
                        backgroundWorker.ReportProgress(1, $"盒子{ip}的数据与中心一致");
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
            backgroundWorker.ReportProgress(1, "查询sync_box_http表");
            List<SyncBoxEntity> syncBoxEntities = new List<SyncBoxEntity>();
            string sql = $"select id,protocoldata,datatype from sync_box_http where ObjId like '%{cmd}' and DATE_ADD(AddTime, INTERVAL {day} DAY)> NOW() limit {limit}";

            DataTable dt = MySqlHelper.ExecuteDataset(DbConnectString, sql).Tables[0];
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
            backgroundWorker.ReportProgress(1, "查询sync_center_downloadprocess表");
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
            backgroundWorker.ReportProgress(1, $"插入遗漏数据Id={syncBoxEntity.Id} 命令字={syncBoxEntity.Cmd}");
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message"></param>

        private void ShowMessage(string message)
        {
            try
            {
                rtxShowMessage.AppendText(message);
                rtxShowMessage.AppendText(Environment.NewLine);
                //超过一定长度后清空richtextbox
                TextRange MyText = new TextRange(rtxShowMessage.Document.ContentStart, rtxShowMessage.Document.ContentEnd);
                if (MyText.Text.Length > 5000)
                    rtxShowMessage.Document.Blocks.Clear();
            }
            catch (Exception)
            {

            }
        }

        //private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    isRuning = false;
        //}

        private void SaveCenterDbConfig(DbConfigEntity dbconfig)
        {
            dbconfig.CenterDbConnStr.Ip = txtCenterIp.Text;
            dbconfig.CenterDbConnStr.Port = Convert.ToInt32(txtCenterDbPort.Text);
            dbconfig.CenterDbConnStr.UserName = txtCenterDbUser.Text;
            dbconfig.CenterDbConnStr.Password = txtCenterDbPwd.Password;
            dbconfig.CenterDbConnStr.DbName = txtCenterDb.Text;

            System.IO.File.WriteAllText("DbCenterConfig.ini", Newtonsoft.Json.JsonConvert.SerializeObject(dbconfig.CenterDbConnStr), Encoding.UTF8);
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

        private void SaveBoxDbConfigFile(DbConfigEntity dbconfig)
        {
            System.IO.File.WriteAllText("DbBoxConfig.ini", Newtonsoft.Json.JsonConvert.SerializeObject(dbconfig.BoxDbConnStrs), Encoding.UTF8);
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

        private void chbVersion_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            { return; }
            txtCmd.Text = "742;743;820;821;74A;811";
        }

        private void chbVersion_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            { return; }
            txtCmd.Text = "82A";
        }
    }
}
