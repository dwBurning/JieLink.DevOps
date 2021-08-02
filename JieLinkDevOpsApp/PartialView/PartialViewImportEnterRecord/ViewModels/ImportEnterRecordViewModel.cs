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

        public DelegateCommand ImportPCSEnterRecordCommand { get; set; }

        public DelegateCommand ClearPCSEnterRecordCommand { get; set; }

        public bool pcsExecute { get; set; }

        private string connStr = string.Empty;

        public ImportEnterRecordViewModel()
        {
            ImportEnterRecordCommand = new DelegateCommand();
            ImportEnterRecordCommand.ExecuteAction = ImportEnterRecord;
            ImportEnterRecordCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });
            ClearEnterRecordCommand = new DelegateCommand();
            ClearEnterRecordCommand.ExecuteAction = Clear;
            Message = "说明：本工具是给其他车场切换到jielink车场，导入场内记录使用的。\r\n欢迎加入jielink阵营!!!\r\n1).导入的记录会有标记，且存在场内记录时，会自动跳过，规避风险。\r\n2).清理功能，只会清理用工具导入的场内记录，其余记录不受影响。\r\n3).请善用此工具！\r\n";

            ImportPCSEnterRecordCommand = new DelegateCommand();
            ImportPCSEnterRecordCommand.ExecuteAction = ImportPCSEnterRecord;
            ImportPCSEnterRecordCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return pcsExecute; });
            ClearPCSEnterRecordCommand = new DelegateCommand();
            ClearPCSEnterRecordCommand.ExecuteAction = PCSClear;
            Info = "说明：该页面为3.x版本数据记录导入页面。在导入数据之前，先确保数据库里至少有一条参考数据。请填写完数据库信息和选中相应的EXCEL文档(导入入场记录模板表)，再点击执行进行数据导入。只提供对pcs_in表和pcs_enter_record表记录的插入。时间格式请确保为yyyy-mm-dd hh:mm:ss文本格式。模板前两列为必填项\r\n";
        }

        #region 2.x数据记录导入
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

                        if ((DateTime.Parse(intime) - DateTime.Now).TotalSeconds > 0)
                        {
                            ShowMessage($"车牌：{plate} 入场时间：{intime} 不能晚于当前时间，直接跳过...");
                            continue;
                        }

                        string sql = $"insert into box_enter_record (CredentialType,CredentialNO,Plate,CarNumOrig,EnterTime,SetmealType,SealName,EGuid,EnterRecordID,EnterDeviceID,EnterDeviceName,WasGone,EventType,EventTypeName,ParkNo,OperatorNo,OperatorName,PersonName,Remark,InDeviceEnterType,OptDate) VALUES(163,'{plate}','{plate}','{plate}','{intime}',{sealId},'{sealName}',UUID(),REPLACE(UUID(),'-',''),'{enterDeviceId}','{enterDeviceName}',0,1,'一般正常记录','00000000-0000-0000-0000-000000000000','9999','超级管理员','临时车主','运维工具导入记录',1,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');";

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
                        OutPut(sqls, "box_enter_record");
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
        #endregion

        #region 3.x数据记录导入
        private bool CheckConn(out string message)
        {
            bool connStatus = false;
            try
            {
                string IP = "1";
                string port = "1";

                if (string.IsNullOrEmpty(ServiceIP))
                {
                    message = "请填写数据库连接信息";
                    return false;
                }

                if (ServiceIP.Contains(","))
                {
                    IP = this.ServiceIP.Substring(0, ServiceIP.IndexOf(',')).Trim();
                    port = this.ServiceIP.Substring(ServiceIP.IndexOf(',') + 1).Trim();
                }
                connStr = $"Data Source={IP};port={port};User ID={UserName};Password={PassWord};Initial Catalog={DataBase};Pooling = false;charset=utf8;";
                using (MySqlConnection mySqlConnection = new MySqlConnection(connStr))
                {
                    mySqlConnection.Open();
                    mySqlConnection.Close();
                }
                connStatus = true;
                message = string.Empty;
            }
            catch (Exception ex)
            {
                connStatus = false;
                connStr = string.Empty;
                message = ex.Message;
            }
            return connStatus;
        }

        private void ImportPCSEnterRecord(object paramater)
        {
            string filePath = PCSFilePath;
            bool IsGenerateScript = this.IsGenerateScript;
            if (!(filePath.EndsWith(".xlsx") || filePath.EndsWith(".xls")))
            {
                MessageBoxHelper.MessageBoxShowWarning("请选择Excel文件！");
                return;
            }
            string errorMessage = string.Empty;
            if (!CheckConn(out errorMessage))
            {
                ShowInfo(errorMessage);
                return;
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    int count = 0;
                    List<string> sqls = new List<string>();
                    DataSet dataSet = NPOIExcelHelper.ExcelToDataSet(filePath, true);
                    if (dataSet == null)
                    {
                        ShowInfo("打开excel表格错误！");
                        return;
                    }

                    if (!dataSet.Tables.Contains("pcs_enter_record"))
                    {
                        ShowInfo("模板错误，请确认！");
                        return;
                    }

                    for (int index = 0; index < dataSet.Tables.Count; index++)
                    {
                        string tableName = dataSet.Tables[index].TableName;

                        if (dataSet.Tables[index].TableName == "pcs_enter_record")
                        {
                            DataSet ds = MySqlHelper.ExecuteDataset(connStr, string.Format("select *from {0} limit 1", tableName));
                            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count <= 0)
                            {
                                ShowInfo(tableName + "不存在数据，请先入场一条数据作为模板，供插入的数据参考！");
                                return;
                            }
                            DataTable dataTable = ds.Tables[0];
                            foreach (DataRow dr in dataSet.Tables[index].Rows)
                            {
                                string areaId = dataTable.Rows[0]["AreaId"].ToString();
                                //string areaLevel = string.IsNullOrEmpty(dr["AreaLevel"].ToString()) ? "0" : dr["停车区级别"].ToString();
                                string areaName = dataTable.Rows[0]["AreaName"].ToString();
                                string enterTime = string.IsNullOrEmpty(dr["入场时间"].ToString()) ? DateTime.Now.ToString() : dr["入场时间"].ToString();
                                string credentialNo = dr["识别到的车牌"].ToString();
                                string recognizedPlate = dr["识别到的车牌"].ToString();
                                if (string.IsNullOrEmpty(recognizedPlate))
                                {
                                    continue;
                                }
                                string recognizedPlateColor = dataTable.Rows[0]["RecognizedPlateColor"].ToString();
                                //string credibleDegree = dr["车牌置信度"].ToString();
                                string vehicleLogo = dataTable.Rows[0]["VehicleLogo"].ToString();
                                string vehicleType = dataTable.Rows[0]["VehicleType"].ToString();
                                string vehicleColor = dataTable.Rows[0]["VehicleColor"].ToString();
                                string correctPlate = dataTable.Rows[0]["CorrectPlate"].ToString();
                                string correctPlatecolor = dataTable.Rows[0]["CorrectPlatecolor"].ToString();
                                //string operatorNo = dr["操作员编号"].ToString();
                                //string operatorName = dr["操作员姓名"].ToString();
                                string operatorId = dataTable.Rows[0]["OperatorId"].ToString();
                                string deviceId = dataTable.Rows[0]["DeviceId"].ToString();
                                string deviceName = dr["设备名称"].ToString();
                                string channelId = dataTable.Rows[0]["ChannelId"].ToString();
                                string channelName = dataTable.Rows[0]["ChannelName"].ToString();
                                string personId = dataTable.Rows[0]["PersonId"].ToString();
                                string personNo = dataTable.Rows[0]["PersonNo"].ToString();
                                string personName = dr["车主名称"].ToString();
                                string personTel = dr["车主电话"].ToString();
                                string sealId = dataTable.Rows[0]["SealId"].ToString();
                                string sealNo = dataTable.Rows[0]["SealNo"].ToString();
                                string sealName = dr["套餐名称"].ToString();
                                //string sealType = dr["套餐类型"].ToString();
                                string originSealId = dataTable.Rows[0]["OriginSealId"].ToString();
                                //string isOfflineRecord = dr["是否为下位机开闸记录"].ToString();
                                //string eventType = dr["事件类型"].ToString();
                                string requestSource = dataTable.Rows[0]["RequestSource"].ToString();
                                string picPath = dataTable.Rows[0]["PicPath"].ToString();
                                string videoPath = dataTable.Rows[0]["VideoPath"].ToString();
                                //string remark = dr["备注"].ToString();
                                string operateType = dataTable.Rows[0]["OperateType"].ToString();
                                string exts = dataTable.Rows[0]["Exts"].ToString();

                                string sql = $"insert into pcs_enter_record (Id,TransId,SessionId,AreaId,AreaLevel,AreaName,EnterTime,CredentialNo,CredentialType,RecognizedPlate," +
                                    $"RecognizedPlateColor,CredibleDegree,VehicleLogo,VehicleType,VehicleColor,CorrectPlate,CorrectPlatecolor,OperatorNo,OperatorName," +
                                    $"OperatorId,DeviceId,DeviceName,ChannelId,ChannelName,PersonId,PersonNo,PersonName,PersonTel,SealId,SealNo,SealName,SealType,OriginSealId," +
                                    $"IsOfflineRecord,EventType,RequestSource,PicPath,VideoPath,Remark,OperateType,Exts)" +
                                    $" VALUES(REPLACE(UUID(),'-',''),REPLACE(UUID(),'-',''),REPLACE(UUID(),'-',''),'{areaId}','0','{areaName}','{enterTime}','{credentialNo}','163','{recognizedPlate}','{recognizedPlateColor}'" +
                                    $",'0','{vehicleLogo}','{vehicleType}','{vehicleColor}','{correctPlate}','{correctPlatecolor}','9999','超级管理员','{operatorId}'" +
                                    $",'{deviceId}','{deviceName}','{channelId}','{channelName}','{personId}','{personNo}','{personName}','{personTel}','{sealId}','{sealNo}','{sealName}','1'" +
                                    $",'{originSealId}','0','5','{requestSource}','{picPath}','{videoPath}','运维工具导入','3','{exts}');";
                                if (IsGenerateScript)
                                {
                                    int result = MySqlHelper.ExecuteNonQuery(connStr, sql);
                                    if (result > 0)
                                    {
                                        count++;
                                        ShowInfo($"补录" + recognizedPlate + "入场记录pcs_enter_record成功！");
                                    }
                                }
                                else
                                {
                                    sqls.Add(sql);
                                }
                            }
                        }
                        else if (dataSet.Tables[index].TableName == "pcs_in")
                        {
                            DataSet ds = MySqlHelper.ExecuteDataset(connStr, string.Format("select *from {0} limit 1", tableName));
                            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count <= 0)
                            {
                                ShowInfo(tableName + "不存在数据，请先入场一条数据作为模板，供插入的数据参考！");
                                return;
                            }
                            DataTable dataTable = ds.Tables[0];
                            foreach (DataRow dr in dataSet.Tables[index].Rows)
                            {
                                //string transId = dr["事务ID"].ToString();
                                //string sessionId = dr["会话ID"].ToString();
                                //string parkId = dr["车场ID"].ToString();
                                string parkName = dataTable.Rows[0]["ParkName"].ToString();
                                string areaId = dataTable.Rows[0]["AreaId"].ToString();
                                string areaName = dataTable.Rows[0]["AreaName"].ToString();
                                //string areaLevel = string.IsNullOrEmpty(dr["停车区级别"].ToString()) ? "0" : dr["停车区级别"].ToString();
                                string enterTime = string.IsNullOrEmpty(dr["入场时间"].ToString()) ? DateTime.Now.ToString() : dr["入场时间"].ToString();
                                string credentialNo = dr["识别到的车牌"].ToString();
                                //string credentialType = string.IsNullOrEmpty(dr["凭证类型"].ToString()) ? "163" : dr["凭证类型"].ToString();
                                string recognizedPlate = dr["识别到的车牌"].ToString();
                                if (string.IsNullOrEmpty(recognizedPlate))
                                {
                                    continue;
                                }
                                string recognizedPlateColor = dataTable.Rows[0]["RecognizedPlateColor"].ToString();
                                //string recognizedPlateColorEnum = dr["识别到的车牌颜色枚举"].ToString();
                                //string convertSealType = dr["实时套餐类型"].ToString();
                                //string convertTime = dr["套餐转换时间"].ToString();
                                string vehicleLogo = dataTable.Rows[0]["VehicleLogo"].ToString();
                                string vehicleType = dataTable.Rows[0]["VehicleType"].ToString();
                                string vehicleColor = dataTable.Rows[0]["VehicleColor"].ToString();
                                string correctPlate = dataTable.Rows[0]["CorrectPlate"].ToString();
                                string correctPlatecolor = dataTable.Rows[0]["CorrectPlatecolor"].ToString();
                                //string correctPlatecolorEnum = dr["纠正后的车牌颜色枚举"].ToString();
                                //string operatorNo = string.IsNullOrEmpty(dr["操作员编号"].ToString()) ? "9999" : dr["操作员编号"].ToString();
                                //string operatorName = string.IsNullOrEmpty(dr["操作员姓名"].ToString()) ? "超级管理员" : dr["操作员姓名"].ToString();
                                string operatorId = dataTable.Rows[0]["operatorId"].ToString();
                                string deviceId = dataTable.Rows[0]["deviceId"].ToString();
                                string deviceName = dr["设备名称"].ToString();
                                string channelId = dataTable.Rows[0]["channelId"].ToString();
                                string channelName = dataTable.Rows[0]["channelName"].ToString();
                                string personId = dataTable.Rows[0]["personId"].ToString();
                                string personNo = dataTable.Rows[0]["personNo"].ToString();
                                string personName = dr["车主名称"].ToString();
                                string personTel = dr["车主电话"].ToString();
                                string sealId = dataTable.Rows[0]["SealId"].ToString();
                                string sealNo = dataTable.Rows[0]["SealNo"].ToString();
                                string sealName = dr["套餐名称"].ToString();
                                //string sealType = dr["套餐类型"].ToString();
                                string originSealId = dataTable.Rows[0]["OriginSealId"].ToString();
                                //string isLocked = dr["是否已锁定"].ToString();
                                //string withholdStatus = dr["验签状态"].ToString();
                                //string withholdLimit = dr["无感限额"].ToString();
                                //string isOfflineRecord = dr["是否为下位机开闸"].ToString();
                                //string eventType = dr["事件类型"].ToString();
                                //string status = dr["状态"].ToString();
                                //string requestSource = dr["请求源"].ToString();
                                string picPath = dataTable.Rows[0]["PicPath"].ToString();
                                //string remark = dr["备注"].ToString();
                                //string operateType = dr["操作类型"].ToString();
                                string exts = dataTable.Rows[0]["Exts"].ToString();

                                string sql = $"insert into pcs_in (Id,TransId,SessionId,ParkId,ParkName,AreaId,AreaName,AreaLevel,EnterTime,CredentialNo,CredentialType," +
                                $"RecognizedPlate,RecognizedPlateColorEnm,RecognizedPlateColor,ConvertSealType,ConvertTime,VehicleLogo,VehicleType,VehicleColor," +
                                $"CorrectPlate,CorrectPlateColor,CorrectPlateColorEnm,OperatorNo,OperatorName,OperatorId,DeviceId,DeviceName,ChannelId,ChannelName," +
                                $"PersonId,PersonNo,PersonName,PersonTel,SealId,SealNo,SealName,SealType,OriginSealId,IsLocked,WithholdStatus,WithholdLimit,IsOfflineRecord," +
                                $"EventType,Status,RequestSource,PicPath,Remark,OperateType,Exts) " +
                                $"VALUES(REPLACE(UUID(),'-',''),REPLACE(UUID(),'-',''),REPLACE(UUID(),'-',''),REPLACE(UUID(),'-',''),'{parkName}','{areaId}','{areaName}','0','{enterTime}','{credentialNo}','163','{recognizedPlate}'" +
                                $",'0','{recognizedPlateColor}','0','1970-01-01 00:00:00','{vehicleLogo}','{vehicleType}','{vehicleColor}','{correctPlate}'" +
                                $",'{correctPlatecolor}','0','9999','超级管理员','{operatorId}','{deviceId}','{deviceName}','{channelId}','{channelName}'" +
                                $",'{personId}','{personNo}','{personName}','{personTel}','{sealId}','{sealNo}','{sealName}','0','{originSealId}','0','0'" +
                                $",'0','0','5','1','5','{picPath}','运维工具导入','3','{exts}');";

                                if (IsGenerateScript)
                                {
                                    int result = MySqlHelper.ExecuteNonQuery(connStr, sql);
                                    if (result > 0)
                                    {
                                        count++;
                                        ShowInfo($"补录" + recognizedPlate + "入场记录pcs_in成功！");
                                    }
                                }
                                else
                                {
                                    sqls.Add(sql);
                                }
                            }
                        }
                    }
                    if (IsGenerateScript)
                    {
                        ShowInfo($"共插入场内记录{count}条");
                    }
                    else
                    {
                        OutPut(sqls, "3.x入场记录");
                    }
                }
                catch (Exception ex)
                {
                    ShowInfo("请确认文件是否被占用或者正处于打开状态以及数据库信息是否正确！");
                    ShowInfo(ex.ToString());
                }
            });
        }

        private void PCSClear(object paramater)
        {
            string errorMessage = string.Empty;
            if (!CheckConn(out errorMessage))
            {
                ShowInfo(errorMessage);
                return;
            }

            Task.Factory.StartNew(() =>
            {
                string sql = "delete from pcs_enter_record";
                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                ShowInfo($"共清理pcs_enter_record表记录{result}条");
            });

            Task.Factory.StartNew(() =>
            {
                string sql = "delete from pcs_in";
                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
                ShowInfo($"共清理pcs_in表记录{result}条");
            });
        }

        public void ShowInfo(string info)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Info != null && Info.Length > 5000)
                {
                    Info = string.Empty;
                }

                if (info.Length > 0)
                {
                    Info += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {info}{Environment.NewLine}";
                }
            }));
        }

        public string PCSFilePath
        {
            get { return (string)GetValue(PCSFilePathProperty); }
            set { SetValue(PCSFilePathProperty, value); }
        }
        public static readonly DependencyProperty PCSFilePathProperty =
            DependencyProperty.Register("filePath", typeof(string), typeof(ImportEnterRecordViewModel));

        public string ServiceIP
        {
            get { return (string)GetValue(ServiceIPProperty); }
            set { SetValue(ServiceIPProperty, value); }
        }
        public static readonly DependencyProperty ServiceIPProperty =
            DependencyProperty.Register("serviceIP", typeof(string), typeof(ImportEnterRecordViewModel));

        public string DataBase
        {
            get { return (string)GetValue(DataBaseProperty); }
            set { SetValue(DataBaseProperty, value); }
        }
        public static readonly DependencyProperty DataBaseProperty =
            DependencyProperty.Register("dataBase", typeof(string), typeof(ImportEnterRecordViewModel));

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("userName", typeof(string), typeof(ImportEnterRecordViewModel));

        public string PassWord
        {
            get { return (string)GetValue(PassWordProperty); }
            set { SetValue(PassWordProperty, value); }
        }
        public static readonly DependencyProperty PassWordProperty =
            DependencyProperty.Register("passWord", typeof(string), typeof(ImportEnterRecordViewModel));

        public bool IsGenerateScript
        {
            get { return (bool)GetValue(IsGenerateScriptProperty); }
            set { SetValue(IsGenerateScriptProperty, value); }
        }
        public static readonly DependencyProperty IsGenerateScriptProperty =
            DependencyProperty.Register("IsGenerateScript", typeof(bool), typeof(ImportEnterRecordViewModel));

        public string Info
        {
            get { return (string)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }
        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register("Info", typeof(string), typeof(ImportEnterRecordViewModel));
        #endregion
    }
}
