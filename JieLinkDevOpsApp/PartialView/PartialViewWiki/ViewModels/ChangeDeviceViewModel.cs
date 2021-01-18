using PartialViewWiki.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewWiki.ViewModels
{
    public class ChangeDeviceViewModel : DependencyObject
    {

        public ChangeDeviceViewModel()
        {
            Devices = new List<Device>() {
                new Device(){
                    DeviceName = "入口",
                    DeviceId = "1",
                    Devices=new List<Device>()
                    {
                        new Device()
                        {
                            DeviceName = "出口",
                            DeviceId = "2"
                        },new Device()
                        {
                            DeviceId = "3",
                            DeviceName = "入口"

                        }
                    }
                }
            };
        }



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
