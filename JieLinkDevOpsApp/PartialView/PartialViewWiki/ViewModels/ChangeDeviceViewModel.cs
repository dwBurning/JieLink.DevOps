using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
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

        public bool CanExecute { get; set; }
        public DelegateCommand ChangeDeviceCommand { get; set; }

        public ChangeDeviceViewModel()
        {
            ChangeDeviceCommand = new DelegateCommand();
            ChangeDeviceCommand.ExecuteAction = ChangeDevice;
            ChangeDeviceCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return CanExecute; });

            Devices = new List<Device>() {
                new Device(){
                    DeviceName = "入口",
                    DeviceId = "1",
                    Devices = new List<Device>()
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
                },
                new Device(){
                    DeviceName = "入口",
                    DeviceId = "1",
                    Devices = new List<Device>()
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

        public void SelectedChanged()
        {
            if (SelectDevice == null)
            {
                CanExecute = false;
            }
            else
            {
                CanExecute = true;
            }
        }

        public void ChangeDevice(object parameter)
        {
            MessageBoxHelper.MessageBoxShowInfo(parameter.ToString());
        }



        public Device SelectDevice
        {
            get { return (Device)GetValue(SelectDeviceIdProperty); }
            set { SetValue(SelectDeviceIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectDeviceId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectDeviceIdProperty =
            DependencyProperty.Register("SelectDeviceId", typeof(Device), typeof(ChangeDeviceViewModel));




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
