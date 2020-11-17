using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.Models
{
    public class TableCharacter
    {
        public string Field { get; set; }

        public string Type { get; set; }

        public bool IsNull { get; set; }

        public bool IsKey { get; set; }

        public string Default { get; set; }

        public string Extra { get; set; }
    }
}
