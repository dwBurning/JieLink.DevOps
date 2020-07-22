using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public class FileHelper
    {
        /// <summary>
        /// 获取所有的dll
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="extension">扩展名</param>
        /// <returns></returns>
        public static List<string> GetAllFiles(string path, string extension = "*.dll")
        {
            return Directory.GetFiles(path, extension).ToList();
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
