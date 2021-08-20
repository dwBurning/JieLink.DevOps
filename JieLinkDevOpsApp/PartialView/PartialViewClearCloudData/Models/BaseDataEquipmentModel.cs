using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewClearCloudData.Models
{
    public class BaseDataEquipmentModel
    {
        public string itemId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string typeClass { get; set; }
        public string parentId { get; set; }
        public string type { get; set; }
        public string ipAddress { get; set; }
        public int port { get; set; }
        public bool supportBluetooth { get; set; }
        public string bluetoothMac { get; set; }
        public bool supportVideo { get; set; }
        public bool supportSpeak { get; set; }
        public int maxLinkNum { get; set; }
        public string voipId { get; set; }
        public bool supportVisitor { get; set; }
        public int commonFloor { get; set; }
        public int doorCount { get; set; }
        public int doorNo { get; set; }
        public int equipOnlyCode { get; set; }
        public string parkId { get; set; }
        public string passgateId { get; set; }
        public bool isDeleted { get; set; }
        public string sn { get; set; }
        public string mac { get; set; }
        public int ioType { get; set; }
        public string timestamp { get; set; }
        public string productModel { get; set; }
    }
}
