using PartialViewInterface.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.DB
{
    public class KeyValueSettingManager
    {
        public void WriteSetting(KeyValueSetting keyValueSetting)
        {
            KeyValueSetting setting = ReadSetting(keyValueSetting.KeyId);
            if (setting == null)//插入
            {
                string error = "";
                EnvironmentInfo.SqliteHelper.UpdateData(out error, $"insert into sys_key_value_setting(KeyType, KeyID, KeyName, ValueText, Remark)values('{keyValueSetting.KeyType}','{keyValueSetting.KeyId}','{keyValueSetting.KeyName}','{keyValueSetting.ValueText}','{keyValueSetting.Remark}');");

                EnvironmentInfo.Settings.Add(keyValueSetting);
            }
            else
            {
                string error = "";
                EnvironmentInfo.SqliteHelper.UpdateData(out error, $"update sys_key_value_setting set ValueText='{keyValueSetting.ValueText}' where KeyID='{keyValueSetting.KeyId}';");
                setting.ValueText = keyValueSetting.ValueText;
            }


        }

        public KeyValueSetting ReadSetting(string key, string defaultValue = "")
        {
            var setting = EnvironmentInfo.Settings.Find(x => x.KeyId == key);
            if (setting == null)
            {
                return new KeyValueSetting() { KeyId = key, ValueText = defaultValue, KeyType = -1 };
            }
            return setting;
        }

        public List<KeyValueSetting> KeyValueSettings()
        {

            string error = "";
            DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, "select * from sys_key_value_setting");
            List<KeyValueSetting> settings = new List<KeyValueSetting>();
            foreach (DataRow dr in dataTable.Rows)
            {
                KeyValueSetting setting = new KeyValueSetting();
                setting.KeyType = int.Parse(dr["KeyType"].ToString());
                setting.KeyId = dr["KeyId"].ToString();
                setting.KeyName = dr["KeyName"].ToString();
                setting.ValueText = dr["ValueText"].ToString();
                setting.Remark = dr["Remark"].ToString();
                settings.Add(setting);
            }

            return settings;
        }
    }
}
