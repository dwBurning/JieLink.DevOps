using MySql.Data.MySqlClient;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewImportEnterRecord.ViewModels
{
    public class ImportEnterRecordViewModel : DependencyObject
    {
        public DelegateCommand ImportEnterRecordCommand { get; set; }

        public DelegateCommand ClearEnterRecordCommand { get; set; }

        public bool canExecute { get; set; }

        public ImportEnterRecordViewModel()
        {
            ImportEnterRecordCommand = new DelegateCommand();
            ImportEnterRecordCommand.ExecuteAction = ImportEnterRecord;
            ImportEnterRecordCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });

            ClearEnterRecordCommand = new DelegateCommand();
            ClearEnterRecordCommand.ExecuteAction = Clear;

            Message = "【禁止外传】：1)该工具是给其他车场切换到jielink车场，导入场内记录使用的。";
        }

        private void Clear(object paramater)
        {
            Task.Factory.StartNew(() =>
            {
                string sql = "delete from box_enter_record where wasgone=0 and remark='运维工具导入记录'";
                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                ShowMessage($"共清理场内记录{result}条");
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
                        string intime = dr["入场时间"].ToString();
                        string sealName = dr["套餐类型"].ToString();
                        if (string.IsNullOrEmpty(sealName))
                        { sealName = "临时套餐A"; }
                        int sealId = 54;
                        int.TryParse(dr["套餐ID"].ToString(), out sealId);
                        if (sealId == 0)
                        { sealId = 54; }
                        string enterDeviceId = dr["入场设备ID"].ToString();
                        if (string.IsNullOrEmpty(enterDeviceId))
                        { enterDeviceId = "188766208"; }
                        string enterDeviceName = dr["入场设备名称"].ToString();
                        if (string.IsNullOrEmpty(enterDeviceName))
                        { enterDeviceName = "虚拟车场入口"; }

                        if (string.IsNullOrEmpty(plate))
                        {
                            continue;
                        }

                        if (IsExists(plate))
                        {
                            ShowMessage($"车牌：{plate} 已存在场内记录，直接跳过...");
                            continue;
                        }

                        string sql = $"insert into box_enter_record (CredentialType,CredentialNO,Plate,CarNumOrig,EnterTime,SetmealType,SealName,EGuid,EnterRecordID,EnterDeviceID,EnterDeviceName,WasGone,EventType,EventTypeName,ParkNo,OperatorNo,OperatorName,PersonName,Remark) VALUES(163,'{plate}','{plate}','{plate}','{intime}',{sealId},'{sealName}',UUID(),REPLACE(UUID(),'-',''),'{enterDeviceId}','{enterDeviceName}',0,1,'一般正常记录','00000000-0000-0000-0000-000000000000','9999','超级管理员','临时车主','运维工具导入记录');";

                        if (isInsert)
                        {
                            int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                            if (result > 0)
                            {
                                count++;
                                ShowMessage($"车牌：{plate} 补录入场记录成功！");
                            }
                        }
                        else
                        {
                            sqls.Add(sql);
                        }
                    }
                    if (isInsert)
                    {
                        ShowMessage($"共插入场内记录{count}条");
                    }
                    else
                    {
                        OutPut(sqls);
                    }

                }
                catch (Exception ex)
                {
                    ShowMessage("请确认文件是否被占用或者正处于打开状态！");
                    ShowMessage(ex.ToString());
                }
            });
        }

        private void OutPut(List<string> cmds)
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

            File.WriteAllText("Box_EnterRecord.sql", stringBuilder.ToString());
            ProcessHelper.StartProcessV2("notepad.exe", "Box_EnterRecord.sql");
        }

        private bool IsExists(string plate)
        {
            string sql = $"select CredentialNO from box_enter_record where CredentialNO='{plate}' and WasGone=0;";
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
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImportEnterRecordViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ImportEnterRecordViewModel));

        public bool IsInsertData
        {
            get { return (bool)GetValue(IsInsertDataProperty); }
            set { SetValue(IsInsertDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInsertData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInsertDataProperty =
            DependencyProperty.Register("IsInsertData", typeof(bool), typeof(ImportEnterRecordViewModel));


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
