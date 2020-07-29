using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Dapper;
using System.Diagnostics;

namespace PartialViewCheckUpdate.ViewModels
{
    class CheckDBUpdateTool
    {
        public static bool CheckDBUpdate(string dbDicDir, string connectString)
        {
            
            try
            {
                var dicFile = dbDicDir;
                if (string.IsNullOrEmpty(dicFile))
                {
                    Console.WriteLine($"没有对比文件{dicFile}，默认成功");
                    return true;
                }

                var fileContent = File.ReadAllText(dicFile);
                var dicStruct = new
                {
                    tableList = new[]
                    {
                        new
                        {
                            tableName = "",
                            Type = 1,
                            columnList = new[]
                            {
                                new
                                {
                                    Field = "",
                                    Type = "",
                                    IsNull = false,
                                    IsKey = false,
                                },
                            },
                        },
                    },
                };

                var targetStruct = JsonConvert.DeserializeAnonymousType(fileContent, dicStruct);

                string newResult = string.Empty;
                string dbName;
                using (var conn = new MySqlConnection(connectString))
                {
                    dbName = conn.Database;
                    var tableListTemp = conn
                        .Query($"SELECT TABLE_NAME,TABLE_TYPE from information_schema.`TABLES` where TABLE_SCHEMA = '{dbName}'", null, null, true, 120 * 1000)
                        .Select(x => new
                        {
                            tableName = x.TABLE_NAME,
                            Type = (x.TABLE_TYPE == "BASE TABLE" ? 1 : 2),
                        })
                        .Where(x => targetStruct.tableList.Any(y => string.Equals(y.tableName, x.tableName, StringComparison.InvariantCultureIgnoreCase)))
                        .ToArray();

                    var tableList = new List<dynamic>();
                    foreach (var table in tableListTemp)
                    {
                        if (table.Type != 1)
                        {
                            tableList.Add(table);
                            continue;
                        }

                        var columnList = conn.Query($"show full COLUMNS from `{table.tableName}`", null, null, true, 120 * 1000)
                            .Select(x => new
                            {
                                Field = x.Field,
                                Type = x.Type,
                                IsNull = x.Null == "YES",
                                IsKey = !string.IsNullOrEmpty(x.Key),
                            }).ToArray();

                        tableList.Add(new { table.Type, table.tableName, columnList, });

                        newResult = JsonConvert.SerializeObject(new { tableList });
                    }

                    var result = true;
                    var newStruct = JsonConvert.DeserializeAnonymousType(newResult, dicStruct);
                    foreach (var targetTableItem in targetStruct.tableList.Where(x => x.Type == 1))
                    {
                        if (!result)
                        {
                            break;
                        }

                        var newTable = newStruct.tableList.FirstOrDefault(x => string.Equals(x.tableName, targetTableItem.tableName, StringComparison.CurrentCultureIgnoreCase));
                        if (targetTableItem?.columnList == null)
                        {
                            continue;
                        }
                        foreach (var targetColumn in targetTableItem.columnList)
                        {
                            var newColumn = newTable?.columnList.FirstOrDefault(x => string.Equals(x.Field, targetColumn.Field, StringComparison.CurrentCultureIgnoreCase));
                            if (newColumn == null)
                            {
                                result = false;
                                break;
                            }

                            result = string.Equals(newColumn.Field, targetColumn.Field, StringComparison.CurrentCultureIgnoreCase) &&
                                newColumn.IsKey == targetColumn.IsKey && newColumn.IsNull == targetColumn.IsNull;
                            if (!result)
                            {
                                break;
                            }
                        }
                    }
                    //if (!result)
                    //{
                    //    var targetContent = JsonConvert.SerializeObject(targetStruct);
                    //    Console.WriteLine("数据库比对失败，请查看修复脚本");

                    //    /* 待修改
                    //    FileEx.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs/targetDbDic.json"), targetContent);
                    //    FileEx.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs/targetDbDic.json"), newResult);
                    //    修复脚本
                    //    */
                    //}

                    return result;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public static bool TestMySqlConn(string connStr)
        {
            
            bool IsConnected = false;
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                IsConnected = true;
            }
            catch (Exception)
            {
                IsConnected = false;
            }
            finally
            {
                conn.Close();
            }
            return IsConnected;

        }
    }
}
