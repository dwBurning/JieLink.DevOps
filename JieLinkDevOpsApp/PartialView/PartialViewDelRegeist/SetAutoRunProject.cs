using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Panuon.UI.Silver;

namespace PartialViewDelRegeist
{
    class SetAutoRunProject
    {

        // Token: 0x04000049 RID: 73
        private string _installPath = "";

        // Token: 0x0400004C RID: 76
        private static SetAutoRunProject _instance;

        private SetAutoRunProject()
        {
            this._installPath = this.DelRegeist("DelRegeist");
        }

        public static SetAutoRunProject Instance()
        {
        //    bool flag = SetAutoRunProject._instance == null;
        //    if (flag)
        //    {
                SetAutoRunProject._instance = new SetAutoRunProject();
        //    }
            return SetAutoRunProject._instance;
        }
        public void DoWork(int action)
        {
            //bool flag = string.IsNullOrEmpty(this._installPath);

        }


        private string DelRegeist(string key)
        {
            try
            {
                if (MessageBoxResult.Cancel == MessageBoxX.Show("该操作会直接删除盒子注册表，只有在安装盒子时提示已安装相同或更高版本时，进行该操作。是否继续？", "警告", null, MessageBoxButton.OKCancel))
                    return "";

                //32位和64位配置
                var useRegistryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, useRegistryView);

                //删除注册表
                if (localMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\SmartBox", true) != null)
                {
                    localMachine.DeleteSubKeyTree("SOFTWARE\\Wow6432Node\\SmartBox", false);
                    MessageBoxX.Show("删除盒子注册表完成HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\SmartBox", "提示");
                }
                else
                {
                    MessageBoxX.Show("未找到盒子注册表路径HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\SmartBox", "提示");
                }

                if (localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Smartbox", true) != null)
                {
                    localMachine.DeleteSubKeyTree("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Smartbox", false);
                    MessageBoxX.Show("删除盒子注册表完成HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Smartbox", "提示");
                }
                else
                {
                    MessageBoxX.Show("未找到盒子注册表路径HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Smartbox", "提示");
                }

                localMachine.Close();

                
                return "";
            }
            catch (Exception ex)
            {
                MessageBoxX.Show(ex.ToString(), "");
                return "";
            }

        }
    }
}
