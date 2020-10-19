using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewSyncTool
{
    public class BoxConnConfig
    {
        public event Action<string> ShowMessage;
        /// <summary>
        /// 连接字符串配置
        /// </summary>
        public DbConfigEntity dbConfig = new DbConfigEntity();

        /// <summary>
        /// 获取所有盒子的连接信息
        /// </summary>
        Dictionary<string, string> dictBoxConnStr = new Dictionary<string, string>();

        public Dictionary<string, string> GetBoxConnString()
        {
            ShowMessage?.Invoke("正在检测盒子的数据库连接，请等待！");
            string sql = "SELECT IP from control_devices where DeviceType = 25";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string ip = dr["IP"].ToString();
                    if (dictBoxConnStr.ContainsKey(ip))
                    {
                        continue;
                    }

                    string boxConn = ReadBoxConfig(ip);

                    //读取到盒子配置文件，加载参数
                    if (string.IsNullOrEmpty(boxConn))
                    {
                        ShowBoxConfigWindow(ip);
                        continue;
                    }

                    try
                    {
                        string cmd = "select * from sys_boxinformation";
                        MySqlHelper.ExecuteDataset(boxConn, cmd);
                        ShowMessage?.Invoke($"盒子{ip}连接成功");
                        //存储盒子连接字符串
                        SaveBoxDbConfig(boxConn);
                        dictBoxConnStr.Add(ip, boxConn);
                    }
                    catch (Exception)
                    {
                        ShowBoxConfigWindow(ip);
                    }
                }
                //全部保存盒子字符串后保存到文件
                SaveBoxDbConfigFile(dbConfig);
            }

            return dictBoxConnStr;
        }

        private void ShowBoxConfigWindow(string ip)
        {
            (Application.Current.MainWindow as WindowX).IsMaskVisible = true;
            DbConfig dbConfig = new DbConfig(ip);
            if (dbConfig.ShowDialog() == true)
            {
                ShowMessage?.Invoke($"盒子{ip}连接成功");
                //存储盒子连接字符串
                SaveBoxDbConfig(dbConfig.DbConnString);
                dictBoxConnStr.Add(ip, dbConfig.DbConnString);
            }
                                (Application.Current.MainWindow as WindowX).IsMaskVisible = false;
        }

        private void SaveBoxDbConfigFile(DbConfigEntity dbconfig)
        {
            System.IO.File.WriteAllText("DbBoxConfig.ini", Newtonsoft.Json.JsonConvert.SerializeObject(dbconfig.BoxDbConnStrs), Encoding.UTF8);
        }

        private void SaveBoxDbConfig(string conbox)
        {
            //string boxConn = $"Data Source={ip};port=10080;User ID=test;Password=123456;Initial Catalog=smartbox;";

            string[] strsplit = conbox.Split(new char[2] { '=', ';' });
            DbConnEntity boxcon = new DbConnEntity();
            boxcon.Ip = strsplit[1];
            boxcon.Port = Convert.ToInt32(strsplit[3]);
            boxcon.UserName = strsplit[5];
            boxcon.Password = strsplit[7];
            boxcon.DbName = strsplit[9];
            dbConfig.BoxDbConnStrs.Add(boxcon);
        }

        private string ReadBoxConfig(string ip)
        {
            if (File.Exists("DbBoxConfig.ini"))
            {
                string ReadStr = File.ReadAllText(@"DbBoxConfig.ini");
                dbConfig.BoxDbConnStrs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DbConnEntity>>(ReadStr);
                if (dbConfig.BoxDbConnStrs == null)
                {
                    File.Delete("DbBoxConfig.ini");
                    dbConfig.BoxDbConnStrs = new List<DbConnEntity>();
                    return "";
                }

                DbConnEntity find = dbConfig.BoxDbConnStrs.Find(a => a.Ip == ip);
                if (find != null)
                {
                    return $"Data Source={find.Ip};port={find.Port};User ID={find.UserName};Password={find.Password};Initial Catalog={find.DbName};";
                }
            }

            return "";
        }
    }
}
