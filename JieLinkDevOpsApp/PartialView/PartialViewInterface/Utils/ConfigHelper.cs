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

        /// <summary>
        /// 获取job实际的cron表达式
        /// 说明：
        /// 1、如果单个配置允许配置多个值，需要以“|”隔开
        /// 2、数据库备份的cron表达式中，末尾加了数据库名称，所以需要特殊处理，获取实际的cron表达式和数据库名
        /// </summary>
        /// <param name="backUpType">配置文件中原始的表达式</param>
        /// <returns></returns>
        public static List<JobCronConfig> GetJobCronConfig(string backUpType)
        {
            string originalCron = GetValue(backUpType, "0 0 0 * * ?");
            var ret = new List<JobCronConfig>();
            if (string.IsNullOrEmpty(originalCron))
                return ret;

            var crons = originalCron.Split('|');
            foreach (var cron in crons)
            {
                var jobCronConfig = new JobCronConfig() { JobTypeName = backUpType };
                var items = cron.Split(' ');
                if (items.Count() >= 7) //如果cron表达式为7位，即为数据库备份策略的cron表达式，最后一位为数据库名
                {
                    jobCronConfig.Cron = string.Join(" ", items.Skip(0).Take(6).ToArray());
                    jobCronConfig.DatabaseName = items[6];
                }
                else
                {
                    jobCronConfig.Cron = cron;
                }
                ret.Add(jobCronConfig);
            }

            return ret;
        }
    }
}
