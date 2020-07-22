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
    class CheckUpdateResult
    {
        public static bool CheckUpdateResultFun(string dbDicDir,string connectString)
        {
            if (!Directory.Exists(dbDicDir))
            {
                Console.WriteLine("没有对比文件，默认成功");
                return true;
            }

            try
            {
                var dicFile = Directory.GetFiles(dbDicDir, "*.dll").FirstOrDefault();
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
                using(var conn = new MySqlConnection(connectString))
                {
                    dbName = conn.Database;
                    var tableListTemp = conn
                        .Query($"SELECT TABLE_NAME,TABLE_TYPE from information_schema.'TABLES' where TABLE_SCHEMA = '{dbName}'", null, null, true, 120 * 1000)
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

                        var columnList = conn.Query($"show full COLUMNS from '{table.tableName}'", null, null, true, 120 * 1000)
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
                    foreach (var targetTableItem in targetStruct.tableList.Where(x => x.Type ==1))
                    {
                        if (!result)
                        {
                            break;
                        }

                        var newTable = newStruct.tableList.FirstOrDefault(x => string.Equals(x.tableName, targetTableItem.tableName, StringComparison.CurrentCultureIgnoreCase));
                        if (targetTableItem?.columnList==null)
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
                    if (!result)
                    {
                        var targetContent = JsonConvert.SerializeObject(targetStruct);
                        Console.WriteLine("数据库比对失败，请查看修复脚本");

                        /* 待修改
                        FileEx.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs/targetDbDic.json"), targetContent);
                        FileEx.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs/targetDbDic.json"), newResult);
                        */
                        /*
                         修复脚本
                         * 
                         */


                    }

                    return result;
                }
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 在命令行中运行命令
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="command"></param>
        /// <param name="FileName"></param>
        public static void StartCmd(string workingDirectory, string command, string FileName = "cmd.exe")
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = FileName;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.StandardInput.WriteLine(command);
            process.StandardInput.WriteLine("exit");
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();
        }

        /// <summary>
        /// 在命令行中运行可执行文件
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="lstcmd"></param>
        public static void ExecuteExeFileOnCommandLine(string fileFullPath, List<string> lstcmd)
        {
            if (lstcmd == null || lstcmd.Any())
            {
                return;
            }
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
            //新窗口
            //process.OutputDataReceived += process_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            //切换到目录
            var strArray = fileFullPath.Split(':')[0];
            process.StandardInput.WriteLine($"{strArray}:");
            process.StandardInput.WriteLine("cd " + fileFullPath);
            //在bin目录下执行命令
            foreach (var cmd in lstcmd)
            {
                process.StandardInput.WriteLine(cmd);
            }
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();

        }
    }
}
