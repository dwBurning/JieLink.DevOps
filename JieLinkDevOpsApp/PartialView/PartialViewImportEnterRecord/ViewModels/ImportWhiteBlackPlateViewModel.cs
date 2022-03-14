using MySql.Data.MySqlClient;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewImportEnterRecord.ViewModels
{
    class ImportWhiteBlackPlateViewModel : DependencyObject
    {
        public DelegateCommand ImportWhiteBlackCommand { get; set; }

        public DelegateCommand ClearWhiteBlackCommand { get; set; }

        public bool canExecute { get; set; }

        public ImportWhiteBlackPlateViewModel()
        {
            ImportWhiteBlackCommand = new DelegateCommand();
            ImportWhiteBlackCommand.ExecuteAction = ImportEnterRecord;
            ImportWhiteBlackCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });
            ClearWhiteBlackCommand = new DelegateCommand();
            ClearWhiteBlackCommand.ExecuteAction = Clear;
            Message = "说明：本工具是给jielink车场导入黑白名单记录使用的。\r\n1).导入的记录会有标记，且存在黑白名单记录时，会自动跳过，规避风险。\r\n2).清理功能，只会清理用工具导入的黑白名单记录，其余记录不受影响。\r\n";
        }


        private void Clear(object paramater)
        {
            Task.Factory.StartNew(() =>
            {
                string sql = "delete from  black_white_grey where Remark='运维工具导入记录'";
                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                ShowMessage($"共清理黑白名单记录{result}条");
            });
        }

        private void ImportEnterRecord(object paramater)
        {
            string filePath = this.FilePath;
            bool isInsert = this.IsInsertData;
            if (!(filePath.EndsWith(".xlsx") || filePath.EndsWith(".xls")))
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择Excel文件！");
                return;
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    DataTable dt = NPOIExcelHelper.ExcelToDataTable(filePath, true);
                    int count = 0;
                    List<string> sqls = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string plate = dr["车牌"].ToString();
                        string type = dr["类型"].ToString();
                        string startDate = dr["开始日期"].ToString();
                        string endDate = dr["结束日期"].ToString();
                        string reason = dr["添加原因"].ToString();

                        int pType = 3;

                        if (type == "黑名单")
                        { pType = 1; }

                        if (type == "灰名单")
                        { pType = 2; }

                        if (type == "白名单")
                        { pType = 3; }


                        if (string.IsNullOrEmpty(plate))
                        {
                            continue;
                        }

                        if (IsExists(plate))
                        {
                            ShowMessage($"车牌：{plate} 已存在黑白名单记录，直接跳过...");
                            continue;
                        }

                        string sql = $"INSERT INTO `black_white_grey` VALUES (uuid(), '{plate}', {pType}, '{startDate} 00:00:00', '{endDate} 23:59:59', '{reason}', 1, '9999', '超级管理员', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', 0, '运维工具导入记录', '00');";

                        if (isInsert)
                        {
                            int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                            if (result > 0)
                            {
                                count++;
                                ShowMessage($"车牌：{plate} 添加黑白名单记录成功！");
                            }
                        }
                        else
                        {
                            sqls.Add(sql);
                        }
                    }
                    if (isInsert)
                    {
                        ShowMessage($"共插入黑白名单记录{count}条");
                    }
                    else
                    {
                        OutPut(sqls, "black_white_grey");
                    }

                }
                catch (Exception ex)
                {
                    ShowMessage("请确认文件是否被占用或者正处于打开状态！");
                    ShowMessage(ex.ToString());
                }
            });
        }

        private void OutPut(List<string> cmds, string tableName)
        {
            if (cmds.Count <= 0)
            {
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var cmd in cmds)
            {
                stringBuilder.AppendLine(cmd);
            }

            File.WriteAllText(tableName + ".sql", stringBuilder.ToString());
            ProcessHelper.StartProcessV2("notepad.exe", tableName + ".sql");
        }

        private bool IsExists(string plate)
        {
            string sql = $"select * from black_white_grey where Plate='{plate}';";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, sql))
            {
                return reader.Read();
            }
        }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImportWhiteBlackPlateViewModel));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ImportWhiteBlackPlateViewModel));

        public bool IsInsertData
        {
            get { return (bool)GetValue(IsInsertDataProperty); }
            set { SetValue(IsInsertDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInsertData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInsertDataProperty =
            DependencyProperty.Register("IsInsertData", typeof(bool), typeof(ImportWhiteBlackPlateViewModel));

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
