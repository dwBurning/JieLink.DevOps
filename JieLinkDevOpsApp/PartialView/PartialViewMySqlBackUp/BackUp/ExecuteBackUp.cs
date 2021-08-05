using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewMySqlBackUp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewMySqlBackUp.BackUp
{
    public class ExecuteBackUp
    {
        MySqlBackUpViewModel viewModel = MySqlBackUpViewModel.Instance();

        /// <summary>
        /// 全库备份
        /// 修改：增加数据库名参数
        /// </summary>
        /// <param name="databaseName"></param>
        public void BackUpDataBase(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                viewModel.ShowMessage("当前备份策略中，数据库名称为空，无法备份，请确认或重新配置策略！");
                return;
            }

            try
            {
                viewModel.ShowMessage("正在执行全库备份...");
                string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_DB_{databaseName}.sql";
                string filePath = "";
                viewModel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    filePath = Path.Combine(viewModel.TaskBackUpPath, fileName);
                    if (!Directory.Exists(viewModel.TaskBackUpPath))
                    {
                        Directory.CreateDirectory(viewModel.TaskBackUpPath);
                    }
                }));
                string mysqlcmd = $"mysqldump --default-character-set=utf8 --single-transaction -h{EnvironmentInfo.DbConnEntity.Ip} -u{EnvironmentInfo.DbConnEntity.UserName} -p{EnvironmentInfo.DbConnEntity.Password} -P{EnvironmentInfo.DbConnEntity.Port}  -B {databaseName} -R > \"{filePath}\"";
                viewModel.ShowMessage(mysqlcmd);
                List<string> cmds = new List<string>();
                cmds.Add(viewModel.MySqlBinPath.Substring(0, 2));
                cmds.Add("cd " + viewModel.MySqlBinPath);
                cmds.Add(mysqlcmd);
                ProcessHelper.ExecuteCommand(cmds);
                ZipHelper.ZipFile(filePath, filePath.Replace(".sql", ".zip"));
                File.Delete(filePath);
                viewModel.ShowMessage("备份完成！");
            }
            catch (Exception ex)
            {
                viewModel.ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// 业务表备份
        /// 修改：增加数据库名参数
        /// </summary>
        /// <param name="databaseName"></param>
        public void BackUpTables(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                viewModel.ShowMessage("当前备份策略中，数据库名称为空，无法备份，请确认或重新配置策略！");
                return;
            }

            try
            {
                viewModel.ShowMessage("正在执行基础业务备份...");
                string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_Table_{databaseName}.sql";
                string filePath = "";
                viewModel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    filePath = Path.Combine(viewModel.TaskBackUpPath, fileName);
                    if (!Directory.Exists(viewModel.TaskBackUpPath))
                    {
                        Directory.CreateDirectory(viewModel.TaskBackUpPath);
                    }
                }));

                string tables = "";
                var tablesConfig = viewModel.BackUpConfig.TablesConfig.FirstOrDefault(x => x.DbName == databaseName);
                if (tablesConfig != null)
                {
                    tablesConfig.Tables.ToList().ForEach((x) =>
                    {
                        tables += x.TableName + " ";
                    });
                }
                else
                {
                    viewModel.ShowMessage("当前备份策略中，未配置业务表范围，无法备份，请确认或重新配置策略！");
                    return;
                }

                string mysqlcmd = $"mysqldump --default-character-set=utf8 --single-transaction -h{EnvironmentInfo.DbConnEntity.Ip} -u{EnvironmentInfo.DbConnEntity.UserName} -p{EnvironmentInfo.DbConnEntity.Password} -P{EnvironmentInfo.DbConnEntity.Port}  -B {databaseName} --tables {tables} > \"{filePath}\"";

                viewModel.ShowMessage(mysqlcmd);
                List<string> cmds = new List<string>();
                cmds.Add(viewModel.MySqlBinPath.Substring(0, 2));
                cmds.Add("cd " + viewModel.MySqlBinPath);
                cmds.Add(mysqlcmd);
                ProcessHelper.ExecuteCommand(cmds);
                ZipHelper.ZipFile(filePath, filePath.Replace(".sql", ".zip"));
                File.Delete(filePath);
                viewModel.ShowMessage("备份完成！");
            }
            catch (Exception ex)
            {
                viewModel.ShowMessage(ex.Message);
            }
        }
    }
}
