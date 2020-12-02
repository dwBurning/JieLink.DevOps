using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor
{
    public class ProcessStatusMonitor
    {
        private PerformanceCounter memoryCount = null;
        private PerformanceCounter cpuCounter = null;
        private PerformanceCounter threadCounter = null;
        private string processName;
        private List<ProcessStatus> lastStatusList = new List<ProcessStatus>();
        private int memoryThreshold = 1024;//1024M
        private int cpuThreshold = 80;//80%
        private int threadCounterThreshold = 700;

        public ProcessStatusMonitor(string processName)
        {
            this.processName = processName;
            memoryCount = new PerformanceCounter("Process", "Working Set - Private", processName);
            cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);
            threadCounter = new PerformanceCounter("Process", "Thread Count", processName);
            memoryThreshold = ConfigHelper.GetValue<int>("ProcessMemoryThreshold", memoryThreshold);
            cpuThreshold = ConfigHelper.GetValue<int>("ProcessCpuThreshold", cpuThreshold);
            threadCounterThreshold = ConfigHelper.GetValue<int>("threadCounterThreshold", threadCounterThreshold);
        }
        public bool ProcessExists()
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }
        public ProcessStatus Refresh()
        {
            ProcessStatus processStatus = new ProcessStatus();
            processStatus.ProcessName = processName;
            processStatus.MemoryUsed = (int)(memoryCount.NextValue() / (1024 * 1024));
            processStatus.CpuUsage = (int)cpuCounter.NextValue();
            processStatus.ThreadCount = (int)threadCounter.NextValue();
            //把状态缓存起来
            lastStatusList.RemoveAll(x => (DateTime.Now - x.CreateTime).TotalMinutes > 1);
            lastStatusList.Add(processStatus);
            Console.WriteLine("{0} 占用内存：{1}，CPU：{2}%，线程：{3}", processName, processStatus.MemoryUsed, processStatus.CpuUsage, processStatus.ThreadCount);
            return processStatus;
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
                return new WarningMessage(enumWarningType.CPU, processName + " cpu使用率超过" + cpuThreshold);
            }
            //内存使用
            if (lastStatusList.Where(x => x.MemoryUsed > memoryThreshold).Count() > warningCount)
            {
                return new WarningMessage(enumWarningType.Memory, processName + " 内存超过" + memoryThreshold);
            }

            if (lastStatusList.Where(x => x.ThreadCount > threadCounterThreshold).Count() > warningCount)
            {
                return new WarningMessage(enumWarningType.Thread, processName + " 线程超过" + threadCounterThreshold);
            }

            return WarningMessage.None;
        }
    }
}
