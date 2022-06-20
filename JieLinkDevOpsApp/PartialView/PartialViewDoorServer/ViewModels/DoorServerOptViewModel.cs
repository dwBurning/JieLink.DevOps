using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Xml.Linq;

namespace PartialViewDoorServer.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class DoorServerOptViewModel : DependencyObject
    {
        public DoorServerOptViewModel()
        {
            Message += "无需配置门禁服务，点击修复门禁常见问题可自动检测常见门禁问题\r\n";
            Message += "目前检测项(ver 1.1.71)：\r\n";
            Message += "1.sync_doornum导致的新发卡无法自动下载到设备问题\r\n";
            Message += "2.门禁服务MAC地址配置导致的门禁服务离线问题，以及是否共机部署的端口问题\r\n";
            Message += "3.HTTP参数不对导致自动下载、完全下载均无法下载人脸到设备问题\r\n";
            Message += "4.Y10A、Y10A_T、Y08A-old搜索显示未知设备的问题\r\n";
            Message += "5.完全下载时提示检测操作失败，请重试问题\r\n";
            SelectPathCommand = new DelegateCommand();
            SelectPathCommand.ExecuteAction = SelectPath;

            GetDoorDeviceCommand = new DelegateCommand();
            GetDoorDeviceCommand.ExecuteAction = SelectDoorDevice;

            RepairCommand = new DelegateCommand();
            RepairCommand.ExecuteAction = RepairDoorClickServer;

            //GetDoorServerInfo();
        }

        /// <summary>
        /// 路径
        /// </summary>
        public DelegateCommand SelectPathCommand { get; set; }
        /// <summary>
        /// 修复常见问题
        /// </summary>
        public DelegateCommand RepairCommand { get; set; }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        /// <summary>
        /// 获取门禁服务
        /// </summary>
        public List<DoorServerInfo> DoorServerInfoList
        {
            get { return _doorServerInfoList; }
            set { _doorServerInfoList = value; }
        }
        private List<DoorServerInfo> _doorServerInfoList = new List<DoorServerInfo>();

        /// <summary>
        /// 当前选中门禁服务
        /// </summary>
        public static DoorServerInfo curDoorServer { get; set; }

        /// <summary>
        /// 获取设备
        /// </summary>
        public DelegateCommand GetDoorDeviceCommand { get; set; }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(DoorServerOptViewModel), new PropertyMetadata(""));

        private List<string> filePathList = new List<string>();
        string defaultfilePath = "";
        /// <summary>
        /// 路径
        /// </summary>
        /// <param name="parameter"></param>
        private void SelectPath(object parameter)
        {
            System.Windows.Forms.FolderBrowserDialog fileDialog = new System.Windows.Forms.FolderBrowserDialog();

            var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
            if (process != null)
            {
                fileDialog.SelectedPath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, "para");
            }
            if (!string.IsNullOrEmpty(defaultfilePath))
            {
                fileDialog.SelectedPath = defaultfilePath;
            }

            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                defaultfilePath = fileDialog.SelectedPath;

                FilePath = fileDialog.SelectedPath;
            }
        }

        /// <summary>
        /// 获取设备
        /// </summary>
        /// <param name="parameter"></param>
        public void SelectDoorDevice(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    Notice.Show("请选择Smartdoor安装目录的para文件夹！.", "通知", 3, MessageBoxIcon.Warning);
                    return;
                }

                UInt32 deviceID = curDoorServer.deviceID;
                if (deviceID == 0)
                {
                    Notice.Show("请选择门禁服务！.", "通知", 3, MessageBoxIcon.Warning);
                    return;
                }

                bool result = false;
                string cmd = "select * from control_devices where deviceID = " + deviceID + " or parentID = " + deviceID;
                List<DeviceInfo> jsipDevList = new List<DeviceInfo>();
                List<PartialViewDoorServer.ViewModels.K02.DeviceInfo> k02DevList = new List<PartialViewDoorServer.ViewModels.K02.DeviceInfo>();
                using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, string.Format(cmd)))
                {
                    while (reader.Read())
                    {
                        ushort deviceType = Convert.ToUInt16(reader["DeviceType"].ToString());
                        if (deviceType == 32)
                        {
                            #region 门禁服务
                            // 门禁服务
                            PartialViewDoorServer.ViewModels.K02.DeviceInfo k02Dev = new PartialViewDoorServer.ViewModels.K02.DeviceInfo();
                            k02Dev.type = deviceType;
                            k02Dev.id = Convert.ToUInt32(reader["DeviceID"].ToString());
                            k02Dev.ip = reader["IP"].ToString();
                            k02Dev.model = "门禁服务客户端";
                            k02Dev.status = 1;
                            k02Dev.statusUpdateTime = DateTime.Now;
                            k02Dev.DeviceAddTime = DateTime.Now;

                            k02DevList.Add(k02Dev);
                            #endregion
                        }
                        else if (deviceType == 100 || deviceType == 116)
                        {
                            #region 领域III
                            // 领域III
                            PartialViewDoorServer.ViewModels.K02.DeviceInfo k02Dev = new PartialViewDoorServer.ViewModels.K02.DeviceInfo();
                            k02Dev.type = deviceType;
                            k02Dev.id = Convert.ToUInt32(reader["DeviceID"].ToString());
                            k02Dev.ip = reader["IP"].ToString();
                            k02Dev.status = 1;
                            k02Dev.productDate = DateTime.Now.ToString();
                            k02Dev.statusUpdateTime = DateTime.Now;
                            k02Dev.DeviceAddTime = DateTime.Now;

                            k02DevList.Add(k02Dev);
                            #endregion
                        }
                        else
                        {
                            #region JSIP设备
                            // JSIP设备
                            DeviceInfo dev = new DeviceInfo();
                            dev.SendHttpTime = DateTime.Now;
                            dev.DeviceAddTime = DateTime.Now;
                            dev.status = 1;
                            dev.state = 1;
                            dev.ID = Convert.ToUInt32(reader["DeviceID"].ToString());
                            dev.ParentDeviceID = 0;
                            dev.Name = reader["DeviceName"].ToString();
                            dev.Type = deviceType.ToString();
                            dev.IP = reader["IP"].ToString();
                            dev.Mac = reader["Mac"].ToString();
                            dev.Mask = reader["Net_Mask"].ToString();
                            dev.GateWay = reader["Gateway_IP"].ToString();

                            dev.SN = string.IsNullOrEmpty(reader["SN"].ToString()) ? 0 : Convert.ToUInt32(reader["SN"].ToString());
                            dev.Company = "捷顺科技";
                            dev.Model = reader["Model"].ToString();
                            dev.HardwareVersion = "V1.0.0";
                            dev.SoftwareVersion = "V1.0.0";
                            dev.Manufacturer = "捷顺科技";
                            dev.ProductDate = DateTime.Now.ToString();
                            dev.DeviceClass = 0;
                            dev.CPUID = reader["CpuID"].ToString();
                            dev.RegisterState = 1;
                            dev.HasInit = true;
                            dev.RegisterID = 1;
                            dev.IsHttpSended = false;
                            dev.AuthorizeFlag = 1;

                            jsipDevList.Add(dev);
                            #endregion
                        }
                        result = true;
                    }
                }

                string k02DevicePath = FilePath + @"\DeviceInfos.xml";
                string jsipDevicePath = FilePath + @"\CriterionDeviceInfos.xml";

                // 备份
                string devicePathBak = FilePath + @"_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                CopyDirectory(FilePath, devicePathBak);

                Notice.Show("para文件夹备份成功：" + devicePathBak, "通知", 3, MessageBoxIcon.Success);

                SerializationHelper.SerializeToXMLFile<List<PartialViewDoorServer.ViewModels.K02.DeviceInfo>>(k02DevicePath, k02DevList);
                SerializationHelper.SerializeToXMLFile<List<DeviceInfo>>(jsipDevicePath, jsipDevList);

                Notice.Show("获取设备成功....", "通知", 3, MessageBoxIcon.Success);
                var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
                if (process != null)
                {
                    process.Kill();
                    Notice.Show("已结束门禁服务!等待自动重启门禁服务", "通知", 3, MessageBoxIcon.Success);
                }
                else
                {
                    Notice.Show("需手动结束门禁服务进程，重启门禁服务!", "通知", 3, MessageBoxIcon.Success);
                }

                // 非2.8.1会获取不到这个路径导致报错
                // 2.8.1以后，自动取这个备份文件夹的文件
                string strParaDataDirDest = string.Format(@"{0}\JieLinkDoor\para", System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                File.Copy(k02DevicePath, strParaDataDirDest + @"\DeviceInfos.xml", true);
                File.Copy(jsipDevicePath, strParaDataDirDest + @"\CriterionDeviceInfos.xml", true);
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourceDirPath"></param>
        /// <param name="SaveDirPath"></param>
        public static void CopyDirectory(string sourceDirPath, string SaveDirPath)
        {
            try
            {
                //如果指定的存储路径不存在，则创建该存储路径
                if (!Directory.Exists(SaveDirPath))
                {
                    //创建
                    Directory.CreateDirectory(SaveDirPath);
                }
                //获取源路径文件的名称
                string[] files = Directory.GetFiles(sourceDirPath);
                //遍历子文件夹的所有文件
                foreach (string file in files)
                {
                    string pFilePath = SaveDirPath + "\\" + Path.GetFileName(file);
                    if (File.Exists(pFilePath))
                        continue;
                    File.Copy(file, pFilePath, true);
                }
                string[] dirs = Directory.GetDirectories(sourceDirPath);
                //递归，遍历文件夹
                foreach (string dir in dirs)
                {
                    CopyDirectory(dir, SaveDirPath + "\\" + Path.GetFileName(dir));
                }
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 获取门禁服务IP
        /// </summary>
        public void GetDoorServerInfo()
        {
            try
            {
                string cmd = "select * from control_devices";

                using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, string.Format(cmd)))
                {
                    _doorServerInfoList.Clear();
                    while (reader.Read())
                    {
                        ushort deviceType = Convert.ToUInt16(reader["DeviceType"].ToString());
                        if (deviceType == 32)
                        {
                            #region 门禁服务
                            // 门禁服务
                            DoorServerInfo doorServer = new DoorServerInfo();
                            doorServer.deviceID = Convert.ToUInt32(reader["DeviceID"].ToString());
                            doorServer.deviceName = reader["DeviceName"].ToString();
                            doorServer.devIp = reader["IP"].ToString();

                            _doorServerInfoList.Add(doorServer);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                //Notice.Show("请先设置数据库连接！", "通知", 3, MessageBoxIcon.Success);
                throw ex;
            }
        }

        /// <summary>
        /// 检测自动下载问题
        /// </summary>
        /// <returns> true:发现问题已修改 </returns>
        public bool CheckSyncDoorNum()
        {
            try
            {
                string sqlstr = "select MAX(ID) from sync_door";
                MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                if (reader.HasRows)
                    reader.Read();
                else
                    return false;
                int maxid = Convert.ToInt32(reader["MAX(ID)"].ToString());

                sqlstr = "select GetNum from sync_doornum";
                MySqlDataReader readerGetNum = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                while (readerGetNum.Read())
                {
                    int GetNum = Convert.ToInt32(readerGetNum["GetNum"].ToString());
                    //getnum远大于maxid时，对所有有问题的getnum执行更新
                    if (GetNum > maxid + 100 || GetNum > maxid * 2)
                    {
                        sqlstr = string.Format("update sync_doornum set GetNum = {0} where GetNum > {0}",maxid);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sqlstr);
                        return true;
                    }
                    if(GetNum> maxid)
                    {
                        ShowMessage("GetNum>maxId，需要手动检查sync_doornum");
                    }
                }
                
                return false;
            }
            catch(Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                return false;
            }
        }

        public string IsConfigMac = string.Empty;
        public string ConfigMAC = string.Empty;
        public string sqlMac = string.Empty;
        public string configpath = string.Empty;
        /// <summary>
        /// 检测门禁服务MAC地址配置
        /// </summary>
        /// <returns></returns>
        public bool CheckDoorServerMac()
        {
            try
            {
                if (_doorServerInfoList.Count == 0)
                    return false;

                //根据门禁服务进程获取config文件路径
                System.Windows.Forms.FolderBrowserDialog fileDialog = new System.Windows.Forms.FolderBrowserDialog();
                var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
                if (process != null)
                {
                    configpath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, "SmartBoxDoor.Infrastructures.Server.DoorServer.exe");
                }
                else
                {
                    return false;
                }

                Configuration config = ConfigurationManager.OpenExeConfiguration(configpath);
                IsConfigMac = config.AppSettings.Settings["IsConfigMac"].Value;
                ConfigMAC = config.AppSettings.Settings["MAC"].Value;

                //顺便检测中心通讯8021端口
                string ClientPort8021 = config.AppSettings.Settings["ClientPort8021"].Value;
                //string ClientPort8022 = config.AppSettings.Settings["ClientPort8022"].Value;
                //string ClientPort8023 = config.AppSettings.Settings["ClientPort8023"].Value;
                //string ClientPort8025 = config.AppSettings.Settings["ClientPort8025"].Value;
                var processcenter = Process.GetProcessesByName("SmartCenter.Host").FirstOrDefault();
                if(processcenter!= null && ClientPort8021 == "8021")
                {
                    ShowMessage("警告：检测到共机部署、门禁配置端口为8021，或导致设备无法通讯！需手动校验！如果该问题无法自行处理请捷服务帮助");
                }
                else if(processcenter == null &&  ClientPort8021 == "9021")
                {
                    ShowMessage("警告：检测到非共机部署、门禁配置端口为9021，或导致设备无法通讯！需手动校验！如果该问题无法自行处理请捷服务帮助");
                }

                //有可能有多个IP
                string sql = string.Format("select MAC from control_devices where IP in ({0}) and devicetype = 42", GetIP());
                MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sql);
                if (reader.HasRows)
                    reader.Read();
                else
                    return false;
                sqlMac =  reader["MAC"].ToString();

                //未配置MAC地址或者MAC地址配置错误
                if (sqlMac != ConfigMAC || IsConfigMac == "false")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            try
            {
                IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
                string ret = "";
                foreach (IPAddress item in IpEntry.AddressList)
                {
                    //AddressFamily.InterNetwork  ipv4
                    //AddressFamily.InterNetworkV6 ipv6
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if(ret == "")
                            ret += "'" + item.ToString() + "'";
                        else
                            ret += ",'" + item.ToString() + "'";
                    }
                }
                return ret;
            }
            catch { return "''"; }
        }

        /// <summary>
        /// 修复门禁服务MAC地址配置
        /// </summary>
        /// <returns></returns>
        public bool FixDoorServerMac()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(configpath);

                config.AppSettings.Settings["IsConfigMac"].Value = "true";
                if (sqlMac.IsNullOrEmpty())
                    return false;
                config.AppSettings.Settings["MAC"].Value = sqlMac;
                config.Save();

                var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
                process.Kill();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                return false;
            }
        }


        //写日志变量和函数
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(DoorServerOptViewModel));
        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (message.Length > 0)
                {
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }
        /// <summary>
        /// 延迟显示消息，显得在工作一样
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageDelay(string message)
        {
            System.Threading.Thread.Sleep(500);
            ShowMessage(message);
            System.Threading.Thread.Sleep(500);
        }

        /// <summary>
        /// 后台运行
        /// </summary>
        public void RepairDoorClickServer(object parameter)
        {
            try
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += RepairDoor;
                bgw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning(ex.ToString());
            }
        }
        private bool IsRunning = false;
        private void RepairDoor(object sender, DoWorkEventArgs e)
        {
            if (IsRunning)
                return;
            else
                IsRunning = true;
            if (_doorServerInfoList.Count == 0)
            {
                ShowMessage("未能找到任何门禁服务");
                return ;
            }

            ///检测总项目
            int CountAll = 0;
            ///通过总项目
            int CountCorrect = 0;

            ShowMessage("=====================");
            #region 检测卡不能自动下载问题
            CountAll++;
            ShowMessageDelay("开始检查sync_doornum......");
            if (CheckSyncDoorNum())
            {
                ShowMessage("检测到sync_doornum数值错误，会导致卡数据无法自动下载！已自动修复！");
            }
            else
            {
                CountCorrect++;
                ShowMessage("sync_doornum无异常 √");
            }
            #endregion

            #region 检测门禁服务配置文件中MAC地址
            CountAll++;
            ShowMessageDelay("开始检查门禁服务配置.......");
            if (CheckDoorServerMac())
            {
                string str = string.Empty;
                if (IsConfigMac == "true")
                {
                    str = string.Format("检测到门禁服务配置MAC地址错误，CONFIG文件中MAC地址配置为{0}，数据库中门禁服务MAC地址为{1}，是否自动修复？", ConfigMAC, sqlMac);
                }
                else
                {
                    str = string.Format("检测到门禁服务配置文件未启用MAC地址配置，可自动配置MAC地址为{0}，是否自动配置？", sqlMac);
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    if (MessageBoxHelper.MessageBoxShowQuestion(str) == MessageBoxResult.Yes)
                    {
                        if (FixDoorServerMac())
                            MessageBoxHelper.MessageBoxShowSuccess("配置门禁服务MAC地址完成！已结束门禁进程等待进程自动重启！");
                        else
                            MessageBoxHelper.MessageBoxShowError("配置门禁服务MAC地址失败！");
                    }
                }));
            }
            else
            {
                CountCorrect++;
                ShowMessage("门禁服务无异常 √");
            }
            #endregion

            #region 检测下载HTTP保存参数
            CountAll++;
            ShowMessageDelay("开始检查HTTP保存参数......");
            switch (CheckHttpConfig())
            {
                case enumHTTPConfig.ALLRIGHT: ShowMessage("HTTP保存参数无异常 √"); CountCorrect++; break;
                case enumHTTPConfig.ERROR: ShowMessage("检查过程中报错，请联系研发");break;
                case enumHTTPConfig.RepairAndRestart: 
                    ShowMessage("HTTP保存参数不一致，已修改为" + AfterRepair_ServerURL + " " + AfterRepair_DownloadServerUrl);
                    ShowMessage("请确认是否正确" );
                    ShowMessage("正在重启门禁服务" );
                    break;
                case enumHTTPConfig.RepairButNoRestart:
                    ShowMessage("HTTP保存参数不一致，已修改为" + AfterRepair_ServerURL + " " + AfterRepair_DownloadServerUrl);
                    ShowMessage("请确认是否正确");
                    ShowMessage("未发现门禁服务"); 
                    break;
            }
            #endregion

            #region 检测未知设备问题
            CountAll++;
            ShowMessageDelay("开始检查未知设备问题......");
            switch(CheckUnkownDevice())
            {
                case enumHTTPConfig.ALLRIGHT: ShowMessage("没有发现未知设备 √"); CountCorrect++; break;
                case enumHTTPConfig.ERROR: ShowMessage("检查过程中报错，请联系研发");break;
                case enumHTTPConfig.RepairAndRestart:
                case enumHTTPConfig.RepairButNoRestart:
                    ShowMessage("检查到未知设备问题，共" + FindSqlError + "个");
                    ShowMessage("该问题需要手动重启IIS服务中的SmartWeb等，请手动重启后再次搜索设备！");
                    break;
            }
            #endregion

            #region 检测操作失败，请重试问题
            CountAll++;
            ShowMessageDelay("开始检查完全下载操作失败，请重试问题......");
            switch (CheckFailProblem())
            {
                case enumHTTPConfig.ALLRIGHT: ShowMessage("没有发现操作失败情况 √"); CountCorrect++; break;
                case enumHTTPConfig.ERROR: ShowMessage("检查过程中报错，请联系研发"); break;
                case enumHTTPConfig.RepairButNoRestart:
                    ShowMessage("已修复完全下载操作失败，请重试问题，请重新完全下载！");
                    break;
            }
            #endregion
            ShowMessageDelay("检查结束，检查项:" + CountAll +"个,通过项:"+ CountCorrect + "个");
            ShowMessage("=====================");
            IsRunning = false;
        }
        public enum enumHTTPConfig
        {
            ERROR = -1,
            ALLRIGHT = 0,
            RepairAndRestart = 1,
            RepairButNoRestart = 2,
            NoFileOrDire = 3,
        }
        public string AfterRepair_ServerURL = string.Empty;
        public string AfterRepair_DownloadServerUrl = string.Empty;
        /// <summary>
        /// 检查下载的HTTP参数 
        /// 有时候jielink保存的HTTP参数无法正确保存到两个comHttpParam.xml，修改后重启门禁服务
        /// </summary>
        /// <returns></returns>
        public enumHTTPConfig CheckHttpConfig()
        {
            try
            {
                //查询JIELINK上显示的文件服务器地址
                string sqlstr = "select serverURL,DownloadServerUrl from control_http_param where type = 1 and status = 0;";
                MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                if (reader.HasRows)
                    reader.Read();
                else
                    return enumHTTPConfig.ERROR;
                //数据库内的文件服务器地址 例http://10.101.98.117:9011
                string DB_ServerURL = reader["serverURL"].ToString();
                string DB_DownloadServerUrl = reader["DownloadServerUrl"].ToString();
                bool changeflag = false;
                AfterRepair_ServerURL = DB_ServerURL;
                AfterRepair_DownloadServerUrl = DB_DownloadServerUrl;
                LogHelper.CommLogger.Info("CheckHttpConfig():DB_ServerURL:" + DB_ServerURL);
                LogHelper.CommLogger.Info("CheckHttpConfig():DB_DownloadServerUrl:" + DB_DownloadServerUrl);

                //查询programdata内的文件服务器地址 例http://10.101.98.117:9011
                if(!File.Exists(@"C:\ProgramData\JieLinkDoor\para\comHttpParam.xml"))
                {
                    ShowMessage(@"提示：未找到C:\ProgramData\JieLinkDoor\para\comHttpParam.xml");
                }
                else
                {
                    XElement xe = XElement.Load(@"C:\ProgramData\JieLinkDoor\para\comHttpParam.xml");
                    string PD_ServerURL = (from ele in xe.Elements("uploadServerUrl") select ele).First().Value;
                    string PD_DownloadServerUrl = (from ele in xe.Elements("downloadServerUrl") select ele).First().Value;
                    LogHelper.CommLogger.Info("CheckHttpConfig():PD_ServerURL:" + PD_ServerURL);
                    LogHelper.CommLogger.Info("CheckHttpConfig():PD_DownloadServerUrl:" + PD_DownloadServerUrl);
                    //如果数据对不上以数据库为准修改
                    if (DB_ServerURL != PD_ServerURL || DB_DownloadServerUrl != PD_DownloadServerUrl)
                    {
                        //修改programdata里的para 
                        (from ele in xe.Elements("uploadServerUrl") select ele).First().ReplaceNodes("", DB_ServerURL);
                        (from ele in xe.Elements("downloadServerUrl") select ele).First().ReplaceNodes("", DB_DownloadServerUrl);
                        LogHelper.CommLogger.Info(@"修改C:\ProgramData\JieLinkDoor\para\comHttpParam.xml文件");
                        xe.Save(@"C:\ProgramData\JieLinkDoor\para\comHttpParam.xml");
                        changeflag = true;
                    }
                }

                //修改门禁盒子路径下的comHttpParam.xml
                var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
                if (process != null)
                {
                    var ParaPath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.FullName, @"para\comHttpParam.xml");
                    XElement xe_smartdoor = XElement.Load(ParaPath);
                    string P_ServerURL = (from ele in xe_smartdoor.Elements("uploadServerUrl") select ele).First().Value;
                    string P_DownloadServerUrl = (from ele in xe_smartdoor.Elements("downloadServerUrl") select ele).First().Value;
                    LogHelper.CommLogger.Info("CheckHttpConfig():P_ServerURL:" + P_ServerURL);
                    LogHelper.CommLogger.Info("CheckHttpConfig():P_DownloadServerUrl:" + P_DownloadServerUrl); 
                    if (DB_ServerURL != P_ServerURL || DB_DownloadServerUrl != P_DownloadServerUrl)
                    {
                        (from ele in xe_smartdoor.Elements("uploadServerUrl") select ele).First().ReplaceNodes("", DB_ServerURL);
                        (from ele in xe_smartdoor.Elements("downloadServerUrl") select ele).First().ReplaceNodes("", DB_DownloadServerUrl);
                        LogHelper.CommLogger.Info("修改" + ParaPath + "文件");
                        xe_smartdoor.Save(ParaPath);
                        changeflag = true;
                    }
                }
                else
                {
                    ShowMessage(@"提示：未找到门禁服务进程");
                }

                if (changeflag)
                {
                    if (process != null)
                    {
                        process.Kill();
                        return enumHTTPConfig.RepairAndRestart;
                    }
                    else
                        return enumHTTPConfig.RepairButNoRestart;
                }
                else
                {
                    return enumHTTPConfig.ALLRIGHT;
                }

            }
            catch(Exception ex)
            {
                ShowMessage(ex.ToString());
                return enumHTTPConfig.ERROR;
            }
        }
        /// <summary>
        /// 错误计数
        /// </summary>
        private int FindSqlError = 0;
        /// <summary>
        /// 部分dic_detail等表的数据偶发莫名其妙没有插入，手动打脚本无报错也没有插入的问题。程序再执行总能插入了吧
        /// </summary>
        /// <returns></returns>
        public enumHTTPConfig CheckUnkownDevice()
        {
            try
            {
                FindSqlError = 0;
                //项目版本号检查 270版本才加入Y10
                string SQL_ProjectVersion = "select valuetext from sys_key_value_setting where keyid = 'ProjectVersion'";
                MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, SQL_ProjectVersion);
                if (reader.HasRows)
                    reader.Read();
                else
                    return enumHTTPConfig.ERROR;
                int version = Convert.ToInt32( reader["valuetext"].ToString().Replace(".","").Replace("V","").Substring(0,3));
                LogHelper.CommLogger.Info("CheckUnkownDevice:版本号:" + version);
                if (version >= 270)
                {
                    #region Y10A修复
                    string sqlstr = "select * from dic_detail where DicDetailId=108 and DicTypeId=1000;";
                    MySqlDataReader readertemp = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                    if(!readertemp.HasRows)
                    {
                        FindSqlError++;
                        string temp = @"INSERT INTO `dic_detail` (`DicTypeId`, `DicDetailId`, `DicDetailName`, `DicEnDetailName`, `NisspCode`, `NisspName`, `Seq`, `Valid`) VALUES (1000, 108, 'Y10A门禁', 'JSMJY10A', '0203', 'Y10A门禁', 108, 1);";
                        LogHelper.CommLogger.Info("SQL语句执行:" + temp);
                        int ret = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString,temp);
                        if(ret >= 0)
                        {
                            ShowMessage("修复成功一句");
                        }
                        else
                        {
                            ShowMessage("修复失败一句");
                        }
                    }

                    sqlstr = "select * from control_device_param where devicetype=108;";
                    readertemp = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                    if (!readertemp.HasRows)
                    {
                        FindSqlError++;
                        string temp = @"INSERT INTO control_device_param(deviceid,devicetype,deviceparam,remark,lastdatetime) VALUE(NULL, 108, '{""deviceId"":0,""name"":"""",""alarmType"":7,""alarmTime"":30,""ewmTime"":1440,""autoWeekSet"":{""day0"":false,""day1"":false,""day2"":false,""day3"":false,""day4"":false,""day5"":false,""day6"":false},""autoTime"":""00:00"",""ownAuth"":1,""doorCfg"":[{""doorId"":0,""doorName"":"""",""doorType"":0,""doorProperty"":1,""doorOverTime"":5,""doorDelayTime"":3,""publicPasswd"":111111,""forcePasswd"":222222,""buzzerAlarmMute"":1,""doorsMagnetFlag"":1,""escapeDoorFlag"":1,""masterIp"":"""",""picServerIp"":"""",""manyCardCount"":1,""manyCardGroup"":[],""doorTimerNo"":[{""mode"":57,""periods"":{""end"":""23:59"",""start"":""00:00""},""weeks"":{""day0"":true,""day1"":true,""day2"":true,""day3"":true,""day4"":true,""day5"":true,""day6"":true}}],""holidayMonth"":[],""holiday"":[],""antiDoorFlag"":0,""allowCallRoom"":1}],""ntpServer"":"""",""speakServer"":"""",""speakId"":"""",""speakToId"":"""",""projectKey"": 0,""antiPryFlag"":1,""faceCaptureFlag"":0}', 'Y10A门禁控制器的默认参数', NOW());";
                        LogHelper.CommLogger.Info("SQL语句执行:" + temp);
                        int ret = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, temp);
                        if (ret >= 0)
                        {
                            ShowMessage("修复成功一句");
                        }
                        else
                        {
                            ShowMessage("修复失败一句");
                        }
                    }
                    #endregion
                }
                if (version >= 281)
                {
                    #region Y10A_T修复
                    string sqlstr = "select * from dic_detail where DicDetailId=118 and DicTypeId=1000;";
                    MySqlDataReader readertemp = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                    if (!readertemp.HasRows)
                    {
                        FindSqlError++;
                        string temp = $"INSERT INTO `dic_detail` (`DicTypeId`, `DicDetailId`, `DicDetailName`, `DicEnDetailName`, `NisspCode`, `NisspName`, `Seq`, `Valid`) VALUES (1000, 118, 'JSMJY10A-T', 'JSMJY10A-T', '0203', '可视对讲门禁星眸第三代标准版', 118, 1);";
                        LogHelper.CommLogger.Info("SQL语句执行:" + temp);
                        int ret = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, temp);
                        if (ret >= 0)
                        {
                            ShowMessage("修复成功一句");
                        }
                        else
                        {
                            ShowMessage("修复失败一句");
                        }
                    }

                    sqlstr = "select * from control_device_param where devicetype=118;";
                    readertemp = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                    if (!readertemp.HasRows)
                    {
                        FindSqlError++;
                        string temp = @"INSERT INTO control_device_param(deviceid,devicetype,deviceparam,remark,lastdatetime) VALUE(NULL,118, '{""deviceId"":0,""name"":"""",""alarmType"":7,""alarmTime"":30,""ewmTime"":1440,""autoWeekSet"":{""day0"":false,""day1"":false,""day2"":false,""day3"":false,""day4"":false,""day5"":false,""day6"":false},""autoTime"":""00:00"",""ownAuth"":1,""doorCfg"":[{""doorId"":0,""doorName"":"""",""doorType"":0,""doorProperty"":1,""doorOverTime"":5,""doorDelayTime"":3,""publicPasswd"":111111,""forcePasswd"":222222,""buzzerAlarmMute"":1,""doorsMagnetFlag"":1,""escapeDoorFlag"":1,""masterIp"":"""",""picServerIp"":"""",""manyCardCount"":1,""manyCardGroup"":[],""doorTimerNo"":[{""mode"":57,""periods"":{""end"":""23:59"",""start"":""00:00""},""weeks"":{""day0"":true,""day1"":true,""day2"":true,""day3"":true,""day4"":true,""day5"":true,""day6"":true}}],""holidayMonth"":[],""holiday"":[],""antiDoorFlag"":0,""allowCallRoom"":1}],""ntpServer"":"""",""speakServer"":"""",""speakId"":"""",""speakToId"":"""",""projectKey"": 0,""antiPryFlag"":1,""faceCaptureFlag"":0}', 'Y10A_T门禁控制器的默认参数', NOW());";
                        LogHelper.CommLogger.Info("SQL语句执行:" + temp);
                        int ret = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, temp);
                        if (ret >= 0)
                        {
                            ShowMessage("修复成功一句");
                        }
                        else
                        {
                            ShowMessage("修复失败一句");
                        }
                    }
                    #endregion

                    #region JSEY08A-old修复
                    sqlstr = "select * from dic_detail where DicDetailId=252 and DicTypeId=1000;";
                    readertemp = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                    if (!readertemp.HasRows)
                    {
                        FindSqlError++;
                        string temp = $"INSERT INTO `dic_detail` (`DicTypeId`, `DicDetailId`, `DicDetailName`, `DicEnDetailName`, `NisspCode`, `NisspName`, `Seq`, `Valid`) VALUES (1000, 252, 'JSMJY08A', 'JSMJY08A', '0203', 'JSMJY08A-Old', 252, 1);";
                        LogHelper.CommLogger.Info("SQL语句执行:" + temp);
                        int ret = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, temp);
                        if (ret >= 0)
                        {
                            ShowMessage("修复成功一句");
                        }
                        else
                        {
                            ShowMessage("修复失败一句");
                        }
                    }

                    sqlstr = "select * from control_device_param where devicetype=252;";
                    readertemp = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sqlstr);
                    if (!readertemp.HasRows)
                    {
                        FindSqlError++;
                        string temp = @"INSERT INTO control_device_param(deviceid,devicetype,deviceparam,remark,lastdatetime) VALUE(NULL,118, '{""deviceId"":0,""name"":"""",""alarmType"":7,""alarmTime"":30,""ewmTime"":1440,""autoWeekSet"":{""day0"":false,""day1"":false,""day2"":false,""day3"":false,""day4"":false,""day5"":false,""day6"":false},""autoTime"":""00:00"",""ownAuth"":1,""doorCfg"":[{""doorId"":0,""doorName"":"""",""doorType"":0,""doorProperty"":1,""doorOverTime"":5,""doorDelayTime"":3,""publicPasswd"":111111,""forcePasswd"":222222,""buzzerAlarmMute"":1,""doorsMagnetFlag"":1,""escapeDoorFlag"":1,""masterIp"":"""",""picServerIp"":"""",""manyCardCount"":1,""manyCardGroup"":[],""doorTimerNo"":[{""mode"":57,""periods"":{""end"":""23:59"",""start"":""00:00""},""weeks"":{""day0"":true,""day1"":true,""day2"":true,""day3"":true,""day4"":true,""day5"":true,""day6"":true}}],""holidayMonth"":[],""holiday"":[],""antiDoorFlag"":0,""allowCallRoom"":1}],""ntpServer"":"""",""speakServer"":"""",""speakId"":"""",""speakToId"":"""",""projectKey"": 0,""antiPryFlag"":1,""faceCaptureFlag"":0}', 'Y10A_T门禁控制器的默认参数', NOW());";
                        LogHelper.CommLogger.Info("SQL语句执行:" + temp);
                        int ret = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, temp);
                        if (ret >= 0)
                        {
                            ShowMessage("修复成功一句");
                        }
                        else
                        {
                            ShowMessage("修复失败一句");
                        }
                    }
                    #endregion
                }

                if(FindSqlError == 0)
                    return enumHTTPConfig.ALLRIGHT;
                else
                {
                    var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer").FirstOrDefault();
                    if(process == null)
                    {
                        return enumHTTPConfig.RepairButNoRestart;
                    }
                    else
                    {
                        return enumHTTPConfig.RepairAndRestart;
                    }
                }
                //return enumHTTPConfig.ERROR;
            }catch(Exception ex)
            {
                ShowMessage(ex.ToString());
                return enumHTTPConfig.ERROR;
            }
        }

        /// <summary>
        /// smartweb下偶发upload文件夹不存在导致完全下载的文件无法写入的问题
        /// </summary>
        /// <returns></returns>
        private enumHTTPConfig CheckFailProblem()
        {
            try
            {
                var process = Process.GetProcessesByName("SmartCenter.Host").FirstOrDefault();
                if (process == null)
                {
                    ShowMessage("未找到中心进程,该项不检查");
                    return enumHTTPConfig.ALLRIGHT;
                }
                else
                {
                    var ParaPath = Path.Combine(new FileInfo(process.MainModule.FileName).Directory.Parent.FullName, @"SmartWeb\Upload");
                    if (Directory.Exists(ParaPath))
                    {
                        return enumHTTPConfig.ALLRIGHT;
                    }else
                    {
                        var ret = Directory.CreateDirectory(ParaPath);
                        if(ret != null)
                            return enumHTTPConfig.RepairButNoRestart;
                        else
                        {
                            ShowMessage("文件夹创建失败，拉满权限或手动检查");
                            return enumHTTPConfig.ERROR;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ShowMessage(ex.ToString());
                return enumHTTPConfig.ERROR;
            }
        }
    }
}