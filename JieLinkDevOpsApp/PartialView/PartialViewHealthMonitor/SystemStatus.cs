using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor
{
    public class SystemStatus
    {
        private int memoryTotal;
        private int memoryUsed;
        private int memoryUsage;
        private int cpuUsage;
        private DateTime createTime = DateTime.Now;
        public int MemoryTotal
        {
            get { return memoryTotal; }
            set { memoryTotal = value; }
        }
        public int MemoryUsed
        {
            get { return memoryUsed; }
            set { memoryUsed = value; }
        }
        public int MemoryUsage
        {
            get { return memoryUsage; }
            set { memoryUsage = value; }
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
