﻿using Microsoft.Win32;
using PartialViewInterface;
using PartialViewInterface.DB;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace JieShun.JieLink.DevOps.App
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        //用文件监控来简单实现进程通信
        FileSystemWatcher watcher;
        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            EnvironmentInfo.CurrentVersion = GetCurrentVersion();

            if (!File.Exists("FileSystemWatcher.txt"))
            { File.Create("FileSystemWatcher.txt"); }

            bool ret;
            watcher = new FileSystemWatcher();
            watcher.Path = AppDomain.CurrentDomain.BaseDirectory;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "FileSystemWatcher.txt";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            mutex = new System.Threading.Mutex(true, "JieShun.JieLink.DevOps.App", out ret);
            if (!ret)
            {
                //当进程已经存在的时候 写文件 通知已经存在的进程将主窗口最大化
                File.WriteAllText("FileSystemWatcher.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                Environment.Exit(0);
            }

            try
            {
                SetSelfStarting(true, "JieShun.JieLink.DevOps.App.exe");
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }

            OnStart();
        }

        void OnStart()
        {
            KeyValueSettingManager keyValueSettingManager = new KeyValueSettingManager();
            BackUpJobConfigManger backUpJobConfigManger = new BackUpJobConfigManger();
            EnvironmentInfo.Settings.AddRange(keyValueSettingManager.KeyValueSettings());
            EnvironmentInfo.BackUpJobConfigs.AddRange(backUpJobConfigManger.BackUpJobConfigs());

            //迁移到数据库中
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JieShun.JieLink.DevOps.App.exe.config");
            var setting = keyValueSettingManager.ReadSetting("IsTransfered");
            if (setting != null && setting.ValueText == "0")
            {
                XDocument myXDoc = XDocument.Load(filePath);
                XElement rootNode = myXDoc.Element("configuration");
                foreach (XElement node in rootNode.Elements("appSettings"))
                {
                    foreach (XElement element in node.Elements("add"))
                    {
                        keyValueSettingManager.WriteSetting(new KeyValueSetting()
                        {
                            KeyId = element.Attribute("key").Value,
                            ValueText = element.Attribute("value").Value
                        });
                    }
                }

                EnvironmentInfo.Settings.Clear();
                EnvironmentInfo.Settings.AddRange(keyValueSettingManager.KeyValueSettings());

                if (keyValueSettingManager.ReadSetting("ServerUrl").ValueText != "http://www.dwburning.top:1688")
                {
                    keyValueSettingManager.WriteSetting(new KeyValueSetting()
                    {
                        KeyId = "ServerUrl",
                        ValueText = "http://www.dwburning.top:1688"//强制搞一下
                    });
                }

                backUpJobConfigManger.WriteBackUpJobConfig(new BackUpJobConfig()
                {
                    Id = Guid.NewGuid().ToString(),
                    DataBaseName = EnvironmentInfo.DbConnEntity.DbName,
                    Cron = "00 00 03 ? * 4",
                    BackUpType = 0
                });

                backUpJobConfigManger.WriteBackUpJobConfig(new BackUpJobConfig()
                {
                    Id = Guid.NewGuid().ToString(),
                    DataBaseName = EnvironmentInfo.DbConnEntity.DbName,
                    Cron = "00 00 03 ? * 2,6",
                    BackUpType = 1
                });

                keyValueSettingManager.WriteSetting(new KeyValueSetting()
                {
                    KeyId = "IsTransfered",
                    ValueText = "1"
                });
            }

            EnvironmentInfo.ServerUrl = keyValueSettingManager.ReadSetting("ServerUrl").ValueText;

            EnvironmentInfo.DbConnEntity = JsonHelper.DeserializeObject<DbConnEntity>(keyValueSettingManager.ReadSetting("ConnectionString").ValueText);

            EnvironmentInfo.AutoStartCorectEntity = JsonHelper.DeserializeObject<AutoStartCorectEntity>(keyValueSettingManager.ReadSetting("AutoStartCorectString").ValueText);

            EnvironmentInfo.AutoStartSyncEntity = JsonHelper.DeserializeObject<AutoStartSyncEntity>(keyValueSettingManager.ReadSetting("AutoStartSyncString").ValueText);

            EnvironmentInfo.IsAutoArchive = keyValueSettingManager.ReadSetting("AutoStartSyncString").ValueText == "1";

            EnvironmentInfo.AutoArchiveMonth = int.Parse(keyValueSettingManager.ReadSetting("AutoArchiveMonth").ValueText);

            EnvironmentInfo.IsJieLink3x = keyValueSettingManager.ReadSetting("IsJieLink3x").ValueText == "1";
        }

        public static void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                //解决跨线程访问的问题
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Window window = Application.Current.MainWindow;
                    window.Visibility = Visibility.Visible;
                    window.ShowInTaskbar = true;
                    window.Activate();
                }));
            }
            catch (Exception ex)
            {
            }
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
                WriteLog(e.ExceptionObject);
            else
                WriteLog(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is Exception)
                WriteLog(e.Exception);
            else
                WriteLog(e);
        }


        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="started">设置开机启动，或取消开机启动</param>
        /// <param name="exeName">注册表中的名称</param>
        /// <returns>开启或停用是否成功</returns>
        public bool SetSelfStarting(bool started, string exeName)
        {
            RegistryKey key = null;
            try
            {

                string exeDir = System.Windows.Forms.Application.ExecutablePath;
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项

                if (key == null)//如果该项不存在的话，则创建该子项
                {
                    key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (started)
                {
                    try
                    {
                        object ob = key.GetValue(exeName, -1);

                        if (!ob.ToString().Equals(exeDir))
                        {
                            if (!ob.ToString().Equals("-1"))
                            {
                                key.DeleteValue(exeName);//取消开机启动
                            }
                            key.SetValue(exeName, exeDir);//设置为开机启动
                        }
                        key.Close();

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        key.DeleteValue(exeName);//取消开机启动
                        key.Close();
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        private void WriteLog(object exception)
        {
            LogHelper.CommLogger.Info(exception.ToString());
        }

        private string GetCurrentVersion()
        {
            //查当前目录下的_v文件，和Jielink一样
            VersionInfo vi = JsonHelper.GetFromFile<VersionInfo>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.json"));
            if (vi == null)
            {
                return "V1.0.0";
            }
            return vi.Version;
        }
    }
}