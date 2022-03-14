using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public class Global
    {
        public static void ValidV2(Action<string, bool> callback)
        {
            if (EnvironmentInfo.IsJieLink3x)
            {
                callback?.Invoke("当前配置为jielink3.x，该功能无法使用", false);
                return;
            }

            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
                callback?.Invoke("", true);
            }
            catch (Exception)
            {
                callback?.Invoke("未查询到jielink2.x的数据库信息，请确认数据库配置信息是否正确？", false);
            }
        }


        public static void ValidV3(Action<string, bool> callback)
        {
            if (!EnvironmentInfo.IsJieLink3x)
            {
                callback?.Invoke("当前配置为jielink2.x，该功能无法使用", false);
                return;
            }

            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sc_operator limit 1");
                callback?.Invoke("", true);
            }
            catch (Exception)
            {
                callback?.Invoke("未查询到jielink2.x的数据库信息，请确认数据库配置信息是否正确？", false);
            }
        }
    }
}
