using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartialViewCheckUpdate;
using System.Windows;
using PartialViewCheckUpdate.Models.Enum;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using PartialViewInterface.Models;
using PartialViewInterface;
using MySql.Data.MySqlClient;
using System.Data;

namespace PartialViewCheckUpdate.ViewModels
{
    class CheckUpdateViewModel : DependencyObject
    {
        public DelegateCommand RepairCommand { get; set; }

        public CheckUpdateViewModel()
        {
            this.RepairCommand = new DelegateCommand();
            this.RepairCommand.ExecuteAction = this.Repair;
            ProcessHelper.ShowOutputMessageEx += ProcessHelper_ShowOutputMessageEx;


            Versions = new List<string>()
            {
                "V1.0.0",
                "V1.0.3",
                "V1.1.0",
                "V1.2.0",
                "V1.2.1",
                "V1.2.2",
                "V1.2.3",
                "V1.3.0",
                "V1.3.1",
                "V2.0.0",
                "V2.2.0",
                "V2.3.0",
                "V2.4.0",
                "V2.4.1",
                "V2.5.0",
                "V2.5.2",
                "V2.6.0",
                "V2.6.1",
                "V2.6.2",
                "V2.7.0",
                "V2.7.1",
                "V2.7.1#E1.0",
                "V2.7.1#E2.0",
                "V2.8.0",
                "V2.8.0#E1.0",
                "V2.8.1",
                "V2.8.1#E1.0",
                "V2.9.0",
            };



            Message = "1.升级辅助工具，只能升级中心，包括门禁服务，不能升级车场盒子\r\n2.一键升级，既替换文件同时也会执行脚本\r\n3.只替换文件顾名思义，只替换文件不执行脚本\r\n4.只执行脚本顾名思义，只执行脚本不替换文件\r\n5.如果版本号的下拉选项中没有你需要的版本号，可以直接输入，格式要求：\r\n非紧急版本，按照V1.0.0的格式输入，\r\n紧急版本，按照V2.7.1#E1.0的格式输入\r\n版本升级将会对数据库执行操作，建议先使用数据备份工具做基础数据备份，再执行升级\r\n";
        }

        private void ProcessHelper_ShowOutputMessageEx(string message)
        {
            ShowMessage(message);
        }

        public string StartVersion
        {
            get { return (string)GetValue(StartVersionProperty); }
            set { SetValue(StartVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartVersionProperty =
            DependencyProperty.Register("StartVersion", typeof(string), typeof(CheckUpdateViewModel));




        public List<string> Versions
        {
            get { return (List<string>)GetValue(VersionsProperty); }
            set { SetValue(VersionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Versions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionsProperty =
            DependencyProperty.Register("Versions", typeof(List<string>), typeof(CheckUpdateViewModel));






        public string EndVersion
        {
            get { return (string)GetValue(EndVersionProperty); }
            set { SetValue(EndVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndVersionProperty =
            DependencyProperty.Register("EndVersion", typeof(string), typeof(CheckUpdateViewModel));




        public string InstallPath
        {
            get { return (string)GetValue(InstallPathProperty); }
            set { SetValue(InstallPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InstallPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstallPathProperty =
            DependencyProperty.Register("InstallPath", typeof(string), typeof(CheckUpdateViewModel));




        public string PackagePath
        {
            get { return (string)GetValue(PackagePathProperty); }
            set { SetValue(PackagePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PackagePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PackagePathProperty =
            DependencyProperty.Register("PackagePath", typeof(string), typeof(CheckUpdateViewModel));




        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(CheckUpdateViewModel));





        private void Repair(object parameter)
        {
            if (string.IsNullOrEmpty(this.PackagePath) || string.IsNullOrEmpty(this.InstallPath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择正确的路径！");
                return;
            }

            if (string.IsNullOrEmpty(this.StartVersion) || string.IsNullOrEmpty(this.EndVersion))
            {
                MessageBoxHelper.MessageBoxShowWarning("请输入正确的版本号！");
                return;
            }

            DirectoryInfo packageDir = new DirectoryInfo(this.PackagePath);

            if (packageDir.Name.Equals("sys") || packageDir.Name.Equals("obj"))
                packageDir = packageDir.Parent;
            var zipPath = Directory.GetFiles(Path.Combine(packageDir.FullName, "obj"), "*.zip").FirstOrDefault();
            if (string.IsNullOrEmpty(zipPath) || !zipPath.Contains("JSOCT"))//加上这个判断，防止选成盒子的包
            {
                MessageBoxHelper.MessageBoxShowWarning("升级包不存在！");
                return;
            }

            if (MessageBoxHelper.MessageBoxShowQuestion($"请确认当前版本为{StartVersion}？") == MessageBoxResult.No)
            {
                return;
            }

            DirectoryInfo installDir = new DirectoryInfo(this.InstallPath);
            if (installDir.Name.Equals("SmartCenter", StringComparison.OrdinalIgnoreCase))
            {
                installDir = installDir.Parent;
            }
            string rootPath = installDir.FullName;
            //检测是否是一个有效的中心按照目录
            if (!File.Exists(Path.Combine(rootPath, "NewG3Uninstall.exe")))
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择正确的中心安装目录！");
                return;
            }

            UpdateRequest updateRequest = new UpdateRequest();
            updateRequest.Guid = Guid.NewGuid().ToString();
            updateRequest.Product = "JSOCT2016";
            updateRequest.RootPath = rootPath;
            updateRequest.PackagePath = zipPath;

            int cmd = int.Parse(parameter.ToString());
            switch (cmd)
            {
                case 0:
                    ExecuteUpdate(updateRequest);
                    ExecuteScript();
                    break;
                case 1:
                    ExecuteUpdate(updateRequest);
                    break;
                case 2:
                    ExecuteScript();
                    break;
            }
        }
        private void ExecuteUpdate(UpdateRequest request)
        {
            //1.升级请求写到update文件夹下
            WriteRequestFile(request);
            //2.启动升级程序
            string executePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\Updater.exe");
            ProcessHelper.StartProcessDotNet(executePath, "-file=UpdateRequest_2016.json");
        }

        private void WriteRequestFile(UpdateRequest request)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update\\UpdateRequest_2016.json");
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            string json = JsonHelper.SerializeObject(request);
            using (FileStream fs = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
        }


        private void ExecuteScript()
        {
            string scriptPath = Path.Combine(this.PackagePath, "dbscript");
            List<FileInfo> scripts = FileHelper.GetAllFileInfo(scriptPath, "*.sql").OrderBy(x => x.Name).ToList();

            string sVersion = this.StartVersion;

            int index = this.StartVersion.IndexOf("#");
            if (index > 0)
            {
                sVersion = this.StartVersion.Substring(0, index);
            }

            index = scripts.FindIndex(x => x.Name.Contains(sVersion));
            if (index < 0)//没有找到对应版本的脚本文件
            {
                MessageBoxHelper.MessageBoxShowWarning("当前版本对应的脚本文件不存在，请确认当前版本号是否正确！");
                return;
            }

            List<FileInfo> fileInfos = new List<FileInfo>();
            for (int i = index; i < scripts.Count; i++)
            {
                fileInfos.Add(scripts[i]);
            }

            string scriptName = $"dbDic_{EndVersion}.json";
            string scriptFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbInitScript", scriptName);

            string jsonText = "";
            if (File.Exists(scriptFile))
            {
                jsonText = File.ReadAllText(scriptFile, Encoding.UTF8);
            }

            Task.Factory.StartNew(() =>
            {
                foreach (var file in fileInfos)
                {
                    string mysqlcmd = $"mysql --default-character-set=utf8 -h{EnvironmentInfo.DbConnEntity.Ip} -u{EnvironmentInfo.DbConnEntity.UserName} -p{EnvironmentInfo.DbConnEntity.Password} -P{EnvironmentInfo.DbConnEntity.Port} {EnvironmentInfo.DbConnEntity.DbName} < \"{file.FullName}\"";

                    ShowMessage(mysqlcmd);
                    List<string> cmds = new List<string>();
                    string mysqlBin = AppDomain.CurrentDomain.BaseDirectory;
                    cmds.Add(mysqlBin.Substring(0, 2));
                    cmds.Add("cd " + mysqlBin);
                    cmds.Add(mysqlcmd);
                    ProcessHelper.ExecuteCommand(cmds, enumToolType.OneKeyUpdate);
                }

                ShowMessage("脚本执行完成，正在校验数据库...");
                CheckTables(jsonText);
            });
        }

        private void CheckTables(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                ShowMessage($"版本{this.EndVersion}的初始化json文件不存在，不执行数据库校验...");
                ShowMessage("升级已经完成，请留意观察JieLink中心使用是否正常...");
                return;
            }

            StringBuilder exceptMessage = new StringBuilder();


            DBVersionScript script = JsonHelper.DeserializeObject<DBVersionScript>(jsonText);

            foreach (var table in script.TableList)
            {
                if (table.Type == 1)
                {
                    ShowMessage($"正在校验{table.TableName}表...");

                    if (TableIsExists(table.TableName))
                    {
                        Table dbTable = GetTable(table.TableName);

                        List<Column> columns = table.ColumnList.Where(x => !dbTable.ColumnList.Exists(y => x.Field.Equals(y.Field))).ToList();

                        foreach (Column column in columns)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append($"ALTER TABLE `{table.TableName}` Add COLUMN `{column.Field}` {column.Type}");
                            if (!column.IsNull)
                            {
                                builder.Append(" NOT NULL");
                            }

                            if (!string.IsNullOrEmpty(column.Default))
                            {
                                builder.Append($" DEFAULT '{column.Default}'");
                            }

                            builder.Append(" COLLATE utf8_unicode_ci");
                            ShowMessage($"添加{column.Field}字段...");
                            try
                            {
                                MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, builder.ToString());
                            }
                            catch (Exception)
                            {
                                exceptMessage.Append($"{table.TableName}表添加{column.Field}字段失败...").Append(Environment.NewLine);
                            }

                        }
                    }
                    else
                    {
                        ShowMessage($"创建{table.TableName}表...");
                        string result = CreateTable(table);
                        if (!string.IsNullOrEmpty(result))
                        {
                            exceptMessage.Append(result).Append(Environment.NewLine);
                        }
                    }
                }
            }

            ShowMessage("数据库校验完成...");
            if (string.IsNullOrEmpty(exceptMessage.ToString()))
            {
                ShowMessage("升级已经完成，请留意观察JieLink中心使用是否正常...");
            }
            else
            {
                ShowMessage("升级已经完成，以下脚本执行异常，需要人工处理：");
                ShowMessage(exceptMessage.ToString());
            }
        }


        private string CreateTable(Table table)
        {
            StringBuilder script = new StringBuilder();

            if (table.Type == 1)
            {
                script.Append($"CREATE TABLE `{table.TableName}` (");
                //字段
                foreach (var field in table.ColumnList)
                {
                    #region int bigint
                    if (field.Type.StartsWith("int") || field.Type.StartsWith("bigint"))
                    {
                        if (field.IsKey)
                        {
                            script.Append($"`{field.Field}` {field.Type} NOT NULL AUTO_INCREMENT,").Append(Environment.NewLine);
                        }
                        else
                        {
                            if (field.IsNull)
                            {
                                if (string.IsNullOrEmpty(field.Default))
                                {
                                    script.Append($"`{field.Field}` {field.Type} DEFAULT '0',").Append(Environment.NewLine);
                                }
                                else
                                {
                                    script.Append($"`{field.Field}` {field.Type} DEFAULT '{field.Default}',").Append(Environment.NewLine);
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(field.Default))
                                {
                                    script.Append($"`{field.Field}` {field.Type} NOT NULL DEFAULT '0',").Append(Environment.NewLine);
                                }
                                else
                                {
                                    script.Append($"`{field.Field}` {field.Type} NOT NULL DEFAULT '{field.Default}',").Append(Environment.NewLine);
                                }
                            }
                        }

                    }
                    #endregion

                    #region varchar char
                    if (field.Type.StartsWith("varchar") || field.Type.StartsWith("char"))
                    {
                        if (field.IsNull)
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} COLLATE utf8_unicode_ci DEFAULT NULL,").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} COLLATE utf8_unicode_ci DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} COLLATE utf8_unicode_ci NOT NULL,").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} COLLATE utf8_unicode_ci NOT NULL DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }
                        }
                    }
                    #endregion

                    #region decimal
                    if (field.Type.StartsWith("decimal"))
                    {
                        if (field.IsNull)
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} DEFAULT '0.00',").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} NOT NULL DEFAULT '0.00',").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} NOT NULL DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }
                        }
                    }
                    #endregion

                    #region float
                    if (field.Type.StartsWith("float"))
                    {
                        if (field.IsNull)
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} DEFAULT '0',").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} NOT NULL DEFAULT '0',").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} NOT NULL DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }
                        }
                    }
                    #endregion

                    #region datetime
                    if (field.Type.StartsWith("datetime"))
                    {
                        if (field.IsNull)
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} DEFAULT NULL,").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }

                        }
                        else
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                script.Append($"`{field.Field}` {field.Type} NOT NULL,").Append(Environment.NewLine);
                            }
                            else
                            {
                                script.Append($"`{field.Field}` {field.Type} NOT NULL DEFAULT '{field.Default}',").Append(Environment.NewLine);
                            }
                        }
                    }
                    #endregion
                }


                //索引
                foreach (var index in table.IndexList)
                {
                    #region 创建索引
                    if (index.NonUnique == 0)
                    {
                        if (index.KeyName == "PRIMARY")
                        {
                            script.Append($"PRIMARY KEY (`{index.ColumnName}`),").Append(Environment.NewLine);
                        }
                        else
                        {
                            script.Append($"UNIQUE KEY `{index.KeyName}` (`{index.ColumnName}`) USING BTREE,").Append(Environment.NewLine);
                        }
                    }

                    if (index.NonUnique == 1)
                    {
                        script.Append($"KEY `{index.KeyName}` (`{index.ColumnName}`) USING BTREE,").Append(Environment.NewLine);
                    }
                    #endregion
                }

                string ddlScript = script.ToString().TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',') + Environment.NewLine;

                ddlScript += $") ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci";

                try
                {
                    MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, ddlScript);
                }
                catch (Exception)
                {
                    return $"表{table.TableName}创建失败！";
                }
            }
            return "";
        }

        private Table GetTable(string tableName)
        {
            string sql = $"desc `{tableName}`";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            Table table = new Table();
            table.TableName = tableName;
            table.ColumnList = new List<Column>();
            table.IndexList = new List<Index>();
            foreach (DataRow dr in dt.Rows)
            {
                Column column = new Column();
                column.Field = dr["Field"].ToString();
                column.Type = dr["Type"].ToString();
                column.IsNull = dr["Null"].ToString().ToUpper().Equals("YES");
                column.IsKey = string.IsNullOrEmpty(dr["Key"].ToString());
                column.Default = dr["Default"].ToString();
                column.Extra = dr["Extra"].ToString();
                table.ColumnList.Add(column);


                if (dr["Key"].ToString().Equals("PRI") || dr["Key"].ToString().Equals("UNI"))
                {
                    Index index = new Index();
                    index.KeyName = "index_" + column.Field;
                    index.NonUnique = 0;
                    index.ColumnName = column.Field;
                    index.SeqInIndex = 1;
                    table.IndexList.Add(index);
                }
                else if (dr["Key"].ToString().Equals("MUL"))
                {
                    Index index = new Index();
                    index.KeyName = "index_" + column.Field;
                    index.NonUnique = 1;
                    index.ColumnName = column.Field;
                    index.SeqInIndex = 1;
                    table.IndexList.Add(index);
                }
            }
            return table;
        }


        /// <summary>
        /// 先判断表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool TableIsExists(string tableName)
        {
            string sql = $"select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='{EnvironmentInfo.DbConnEntity.DbName}' and TABLE_NAME='{tableName}'";
            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
            return dt.Rows.Count > 0;
        }


        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Message != null && Message.Length > 5000)
                {
                    Message = string.Empty;
                }

                if (message.Length > 0)
                {
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }

    }
}
