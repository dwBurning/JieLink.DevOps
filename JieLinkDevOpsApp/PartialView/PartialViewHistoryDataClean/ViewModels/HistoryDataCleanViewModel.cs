using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PartialViewHistoryDataClean.ViewModels
{
    public class HistoryDataCleanViewModel : DependencyObject
    {
        public DelegateCommand CleanDataCommand { get; set; }

        public bool CanExecute { get; set; }

        public string XmppIbd { get; set; }

        public enumCleanDataStep CleanDataStep { get; set; }

        public HistoryDataCleanViewModel()
        {
            TableSpace = "0KB";
            WarningColor = "#22AC38".ToColor().ToBrush();
            CleanDataCommand = new DelegateCommand();
            CleanDataCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return CanExecute; });
            CleanDataCommand.ExecuteAction = CleanData;
            Message = "执行数据清理前，建议先使用数据备份功能备份基础业务数据！！！";
        }


        public void Load()
        {
            try
            {
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select @@basedir as basePath from dual;").Tables[0];
                string mysqlPath = dt.Rows[0]["basePath"].ToString();
                DBJKDataPath = Path.Combine(mysqlPath, "data\\db_newg3_main");
                string xmppidb = Path.Combine(DBJKDataPath, "sync_xmpp.ibd");
                FileInfo fileInfo = new FileInfo(xmppidb);
                ConvertToSizeString(fileInfo.Length);
            }
            catch (Exception)
            {
                //CanExecute = false;
            }
        }

        private void CleanData(object parameter)
        {
            if (string.IsNullOrEmpty(DBJKDataPath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请先填写MySql的路径！");
                return;
            }

            string xmppidb = Path.Combine(DBJKDataPath, "sync_xmpp.ibd");
            if (!File.Exists(xmppidb))
            {
                MessageBoxHelper.MessageBoxShowWarning("没有找到sync_xmpp.ibd文件！");
                return;
            }

            if (MessageBoxHelper.MessageBoxShowQuestion("清理xmpp表数据不可恢复，且会短暂影响使用，是否继续？") == MessageBoxResult.No)
            {
                return;
            }

            //显示当前表空间大小
            FileInfo fileInfo = new FileInfo(xmppidb);
            ConvertToSizeString(fileInfo.Length);

            string ddlScript = DDLScript();
            if (string.IsNullOrEmpty(ddlScript))
            {
                MessageBoxHelper.MessageBoxShowWarning("无法获取XMPP表结构！");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                CanExecute = false;

                StopMySql();

                DeleteFile(xmppidb);

                StartMySql();

                DropTable();

                CreateTable(ddlScript);
               
                fileInfo = new FileInfo(xmppidb);
                ConvertToSizeString(fileInfo.Length);

                CanExecute = true;
            });
        }

        private string DDLScript()
        {
            CleanDataStep = enumCleanDataStep.DDLScript;
            DataTable dataTable = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "show create table sync_xmpp;").Tables[0];
            string ddlScript = dataTable.Rows[0]["Create Table"].ToString();
            return ddlScript;
        }

        private void StopMySql()
        {
            if (CleanDataStep > enumCleanDataStep.StopMySql) return;
            CleanDataStep = enumCleanDataStep.StopMySql;
            ShowMessage("正在停止MySql服务...");
            if (ProcessHelper.IsServiceRunning("mysql"))
            {
                ProcessHelper.StopService("mysql");
            }

            if (ProcessHelper.IsServiceRunning("mysql"))
            {
                ShowMessage("MySql服务暂停失败！请重新尝试或者手动停止...");
                CleanDataStep = enumCleanDataStep.Finsh;
                return;
            }
            else
            {
                ShowMessage("MySql服务已停止...");
            }
        }

        private void DeleteFile(string xmppidb)
        {
            if (CleanDataStep > enumCleanDataStep.DeleteFile) return;
            CleanDataStep = enumCleanDataStep.DeleteFile;
            ShowMessage("正在删除sync_xmpp.ibd...");
            if (File.Exists(xmppidb))
            {
                File.Delete(xmppidb);
                ShowMessage("sync_xmpp.ibd文件已删除...");
            }
        }

        private void DropTable()
        {
            if (CleanDataStep > enumCleanDataStep.DropTable) return;
            CleanDataStep = enumCleanDataStep.DropTable;
            ShowMessage("正在销毁sync_xmpp表...");
            try
            {
                MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, "drop table sync_xmpp");
                ShowMessage("sync_xmpp表已销毁...");
            }
            catch (Exception)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("sync_xmpp表销毁失败，请复制下面的脚本在Navicat中执行：").Append(Environment.NewLine);
                stringBuilder.Append("drop table sync_xmpp;").Append(Environment.NewLine);
            }
        }

        private void CreateTable(string ddlScript)
        {
            if (CleanDataStep > enumCleanDataStep.CreateTable) return;
            CleanDataStep = enumCleanDataStep.CreateTable;
            ShowMessage("正在创建sync_xmpp表...");
            try
            {
                MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, ddlScript);
                ShowMessage("sync_xmpp表已重新创建...");
                CleanDataStep = enumCleanDataStep.DDLScript;
                ShowMessage("sync_xmpp表数据清理完成...");
            }
            catch (Exception)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("sync_xmpp表创建失败，请复制下面的脚本在Navicat中执行：").Append(Environment.NewLine);
                stringBuilder.Append(ddlScript).Append(";").Append(Environment.NewLine);
            }
        }

        private void StartMySql()
        {
            if (CleanDataStep > enumCleanDataStep.StartMySql) return;
            CleanDataStep = enumCleanDataStep.StartMySql;
            ShowMessage("正在启动MySql服务...");
            if (!ProcessHelper.IsServiceRunning("mysql"))
            {
                ProcessHelper.StartService("mysql");
            }

            if (!ProcessHelper.IsServiceRunning("mysql"))
            {
                ShowMessage("MySql服务启动失败，请手动重启...");
            }
            else
            {
                ShowMessage("MySql服务已启动...");
            }
        }

        private void ConvertToSizeString(long length)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                int kb = (int)length / 1024;
                if (kb < 1024)
                {
                    WarningColor = "#22AC38".ToColor().ToBrush();//绿色
                    TableSpace = $"{kb}KB";
                }
                else
                {
                    int m = kb / 1024;
                    if (m < 1024)
                    {
                        WarningColor = "#22AC38".ToColor().ToBrush();//绿色
                        TableSpace = $"{m}M";
                        CanExecute = false;
                    }
                    else
                    {
                        int g = m / 1024;
                        if (g < 5)
                        {
                            WarningColor = "#F39800".ToColor().ToBrush();//黄色
                        }
                        else
                        {
                            WarningColor = "#D91F2D".ToColor().ToBrush();//红色
                        }
                        TableSpace = $"{g}G";
                        CanExecute = true;
                    }
                }
            }));
        }


        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Message = message;
            }));
        }

        public string TableSpace
        {
            get { return (string)GetValue(TableSpaceProperty); }
            set { SetValue(TableSpaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TableSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TableSpaceProperty =
            DependencyProperty.Register("TableSpace", typeof(string), typeof(HistoryDataCleanViewModel));


        public Brush WarningColor
        {
            get { return (Brush)GetValue(WarningColorProperty); }
            set { SetValue(WarningColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WarningColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WarningColorProperty =
            DependencyProperty.Register("WarningColor", typeof(Brush), typeof(HistoryDataCleanViewModel));


        public string DBJKDataPath
        {
            get { return (string)GetValue(DBJKDataPathProperty); }
            set { SetValue(DBJKDataPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DBJKDataPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DBJKDataPathProperty =
            DependencyProperty.Register("DBJKDataPath", typeof(string), typeof(HistoryDataCleanViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(HistoryDataCleanViewModel));

    }
}
