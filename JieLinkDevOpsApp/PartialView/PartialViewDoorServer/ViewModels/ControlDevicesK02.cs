using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDoorServer.ViewModels.K02
{
    public class DeviceInfo
	{
		public DeviceInfo()
		{
			this.status = 0;
		}

		public ushort type;

		public uint id;

		public string ip;

		public string model;

		public int status;

		public string productDate;

		public DateTime statusUpdateTime;

		public DateTime DeviceAddTime;
	}
}
