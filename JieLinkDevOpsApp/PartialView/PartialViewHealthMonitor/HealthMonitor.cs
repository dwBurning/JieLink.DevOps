using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor
{
    public class HealthMonitor
    {
        private bool isRunning;
        private bool isMonitorSystem = true;//是否监控系统的运行（内存、CPU）
        private string processName = "SmartCenter.Host";
        private Thread monitorThread;
        private DateTime nextWarningTime;
        public HealthMonitor()
        {

        }
        public HealthMonitor(string processName, bool isMonitorSystem)
        {

            this.processName = processName;
            this.isMonitorSystem = isMonitorSystem;
        }
        public void Start()
        {
            if (isRunning)
            {
                return;
            }
            nextWarningTime = DateTime.Now;
            isRunning = true;
            monitorThread = new Thread(HealthCheck);
            monitorThread.IsBackground = true;
            monitorThread.Name = "monitorThread";
            monitorThread.Start();
        }
        public void Stop()
        {
            isRunning = false;
            if (monitorThread != null && monitorThread.IsAlive)
                monitorThread.Join(1000);
            monitorThread = null;
        }
        private void HealthCheck()
        {
            try
            {
                SystemStatusMonitor systemMonitor = new SystemStatusMonitor();
                ProcessStatusMonitor processMonitor = new ProcessStatusMonitor(processName);
                int warningInterval = 6;//6小时
                while (isRunning)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        if (processMonitor.ProcessExists())
                        {
                            processMonitor.Refresh();
                            systemMonitor.Refresh();

                            var warning = systemMonitor.HasWarning();
                            var processWarning = processMonitor.HasWarning();
                            if (warning.WarningType == enumWarningType.None)
                                warning = processWarning;
                            if (warning.WarningType != enumWarningType.None && DateTime.Now > nextWarningTime)
                            {
                                nextWarningTime = DateTime.Now.AddHours(warningInterval);
                                //需要报警
                                SendEvent(warning);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        //try catch只是为了让它能继续执行下一次检测，不至于线程退出
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //PerformanceCounter有些电脑会不可用，用不了就算了吧
                Console.WriteLine(ex.ToString());
            }

        }
        private void SendEvent(WarningMessage warning)
        {
            Console.WriteLine("发送报警事件，{0}", warning.Message);
            DevOpsEvent opsEvent = new DevOpsEvent();
            opsEvent.EventType = (int)warning.WarningType;
            opsEvent.OperatorDate = DateTime.Now;
            opsEvent.RemoteAccount = "123456789";
            opsEvent.ContactPhone = string.Empty;
            opsEvent.ContactName = string.Empty;

            try
            {
                var returnData = HttpHelper.PostAsync<ReturnData>("http://localhost:8080", opsEvent).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
