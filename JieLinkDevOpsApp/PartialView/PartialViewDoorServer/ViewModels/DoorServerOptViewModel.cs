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

namespace PartialViewDoorServer.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class DoorServerOptViewModel : DependencyObject
    {
        public DoorServerOptViewModel()
        {
            SelectPathCommand = new DelegateCommand();
            SelectPathCommand.ExecuteAction = SelectPath;

            GetDoorDeviceCommand = new DelegateCommand();
            GetDoorDeviceCommand.ExecuteAction = SelectDoorDevice;

            //GetDoorServerInfo();
        }

        /// <summary>
        /// 路径
        /// </summary>
        public DelegateCommand SelectPathCommand { get; set; }

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

            // 2.8.1以后，自动取这个备份文件夹的文件
            string strParaDataDirDest = string.Format(@"{0}\JieLinkDoor\para", System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            File.Copy(k02DevicePath, strParaDataDirDest + @"\DeviceInfos.xml", true);
            File.Copy(jsipDevicePath, strParaDataDirDest + @"\CriterionDeviceInfos.xml", true);

            Notice.Show("获取设备成功....", "通知", 3, MessageBoxIcon.Success);

            Notice.Show("直接结束门禁服务进程，重启门禁服务!", "通知", 3, MessageBoxIcon.Success);
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
                Console.WriteLine(ex.Message);
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
                if(_doorServerInfoList.Count == 0)
                        return false;
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
                    if (GetNum > maxid + 200 || GetNum > maxid * 2)
                    {
                        sqlstr = string.Format("update sync_doornum set GetNum = {0} where GetNum > {0}",maxid);
                        MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sqlstr);
                        return true;
                    }
                }
                
                return false;
            }
            catch(Exception ex)
            {
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
                return false;
            }
        }
    }
}
