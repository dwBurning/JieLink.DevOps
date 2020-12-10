using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    public enum enumWarningType
    {
        None = 0,
        Memory = 1,
        CPU = 2,
        Thread = 3,
        Disk = 4,

    }
    public class WarningMessage
    {
        public WarningMessage()
        {
            CreateTime = DateTime.Now;
        }
        public WarningMessage(enumWarningType warningType, string message) : this()
        {
            WarningType = warningType;
            Message = message;

        }

        public enumWarningType WarningType { get; set; }
        public string Message { get; set; }
        public DateTime CreateTime { get; set; }
        public static WarningMessage None
        {
            get
            {
                return new WarningMessage() { WarningType = enumWarningType.None };
            }
        }
    }
}
