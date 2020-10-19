using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDoorServer.ViewModels
{
    /// <summary>
    /// 设备
    /// </summary>
	[Serializable]
	public class DeviceInfo
	{
		public DeviceInfo()
		{
		}

		public uint ID { get; set; }

		public uint ParentDeviceID { get; set; }

		public string Name { get; set; }

		public string Type { get; set; }

		public string IP
		{
			get
			{
				return this._ip;
			}
			set
			{
				this._ip = value;
			}
		}

		public string Mac { get; set; }

		public string[] Mac2 { get; set; }

		public string Mask { get; set; }

		public string GateWay { get; set; }

		public uint SN { get; set; }

		public string Company { get; set; }

		public string Model { get; set; }

		public string HardwareVersion { get; set; }

		public string SoftwareVersion { get; set; }

		public string Manufacturer { get; set; }

		public string ProductDate { get; set; }

		public ushort DeviceClass { get; set; }

		public string Summary { get; set; }

		public string NamePlateDna { get; set; }

		public string CPUID
		{
			get
			{
				return this._cpuid;
			}
			set
			{
				this._cpuid = value;
			}
		}

		public int RegisterState { get; set; }

		public bool HasInit { get; set; }

		public int RegisterID { get; set; }

		public bool IsHttpSended { get; set; }

		public int AuthorizeFlag { get; set; }

		private string _ip = "";

		private string _cpuid = "";

		public DateTime SendHttpTime;

		public DateTime DeviceAddTime;

		public int status;

		public int state;
	}
}
