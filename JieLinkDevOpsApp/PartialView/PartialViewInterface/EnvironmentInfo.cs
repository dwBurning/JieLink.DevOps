using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public static class EnvironmentInfo
    {
        public static string ProjectNo { get; set; }

        public static string RemoteAccount { get; set; }

        public static string RemotePassword { get; set; }

        public static string ContactName { get; set; }

        public static string ContactPhone { get; set; }

        public static string ServerUrl = GetValue("ServerUrl", "");

        public static string CurrentVersion { get; set; }

        public static string GetValue(string key, string value = "")
        {
            return ConfigurationManager.AppSettings[key] ?? value;
        }
    }
}
