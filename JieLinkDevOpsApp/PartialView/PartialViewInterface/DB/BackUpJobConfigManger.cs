using PartialViewInterface;
using PartialViewInterface.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.DB
{
    public class BackUpJobConfigManger
    {
        public List<BackUpJobConfig> BackUpJobConfigs()
        {
            string error = "";
            DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, "select * from sys_backup_job;");
            List<BackUpJobConfig> configs = new List<BackUpJobConfig>();
            foreach (DataRow dr in dataTable.Rows)
            {
                BackUpJobConfig config = new BackUpJobConfig();
                config.Id = dr["Id"].ToString();
                config.DataBaseName = dr["DataBaseName"].ToString();
                config.Cron = dr["Cron"].ToString();
                config.BackUpType = int.Parse(dr["BackUpType"].ToString());
                configs.Add(config);
            }

            return configs;
        }

        public void WriteBackUpJobConfig(BackUpJobConfig backUpJobConfig)
        {
            BackUpJobConfig config = ReadBackUpJobConfig(backUpJobConfig.Id);
            if (config == null)
            {
                string error = "";
                EnvironmentInfo.SqliteHelper.UpdateData(out error, $"insert into sys_backup_job values('{backUpJobConfig.Id}','{backUpJobConfig.DataBaseName}','{backUpJobConfig.Cron}',{backUpJobConfig.BackUpType});");

                EnvironmentInfo.BackUpJobConfigs.Add(backUpJobConfig);
            }
            else
            {
                string error = "";
                EnvironmentInfo.SqliteHelper.UpdateData(out error, $"update sys_backup_job set DataBaseName='{backUpJobConfig.DataBaseName}',Cron='{backUpJobConfig.Cron}',BackUpType={backUpJobConfig.BackUpType} where Id='{backUpJobConfig.Id}';");
                config.DataBaseName = backUpJobConfig.DataBaseName;
                config.Cron = backUpJobConfig.Cron;
                config.BackUpType = backUpJobConfig.BackUpType;
            }

        }

        public void RemoveBackUpJobConfig(string id)
        {
            BackUpJobConfig backUpJobConfig = ReadBackUpJobConfig(id);
            if (backUpJobConfig != null)
            {
                string error = "";
                EnvironmentInfo.SqliteHelper.UpdateData(out error, $"delete from sys_backup_job where Id='{id}';");
                EnvironmentInfo.BackUpJobConfigs.Remove(backUpJobConfig);
            }
        }

        public BackUpJobConfig ReadBackUpJobConfig(string id)
        {
            return EnvironmentInfo.BackUpJobConfigs.Find(x => x.Id == id);
        }
    }
}
