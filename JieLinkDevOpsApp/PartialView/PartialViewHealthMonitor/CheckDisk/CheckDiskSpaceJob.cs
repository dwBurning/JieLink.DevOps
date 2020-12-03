using PartialViewHealthMonitor.Models;
using PartialViewInterface;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace PartialViewHealthMonitor.CheckDisk
{
    public class CheckDiskSpaceJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //获取SmartCenter所在的目录
            var process = Process.GetProcessesByName("SmartCenter.Host").FirstOrDefault();
            if (process != null)
            {
                string fileName = process.MainModule.FileName;
                string diskName = fileName.Substring(0, fileName.IndexOf(":")+1);
                ulong space = Win32API.GetDiskFreeSpaceEx(diskName);
                if(space<5*1024)
                {
                    WarningMessage warningMessage = new WarningMessage();
                    warningMessage.Message = "磁盘空间不足";
                    warningMessage.WarningType = enumWarningType.Disk;
                    DevOpsAPI.SendEvent(warningMessage);
                }
            }
            
            
            

        }
    }
}
