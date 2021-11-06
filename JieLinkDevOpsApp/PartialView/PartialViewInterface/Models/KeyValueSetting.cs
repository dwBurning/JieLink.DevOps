using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Models
{
    public class KeyValueSetting
    {
        public int KeyType { get; set; }

        public string KeyId { get; set; }

        public string KeyName { get; set; }

        public string ValueText { get; set; }

        public string Remark { get; set; }
    }
}
