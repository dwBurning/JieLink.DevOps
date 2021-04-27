using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;

namespace PartialViewHealthMonitor
{
    class LogStatusMonitor
    {

        private string processName;
        private DateTime nextCheckTime;

        //检测间隔时间（小时）
        private int CheckInterval = 6;

        public LogStatusMonitor(string processName)
        {
            nextCheckTime = DateTime.Now;
            this.processName = processName;
        }

        public WarningMessage HasWarning()
        {
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
                if (process != null && DateTime.Now > nextCheckTime)
                {
                    string path = getLogPath(process);
                    if (!string.IsNullOrEmpty(path))
                    {
                        //开始读文件
                        using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("gb2312")))
                        {
                            nextCheckTime = DateTime.Now.AddHours(CheckInterval);
                            System.Diagnostics.Stopwatch watch = new Stopwatch();
                            watch.Start();
                            //暂定几个关键字 关键：Out of ， 一般：捕获异常 ，catch 
                            while (!sr.EndOfStream)
                            {

                                string line = sr.ReadLine();
                                if (line.Contains("OutOfMemory"))
                                {
                                    //TODO
                                    return new WarningMessage(enumWarningType.CenterLogOOM, "中心日志文件检测到内存溢出");
                                }
                            }
                            watch.Stop();
                            var mSeconds = watch.ElapsedMilliseconds;
                            //17M耗时约400ms
                            LogHelper.CommLogger.Info("中心文件日志排查耗时：" + mSeconds.ToString() + "ms");
                        }
                    }

                }
                return WarningMessage.None;
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 获取今天最新的日志文件路径
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private string getLogPath(System.Diagnostics.Process process)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(process.MainModule.FileName) + "\\Logs\\";

                string FormDate = DateTime.Now.ToString("yyyyMMdd");

                //低于一定版本的没有时间文件夹
                if (File.Exists(path + DateTime.Now.ToString("yyyy-MM-dd") + "_JieLink_CENTER.log"))
                {
                    return path + DateTime.Now.ToString("yyyy-MM-dd") + "_JieLink_CENTER.log";
                }

                string logpath = path + FormDate + "\\" + "JieLink_Center_Comm_" + FormDate + ".log";
                if (File.Exists(logpath))
                {
                    return logpath;
                }

                return string.Empty;
            }
            catch (Exception)
            {
                LogHelper.CommLogger.Error(ex.ToString());
                throw;
            }
        }
    }
}
