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
        public void BackUpDataBase()
        {
            try
            {
                viewModel.ShowMessage("正在执行全库备份...");
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_db.sql";
                string filePath = "";
                viewModel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    filePath = Path.Combine(viewModel.TaskBackUpPath, fileName);
                }));
                string mysqlcmd = $"mysqldump --default-character-set=utf8 --single-transaction -h{EnvironmentInfo.DbConnEntity.Ip} -u{EnvironmentInfo.DbConnEntity.UserName} -p{EnvironmentInfo.DbConnEntity.Password} -P{EnvironmentInfo.DbConnEntity.Port}  -B {EnvironmentInfo.DbConnEntity.DbName} -R > \"{filePath}\"";

                viewModel.ShowMessage(mysqlcmd);
                List<string> cmds = new List<string>();
                cmds.Add(viewModel.MySqlBinPath.Substring(0, 2));
                cmds.Add("cd " + viewModel.MySqlBinPath);
                cmds.Add(mysqlcmd);
                ProcessHelper.ExecuteCommand(cmds);
                ZipHelper.ZipFile(filePath, filePath.Replace(".sql", ".zip"));
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                viewModel.ShowMessage(ex.Message);
            }
        }

        public void BackUpTables()
        {
            try
            {
                viewModel.ShowMessage("正在执行基础业务备份...");
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_table.sql";
                string filePath = "";
                viewModel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    filePath = Path.Combine(viewModel.TaskBackUpPath, fileName);
                }));
                string tables = "";
                if (viewModel.Tables.FindIndex(x => x.IsChecked) >= 0)
                {
                    viewModel.Tables.ForEach(x =>
                    {
                        if (x.IsChecked)
                        {
                            tables += x.TableName + " ";
                        }
                    });
                }

                string mysqlcmd = $"mysqldump --default-character-set=utf8 --single-transaction -h{EnvironmentInfo.DbConnEntity.Ip} -u{EnvironmentInfo.DbConnEntity.UserName} -p{EnvironmentInfo.DbConnEntity.Password} -P{EnvironmentInfo.DbConnEntity.Port}  -B {EnvironmentInfo.DbConnEntity.DbName} --tables {tables} > \"{filePath}\"";

                viewModel.ShowMessage(mysqlcmd);
                List<string> cmds = new List<string>();
                cmds.Add(viewModel.MySqlBinPath.Substring(0, 2));
                cmds.Add("cd " + viewModel.MySqlBinPath);
                cmds.Add(mysqlcmd);
                ProcessHelper.ExecuteCommand(cmds);
                ZipHelper.ZipFile(filePath, filePath.Replace(".sql", ".zip"));
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                viewModel.ShowMessage(ex.Message);
            }

        }
    }
}
