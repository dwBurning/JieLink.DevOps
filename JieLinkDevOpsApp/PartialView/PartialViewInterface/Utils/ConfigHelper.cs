using PartialViewInterface.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class ConfigHelper
    {
        public static T GetValue<T>(string key, T defaultVal)
        {
            object val = ConfigurationManager.AppSettings[key];
            if (val == null)
            {
                return defaultVal;
            }
            val = Convert.ChangeType(val, typeof(T));
            if (val == null)
            {
                return defaultVal;
            }
            return (T)val;
        }

        public static void WriterAppConfig(string key, string value)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JieShun.JieLink.DevOps.App.exe");
            Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void AddAppConfig(string key, string value)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JieShun.JieLink.DevOps.App.exe");
            Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        public static string ReadAppConfig(string key, string value = "")
        {
            return ConfigurationManager.AppSettings[key] ?? value;
        }
    }
}
