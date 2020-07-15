using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace SetWasgoneForSmartBox
{
    // Token: 0x02000003 RID: 3
    public class RegisterHelper
    {
        // Token: 0x06000007 RID: 7 RVA: 0x00002184 File Offset: 0x00000384
        public static bool CheckInstallRegister(RegisterInfo info)
        {
            bool result;
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node");
                string[] subKeyNames = registryKey.GetSubKeyNames();
                int i = 0;
                while (i < subKeyNames.Length)
                {
                    string text = subKeyNames[i];
                    bool flag = text == info.RegKey;
                    if (flag)
                    {
                        RegistryKey registryKey2 = registryKey.OpenSubKey(text);
                        object value = registryKey2.GetValue("path");
                        bool flag2 = value != null;
                        if (flag2)
                        {
                            info.Path = value.ToString();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        i++;
                    }
                }
                registryKey.Close();
                result = false;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002234 File Offset: 0x00000434
        public static void GetInstallInfo(RegisterInfo regInfo)
        {
            regInfo.Path = "";
            regInfo.InstallLocation = "";
            regInfo.LstDeskTopLnkPath = new List<string>();
            regInfo.StartMenuLnkPath = "";
            regInfo.ExecuteFileName = "";
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node");
            foreach (string text in registryKey.GetSubKeyNames())
            {
                bool flag = text == regInfo.RegKey;
                if (flag)
                {
                    RegistryKey registryKey2 = registryKey.OpenSubKey(text);
                    List<string> list = new List<string>();
                    foreach (string text2 in registryKey2.GetValueNames())
                    {
                        bool flag2 = text2.Contains("lnk");
                        if (flag2)
                        {
                            list.Add(registryKey2.GetValue(text2).ToString());
                        }
                    }
                    regInfo.LstDeskTopLnkPath = list;
                    object value = registryKey2.GetValue("path");
                    registryKey2.GetValue("InstallPath");
                    registryKey2.GetValue("DeskTopLnkPath");
                    object value2 = registryKey2.GetValue("StartMenuLnkPath");
                    object value3 = registryKey2.GetValue("ExecuteFileName");
                    object value4 = registryKey2.GetValue("DisplayVersion");
                    bool flag3 = value != null;
                    if (flag3)
                    {
                        regInfo.Path = value.ToString();
                        regInfo.InstallLocation = regInfo.Path;
                        regInfo.StartMenuLnkPath = ((value2 == null) ? "" : value2.ToString());
                        regInfo.ExecuteFileName = ((value3 == null) ? "" : value3.ToString());
                        regInfo.DisplayVersion = ((value4 == null) ? "" : value4.ToString());
                    }
                    registryKey2.Close();
                    break;
                }
            }
            registryKey.Close();
        }
    }
}
