using Microsoft.Win32;
using Panuon.UI.Silver;
using PartialViewInterface.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SetWasgoneForSmartBox
{
    public class SetAutoRunProject
    {
        public void ShowTextMessage(string str)
        {
            MessageBox.Show(str, "提示");
        }


        private SetAutoRunProject()
        {
            this._installPath = this.GetInstallPath("InstallLocation");
        }

        public static SetAutoRunProject Instance()
        {
            bool flag = SetAutoRunProject._instance == null;
            if (flag)
            {
                SetAutoRunProject._instance = new SetAutoRunProject();
            }
            return SetAutoRunProject._instance;
        }


        public void SetRunItem(string value)
        {
            try
            {
                //RegistryKey localMachine = Registry.LocalMachine;
                //解决部分电脑看得见但是注册表访问不到的问题
                //参考地址 https://blog.csdn.net/xuhuo1234/article/details/101564085
                var useRegistryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, useRegistryView);

                RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true);
                bool flag = registryKey != null;
                if (flag)
                {
                    string text = registryKey.GetValue("Shell").ToString();
                    bool flag2 = !text.Equals(value);
                    if (flag2)
                    {
                        registryKey.SetValue("Shell", value);
                    }
                    registryKey.Flush();
                    localMachine.Close();
                    registryKey.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowInfo(ex.Message);
            }
        }


        private string GetInstallPath(string key)
        {
            try
            {
                //RegistryKey localMachine = Registry.LocalMachine;
                //解决部分电脑看得见但是注册表访问不到的问题
                //参考地址 https://blog.csdn.net/xuhuo1234/article/details/101564085

                var useRegistryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, useRegistryView);

                RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\SmartBox\\");
                //RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion");
                bool flag = registryKey != null;
                if (flag)
                {
                    string result = registryKey.GetValue(key).ToString();
                    localMachine.Close();
                    registryKey.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.MessageBoxShowInfo(ex.Message);
            }
            return "";
        }


        public void DoWork(int action)
        {
            bool flag = string.IsNullOrEmpty(this._installPath);
            if (flag)
            {
                MessageBoxHelper.MessageBoxShowInfo("获取盒子安装路径失败！");
            }
            else
            {
                if (action != 0)
                {
                    if (action == 1)
                    {
                        this.SetRunItem("explorer.exe");
                        Process.Start("explorer.exe");
                    }
                }
                else
                {
                    //this.ShowTextMessage("获取到的盒子安装路径为：" + this._installPath);
                    string text = Path.Combine(this._installPath, "SmartBox.Infrastructures.View.Main.exe");
                    bool flag4 = File.Exists(text);
                    if (flag4)
                    {
                        this.SetRunItem(text);
                    }
                }
                Notice.Show("设置完成！", "提示", 3, MessageBoxIcon.Info);
            }
        }

        private string _installPath = "";

        private static SetAutoRunProject _instance;
    }
}
