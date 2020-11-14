using MySql.Data.MySqlClient;
using PartialViewDataArchiving.Models;
using PartialViewDataArchiving.ViewModels;
using PartialViewInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.DataArchive
{
    class ExecuteDataArchive
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
            string sql = $"create table {archiveTableName} like {bllTableName}";
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
                string sql = $"ALTER TABLE `{archiveTableName}` Add COLUMN `{tableCharacter.Field}` {tableCharacter.Type} COLLATE utf8_unicode_ci";
                MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
            }
        }

        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<TableCharacter> GetTableCharacters(string tableName)
        {
            string sql = $"desc {tableName}";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            List<TableCharacter> tableCharacters = new List<TableCharacter>();
            foreach (DataRow dr in dt.Rows)
            {
                TableCharacter tableCharacter = new TableCharacter();
                tableCharacter.Field = dr["Field"].ToString();
                tableCharacter.Type = dr["Type"].ToString();
                tableCharacters.Add(tableCharacter);
            }

            return tableCharacters;
        }

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
                    cmd.Transaction = transaction;
                    try
                    {
                        string sql = $"insert into {archiveTableName} select * from {bllTableName} where DATE_ADD({GetTimeField(bllTableName)},INTERVAL {EnvironmentInfo.AutoArchiveMonth} Month) < now()";
                        if (bllTableName == "box_enter_record")
                        {
                            sql += " and wasgone=1";
                        }
                        cmd.CommandText = sql;
                        int x = cmd.ExecuteNonQuery();

                        string script = $"delete from {bllTableName} where  DATE_ADD({GetTimeField(bllTableName)},INTERVAL {EnvironmentInfo.AutoArchiveMonth} Month) < now()";
                        if (bllTableName == "box_enter_record")
                        {
                            script += " and wasgone=1";
                        }
                        cmd.CommandText = script;
                        int y = cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        DataArchivingViewModel.Instance().ShowMessage("数据归档遇到些问题");
                    }

                }
            }
            catch (Exception ex)
            {
                DataArchivingViewModel.Instance().ShowMessage("数据归档遇到些问题");
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
                case "boxdoor_door_record":
                    return "ActionTime";
                case "business_discount":
                    return "CreateTime";
            }

            return "";
        }

        public void Execute()
        {
            int step = 100 / DataArchivingViewModel.Instance().Tables.Count;
            int progress = 0;
            foreach (var table in DataArchivingViewModel.Instance().Tables)
            {
                if (progress > 100) progress = 100;

                DataArchivingViewModel.Instance().ShowMessage($"正在归档{table}表...请等待...", progress);

                string archiveTable = $"{table}_{DateTime.Now.Year.ToString()}";

                if (!TableIsExists(archiveTable))
                {
                    CreateTable(table, archiveTable);
                }

                CompareColumns(table, archiveTable);
                DataArchive(table, archiveTable);
                progress += step;
            }

            DataArchivingViewModel.Instance().ShowMessage($"数据归档完成", 100);
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
