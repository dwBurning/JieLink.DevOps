using MySql.Data.MySqlClient;
using PartialViewDataArchiving.Models;
using PartialViewDataArchiving.ViewModels;
using PartialViewInterface;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.DataArchive
{
    public class ExecuteDataArchive
    {
        /// <summary>
        /// 判断对应年份的归档表 是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool TableIsExists(string tableName)
        {
            string sql = $"select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='{EnvironmentInfo.DbConnEntity.DbName}' and TABLE_NAME='{tableName}'";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// 创建归档表
        /// </summary>
        /// <param name="tableName"></param>
        private void CreateTable(string bllTableName, string archiveTableName)
        {
            string sql = $"create table `{archiveTableName}` like `{bllTableName}`";
            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
        }

        /// <summary>
        /// 比较字段是否一致
        /// </summary>
        private void CompareColumns(string bllTableName, string archiveTableName)
        {
            List<TableCharacter> bllCharacter = GetTableCharacters(bllTableName);
            List<TableCharacter> archiveCharacter = GetTableCharacters(archiveTableName);

            List<TableCharacter> tableCharacters = bllCharacter.Where(x => !archiveCharacter.Exists(y => x.Field.Equals(y.Field))).ToList();

            foreach (TableCharacter tableCharacter in tableCharacters)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"ALTER TABLE `{archiveTableName}` Add COLUMN `{tableCharacter.Field}` {tableCharacter.Type}");
                if (!tableCharacter.IsNull)
                {
                    builder.Append(" NOT NULL");
                }

                builder.Append(" COLLATE utf8_unicode_ci");

                if (!string.IsNullOrEmpty(tableCharacter.Default))
                {
                    builder.Append($" DEFAULT '{tableCharacter.Default}'");
                }

                MySqlHelperEx.ExecuteNonQueryEx(EnvironmentInfo.ConnectionString, builder.ToString());
            }
        }

        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<TableCharacter> GetTableCharacters(string tableName)
        {
            string sql = $"desc `{tableName}`";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            List<TableCharacter> tableCharacters = new List<TableCharacter>();
            foreach (DataRow dr in dt.Rows)
            {
                TableCharacter tableCharacter = new TableCharacter();
                tableCharacter.Field = dr["Field"].ToString();
                tableCharacter.Type = dr["Type"].ToString();
                tableCharacter.IsNull = dr["Null"].ToString().ToUpper().Equals("YES");
                tableCharacter.IsKey = string.IsNullOrEmpty(dr["Key"].ToString());
                tableCharacter.Default = dr["Default"].ToString();
                tableCharacter.Extra = dr["Extra"].ToString();
                tableCharacters.Add(tableCharacter);
            }

            return tableCharacters;
        }

        object obj = new object();

        /// <summary>
        /// 开始归档
        /// </summary>
        private void DataArchive(string bllTableName, string archiveTableName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(EnvironmentInfo.ConnectionString))
                {
                    conn.Open();
                    MySqlTransaction transaction = conn.BeginTransaction();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandTimeout = 3600;//超时时间设置60分钟
                    cmd.Transaction = transaction;
                    try
                    {
                        DateTime archiveDate = DateTime.Now.Date.AddMonths(-EnvironmentInfo.AutoArchiveMonth);
                        string sql = $"insert into `{archiveTableName}` select * from `{bllTableName}` where {GetTimeField(bllTableName)} < '{archiveDate.ToString("yyyy-MM-dd HH:mm:ss")}'";
                        if (bllTableName == "box_enter_record")
                        {
                            sql += " and wasgone=1";
                        }
                        cmd.CommandText = sql;
                        LogHelper.CommLogger.Info(sql);
                        int x = cmd.ExecuteNonQuery();

                        string script = $"delete from `{bllTableName}` where {GetTimeField(bllTableName)} < '{archiveDate.ToString("yyyy-MM-dd HH:mm:ss")}'";
                        if (bllTableName == "box_enter_record")
                        {
                            script += " and wasgone=1";
                        }
                        cmd.CommandText = script;
                        LogHelper.CommLogger.Info(script);
                        int y = cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.CommLogger.Error("数据归档遇到些问题，事务回滚：" + ex.ToString());
                    }
                }
                progress += 20;
                DataArchivingViewModel.Instance().ShowMessage($"表{bllTableName}归档完成...请继续等待...", progress);

            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error("数据归档遇到些问题：" + ex.ToString());
            }
        }

        private string GetTimeField(string tableName)
        {
            switch (tableName.ToLower())
            {
                case "box_enter_record":
                    return "EnterTime";
                case "box_out_record":
                    return "OutTime";
                case "box_bill":
                    return "PayTime";
                case "business_discount":
                    return "CreateTime";
                case "boxdoor_door_record":
                    return "ActionTime";
            }

            return "";
        }

        private void BackUpTables()
        {
            DataArchivingViewModel.Instance().ShowMessage($"正在执行备份...请等待...");

            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string tables = "box_enter_record box_out_record box_bill boxdoor_door_record business_discount";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_archive.sql";
            string filePath = Path.Combine(EnvironmentInfo.TaskBackUpPath, fileName);
            if (!Directory.Exists(EnvironmentInfo.TaskBackUpPath))
            {
                Directory.CreateDirectory(EnvironmentInfo.TaskBackUpPath);
            }
            string mysqlcmd = $"mysqldump --default-character-set=utf8 --single-transaction -h{EnvironmentInfo.DbConnEntity.Ip} -u{EnvironmentInfo.DbConnEntity.UserName} -p{EnvironmentInfo.DbConnEntity.Password} -P{EnvironmentInfo.DbConnEntity.Port}  -B {EnvironmentInfo.DbConnEntity.DbName} --tables {tables} > \"{filePath}\"";
            List<string> cmds = new List<string>();
            cmds.Add(BaseDirectoryPath.Substring(0, 2));
            cmds.Add("cd " + BaseDirectoryPath);
            cmds.Add(mysqlcmd);
            ProcessHelper.ExecuteCommand(cmds);
            ZipHelper.ZipFile(filePath, filePath.Replace(".sql", ".zip"));
            File.Delete(filePath);
        }

        int progress = 0;

        public void Execute()
        {
            //BackUpTables();
            progress = 0;

            var tasks = new List<Task>();
            DataArchivingViewModel.Instance().ShowMessage($"正在执行归档...请等待...");
            foreach (var table in DataArchivingViewModel.Instance().Tables)
            {
                //Thread.Sleep(2000);
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    string archiveTable = $"{table}_{DateTime.Now.Year.ToString()}";
                    if (!TableIsExists(archiveTable))
                    {
                        CreateTable(table, archiveTable);
                    }

                    CompareColumns(table, archiveTable);
                    DataArchive(table, archiveTable);

                }));
            }
            Task.WaitAll(tasks.ToArray());
            DataArchivingViewModel.Instance().ShowMessage($"数据归档已全部完成", 100);
        }

        /// <summary>
        /// 只执行比对表结构的任务
        /// </summary>
        public void ExecuteEx()
        {
            foreach (var table in DataArchivingViewModel.Instance().Tables)
            {
                string archiveTable = $"{table}_{DateTime.Now.Year.ToString()}";
                if (!TableIsExists(archiveTable))
                {
                    CreateTable(table, archiveTable);
                }

                CompareColumns(table, archiveTable);
            }
        }
    }
}
