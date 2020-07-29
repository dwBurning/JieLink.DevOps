using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JieShun.JieLink.DevOps.Updater.Utils
{
    public class ProcessHelper
    {
        public static bool IsServiceRunning(string serviceName)
        {
            ServiceController serviceController = ServiceController.GetServices().FirstOrDefault((ServiceController x) => x.ServiceName == serviceName);
            return serviceController != null && (serviceController.Status == ServiceControllerStatus.Running || serviceController.Status == ServiceControllerStatus.StartPending);
        }


        public static bool StartService(string serviceName)
        {
            ServiceController serviceController = ServiceController.GetServices().FirstOrDefault((ServiceController x) => x.ServiceName == serviceName);
            if (serviceController == null)
            {
                return false;
            }
            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(20000.0));
            Thread.Sleep(100);
            return serviceController.Status == ServiceControllerStatus.Running;
        }


        public static void StopService(string serviceName)
        {
            ServiceController serviceController = ServiceController.GetServices().FirstOrDefault((ServiceController x) => x.ServiceName == serviceName);
            if (serviceController == null)
            {
                return;
            }
            if (serviceController.Status == ServiceControllerStatus.Running || serviceController.Status == ServiceControllerStatus.StartPending)
            {
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(10000.0));
            }
        }


        public static bool IsProcessRunning(string name)
        {
            if (name.EndsWith(".exe"))
            {
                name = name.Substring(0, name.Length - 4);
            }
            return Process.GetProcessesByName(name).Length != 0;
        }


        public static bool StartProcess(string executePath, string cmd = "")
        {
            try
            {
                if (Win32API.CreateProcessV1(executePath, cmd))
                    return true;
                return Win32API.CreateProcessV2(executePath, cmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartProcess:"+ex.Message);
                return false;
            }
        }


        public static void StopProcess(string name)
        {
            if (name.EndsWith(".exe"))
            {
                name = name.Substring(0, name.Length - 4);
            }
            Process[] processesByName = Process.GetProcessesByName(name);
            for (int i = 0; i < processesByName.Length; i++)
            {
                processesByName[i].Kill();
                Thread.Sleep(10);
            }
        }


        public static void RestartComputer()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine("shutdown /f /r /t 0");
        }
    }
}
