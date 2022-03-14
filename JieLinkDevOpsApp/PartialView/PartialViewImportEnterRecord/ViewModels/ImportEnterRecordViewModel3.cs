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
    class ParkInfo
    {
        public string areaId { get; set; }
        public string areaName  { get; set; }
        public string parentId { get; set; }
        public string level { get; set; }
        public string channelId { get; set; }
        public string channelName { get; set; }
        public string channelIoType { get; set; }
        public string deviceId { get; set; }
        public string deviceName { get; set; }
    }

    class Area {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string Level { get; set; }
    }

    class ImportEnterRecordViewModel3 : DependencyObject
    {
        public DelegateCommand ImportEnterRecordCommand { get; set; }

        public DelegateCommand ClearEnterRecordCommand { get; set; }

        public bool canExecute { get; set; }

        public DelegateCommand ImportPCSEnterRecordCommand { get; set; }

        public DelegateCommand ClearPCSEnterRecordCommand { get; set; }

        public bool pcsExecute { get; set; }

        private string connStr = string.Empty;

        private string[] dbs = new string[] { "jielink", "jielink_pcs" };

        private string GetConnStr(string dbName)
        {
            return $"Data Source={EnvironmentInfo.DbConnEntity.Ip};port={EnvironmentInfo.DbConnEntity.Port};User ID={EnvironmentInfo.DbConnEntity.UserName};Password={EnvironmentInfo.DbConnEntity.Password};Initial Catalog={dbName};Pooling=true;charset=utf8;";
        }

        public ImportEnterRecordViewModel3()
        {
            ImportEnterRecordCommand = new DelegateCommand();
            ImportEnterRecordCommand.ExecuteAction = ImportEnterRecord;
            ImportEnterRecordCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });
            ClearEnterRecordCommand = new DelegateCommand();
            ClearEnterRecordCommand.ExecuteAction = Clear;
            Message = "说明：本工具是给其他车场切换到jielink车场，导入场内记录使用的。\r\n欢迎加入jielink阵营!!!\r\n1).导入的记录会有标记，且存在场内记录时，会自动跳过，规避风险。\r\n2).清理功能，只会清理用工具导入的场内记录，其余记录不受影响。\r\n3).请善用此工具！\r\n";
        }

        private void Clear(object paramater)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (string db in dbs)
                {
                    string connStr = GetConnStr(db);
                    string sql = "delete from pcs_enter_record where Remark='运维工具导入';";
                    int result = MySqlHelper.ExecuteNonQuery(connStr, sql);
                    ShowMessage($"数据库 {db} 共清理pcs_enter_record表记录{result}条");
                }

            });

            Task.Factory.StartNew(() =>
            {
                foreach (string db in dbs)
                {
                    string connStr = GetConnStr(db);
                    string sql = "delete from pcs_in where Remark='运维工具导入';";
                    int result = MySqlHelper.ExecuteNonQuery(connStr, sql);
                    ShowMessage($"数据库 {db} 共清理pcs_in表记录{result}条");
                }
            });
        }

        #region ImportEnterRecord
        private void ImportEnterRecord(object paramater)
        {

            Dictionary<string, string> areaNameToAreaId = new Dictionary<string, string>();
            Dictionary<string, ParkInfo> areaToChannelDevice = new Dictionary<string, ParkInfo>();
            Dictionary<string, List<string>> areaToParentArea = new Dictionary<string, List<string>>();
            string filePath = FilePath;
            bool isInsert = IsInsertData;
            try
            {
                
                if (!(filePath.EndsWith(".xlsx") || filePath.EndsWith(".xls")))
                {
                    MessageBoxHelper.MessageBoxShowWarning("请选择Excel文件！");
                    return;
                }
                DataTable dtTable = NPOIExcelHelper.ExcelToDataTable(filePath, true);
                if(dtTable == null || dtTable.Rows.Count <= 0)
                {
                    ShowMessage("解析表格数据出错或者表格数据为空");
                    return;
                }
                IEnumerable<IGrouping<object, DataRow>> groupArea = dtTable.Rows.Cast<DataRow>().GroupBy(x => x["车场区域名称"]);
                List<object> areas = groupArea.Select(x => x.Key).ToList();
                DataSet dsArea = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select Id, Name, ParentId, Level from pms_area");
                if (dsArea.Tables.Count <= 0 || dsArea.Tables[0].Rows.Count <= 0)
                {
                    MessageBoxHelper.MessageBoxShowWarning("该车场无车场区域数据！");
                    return;
                }
                List<Area> areaList = dsArea.Tables[0].AsEnumerable().Select(x => new Area
                {
                    Id = x.ItemArray[0].ToString(),
                    Name = x.ItemArray[1].ToString(),
                    ParentId = x.ItemArray[2].ToString(),
                    Level = x.ItemArray[3].ToString()
                }).ToList();
                foreach (Area area in areaList)
                {
                    areaNameToAreaId.Add(area.Name.ToString(), area.Id.ToString());
                }
                foreach (object areaName in areas)
                {
                    if (!areaNameToAreaId.ContainsKey(areaName.ToString()))
                    {
                        MessageBoxHelper.MessageBoxShowWarning($"该车场无 {areaName.ToString()} 车场区域数据！");
                        return;
                    }
                }

                DataSet dsParkInfo = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select area.Id as areaId, area.Name as areaName, area.ParentId, area.Level, channel.Id as channelId, channel.Name as channelName, channel.IoType as channelIoType, device.Id as deviceId, device.Name as deviceName from pms_area as area, pms_area_channel_rel as areaTochannel, pms_channel as channel, pms_channel_device_rel as channelTodevice, pms_device as device where area.Id = areaTochannel.AreaId and areaTochannel.ChannelId = channel.Id and areaTochannel.ChannelId = channelTodevice.ChannelId and channelTodevice.DeviceId = device.Id and channel.IoType = 1 and channelTodevice.SubChannelType = 10");
                if (dsParkInfo.Tables.Count <= 0 || dsParkInfo.Tables[0].Rows.Count <= 0)
                {
                    MessageBoxHelper.MessageBoxShowWarning("该车场无通道设备信息数据！");
                    return;
                }
                foreach (object areaName in areas)
                {
                    string areaId = areaNameToAreaId[areaName.ToString()];
                    if (!areaToChannelDevice.ContainsKey(areaId))
                    {
                        var item = dsParkInfo.Tables[0].AsEnumerable().Where(x => x.ItemArray[0].ToString() == areaId).FirstOrDefault();
                        if (item == null)
                        {
                            MessageBoxHelper.MessageBoxShowWarning("该车场无" + areaName.ToString() + "车场区域下的通道设备信息数据！");
                            return;
                        }
                        areaToChannelDevice.Add(areaId, new ParkInfo
                        {
                            areaId = item["areaId"].ToString(),
                            areaName = item["areaName"].ToString(),
                            parentId = item["ParentId"].ToString(),
                            level = item["Level"].ToString(),
                            channelId = item["channelId"].ToString(),
                            channelName = item["channelName"].ToString(),
                            channelIoType = item["channelIoType"].ToString(),
                            deviceId = item["deviceId"].ToString(),
                            deviceName = item["deviceName"].ToString()
                        });
                    }
                }

                foreach (object areaName in areas)
                {
                    string areaId = areaNameToAreaId[areaName.ToString()];
                    var areaInfo = areaList.Where(x => x.Id == areaId && x.ParentId != "root").FirstOrDefault();
                    while (areaInfo != null)
                    {
                        if (areaToParentArea.ContainsKey(areaId))
                        {
                            areaToParentArea[areaId].Add(areaInfo.ParentId);
                        }
                        else
                        {
                            areaToParentArea.Add(areaId, new List<string>() { areaInfo.ParentId });
                        }
                        areaInfo = areaList.Where(x => x.Id == areaInfo.ParentId && x.ParentId != "root").FirstOrDefault();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBoxHelper.MessageBoxShowWarning("执行出错:" + ex.ToString());
                return;
            }

            #region 读取套餐
            DataTable setmealTypeDT = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from pms_setmeal_type;").Tables[0];
            List<SetmealType> setmealTypes = new List<SetmealType>();
            foreach (DataRow row in setmealTypeDT.Rows)
            {
                setmealTypes.Add(new SetmealType()
                {
                    SealNo = int.Parse(row["SealNo"].ToString()),
                    Id = row["Id"].ToString(),
                    Name = row["Name"].ToString(),
                    UserType = int.Parse(row["UserType"].ToString())
                });
            }
            #endregion

            Task.Factory.StartNew(() =>
            {
                try
                {
                    int count = 0;
                    List<string> sqls = new List<string>();
                    DataTable dt = NPOIExcelHelper.ExcelToDataTable(filePath, true);
                    if(dt == null)
                    {
                        ShowMessage("解析表格数据出错");
                        return;
                    }
                    foreach (DataRow row in dt.Rows)
                    {
                        string plate = row["车牌"].ToString();

                        string enterTime = string.IsNullOrEmpty(row["入场时间"].ToString()) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : row["入场时间"].ToString();

                        SetmealType setmealType = setmealTypes.FirstOrDefault(x => x.SealNo == 54);
                        int sealId = 54;
                        if (int.TryParse(row["套餐ID"].ToString(), out sealId))
                        {
                            setmealType = setmealTypes.FirstOrDefault(x => x.SealNo == sealId);
                        }
                        if (!string.IsNullOrEmpty(row["套餐类型"].ToString()))
                        {
                            setmealType.Name = row["套餐类型"].ToString();
                        }

                        if (string.IsNullOrEmpty(plate))
                        {
                            continue;
                        }

                        if (IsExists(plate))
                        {
                            ShowMessage($"车牌：{plate} 已存在场内记录，直接跳过...");
                            continue;
                        }

                        if ((DateTime.Parse(enterTime) - DateTime.Now).TotalSeconds > 0)
                        {
                            ShowMessage($"车牌：{plate} 入场时间：{enterTime} 不能晚于当前时间，直接跳过...");
                            continue;
                        }
                        string areaName = row["车场区域名称"].ToString();
                        string areaId = areaNameToAreaId[areaName.ToString()];
                        string sql = $"insert into pcs_enter_record (Id,TransId,SessionId,AreaId,AreaLevel,AreaName,EnterTime,CredentialNo,CredentialType,RecognizedPlate,RecognizedPlateColor,CredibleDegree,VehicleLogo,VehicleType,VehicleColor,CorrectPlate,CorrectPlatecolor,OperatorNo,OperatorName,OperatorId,DeviceId,DeviceName,ChannelId,ChannelName,PersonId,PersonNo,PersonName,PersonTel,SealId,SealNo,SealName,SealType,OriginSealId,IsOfflineRecord,EventType,RequestSource,PicPath,VideoPath,Remark,OperateType,Exts) VALUES('{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','{areaId}','0','{areaToChannelDevice[areaId].areaName}','{enterTime}','{plate}','163','{plate}','蓝牌','0','','小型汽车','','','','9999','超级管理员','','{areaToChannelDevice[areaId].deviceId}','{areaToChannelDevice[areaId].deviceName}','{areaToChannelDevice[areaId].channelId}','{areaToChannelDevice[areaId].channelName}','','','临时用户','','{setmealType.Id}',{setmealType.SealNo},'{setmealType.Name}',{setmealType.UserType},'','0','1','1','','','运维工具导入','0','');";
                        string cmd = $"insert into pcs_in (Id,TransId,SessionId,ParkId,ParkName,AreaId,AreaName,AreaLevel,EnterTime,CredentialNo,CredentialType,RecognizedPlate,RecognizedPlateColorEnm,RecognizedPlateColor,ConvertSealType,ConvertTime,VehicleLogo,VehicleType,VehicleColor,CorrectPlate,CorrectPlateColor,CorrectPlateColorEnm,OperatorNo,OperatorName,OperatorId,DeviceId,DeviceName,ChannelId,ChannelName,PersonId,PersonNo,PersonName,PersonTel,SealId,SealNo,SealName,SealType,OriginSealId,IsLocked,WithholdStatus,WithholdLimit,IsOfflineRecord,EventType,Status,RequestSource,PicPath,Remark,OperateType,Exts) VALUES('{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','','','{areaId}','{areaToChannelDevice[areaId].areaName}','0','{enterTime}','{plate}','163','{plate}','0','蓝牌','0','1970-01-01 00:00:00','','小型汽车','','','','0','9999','超级管理员','','{areaToChannelDevice[areaId].deviceId}','{areaToChannelDevice[areaId].deviceName}','{areaToChannelDevice[areaId].channelId}','{areaToChannelDevice[areaId].channelName}','','','临时用户','','{sealId}','{setmealType.SealNo}','{setmealType.Name}','{setmealType.UserType}','','0','0','0','0','1','1','1','','运维工具导入','0','');";
                        string sql1 = string.Empty;
                        string cmd1 = string.Empty; 
                        
                        if (areaToParentArea.ContainsKey(areaId))
                        {
                            foreach(var parentAreaId in areaToParentArea[areaId])
                            {
                                sql1 += $"insert into pcs_enter_record (Id,TransId,SessionId,AreaId,AreaLevel,AreaName,EnterTime,CredentialNo,CredentialType,RecognizedPlate,RecognizedPlateColor,CredibleDegree,VehicleLogo,VehicleType,VehicleColor,CorrectPlate,CorrectPlatecolor,OperatorNo,OperatorName,OperatorId,DeviceId,DeviceName,ChannelId,ChannelName,PersonId,PersonNo,PersonName,PersonTel,SealId,SealNo,SealName,SealType,OriginSealId,IsOfflineRecord,EventType,RequestSource,PicPath,VideoPath,Remark,OperateType,Exts) VALUES('{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','{parentAreaId}','0','{areaToChannelDevice[parentAreaId].areaName}','{enterTime}','{plate}','163','{plate}','蓝牌','0','','小型汽车','','','','9999','超级管理员','','{areaToChannelDevice[parentAreaId].deviceId}','{areaToChannelDevice[parentAreaId].deviceName}','{areaToChannelDevice[parentAreaId].channelId}','{areaToChannelDevice[parentAreaId].channelName}','','','临时用户','','{setmealType.Id}',{setmealType.SealNo},'{setmealType.Name}',{setmealType.UserType},'','0','1','1','','','运维工具导入','0','');";
                                cmd += $"insert into pcs_in (Id,TransId,SessionId,ParkId,ParkName,AreaId,AreaName,AreaLevel,EnterTime,CredentialNo,CredentialType,RecognizedPlate,RecognizedPlateColorEnm,RecognizedPlateColor,ConvertSealType,ConvertTime,VehicleLogo,VehicleType,VehicleColor,CorrectPlate,CorrectPlateColor,CorrectPlateColorEnm,OperatorNo,OperatorName,OperatorId,DeviceId,DeviceName,ChannelId,ChannelName,PersonId,PersonNo,PersonName,PersonTel,SealId,SealNo,SealName,SealType,OriginSealId,IsLocked,WithholdStatus,WithholdLimit,IsOfflineRecord,EventType,Status,RequestSource,PicPath,Remark,OperateType,Exts) VALUES('{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','{Guid.NewGuid().ToString().Replace("-","")}','','','{parentAreaId}','{areaToChannelDevice[parentAreaId].areaName}','0','{enterTime}','{plate}','163','{plate}','0','蓝牌','0','1970-01-01 00:00:00','','小型汽车','','','','0','9999','超级管理员','','{areaToChannelDevice[parentAreaId].deviceId}','{areaToChannelDevice[parentAreaId].deviceName}','{areaToChannelDevice[parentAreaId].channelId}','{areaToChannelDevice[areaId].channelName}','','','临时用户','','{sealId}','{setmealType.SealNo}','{setmealType.Name}','{setmealType.UserType}','','0','0','0','0','1','1','1','','运维工具导入','0','');";
                            }
                        }

                        if (isInsert)
                        {
                            foreach (string db in dbs)
                            {
                                string connStr = GetConnStr(db);
                                int result = MySqlHelper.ExecuteNonQuery(connStr, sql + cmd + sql1 + cmd1);
                                if (result > 0)
                                {
                                    ShowMessage($"数据库 {db} 补录 { plate} 入场记录成功！");
                                }
                            }
                            count++;
                        }
                        else
                        {
                            sqls.Add(sql + cmd + sql1 + cmd1);
                        }
                    }

                    if (isInsert)
                    {
                        ShowMessage($"共插入场内记录{count}条");
                    }
                    else
                    {
                        OutPut(sqls, "pcs_enter_record");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("请确认文件是否被占用或者正处于打开状态以及数据库信息是否正确！");
                    ShowMessage(ex.ToString());
                }
            });
        }
        #endregion

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
            string sql = $"select CredentialNO from pcs_in where CredentialNO='{plate}' and `Status`=1;";
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
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImportEnterRecordViewModel3));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ImportEnterRecordViewModel3));

        public bool IsInsertData
        {
            get { return (bool)GetValue(IsInsertDataProperty); }
            set { SetValue(IsInsertDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInsertData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInsertDataProperty =
            DependencyProperty.Register("IsInsertData", typeof(bool), typeof(ImportEnterRecordViewModel3));

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
