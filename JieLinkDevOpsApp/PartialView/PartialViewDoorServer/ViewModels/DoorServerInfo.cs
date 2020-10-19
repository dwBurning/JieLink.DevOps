using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDoorServer.ViewModels
{
    public class DoorServerInfo
    {
        public UInt32 deviceID = 0;

        public string deviceName = "";

        public string devIp = "";

        public override string ToString()
        {
            return deviceName;
        }
    }
}
