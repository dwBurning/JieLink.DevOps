using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor
{
    public class ProcessStatus
    {
        private string processName;
        private int memoryUsed;
        private int cpuUsage;
        private DateTime createTime = DateTime.Now;
        public string ProcessName
        {
            get { return processName; }
            set { processName = value; }
        }
        public int MemoryUsed
        {
            get { return memoryUsed; }
            set { memoryUsed = value; }
        }
        public int CpuUsage
        {
            get { return cpuUsage; }
            set { cpuUsage = value; }
        }
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
    }
}
