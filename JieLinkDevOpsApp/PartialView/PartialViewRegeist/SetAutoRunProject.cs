using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SetWasgoneForSmartBox
{
    // Token: 0x0200000B RID: 11
    public class SetAutoRunProject
    {
        // Token: 0x14000007 RID: 7
        // (add) Token: 0x0600005E RID: 94 RVA: 0x000050E8 File Offset: 0x000032E8
        // (remove) Token: 0x0600005F RID: 95 RVA: 0x00005120 File Offset: 0x00003320
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //public event Action<string> ShowTextMessage;
        public void ShowTextMessage(string str)
        {
            MessageBox.Show(str, "提示");
        }

        // Token: 0x14000008 RID: 8
        // (add) Token: 0x06000060 RID: 96 RVA: 0x00005158 File Offset: 0x00003358
        // (remove) Token: 0x06000061 RID: 97 RVA: 0x00005190 File Offset: 0x00003390
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //public event Action<string> ShowPopuMessage;
        public void ShowPopuMessage(string str)
        {
            MessageBox.Show(str, "提示");
        }

        // Token: 0x06000062 RID: 98 RVA: 0x000051C5 File Offset: 0x000033C5
        private SetAutoRunProject()
        {
            this._installPath = this.GetInstallPath("InstallLocation");
        }

        // Token: 0x06000063 RID: 99 RVA: 0x000051EC File Offset: 0x000033EC
        public static SetAutoRunProject Instance()
        {
            bool flag = SetAutoRunProject._instance == null;
            if (flag)
            {
                SetAutoRunProject._instance = new SetAutoRunProject();
            }
            return SetAutoRunProject._instance;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x0000521C File Offset: 0x0000341C
        public void SetRunItem(string value)
        {
            try
            {
                RegistryKey localMachine = Registry.LocalMachine;
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
                //bool flag3 = this.ShowPopuMessage != null;
                //if (flag3)
                //{
                    this.ShowPopuMessage(ex.Message);
                //}
            }
        }

        // Token: 0x06000065 RID: 101 RVA: 0x000052CC File Offset: 0x000034CC
        private string GetInstallPath(string key)
        {
            try
            {
                RegistryKey localMachine = Registry.LocalMachine;
                RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\SmartBox\\");
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
                MessageBox.Show(ex.Message, "错误");

                //bool flag2 = this.ShowPopuMessage != null;
                //if (flag2)
                //{
                //    this.ShowPopuMessage(ex.Message);
                //}
            }
            return "";
        }

        // Token: 0x06000066 RID: 102 RVA: 0x00005360 File Offset: 0x00003560
        public void DoWork(int action)
        {
            bool flag = string.IsNullOrEmpty(this._installPath);
            if (flag)
            {
                //bool flag2 = this.ShowPopuMessage != null;
                //if (flag2)
                //{
                    this.ShowPopuMessage("获取盒子安装路径失败！");
                //}
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
                    //bool flag3 = this.ShowTextMessage != null;
                    //if (flag3)
                    //{
                        this.ShowTextMessage("获取到的盒子安装路径为：" + this._installPath);
                    //}
                    string text = Path.Combine(this._installPath, "SmartBox.Infrastructures.View.Main.exe");
                    bool flag4 = File.Exists(text);
                    if (flag4)
                    {
                        this.SetRunItem(text);
                    }
                }
                this.ShowPopuMessage("设置完成！");
            }
        }

        // Token: 0x04000049 RID: 73
        private string _installPath = "";

        // Token: 0x0400004C RID: 76
        private static SetAutoRunProject _instance;
    }
}
