using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.Models
{
    public class TableCharacter
    {
        string field = "";
        string type = "";
        public string Field
        {
            get { return field; }
            set { field = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
