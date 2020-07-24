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
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(exePath);
            cfa.AppSettings.Settings[key].Value = value;
            cfa.Save();
        }

        public static string ReadAppConfig(string key, string value = "")
        {
            return ConfigurationManager.AppSettings[key] ?? value;
        }
    }
}
