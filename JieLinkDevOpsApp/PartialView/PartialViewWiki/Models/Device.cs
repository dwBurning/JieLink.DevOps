using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewWiki.Models
{
    public class Device : PropertyChangedBase
    {
        private string _deviceName;

        [DisplayName("设备名称")]
        [ReadOnlyColumn]
        [ColumnWidth("*")]
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; NotifyPropertyChanged(); }
        }

        private string _deviceId;

        [DisplayName("设备ID")]
        [ReadOnlyColumn]
        [ColumnWidth("*")]
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; NotifyPropertyChanged(); }
        }

        private List<Device> _devices;

        [DisplayName("设备列表")]
        [ReadOnlyColumn]
        [ColumnWidth("*")]
        public List<Device> Devices
        {
            get { return _devices; }
            set { _devices = value; NotifyPropertyChanged(); }
        }


        public Device SelectedDevice { get; set; }
    }
}
