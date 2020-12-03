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

            var process = Process.GetProcessesByName("SmartBoxDoor.Infrastructures.Server.DoorServer.exe").FirstOrDefault();
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
    }
}
