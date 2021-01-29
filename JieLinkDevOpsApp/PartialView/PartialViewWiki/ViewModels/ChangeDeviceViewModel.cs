using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using PartialViewWiki.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewWiki.ViewModels
{
    public class ChangeDeviceViewModel : DependencyObject
    {
        public DelegateCommand ChangeDeviceCommand { get; set; }

        public ChangeDeviceViewModel()
        {
            ChangeDeviceCommand = new DelegateCommand();
            ChangeDeviceCommand.ExecuteAction = ChangeDevice;

            Devices = GetNotExistDevice();
        }

        public void ChangeDevice(object parameter)
        {
            Device device = this.Devices.Find(x => x.DeviceId == parameter.ToString());
            if (device.SelectedDevice == null)
            {
                Notice.Show("请选择你要更换的设备！", "通知", 3, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBoxHelper.MessageBoxShowQuestion($"确定要将[{parameter.ToString()}]更改成[{device.SelectedDevice.DeviceId}]吗？") == MessageBoxResult.Yes)
            {
                int result = UpdateEnterDeviceId(parameter.ToString(), device.SelectedDevice.DeviceId);
                if (result > 0)
                {
                    Notice.Show($"更新场内记录{result}条", "通知", 3, MessageBoxIcon.Success);
                }
            }
        }


        private List<Device> GetNotExistDevice()
        {
            // 查询场内记录中 当前设备列表不存在的设备ID，和设备名称
            string sql = @"select DISTINCT EnterDeviceID,EnterDeviceName from box_enter_record where WasGone=0 and EnterDeviceID not in (
select DeviceID from control_devices where DeviceType in(select DicDetailId from dic_detail where DicTypeId = 1000 and NisspCode = '0207')); ";

            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            List<Device> devices = new List<Device>();
            List<Device> allDevices = GetAllDevice();
            foreach (DataRow dr in dt.Rows)
            {
                Device device = new Device();
                device.DeviceName = dr["EnterDeviceName"].ToString();
                device.DeviceId = dr["EnterDeviceID"].ToString();
                device.Devices = allDevices;
                devices.Add(device);
            }
            return devices;
        }

        private List<Device> GetAllDevice()
        {
            string sql = "select DeviceID,DeviceName from control_devices where DeviceType in(select DicDetailId from dic_detail where DicTypeId = 1000 and NisspCode = '0207');";

            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            List<Device> devices = new List<Device>();
            foreach (DataRow dr in dt.Rows)
            {
                Device device = new Device();
                device.DeviceName = dr["DeviceName"].ToString();
                device.DeviceId = dr["DeviceID"].ToString();
                devices.Add(device);
            }

            return devices;
        }

        private int UpdateEnterDeviceId(string oDeviceId, string nDeviceId)
        {
            string sql = $"update box_enter_record set EnterDeviceID='{nDeviceId}',Remark='运维工具更新{oDeviceId}->{nDeviceId}' where EnterDeviceID='{oDeviceId}' and WasGone=0;";
            return MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
        }


        public Device SelectChangeDevice
        {
            get { return (Device)GetValue(SelectChangeDeviceProperty); }
            set { SetValue(SelectChangeDeviceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectChangeDevice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectChangeDeviceProperty =
            DependencyProperty.Register("SelectChangeDevice", typeof(Device), typeof(ChangeDeviceViewModel));


        public Device SelectDevice
        {
            get { return (Device)GetValue(SelectDeviceProperty); }
            set { SetValue(SelectDeviceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectDevice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectDeviceProperty =
            DependencyProperty.Register("SelectDevice", typeof(Device), typeof(ChangeDeviceViewModel));



        public List<Device> Devices
        {
            get { return (List<Device>)GetValue(DevicesProperty); }
            set { SetValue(DevicesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Devices.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DevicesProperty =
            DependencyProperty.Register("Devices", typeof(List<Device>), typeof(ChangeDeviceViewModel));


    }
}
