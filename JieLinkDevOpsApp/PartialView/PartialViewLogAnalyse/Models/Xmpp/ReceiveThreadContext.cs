using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models
{
    public class ReceiveThreadContext
    {
        public string ThreadNum;
        public string Name;
        public DateTime StartTime;
        public DateTime EndTime;
        public List<ReceiveData> LastLines=new List<ReceiveData>();
        
    }
}
