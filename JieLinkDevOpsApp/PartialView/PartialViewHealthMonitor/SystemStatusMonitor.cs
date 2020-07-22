using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using static Vanara.PInvoke.Kernel32;

namespace PartialViewHealthMonitor
{
    public class SystemStatusMonitor
    {
        private List<SystemStatus> lastStatusList = new List<SystemStatus>();
        private PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private int memoryThreshold = 90;//95%
        private int cpuThreshold = 80;//80%
        public SystemStatusMonitor()
        {
            memoryThreshold = ConfigHelper.GetValue<int>("MemoryThreshold", memoryThreshold);
            cpuThreshold = ConfigHelper.GetValue<int>("CpuThreshold", cpuThreshold);
        }
        public SystemStatus Refresh()
        {
            
            SystemStatus systemStatus = new SystemStatus();
            systemStatus.CpuUsage = (int)cpuCounter.NextValue();
            //暂时注释系统内存的使用，换种方式，带了太多依赖的dll来了
            //MEMORYSTATUSEX memStat = new MEMORYSTATUSEX();
            //memStat.dwLength = (uint)Marshal.SizeOf(memStat);
            //if (GlobalMemoryStatusEx(ref memStat))
            //{
            //    //Console.WriteLine("总共内存{0}M，已用内存{1}M", memStat.ullTotalPhys / (1024 * 1024), (memStat.ullTotalPhys - memStat.ullAvailPhys) / (1024 * 1024));
            //    systemStatus.MemoryTotal = (int)(memStat.ullTotalPhys / (1024 * 1024));
            //    systemStatus.MemoryUsed = (int)((memStat.ullTotalPhys - memStat.ullAvailPhys) / (1024 * 1024));
            //    systemStatus.MemoryUsage = (int)((systemStatus.MemoryUsed * 100.0f / systemStatus.MemoryTotal));
            //}
            Console.WriteLine("总共内存{0}M，已用内存{1}M,CPU:{2}%", systemStatus.MemoryTotal, systemStatus.MemoryUsed, systemStatus.CpuUsage);
            //把状态缓存起来
            lastStatusList.RemoveAll(x => (DateTime.Now - x.CreateTime).TotalMinutes > 1);
            lastStatusList.Add(systemStatus);
            return systemStatus;
        }
        public WarningMessage HasWarning()
        {
            if (lastStatusList.Count < 30
                || (DateTime.Now - lastStatusList.First().CreateTime).TotalSeconds < 30)
            {
                //至少监控了30s以上才有点可信度，短时间的采样是不准的
                return WarningMessage.None;
            }
            //cpu使用率
            int warningCount = (int)(lastStatusList.Count * 0.8);//80%的时间内，cpu的使用率都超过了cpuThreshold
            if (lastStatusList.Where(x => x.CpuUsage > cpuThreshold).Count() > warningCount)
            {
                return new WarningMessage(enumWarningType.CPU, "电脑 cpu使用率超过" + cpuThreshold);
            }
            //内存使用
            if (lastStatusList.Where(x => x.MemoryUsage > memoryThreshold).Count() > warningCount)
            {
                return new WarningMessage(enumWarningType.Memory, "电脑 内存使用率超过" + memoryThreshold);
            }

            return WarningMessage.None;
        }
    }
}
