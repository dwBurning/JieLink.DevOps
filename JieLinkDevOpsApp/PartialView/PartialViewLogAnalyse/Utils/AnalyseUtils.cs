using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartialViewInterface.Utils;
using PartialViewLogAnalyse.Models;
using PartialViewLogAnalyse.Models.JSIP;

namespace PartialViewLogAnalyse.Utils
{
    public class AnalyseUtils
    {
        public static int GetTotalLines(List<string> filePaths)
        {
            int totalLines = 0;
            foreach (var filePath in filePaths)
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("gb2312")))
                {
                    while (!sr.EndOfStream)
                    {
                        sr.ReadLine();
                        ++totalLines;

                    }
                }
            }
            return totalLines;
        }
        public static void ParseRecord(RecordContext context, string line, List<string> lastLines)
        {
            if (ParseUpLoadParkOutRecord(context, line, lastLines))
                return;
            if (ParseUpLoadParkInRecord(context, line, lastLines))
                return;
            if (ParseRequestPass(context, line, lastLines))
                return;
            if (ParseMatchEnterRecord(context, line, lastLines))
                return;
            if (ParseAddToDeviceQueue(context, line, lastLines))
                return;
            if (ParseProcessOnlineBegin(context, line, lastLines))
                return;
            if (ParseShowMessage(context, line, lastLines))
                return;
            if (ParseConfirmOpenGateOut(context, line, lastLines))
                return;
            if (ParseConfirmOpenGateEnter(context, line, lastLines))
                return;
            if (ParseSuspendFlowBegin(context, line, lastLines))
                return;
            if (ParseSuspendFlowEnd(context, line, lastLines))
                return;
            if (ParseRemoteEvent(context, line, lastLines))
                return;
            if (ParseBillChanged(context, line, lastLines))
                return;
            if (ParseOpenGateBegin(context, line, lastLines))
                return;
            if (ParseOpenGateSuccess(context, line, lastLines))
                return;
            if (ParseOpenGateFailed(context, line, lastLines))
                return;
            if (ParseSaveBill(context, line, lastLines))
                return;
            if (ParseVehicleBack(context, line, lastLines))
                return;
            if (ParseSaveRecord(context, line, lastLines))
                return;
            if (ParseCloudSeatBegin(context, line, lastLines))
                return;
            if (ParseCloudRemoteOpenGate(context, line, lastLines))
                return;
            if (ParseReceiveFacePay(context, line, lastLines))
                return;
            if (ParseFacePayBegin(context, line, lastLines))
                return;
            if (ParseFacePayTimeOut(context, line, lastLines))
                return;
            if (ParseFacePayReturn(context, line, lastLines))
                return;
            if (ParseFacePayEnd(context, line, lastLines))
                return;
            if (ParseFacePayEnd2(context, line, lastLines))
                return;
            if (ParseNoPlateScanCode(context, line, lastLines))
                return;
            if (ParseEndDeviceFlowProcess(context, line, lastLines))
                return;
        }
        public static void ParseOrderRecord(RecordContext context, string line, List<string> lastLines)
        {
            if (ParseJsPayOrder(context, line, lastLines))
                return;
            if (ParseOrderMatchEnterRecord(context, line, lastLines))
                return;
            if (ParseOrderReturn(context, line, lastLines))
                return;
            if (ParseGetOrder(context, line, lastLines))
                return;
            //if (ParseOrderMatchCurrentRecord(context, line, lastLines))
            //    return;
            if (ParseGetOrderReturn(context, line, lastLines))
                return;
            if (GetOrderNotifyPay(context, line, lastLines))
                return;
        }
        public static void MergeParkRecordAndOrder(RecordContext context)
        {
            //更新iotype
            foreach (var parkRecord in context.ParkRecords)
            {
                if (parkRecord.IoType == 0)
                {
                    var device = context.DeviceCache.FirstOrDefault(x => x.DeviceName == parkRecord.DeviceName && x.IoType > 0);
                    if (device != null)
                    {
                        parkRecord.DeviceId = device.DeviceId;
                        parkRecord.IoType = device.IoType;
                    }
                }
            }
            foreach (var orderRecord in context.OrderRecords)
            {
                if (string.IsNullOrEmpty(orderRecord.CredentialNo) || orderRecord.CredentialNo == "未识别")
                {
                    continue;
                }
                ParkRecord parkRecord = context.ParkRecords.FirstOrDefault(x => x.IoType == 2 && orderRecord.ReceiveTime < x.EndTime
                     && (FuzzyCompare(x.CredentialNo, orderRecord.CredentialNo) || x.HistoryCredentialNo.Any(y => FuzzyCompare(y, orderRecord.CredentialNo))));

                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.GetOrder;
                    logNode.ThreadName = orderRecord.ThreadName;
                    logNode.LogTime = orderRecord.ReceiveTime;
                    logNode.Message = string.Format("{0},{1},查询订单信息，订单号={2}，金额={3}，返回码={4}，返回消息={5}"
                        , "线上扫码", orderRecord.CredentialNo, orderRecord.OrderNo, orderRecord.TotalFee, orderRecord.ErrorCode, orderRecord.ErrorMessage);
                    parkRecord.LogNodes.Add(logNode);
                    parkRecord.LogNodes.Sort((a, b) => (int)(a.LogTime - b.LogTime).TotalMilliseconds);

                    if (orderRecord.NotifyTime.Year > 2000)
                    {
                        logNode = new LogNode();
                        logNode.LogNodeType = LogNodeType.NotifyPay;
                        logNode.ThreadName = orderRecord.ThreadName;
                        logNode.LogTime = orderRecord.NotifyTime;
                        logNode.Message = string.Format("{0},{1},下发支付通知，订单号={2}，金额={3}"
                            , "线上扫码", orderRecord.CredentialNo, orderRecord.OrderNo, orderRecord.TotalFee);
                        parkRecord.LogNodes.Add(logNode);
                        parkRecord.LogNodes.Sort((a, b) => (int)(a.LogTime - b.LogTime).TotalMilliseconds);
                    }

                    orderRecord.IsMatchOutRecord = true;
                }
            }
        }
        public static bool FuzzyCompare(string val1, string val2)
        {
            if (string.IsNullOrEmpty(val1) || string.IsNullOrEmpty(val2))
            {
                return false;
            }
            if (val1 == "未识别" || val2 == "未识别")
            {
                return false;
            }
            if (string.Compare(val1, val2, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            if (val1.Length != val2.Length || val1.Length <= 3)
            {
                return false;
            }

            int num = 0;
            for (int j = 0; j < val1.Length; j++)
            {
                if (val1[j] != val2[j])
                {
                    num++;
                    if (num > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        static bool ParseUpLoadParkOutRecord(RecordContext context, string line, List<string> lastLines)
        {
            if (line.Contains("UpLoadParkOutRecord"))
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);

                var newLine = line.Substring(line.IndexOf('{'));
                //prkCenterAuthOperation operation = JsonHelper.DeserializeObject<prkCenterAuthOperation>(newLine);
                prkParkOutRecord parkOutRecord = JsonHelper.DeserializeObject<prkParkOutRecord>(newLine);


                ParkRecord parkRecord = new ParkRecord();
                parkRecord.CredentialNo = parkOutRecord.credentialNo;
                if (string.IsNullOrEmpty(parkRecord.CredentialNo))
                    parkRecord.CredentialNo = parkOutRecord.carNo;
                if (string.IsNullOrEmpty(parkRecord.CredentialNo))
                    parkRecord.CredentialNo = "未识别";
                parkRecord.RecordId = parkOutRecord.recordId;
                parkRecord.DeviceId = parkOutRecord.deviceId.ToString();
                parkRecord.DeviceName = parkOutRecord.deviceName;
                parkRecord.EventTime = DateTime.Parse(parkOutRecord.time.time);
                parkRecord.IoType = parkOutRecord.ioType;
                parkRecord.Online = parkOutRecord.recordFlags.offlineFlag == 0 && parkOutRecord.recordFlags.extendFlag != 0;
                parkRecord.LogNodes = new List<LogNode>();

                if (!context.DeviceCache.Any(x => x.DeviceId == parkRecord.DeviceId))
                {
                    DeviceInfo deviceInfo = new DeviceInfo();
                    deviceInfo.DeviceId = parkRecord.DeviceId;
                    deviceInfo.DeviceName = parkRecord.DeviceName;
                    deviceInfo.IoType = parkOutRecord.ioType;
                    context.DeviceCache.Add(deviceInfo);
                }
                else
                {
                    var deviceInfo = context.DeviceCache.FirstOrDefault(x => x.DeviceId == parkRecord.DeviceId);
                    parkRecord.DeviceName = deviceInfo.DeviceName;
                }
                //看前面还有没有未处理的车
                ParkRecord lastParkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceId == parkOutRecord.deviceId.ToString());
                if (parkRecord.Online && lastParkRecord != null && !lastParkRecord.IsEnd)
                {
                    LogNode lastVehicleLogNode = new LogNode();
                    lastVehicleLogNode.LogNodeType = LogNodeType.Begin;
                    lastVehicleLogNode.ThreadName = threadName;
                    lastVehicleLogNode.LogTime = logTime;
                    lastVehicleLogNode.Message = string.Format("{0},{1},下位机上传新识别记录,{2}，覆盖当前车辆", lastParkRecord.DeviceName, lastParkRecord.CredentialNo, parkRecord.CredentialNo);
                    lastParkRecord.LogNodes.Add(lastVehicleLogNode);
                    lastParkRecord.LogNodes.Sort((a, b) => (int)(a.LogTime - b.LogTime).TotalMilliseconds);
                }

                LogNode logNode = new LogNode();
                logNode.LogNodeType = LogNodeType.Begin;
                logNode.ThreadName = threadName;
                logNode.LogTime = logTime;
                if (parkRecord.Online)
                {
                    logNode.Message = string.Format("{0},{1},下位机识别请求中心鉴权，识别时间{2}", parkRecord.DeviceName, parkRecord.CredentialNo, parkRecord.EventTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    if (parkOutRecord.eventType == 6)
                    {
                        logNode.Message = "手动开闸";
                    }
                    else
                    {
                        logNode.Message = string.Format("{0}下位机开闸，开闸时间{1}", parkRecord.CredentialNo, parkRecord.EventTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                parkRecord.LogNodes.Add(logNode);
                context.ParkRecords.Add(parkRecord);


                return true;
            }
            return false;
        }
        static bool ParseUpLoadParkInRecord(RecordContext context, string line, List<string> lastLines)
        {
            if (line.Contains("UpLoadParkInRecord"))
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);

                var newLine = line.Substring(line.IndexOf('{'));
                //prkCenterAuthOperation operation = JsonHelper.DeserializeObject<prkCenterAuthOperation>(newLine);
                prkParkInRecord parkInRecord = JsonHelper.DeserializeObject<prkParkInRecord>(newLine);
                ParkRecord parkRecord = new ParkRecord();
                parkRecord.CredentialNo = parkInRecord.credentialNo;
                if (string.IsNullOrEmpty(parkRecord.CredentialNo))
                    parkRecord.CredentialNo = parkInRecord.carNo;
                if (string.IsNullOrEmpty(parkRecord.CredentialNo))
                    parkRecord.CredentialNo = "未识别";
                parkRecord.RecordId = parkInRecord.recordId;
                parkRecord.DeviceId = parkInRecord.deviceId.ToString();
                parkRecord.DeviceName = parkInRecord.deviceName;
                parkRecord.EventTime = DateTime.Parse(parkInRecord.time.time);
                parkRecord.IoType = parkInRecord.ioType;
                parkRecord.Online = parkInRecord.recordFlags.offlineFlag == 0 && parkInRecord.recordFlags.extendFlag != 0;
                parkRecord.LogNodes = new List<LogNode>();

                if (!context.DeviceCache.Any(x => x.DeviceId == parkRecord.DeviceId))
                {
                    DeviceInfo deviceInfo = new DeviceInfo();
                    deviceInfo.DeviceId = parkRecord.DeviceId;
                    deviceInfo.DeviceName = parkRecord.DeviceName;
                    deviceInfo.IoType = parkInRecord.ioType;
                    context.DeviceCache.Add(deviceInfo);
                }
                else
                {
                    var deviceInfo = context.DeviceCache.FirstOrDefault(x => x.DeviceId == parkRecord.DeviceId);
                    parkRecord.DeviceName = deviceInfo.DeviceName;
                }
                //看前面还有没有未处理的车
                ParkRecord lastParkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceId == parkInRecord.deviceId.ToString());
                if (parkRecord.Online && lastParkRecord != null && !lastParkRecord.IsEnd)
                {
                    LogNode lastVehicleLogNode = new LogNode();
                    lastVehicleLogNode.LogNodeType = LogNodeType.Begin;
                    lastVehicleLogNode.ThreadName = threadName;
                    lastVehicleLogNode.LogTime = logTime;
                    lastVehicleLogNode.Message = string.Format("{0},{1},下位机上传新识别记录,{2}，覆盖当前车辆", lastParkRecord.DeviceName, lastParkRecord.CredentialNo, parkRecord.CredentialNo);
                    lastParkRecord.LogNodes.Add(lastVehicleLogNode);
                    lastParkRecord.LogNodes.Sort((a, b) => (int)(a.LogTime - b.LogTime).TotalMilliseconds);
                }

                LogNode logNode = new LogNode();
                logNode.LogNodeType = LogNodeType.Begin;
                logNode.ThreadName = threadName;
                logNode.LogTime = logTime;
                if (parkRecord.Online)
                {
                    logNode.Message = string.Format("{0},{1},下位机识别请求中心鉴权，识别时间{2}", parkRecord.DeviceName, parkRecord.CredentialNo, parkRecord.EventTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    if (parkInRecord.eventType == 6)
                    {
                        logNode.Message = "手动开闸";
                    }
                    else
                    {
                        logNode.Message = string.Format("{0}下位机已开闸，开闸时间{1}", parkRecord.CredentialNo, parkRecord.EventTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                parkRecord.LogNodes.Add(logNode);
                context.ParkRecords.Add(parkRecord);
                return true;
            }
            return false;
        }
        static bool ParseRequestPass(RecordContext context, string line, List<string> lastLines)
        {
            if (line.Contains("通行鉴权开始"))
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;
                int recordIdIndex = line.IndexOf(",recordId=");
                int recordIdStartIndex = recordIdIndex + ",recordId=".Length;
                int transIdIndex = line.IndexOf(",TransId=");
                int transIdStartIndex = transIdIndex + ",TransId=".Length;
                string credentialNo = line.Substring(credentialNoStartIndex, recordIdIndex - credentialNoStartIndex);
                string recordId = line.Substring(recordIdStartIndex, transIdIndex - recordIdStartIndex);
                string transId = line.Substring(transIdStartIndex);
                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.RecordId == recordId);

                if (parkRecord == null)
                {
                    parkRecord = new ParkRecord();
                    parkRecord.EventTime = logTime;
                    parkRecord.Online = true;
                    parkRecord.LogNodes = new List<LogNode>();

                    context.ParkRecords.Add(parkRecord);
                }
                else
                {
                    if (!string.IsNullOrEmpty(parkRecord.CredentialNo) && parkRecord.CredentialNo != credentialNo && !parkRecord.HistoryCredentialNo.Contains(parkRecord.CredentialNo))
                    {
                        parkRecord.HistoryCredentialNo.Add(parkRecord.CredentialNo);
                    }
                }
                parkRecord.CredentialNo = credentialNo;
                parkRecord.TransId = transId;
                parkRecord.RecordId = recordId;
                parkRecord.DeviceName = deviceName;
                #region 更新缓存的设备吧，设备信息东拼西凑，因为无法查数据库
                if (!string.IsNullOrEmpty(parkRecord.DeviceId))
                {
                    var device = context.DeviceCache.FirstOrDefault(x => x.DeviceId == parkRecord.DeviceId);
                    if (device != null)
                    {
                        device.DeviceName = deviceName;
                    }
                }
                #endregion
                if (!line.Contains("RecordMonitor线程"))
                {
                    //可能是云坐席纠正、关联场内记录、远程开闸
                    string adjustLog = lastLines.LastOrDefault(x => x.Contains("[" + threadName + "]")
                    && (x.Contains("云坐席2.0下发车牌矫正指令")
                    || x.Contains("云坐席2.0下发记录关联指令")
                    || x.Contains("智能平台下发数据【md.equip.action.equipoperate】")
                    || x.Contains("盒子中心鉴权请求开始，Cmd=ManualEnterOut")

                    ));
                    if (adjustLog != null)
                    {
                        DateTime adjustLogTime = DateTime.ParseExact(adjustLog.Substring(0, 23), "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                        if ((logTime - adjustLogTime).TotalSeconds < 10)
                        {
                            LogNode adjustLogNode = new LogNode();
                            adjustLogNode.LogNodeType = LogNodeType.CloudSeatAdjust;
                            adjustLogNode.ThreadName = threadName;
                            adjustLogNode.LogTime = adjustLogTime;
                            //如果是关联场内记录，解析场内记录guid
                            if (adjustLog.Contains("云坐席2.0下发记录关联指令"))
                            {
                                adjustLogNode.LogNodeType = LogNodeType.CloudSeatAdjustIn;
                                string strAdjustInData = adjustLog.Substring(adjustLog.IndexOf('{'));
                                CloudSeatDownCommandParam param = JsonHelper.DeserializeObject<CloudSeatDownCommandParam>(strAdjustInData);

                                if (!string.IsNullOrEmpty(param.inRecordId))
                                {
                                    adjustLogNode.Message = string.Format("{0},{1},云坐席指定入场记录，{2}", parkRecord.DeviceName, parkRecord.CredentialNo, param.inRecordId);
                                }
                                else
                                {
                                    adjustLogNode.Message = string.Format("{0},{1},云坐席指定入场时间，{2}", parkRecord.DeviceName, parkRecord.CredentialNo, param.inTime);
                                }

                            }
                            else if (adjustLog.Contains("云坐席2.0下发车牌矫正指令"))
                            {
                                adjustLogNode.Message = string.Format("{0},{1},云坐席纠正车牌", parkRecord.DeviceName, parkRecord.CredentialNo);

                            }
                            else if (adjustLog.Contains("智能平台下发数据【md.equip.action.equipoperate】"))
                            {
                                adjustLogNode.Message = string.Format("{0},{1}云坐席远程开闸", parkRecord.DeviceName, parkRecord.CredentialNo);
                                adjustLogNode.LogNodeType = LogNodeType.CloudRemoteOpenGate;

                            }
                            else if (adjustLog.Contains("盒子中心鉴权请求开始，Cmd=ManualEnterOut"))
                            {
                                string strManualEnterOutData = adjustLog.Substring(adjustLog.IndexOf('{'));
                                ManualEnterOutArgs args = JsonHelper.DeserializeObject<ManualEnterOutArgs>(strManualEnterOutData);
                                parkRecord.DeviceId = args.DeviceID;
                                adjustLogNode.Message = string.Format("{0},{1}盒子人工输车牌放行", parkRecord.DeviceName, parkRecord.CredentialNo);
                                adjustLogNode.LogNodeType = LogNodeType.ManualEnterOut;

                            }
                            parkRecord.LogNodes.Add(adjustLogNode);
                            parkRecord.LogNodes.Sort((a, b) => (int)(a.LogTime - b.LogTime).TotalMilliseconds);
                        }

                    }

                }

                LogNode logNode = new LogNode();
                logNode.LogNodeType = LogNodeType.RequestPass;
                logNode.ThreadName = threadName;
                logNode.LogTime = logTime;
                logNode.Message = string.Format("{0},{1}鉴权开始,RecordId={2},TransId={3}", parkRecord.DeviceName, parkRecord.CredentialNo, parkRecord.RecordId, parkRecord.TransId);
                parkRecord.LogNodes.Add(logNode);
                return true;
            }
            return false;
        }
        static bool ParseMatchEnterRecord(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 23:59:43,024 [RecordMonitor线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权:凭证=粤S8Q68S，匹配场内记录id=0B0BEE00200826184916000059C2,EnterTime=2020/8/26 18:49:16,CredentialNO=粤S8Q68S,IsVirtualRecord=False
            int flagIndex = line.IndexOf("匹配场内记录");
            if (flagIndex > 0)
            {
                if (!line.Contains("场内扫码"))
                {
                    string strLogTime = line.Substring(0, 23);
                    DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                    string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                    string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                    bool haveEnterRecord = line.EndsWith("IsVirtualRecord=False");
                    string message = string.Empty;
                    if (haveEnterRecord)
                        message = line.Substring(flagIndex);
                    else
                        message = "未获取到场内记录";
                    int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                    string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - 1 - credentialNoStartIndex);

                    ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                    if (parkRecord != null)
                    {
                        LogNode logNode = new LogNode();
                        logNode.LogNodeType = LogNodeType.MatchEnterRecord;
                        logNode.ThreadName = threadName;
                        logNode.LogTime = logTime;
                        logNode.Message = string.Format("{0},{1},{2}", deviceName, credentialNo, message);
                        parkRecord.LogNodes.Add(logNode);
                        return true;

                    }
                }

            }
            return false;
        }
        static bool ParseAddToDeviceQueue(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 07:39:14,895 [RecordMonitor线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权:AddToDeviceQueue,凭证=粤S386XC,队列数量=0
            int flagIndex = line.IndexOf("AddToDeviceQueue");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;
                int queueCountIndex = line.IndexOf(",队列数量=");
                string credentialNo = line.Substring(credentialNoStartIndex, queueCountIndex - credentialNoStartIndex);


                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.AddToDeviceQueue;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},鉴权完成，加入设备队列等待处理，前面还有{2}条记录", deviceName, credentialNo, line.Substring(line.LastIndexOf('=') + 1));
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseProcessOnlineBegin(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 14:55:11,226 [出口鉴权线程] INFO  CenterAuthLogger - 【出口】中心鉴权:ProcessOnline开始，凭证=粤B3N61U
            int flagIndex = line.IndexOf("ProcessOnline开始");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.ProcessOnlineBegin;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},从设备队列取出记录，作为当前服务车辆开始处理", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseShowMessage(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 12:10:57,063 [87] INFO  CenterAuthLogger - 【入口】中心鉴权：粤K257X5 ShowMessage:粤K257X5车场满位，请等待
            int flagIndex = line.IndexOf("ShowMessage:");
            if (flagIndex > 0 && line.IndexOf("上送事件") < 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf(":") + 1;
                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf(' ', credentialNoStartIndex) - credentialNoStartIndex);

                int showMessageIndex = flagIndex + "ShowMessage:".Length;
                string message = line.Substring(showMessageIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.ShowMessage;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},{2}", deviceName, credentialNo, message);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseConfirmOpenGateOut(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-06 09:05:39,078 [无人值守卫士鉴权线程] INFO  CenterAuthLogger - 【70捷EIII】中心鉴权：66F628B0 出口确认开闸,应收0.02,优惠0,实收0.02,计费详情:66F628B0月租套餐过期转临时凭证;
            int flagIndex = line.IndexOf("出口确认开闸");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("中心鉴权：") + "中心鉴权：".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - 1 - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.ConfirmOpenGateOut;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},{2}", deviceName, credentialNo, line.Substring(flagIndex));
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseConfirmOpenGateEnter(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-06 09:40:03,938 [70捷EIII鉴权线程] INFO  CenterAuthLogger - 【70捷EIII】中心鉴权：66F628B0 入口确认开闸
            int flagIndex = line.IndexOf("入口确认开闸");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("中心鉴权：") + "中心鉴权：".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - 1 - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.ConfirmOpenGateEnter;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},入口确认开闸", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseSuspendFlowBegin(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 14:54:49,158 [出口鉴权线程] INFO  CenterAuthLogger - 【出口】在线鉴权:暂停线程SuspendFlow开始，凭证=粤B3N61U,原因=AuthFaildWait
            //2020-08-26 14:57:41,897 [入口鉴权线程] INFO  CenterAuthLogger - 【入口】在线鉴权:暂停线程SuspendFlow开始，凭证=未识别,原因=WaitScanCode
            int flagIndex = line.IndexOf("暂停线程SuspendFlow开始");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf(',', credentialNoStartIndex) - credentialNoStartIndex);
                string reason = line.Substring(line.IndexOf("原因="));


                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.SuspendFlowBegin;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},暂停流程，{2}", deviceName, credentialNo, reason);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseSuspendFlowEnd(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 15:04:54,231 [96] INFO  CenterAuthLogger - 盒子中心鉴权请求开始，Cmd=UpLoadParkOutRecord,Data={"accountReceivable":0,"allBenefit":0,"allCharge":0,"allDerate":0,"billRecord":null,"boxId":2778698496,"carColor":"","carLogo":"","carNo":"粤B4Q81T","carNumColor":3,"carNumOrig":"粤B4Q81T","chargeType":0,"credentialNo":"","credentialType":163,"credibleDegree":1,"deviceId":189730304,"deviceName":"JSTC2401_189730304","eventType":1,"inRecordId":null,"ioType":2,"operatorNo":"1002","operatorTime":null,"pictureFilePath":["down/pic/20200826/park/189730304/150453_粤B4Q81T_BLUE_SB2.jpg","down/pic/20200826/park/189730304/150453_XT.bmp"],"recordFlags":{"extendFlag":1,"highModeFlag":0,"offlineFlag":0,"trusteeFlag":0},"recordId":"0B4F0E00200826150453000029BE","sealId":54,"sealName":"","shareOrderNo":null,"time":{"time":"2020-08-26 15:04:53","millisecond":0},"withHoldMoney":0,"chargeIdList":null,"sourceDeviceId":189730304,"haveCar":true,"guid":"038b4e2f-6045-45fd-9ef8-f7d047bb6dfd"}
            //2020-08-26 14:54:49,158 [出口鉴权线程] INFO  CenterAuthLogger - 【出口】在线鉴权:暂停线程SuspendFlow开始，凭证=粤B3N61U,原因=AuthFaildWait
            //2020-08-26 14:57:41,897 [入口鉴权线程] INFO  CenterAuthLogger - 【入口】在线鉴权:暂停线程SuspendFlow开始，凭证=未识别,原因=WaitScanCode
            int flagIndex = line.IndexOf("暂停线程SuspendFlow结束");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf(',', credentialNoStartIndex) - credentialNoStartIndex);
                string reason = line.Substring(line.IndexOf("原因="));


                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => !x.IsEnd && x.DeviceName == deviceName && x.CredentialNo == credentialNo);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.SuspendFlowEnd;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},恢复流程，{2}", deviceName, credentialNo, reason);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseRemoteEvent(RecordContext context, string line, List<string> lastLines)
        {

            int flagIndex = line.IndexOf("盒子中心鉴权请求开始，Cmd=RemoteEvent");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                //string deviceName = line.Substring(line.IndexOf('【')+1, line.IndexOf('】') - line.IndexOf('【')-1);

                RemoteEventArgs remoteEventArgs = JsonHelper.DeserializeObject<RemoteEventArgs>(line.Substring(line.IndexOf('{')));

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.TransId == remoteEventArgs.TransactionId);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.SuspendFlowEnd;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    if (remoteEventArgs.EventType == EnumFlowClientEventID.ConfirmOpenGate)
                    {
                        ConfirmOpenGateArgs confirmOpenGateArgs = JsonHelper.DeserializeObject<ConfirmOpenGateArgs>(remoteEventArgs.DataJson);
                        if (confirmOpenGateArgs.Action == 0)
                        {
                            logNode.Message = string.Format("{0},{1},收费框确定开闸", parkRecord.DeviceName, parkRecord.CredentialNo);
                            parkRecord.LogNodes.Add(logNode);
                        }
                        else if (confirmOpenGateArgs.Action == 1)
                        {
                            logNode.Message = string.Format("{0},{1},收费框取消开闸", parkRecord.DeviceName, parkRecord.CredentialNo);
                            parkRecord.LogNodes.Add(logNode);
                        }
                        else if (confirmOpenGateArgs.Action == 2)
                        {
                            logNode.Message = string.Format("{0},{1},收费框免费开闸", parkRecord.DeviceName, parkRecord.CredentialNo);
                            parkRecord.LogNodes.Add(logNode);
                        }
                        else if (confirmOpenGateArgs.Action == 3)
                        {
                            logNode.Message = string.Format("{0},{1},收费框欠费开闸", parkRecord.DeviceName, parkRecord.CredentialNo);
                            parkRecord.LogNodes.Add(logNode);
                        }
                    }
                    else if (remoteEventArgs.EventType == EnumFlowClientEventID.ChangePlate)
                    {
                        ChangePlateArgs changePlateArgs = JsonHelper.DeserializeObject<ChangePlateArgs>(remoteEventArgs.DataJson);
                        logNode.Message = string.Format("{0},{1},收费框纠正车牌为{2}", parkRecord.DeviceName, parkRecord.CredentialNo, changePlateArgs.NewPlate);
                        parkRecord.LogNodes.Add(logNode);
                    }
                    else if (remoteEventArgs.EventType == EnumFlowClientEventID.ChangeSetmeal)
                    {
                        ChangeSetMealArgs changeSetMealArgs = JsonHelper.DeserializeObject<ChangeSetMealArgs>(remoteEventArgs.DataJson);
                        logNode.Message = string.Format("{0},{1},收费框变更套餐为{2}", parkRecord.DeviceName, parkRecord.CredentialNo, changeSetMealArgs.SetMealNo);
                        parkRecord.LogNodes.Add(logNode);
                    }
                    else if (remoteEventArgs.EventType == EnumFlowClientEventID.ConfirmPictureWall)
                    {
                        ConfirmPictureWallArgs confirmPictureWallArgs = JsonHelper.DeserializeObject<ConfirmPictureWallArgs>(remoteEventArgs.DataJson);
                        if (confirmPictureWallArgs.Action == 0)
                        {
                            if (confirmPictureWallArgs.SelectEnterTime.Year > 2000)
                            {

                                logNode.Message = string.Format("{0},{1},图片墙指定入场时间{2}", parkRecord.DeviceName, parkRecord.CredentialNo, confirmPictureWallArgs.SelectEnterTime.ToString("yyyy-MM-dd HH:mm:ss"));
                                parkRecord.LogNodes.Add(logNode);
                            }
                            else
                            {
                                logNode.Message = string.Format("{0},{1},图片墙指定入场记录{2}", parkRecord.DeviceName, parkRecord.CredentialNo, confirmPictureWallArgs.EnterRecordId);
                                parkRecord.LogNodes.Add(logNode);

                            }
                        }
                    }
                    return true;
                }

            }
            return false;
        }
        static bool ParseBillChanged(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-16 23:42:51,016 [粤S3B0W6反查线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权:订单状态更改，自动开闸，凭证=粤S3B0W6
            int flagIndex = line.IndexOf("订单状态更改，自动开闸");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.BillChanged;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},订单状态更改，自动开闸", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseOpenGateBegin(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 14:18:11,882 [广场出口鉴权线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权：发送业务事件，凭证=粤S782ZU，事件类型=OpenGateBegin，发送耗时=0ms
            int flagIndex = line.IndexOf("，事件类型=OpenGateBegin");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.OpenGateBegin;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},发送开闸", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseOpenGateSuccess(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 14:18:11,911 [广场出口鉴权线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权：发送业务事件，凭证=粤S782ZU，事件类型=OpenGateSuccess，发送耗时=0ms
            int flagIndex = line.IndexOf("，事件类型=OpenGateSuccess");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.OpenGateSuccess;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},开闸成功", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseOpenGateFailed(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 14:18:11,911 [广场出口鉴权线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权：发送业务事件，凭证=粤S782ZU，事件类型=OpenGateFailed，发送耗时=0ms
            int flagIndex = line.IndexOf("，事件类型=OpenGateFailed");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.OpenGateFailed;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},开闸失败", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseSaveBill(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 16:59:56,047 [出口鉴权线程] INFO  CenterAuthLogger - 【出口】中心鉴权：凭证=粤B456NF 保存收费账单，付款方式=微信
            int flagIndex = line.IndexOf(" 保存收费账单");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.SaveBill;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},{2}", deviceName, credentialNo, line.Substring(flagIndex + 1));
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseSaveRecord(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 13:45:23,595 [广场出口鉴权线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权：凭证=粤S1E1Z5 保存出入场记录
            int flagIndex = line.IndexOf(" 保存出入场记录");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                if (!threadName.Contains("鉴权线程"))
                {
                    return false;
                }

                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.SaveRecord;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},保存出入场记录", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseVehicleBack(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 15:00:11,521 [20] INFO  CenterAuthLogger - 【出口】中心鉴权:车辆倒车，取消记录，凭证=粤B3N61U
            int flagIndex = line.IndexOf("车辆倒车，取消记录");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);


                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.ParseVehicleBack;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},车辆倒车，取消记录", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseEndDeviceFlowProcess(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 13:45:23,629 [广场出口鉴权线程] INFO  CenterAuthLogger - 【广场出口】中心鉴权:EndDeviceFlowProcess，凭证=粤S1E1Z5,PassStatus={"CreateTime":"2020-08-27 13:45:23","EndTime":"9999-12-31 23:59:59","AddDeviceQueueTime":"2020-08-27 13:45:23","HasOpenGate":true,"HasLeave":false,"MaybeLeave":false,"HasCancel":false,"HasCloudPaySuccess":false,"OpenGateTime":"2020-08-27 13:45:23","LeaveCheckStartTime":"0001-01-01 00:00:00","LeaveCheckEndTime":"0001-01-01 00:00:00","DGStartTime":"0001-01-01 00:00:00","DGEndTime":"0001-01-01 00:00:00","LastDGTime":"2020-08-27 13:45:23","HasSoundWillLeaveWarning":false,"RecordId":"0B0A12002008271345220000634D","CredentialNo":"粤S1E1Z5","DeviceId":185209344,"QueueDeviceId":185209344,"AuthResult":0,"AccountReceivable":0.0,"UseFlowOptimize":false,"WaitSecondsBeforeDG":4}
            int flagIndex = line.IndexOf("EndDeviceFlowProcess");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                if (!threadName.Contains("鉴权线程"))
                {
                    return false;
                }

                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf(',', credentialNoStartIndex) - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => !x.IsEnd && x.DeviceName == deviceName && x.CredentialNo == credentialNo);
                if (parkRecord != null)
                {
                    parkRecord.IsEnd = true;
                    parkRecord.EndTime = logTime;
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.EndDeviceFlowProcess;
                    logNode.ThreadName = threadName;
                    logNode.LogTime = logTime;
                    logNode.Message = string.Format("{0},{1},结束流程", deviceName, credentialNo);
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }

            }
            return false;
        }
        static bool ParseCloudSeatBegin(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 11:43:27,271 [70] INFO  CenterAuthLogger - 云坐席2.0进入处理：{"EventId":null,"DeviceId":"187708416","SourceDeviceId":"187708416","TalkDeviceId":null,"VoipId":"p200520774_187708416","VoipName":null,"SpeakTecType":2,"Source":2,"EventReson":null,"EventResonCode":0,"EventResonCodeStr":"None","EventLevel":0,"Picture":"down/pic/20200826/park/187708416/114254_粤BV6Z85_BLUE_SB2.jpg","OutRecordId":null,"RecordId":null,"Plate":null,"PlateType":null,"SealName":null,"PlateColor":0,"EventTime":"0001-01-01 00:00:00","EventType":null,"OutSeatInfo":null,"TeleNo":null,"PersonName":null,"ImageType":0,"Seconds":40,"HasSound":false}
            int flagIndex = line.IndexOf("云坐席2.0进入处理");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                //string deviceName = line.Substring(line.IndexOf('【')+1, line.IndexOf('】') - line.IndexOf('【')-1);


                int sourceStartIndex = line.IndexOf("Source\":") + "Source\":".Length;
                string source = line.Substring(sourceStartIndex, line.IndexOf(',', sourceStartIndex) - sourceStartIndex);

                int deviceIdStartIndex = line.IndexOf("DeviceId\":\"") + "DeviceId\":\"".Length;
                string deviceId = line.Substring(deviceIdStartIndex, line.IndexOf('\"', deviceIdStartIndex) - deviceIdStartIndex);
                if (source == "1" || source == "2" || line.Contains("鉴权线程"))
                {
                    ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceId == deviceId && !x.IsEnd);
                    if (parkRecord != null)
                    {
                        LogNode logNode = new LogNode();
                        logNode.LogNodeType = LogNodeType.CloudSeatBegin;
                        logNode.ThreadName = threadName;
                        logNode.LogTime = logTime;
                        if (source == "1")
                        {
                            logNode.Message = string.Format("{0},{1},{2}", parkRecord.DeviceName, parkRecord.CredentialNo, "按钮呼叫云坐席");
                        }
                        else if (source == "2")
                        {
                            logNode.Message = string.Format("{0},{1},{2}", parkRecord.DeviceName, parkRecord.CredentialNo, "压地感触发云坐席");
                        }
                        else
                        {
                            logNode.Message = string.Format("{0},{1},{2}", parkRecord.DeviceName, parkRecord.CredentialNo, "软件自动触发对讲");
                        }
                        parkRecord.LogNodes.Add(logNode);
                        return true;
                    }
                }


            }
            return false;
        }
        static bool ParseCloudRemoteOpenGate(RecordContext context, string line, List<string> lastLines)
        {
            //2020-09-02 15:58:44,325 [163] INFO  CenterAuthLogger - 云坐席2.0开闸指令RemoteOpenGate：{"command":1,"deviceId":167388928,"from":7,"operateMode":0,"operatorNo":"","operatorName":null,"personNo":"","trascationId":"69025c0a-b49c-4d20-813c-3e9e1621b85d","isWebboxMode":false,"credentialNo":null,"extend":null,"seqId":null}
            //2020-09-02 15:58:44,328 [163] INFO  CenterAuthLogger - 【出口】中心鉴权：CustomClientEvent,事件类型=收费框放行操作 ret.code=0,ret.msg=,args={"TransactionId":"703c2a20-f938-464d-8673-314ce310f521","EventType":0,"Data":{"Action":4,"PayTypeId":0,"Remark":null,"CancelReason":0,"OperatorNo":null,"OperatorID":null,"OperatorName":null,"Discount":null,"CapturePath":null,"EventType":5,"Extensions":{}},"DataJson":null}
            int flagIndex = line.IndexOf("事件类型=收费框放行操作");
            if (flagIndex > 0)
            {
                //string strLogTime = line.Substring(0, 23);
                //DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                for (int i = lastLines.Count - 1, j = 0; i >= 0 && j < 10; --i, ++j)
                {
                    var lastLine = lastLines[i];
                    if (lastLine.Contains("[" + threadName + "] INFO  CenterAuthLogger - 云坐席2.0开闸指令RemoteOpenGate"))
                    {
                        string strLogTime = lastLine.Substring(0, 23);
                        DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                        ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                        if (parkRecord != null)
                        {
                            parkRecord.IsEnd = true;
                            parkRecord.EndTime = logTime;
                            LogNode logNode = new LogNode();
                            logNode.LogNodeType = LogNodeType.CloudRemoteOpenGate;
                            logNode.ThreadName = threadName;
                            logNode.LogTime = logTime;
                            logNode.Message = string.Format("{0},{1},云坐席远程开闸", deviceName, parkRecord.CredentialNo);
                            parkRecord.LogNodes.Add(logNode);
                            parkRecord.LogNodes.Sort((a, b) => (int)(a.LogTime - b.LogTime).TotalMilliseconds);
                            return true;
                        }

                    }
                }
            }
            return false;
        }
        static bool ParseReceiveFacePay(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 16:59:24,842 [193] INFO  CommonLogger - 下位机当面付记录进入盒子，data={"chargeType":1,"payCode":"134956884176121062","signature":null,"transactionId":"0B0A1200200827165920000063B7","cashInfo":null,"deviceId":0}
            int flagIndex = line.IndexOf("下位机当面付记录进入盒子，data=");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);

                string strData = line.Substring(flagIndex + "下位机当面付记录进入盒子，data=".Length);
                prkPayCode payCode = JsonHelper.DeserializeObject<prkPayCode>(strData);
                if (payCode != null && !string.IsNullOrEmpty(payCode.transactionId))
                {
                    ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.RecordId == payCode.transactionId);
                    if (parkRecord != null)
                    {
                        LogNode logNode = new LogNode();
                        logNode.LogNodeType = LogNodeType.ReceiveFacePay;
                        logNode.ThreadName = threadName;
                        logNode.LogTime = logTime;
                        if (payCode.chargeType == 2)
                        {
                            logNode.Message = string.Format("{0},{1},接收到投币记录", parkRecord.DeviceName, parkRecord.CredentialNo);
                        }
                        else
                        {
                            if (payCode.payCode.StartsWith("http"))
                            {
                                logNode.Message = string.Format("{0},{1},下位机当面付记录进入盒子,付款码错误，扫到了车身上的二维码={2}", parkRecord.DeviceName, parkRecord.CredentialNo, payCode.payCode);
                            }
                            else
                            {
                                logNode.Message = string.Format("{0},{1},下位机当面付记录进入盒子,付款码={2}", parkRecord.DeviceName, parkRecord.CredentialNo, payCode.payCode);
                            }
                        }
                        parkRecord.LogNodes.Add(logNode);
                        return true;
                    }
                }


            }
            return false;
        }
        static bool ParseFacePayBegin(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 08:52:27,195 [217] INFO  CenterAuthLogger - 【广场出口】中心鉴权:凭证=粤SN22M7,云平台当面付请求
            int flagIndex = line.IndexOf("当面付请求");
            if (line.EndsWith("当面付请求"))
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.FacePayBegin;
                    logNode.ThreadName = threadName;
                    logNode.Message = string.Format("{0},{1},{2}", parkRecord.DeviceName, parkRecord.CredentialNo, line.Substring(line.LastIndexOf(",") + 1));
                    logNode.LogTime = logTime;
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }
            }
            return false;
        }
        static bool ParseFacePayTimeOut(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-17 17:47:39,958 [74] INFO  XMPPLogger - ======03.CloudRequest异常====System.Net.WebException: 远程服务器返回错误: (404) 未找到。
            //2020-08-20 16:13:34,435 [429] INFO  XMPPLogger - ======03.CloudRequest异常====System.Net.WebException: 无法连接到远程服务器 ---> System.Net.Sockets.SocketException: 由于连接方在一段时间后没有正确答复或连接的主机没有反应，连接尝试失败。 112.74.142.211:80
            int flagIndex = line.IndexOf("03.CloudRequest异常");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                //string deviceName = line.Substring(line.IndexOf('【')+1, line.IndexOf('】') - line.IndexOf('【')-1);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.LogNodes.Count > 0 && x.LogNodes.Last().LogNodeType == LogNodeType.FacePayBegin && x.LogNodes.Last().ThreadName == threadName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.FacePayTimeOut;
                    logNode.ThreadName = threadName;
                    logNode.Message = string.Format("{0},{1},当面付失败,{2}", parkRecord.DeviceName, parkRecord.CredentialNo, line.Substring(flagIndex));
                    logNode.LogTime = logTime;
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }
            }
            return false;
        }
        static bool ParseFacePayReturn(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 08:52:28,289 [217] INFO  XMPPLogger - =====02.向云平台请求返回结果：{"bankType":"支付宝","msg":"当面付付款成功","orderNo":"BK200827085227195p20062193626474","payFrom":"jieshun","payTime":"2020-08-27 08:52:29","payType":"ZFB","result":"0","status":"0","time":"2020-08-27 08:52:28","transactionId":"2020082722001435231456143769"}
            //2020-08-27 08:53:16,910 [粤SG97V5反查线程] INFO  XMPPLogger - =====02.向云平台请求返回结果：{"msg":"查询成功","orderNo":"BK200827085312187p20062193611359","result":"0","status":"-1","time":"2020-08-27 08:53:17"}
            int flagIndex = line.IndexOf("02.向云平台请求返回结果：");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                //string deviceName = line.Substring(line.IndexOf('【')+1, line.IndexOf('】') - line.IndexOf('【')-1);
                if (!threadName.Contains("反查线程"))
                {
                    ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.LogNodes.Count > 0 && x.LogNodes.Last().LogNodeType == LogNodeType.FacePayBegin && x.LogNodes.Last().ThreadName == threadName);
                    if (parkRecord != null)
                    {
                        LogNode logNode = new LogNode();
                        logNode.LogNodeType = LogNodeType.FacePayReturn;
                        logNode.ThreadName = threadName;
                        logNode.Message = string.Format("{0},{1},当面付平台返回,{2}", parkRecord.DeviceName, parkRecord.CredentialNo, line.Substring(flagIndex) + "02.向云平台请求返回结果：".Length);
                        logNode.LogTime = logTime;
                        parkRecord.LogNodes.Add(logNode);
                        return true;
                    }
                }

            }
            return false;
        }
        static bool ParseFacePayEnd(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-16 20:10:05,124 [62] INFO  CenterAuthLogger - 【广场出口】中心鉴权:凭证=云MFS582,云平台当面付返回失败,errorCode=,errorMsg=支付失败,网络异常
            //2020-08-16 11:15:01,093 [16] INFO  CenterAuthLogger - 【广场出口】中心鉴权:凭证=粤S1EL31,云平台当面付返回,errorCode=,errorMsg=,payType=WX
            int flagIndex = line.IndexOf("当面付返回");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf(',', credentialNoStartIndex) - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.FacePayEnd;
                    logNode.ThreadName = threadName;
                    logNode.Message = string.Format("{0},{1},{2}", parkRecord.DeviceName, credentialNo, line.Substring(flagIndex));
                    logNode.LogTime = logTime;
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }
            }
            return false;
        }
        static bool ParseFacePayEnd2(RecordContext context, string line, List<string> lastLines)
        {
            //2020-09-02 19:34:13,016 [71] ERROR CenterAuthLogger - 【左出口】中心鉴权:当面付请求失败,已检测车辆已离开,凭证=川A495M8
            int flagIndex = line.IndexOf("当面付请求失败,已检测车辆已离开");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;


                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.FacePayEnd;
                    logNode.ThreadName = threadName;
                    logNode.Message = string.Format("{0},{1},{2}", parkRecord.DeviceName, parkRecord.CredentialNo, "当面付失败，超过15分钟认为车辆已离开");
                    logNode.LogTime = logTime;
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }
            }
            return false;
        }
        static bool ParseNoPlateScanCode(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 15:06:07,093 [41] INFO  CenterAuthLogger - 【出口】中心鉴权：CustomClientEvent,事件类型=无牌车扫码入出场 ret.code=0,ret.msg=操作成功,args={"TransactionId":"57687906-f94a-45aa-a0ae-64c574dd5516","EventType":8,"Data":{"VoucherNo":"ON61LXJH5E_U9HNBLOWUV7V8O4Q","Plate":""},"DataJson":null}
            int flagIndex = line.IndexOf("事件类型=无牌车扫码入出场");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                string deviceName = line.Substring(line.IndexOf('【') + 1, line.IndexOf('】') - line.IndexOf('【') - 1);

                int credentialNoStartIndex = line.IndexOf("VoucherNo\":\"") + "VoucherNo\":\"".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf('"', credentialNoStartIndex) - credentialNoStartIndex);

                ParkRecord parkRecord = context.ParkRecords.LastOrDefault(x => x.DeviceName == deviceName);
                if (parkRecord != null)
                {
                    LogNode logNode = new LogNode();
                    logNode.LogNodeType = LogNodeType.NoPlateScanCode;
                    logNode.ThreadName = threadName;
                    logNode.Message = string.Format("{0},{1},无牌车扫码完成", parkRecord.DeviceName, credentialNo);
                    logNode.LogTime = logTime;
                    parkRecord.LogNodes.Add(logNode);
                    return true;
                }
            }
            return false;
        }
        static bool ParseJsPayOrder(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 23:58:23,657 [174] INFO  XMPPLogger - 智能平台下发数据【md.jspay.order】=====：{"attributes":{"projectCode":"p200621936","__jht_orig_req_id":null},"dataItems":[{"attributes":{"goodsName":"JSPAY","parkCode":"p200621936","SUBSYSTEM_CODE":"p200621936","carNo":"皖-S99539","URL":"http://127.0.0.1/JSTPay/BookSearchByCarNO.aspx?attach=&gcode_id=p200621936&goods_name=JSPAY&input_charset=GBK&member_no=&mer_gid=%CD%EE-S99539&partner=000000008021041&service_version=1.0&sign_type=MD5&sign=F80218F6A15CD650C858C82CA2EDFE56","cardId":"皖-S99539","partner":"000000008021041"},"objectId":"","operateType":"READ","subItems":[]}],"requestType":"DIRECTIVE","seqId":"36fa915b0a494879b7fa4c3a483b76f3","serviceId":"md.jspay.order","source":""}
            int flagIndex = line.IndexOf("智能平台下发数据【md.jspay.order】=====：");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                //string deviceName = line.Substring(line.IndexOf('【')+1, line.IndexOf('】') - line.IndexOf('【')-1);

                int credentialNoStartIndex = line.IndexOf("\"carNo\":\"") + "\"carNo\":\"".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf('\"', credentialNoStartIndex) - credentialNoStartIndex).Replace("-", "");

                int seqIdStartIndex = line.LastIndexOf("seqId\":\"") + "seqId\":\"".Length;
                string seqId = line.Substring(seqIdStartIndex, line.IndexOf('\"', seqIdStartIndex) - seqIdStartIndex);

                JspayOrderRecord orderRecord = new JspayOrderRecord();
                orderRecord.MiddleValue = credentialNo;
                orderRecord.ThreadName = threadName;
                orderRecord.SeqId = seqId;
                orderRecord.ReceiveTime = logTime;
                context.OrderRecords.Add(orderRecord);
            }
            return false;
        }
        static bool ParseOrderMatchEnterRecord(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 23:58:23,705 [174] INFO  CenterAuthLogger - 【场内扫码】中心鉴权:凭证=皖S99539，匹配场内记录id=20082623582370592580,EnterTime=2020/8/26 23:58:23,CredentialNO=皖S99539,IsVirtualRecord=True
            int flagIndex = line.IndexOf("匹配场内记录");
            if (flagIndex > 0)
            {

                if (line.Contains("场内扫码"))
                {
                    string strLogTime = line.Substring(0, 23);
                    DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                    string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                    //string deviceName = line.Substring(line.IndexOf('【')+1, line.IndexOf('】') - line.IndexOf('【')-1);
                    string enterRecordId = line.Substring(flagIndex + 9, line.IndexOf(',', flagIndex) - (flagIndex + 9));
                    bool haveEnterRecord = line.EndsWith("IsVirtualRecord=False");
                    string message = string.Empty;
                    if (haveEnterRecord)
                        message = line.Substring(flagIndex);
                    else
                        message = "未获取到场内记录";
                    int credentialNoStartIndex = line.IndexOf("凭证=") + "凭证=".Length;

                    string credentialNo = line.Substring(credentialNoStartIndex, flagIndex - 1 - credentialNoStartIndex);

                    if (!string.IsNullOrEmpty(threadName))
                    {
                        JspayOrderRecord orderRecord = context.OrderRecords.LastOrDefault(x => x.ThreadName == threadName && (logTime - x.ReceiveTime).TotalSeconds < 30);
                        if (orderRecord != null)
                        {
                            orderRecord.EnterRecordId = enterRecordId;
                            orderRecord.HaveEnterRecord = haveEnterRecord;

                        }

                    }

                }

            }
            return false;
        }
        static bool ParseOrderReturn(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-26 23:58:23,709 [174] INFO  XMPPLogger - 【md_jspay_order】返回数据给智能平台:{"attributes":{"AUTH_CODE":""},"dataItems":[{"attributes":{"deductFee":0,"goodsTitle":"停车费用","totalFee":0,"surplusMinute":"0","serviceFeeTime":1,"carLocation":"","serialNo":"20082623582369296822","invoiceReceivingPlace":"","serviceFee":0,"couponlist":"","errorCode":1002,"parkName":"东莞中达商业广场","partner":"000000008021041","inImage":"","inCarImageId":"","overtimeChargeFlag":"0","parkId":"06e1e972bdd211eabdf00c9d92138bf4","couponNum":"","integralRule":"","attach":"","memberKey":"皖-S99539","carNo":"皖-S99539","discountFee":0,"merGid":"皖-S99539","createTime":"2020-08-26 23:58:23","serviceStime":"2020-08-26 23:58:23","policyMinute":"0","serviceEtime":"2020-08-26 23:58:23","otherFee":0,"freeMinute":"0","parkCode":"p200621936","chargeStandardDesc":"","errorMessage":"未入场","curType":1,"outTradeNo":"BK200826235823709p20062193616472","serviceExpire":""},"objectId":"md.jspay.order","operateType":"READ","subItems":[]}],"message":"下发成功","resultCode":0,"seqId":"36fa915b0a494879b7fa4c3a483b76f3","serviceId":"md.jspay.order","source":""}
            //2020-08-27 10:07:53,452 [174] INFO  XMPPLogger - 【md_jspay_order】返回数据给智能平台:{"attributes":{"AUTH_CODE":""},"dataItems":[{"attributes":{"deductFee":0,"goodsTitle":"停车费用","totalFee":0,"surplusMinute":"0","serviceFeeTime":4350,"carLocation":"","serialNo":"20082710075288219651","invoiceReceivingPlace":"","serviceFee":600,"couponlist":"","errorCode":1008,"parkName":"东莞中达商业广场","partner":"000000008021041","inImage":"bd335479234f493a80ee72d2dc046f06","inCarImageId":"","overtimeChargeFlag":"0","parkId":"06e1e972bdd211eabdf00c9d92138bf4","couponNum":"","integralRule":"","attach":"","memberKey":"粤-S0J785","carNo":"粤-S0J785","discountFee":600,"merGid":"粤-S0J785","createTime":"2020-08-27 10:07:53","serviceStime":"2020-08-27 08:55:23","policyMinute":"60","serviceEtime":"2020-08-27 10:07:53","otherFee":0,"freeMinute":"15","parkCode":"p200621936","chargeStandardDesc":"免费时长(分钟) 30,是否包含免费时长 true,免费时长是否循环 false,连续停放24小时最高收费(元) 0,第1时段开始时间(不填表示不启动该段时间) 00:00,第1时段结束时间(不填表示不启动该段时间) +00:00,第1时段跨段拆分方式 false,第1时段首时段长度(小时)(0表示没有首时段) 1,第1时段首时段是否循环 false,第1时段首时段收费-计费单位 hour,第1时段首时段收费-计费基数 1,第1时段首时段收费 5,第1时段首时段后收费-计费单位 hour,第1时段首时段后收费-计费基数 1,第1时段首时段后收费 1,第2时段开始时间(不填表示不启动该段时间) ,第2时段结束时间(不填表示不启动该段时间) ,第2时段跨段拆分方式 false,第2时段收费-计费单位 hour,第2时段收费-计费基数 1,第2时段收费 0,","errorMessage":"在打折减免时间内","curType":1,"outTradeNo":"BK200827100753409p20062193628656","serviceExpire":""},"objectId":"md.jspay.order","operateType":"READ","subItems":[]}],"message":"下发成功","resultCode":0,"seqId":"f37fec2eea7d44e68e8d80951a12a1f9","serviceId":"md.jspay.order","source":""}
            int flagIndex = line.IndexOf("【md_jspay_order】返回");
            if (flagIndex > 0 && line.LastIndexOf("totalFee\":") >= 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);

                int seqIdStartIndex = line.LastIndexOf("seqId\":\"") + "seqId\":\"".Length;
                string seqId = line.Substring(seqIdStartIndex, line.IndexOf('\"', seqIdStartIndex) - seqIdStartIndex);

                int orderNoStartIndex = line.LastIndexOf("outTradeNo\":\"") + "outTradeNo\":\"".Length;
                string orderNo = line.Substring(orderNoStartIndex, line.IndexOf('\"', orderNoStartIndex) - orderNoStartIndex);

                int totalFeeStartIndex = line.LastIndexOf("totalFee\":") + "totalFee\":".Length;
                string strTotalFee = line.Substring(totalFeeStartIndex, line.IndexOf(',', totalFeeStartIndex) - totalFeeStartIndex);
                double totalFee = double.Parse(strTotalFee) / 100.0d;

                int errorCodeStartIndex = line.LastIndexOf("errorCode\":") + "errorCode\":".Length;
                string errorCode = line.Substring(errorCodeStartIndex, line.IndexOf('\"', errorCodeStartIndex) - errorCodeStartIndex);

                int errorMessageStartIndex = line.LastIndexOf("errorMessage\":\"") + "errorMessage\":\"".Length;
                string errorMessage = line.Substring(errorMessageStartIndex, line.IndexOf('\"', errorMessageStartIndex) - errorMessageStartIndex);

                int credentialNoStartIndex = line.IndexOf("carNo\":\"") + "carNo\":\"".Length;
                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf('\"', credentialNoStartIndex) - credentialNoStartIndex).Replace("-", "");


                JspayOrderRecord orderRecord = context.OrderRecords.LastOrDefault(x => x.SeqId == seqId);
                if (orderRecord != null)
                {
                    orderRecord.OrderNo = orderNo;
                    orderRecord.TotalFee = totalFee;
                    orderRecord.ErrorCode = errorCode;
                    orderRecord.ErrorMessage = errorMessage;
                    orderRecord.CredentialNo = credentialNo.Replace("-", "");
                }

            }
            return false;
        }
        static bool ParseGetOrder(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 22:32:51,191 [174] INFO  XMPPLogger - 智能平台下发数据【md.park.getOrder】=====：{"attributes":{"projectCode":"p200621936"},"dataItems":[{"attributes":{"credentialNo":"无-SPT25J","parkCode":"p200621936","equipCode":"185209344","credentialType":"180"},"objectId":"","operateType":"READ","subItems":[]}],"requestType":"DIRECTIVE","seqId":"ba160aadef0c4e50a02f7230c9a235d3","serviceId":"md.park.getOrder","source":""}
            int flagIndex = line.IndexOf("智能平台下发数据【md.park.getOrder】=====：");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);
                //string deviceName = line.Substring(line.IndexOf('【')+1, line.IndexOf('】') - line.IndexOf('【')-1);

                int credentialNoStartIndex = line.IndexOf("\"credentialNo\":\"") + "\"credentialNo\":\"".Length;

                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf('\"', credentialNoStartIndex) - credentialNoStartIndex).Replace("-", "");

                int seqIdStartIndex = line.LastIndexOf("seqId\":\"") + "seqId\":\"".Length;
                string seqId = line.Substring(seqIdStartIndex, line.IndexOf('\"', seqIdStartIndex) - seqIdStartIndex);

                JspayOrderRecord orderRecord = new JspayOrderRecord();
                orderRecord.MiddleValue = credentialNo;
                orderRecord.ThreadName = threadName;
                orderRecord.SeqId = seqId;
                orderRecord.ReceiveTime = logTime;
                context.OrderRecords.Add(orderRecord);
            }
            return false;
        }
        //static bool ParseOrderMatchCurrentRecord(RecordContext context, string line, List<string> lastLines)
        //{
        //    //2020-08-27 22:32:49,471 [174] INFO  CenterAuthLogger - 扫码免输入车牌,当前设备185209344 识别车牌号码 无SPT25J
        //    //2020 - 08 - 27 22:32:49,504[174] INFO CenterAuthLogger -扫码缴费找到当前出口的服务车辆：无SPT25J
        //    int flagIndex = line.IndexOf("扫码缴费找到当前出口的服务车辆：");
        //    if (flagIndex > 0)
        //    {
        //        string strLogTime = line.Substring(0, 23);
        //        DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
        //        string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);

        //        //int flagStartIndex = flagIndex + "扫码缴费找到当前出口的服务车辆：".Length;
        //        //string deviceId = line.Substring(flagStartIndex, line.IndexOf(" 识别") - flagStartIndex);

        //        JspayOrderRecord orderRecord = context.OrderRecords.LastOrDefault(x => x.ThreadName == threadName && (logTime - x.ReceiveTime).TotalSeconds < 30);
        //        if (orderRecord != null)
        //        {
        //            //orderRecord.DeviceId = deviceId;
        //            orderRecord.IsOutScanCode = true;
        //        }
        //    }
        //    return false;
        //}
        static bool ParseGetOrderReturn(RecordContext context, string line, List<string> lastLines)
        {
            //2020-08-27 22:32:51,259 [174] INFO  XMPPLogger - 【md_park_getOrder】返回数据给智能平台:{"attributes":{"AUTH_CODE":""},"dataItems":[{"attributes":{"isMonth":0,"serviceFee":"1200","overtimeChargeFlag":"0","errorCode":0,"parkName":"东莞中达商业广场","endTime":"2020-08-27 22:32:41","errorMessage":"下发成功","parkId":"06e1e972bdd211eabdf00c9d92138bf4","surplusMinute":"0","parkCode":"p200621936","discountFee":"0","feeTimes":"28558","credentialNo":"无-SPT25J","startTime":"2020-08-27 14:36:43","freeMinute":"0","inCarImageId":"","serialNo":"20082722324950464279","orderNo":"BK200827223251230p20062193617363","totalFee":"1200"},"objectId":"md.park.getOrder","operateType":"READ","subItems":[]}],"message":"下发成功","resultCode":0,"seqId":"ba160aadef0c4e50a02f7230c9a235d3","serviceId":"md.park.getOrder","source":""} 
            int flagIndex = line.IndexOf("【md_park_getOrder】返回");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);

                int seqIdStartIndex = line.LastIndexOf("seqId\":\"") + "seqId\":\"".Length;
                string seqId = line.Substring(seqIdStartIndex, line.IndexOf('\"', seqIdStartIndex) - seqIdStartIndex);

                int orderNoStartIndex = line.LastIndexOf("orderNo\":\"") + "orderNo\":\"".Length;
                string orderNo = line.Substring(orderNoStartIndex, line.IndexOf('\"', orderNoStartIndex) - orderNoStartIndex);
                if (string.IsNullOrEmpty(orderNo))
                {
                    return false;
                }
                int totalFeeStartIndex = line.LastIndexOf("totalFee\":\"") + "totalFee\":\"".Length;
                string strTotalFee = line.Substring(totalFeeStartIndex, line.IndexOf('\"', totalFeeStartIndex) - totalFeeStartIndex);
                double totalFee = double.Parse(strTotalFee) / 100.0d;

                int errorCodeStartIndex = line.LastIndexOf("errorCode\":") + "errorCode\":".Length;
                string errorCode = line.Substring(errorCodeStartIndex, line.IndexOf('\"', errorCodeStartIndex) - errorCodeStartIndex);

                int errorMessageStartIndex = line.LastIndexOf("errorMessage\":\"") + "errorMessage\":\"".Length;
                string errorMessage = line.Substring(errorMessageStartIndex, line.IndexOf('\"', errorMessageStartIndex) - errorMessageStartIndex);

                int credentialNoStartIndex = line.LastIndexOf("credentialNo\":\"") + "credentialNo\":\"".Length;
                string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf('\"', credentialNoStartIndex) - credentialNoStartIndex).Replace("-", "");


                JspayOrderRecord orderRecord = context.OrderRecords.LastOrDefault(x => x.SeqId == seqId);
                if (orderRecord != null)
                {
                    orderRecord.OrderNo = orderNo;
                    orderRecord.TotalFee = totalFee;
                    orderRecord.ErrorCode = errorCode;
                    orderRecord.ErrorMessage = errorMessage;
                    orderRecord.CredentialNo = credentialNo;
                }

            }
            return false;
        }
        static bool GetOrderNotifyPay(RecordContext context, string line, List<string> lastLines)
        {
            //2020-09-03 23:52:27,396 [104] INFO  XMPPLogger - 智能平台下发数据【md.jspay.notify.pay.success】=====：{"attributes":{"projectCode":"p200393252"},"dataItems":[{"attributes":{"VEHICLE_NO":"贵-A3FV02","outTradeNo":"BK200903235220423p20039325213673","bankType":"微信","tradeState":"0","payFrom":"jieshun","SUBSYSTEM_CODE":"p200393252","IN_TIME":"2020-09-03 20:39:56","payInfo":"JSPAY","bankBillNo":"BK200903235220423p20039325213673","partner":"000000008016914","payType":"WX","timeEnd":"2020-09-03 23:52:27","URL":"http://127.0.0.1/JSTPay/paynotify.aspx?sign_type=MD5&service_version=1.0&input_charset=GBK&trade_state=0&pay_info=JSPAY&partner=000000008016914&bank_billno=BK200903235220423p20039325213673&transaction_id=4200000687202009036898237350&total_fee=1600&fee_type=1&out_trade_no=BK200903235220423p20039325213673&time_end=2020-09-03+23%3A52%3A27&create_time=2020-09-03+23%3A52%3A21&bank_type=%CE%A2%D0%C5&pay_from=jieshun&pay_type=WX&sign=AF7A4ECA51CD0AA8F1EF9210F7262139","BIZDATA":"BIZDATA","transactionId":"4200000687202009036898237350","feeType":"1","totalFee":"1600"},"objectId":"","operateType":"READ","subItems":[]}],"requestType":"DIRECTIVE","seqId":"v22ylm_55g67","serviceId":"md.jspay.notify.pay.success","source":""} 
            //2020-08-26 11:21:28,069 [45] INFO  XMPPLogger - 智能平台下发数据【md.jspay.notify.pay.success】=====：{"attributes":{"projectCode":"p190913745"},"dataItems":[{"attributes":{"VEHICLE_NO":"粤-BN52X7","outTradeNo":"BK200826112119471p20052077416237","bankType":"微信","tradeState":"0","payFrom":"jieshun","SUBSYSTEM_CODE":"p190913745","IN_TIME":"2020-08-26 09:55:24","payInfo":"JSPAY","bankBillNo":"BK200826112119471p20052077416237","partner":"000000008011036","payType":"WX","timeEnd":"2020-08-26 11:21:27","URL":"http://127.0.0.1/JSTPay/paynotify.aspx?sign_type=MD5&service_version=1.0&input_charset=GBK&trade_state=0&pay_info=JSPAY&partner=000000008011036&bank_billno=BK200826112119471p20052077416237&transaction_id=4200000680202008266957163823&total_fee=1200&fee_type=1&out_trade_no=BK200826112119471p20052077416237&time_end=2020-08-26+11%3A21%3A27&create_time=2020-08-26+11%3A21%3A20&bank_type=%CE%A2%D0%C5&pay_from=jieshun&pay_type=WX&sign=E014EB8BBC5D61B1768F8923C1F2D16A","BIZDATA":"BIZDATA","transactionId":"4200000680202008266957163823","feeType":"1","totalFee":"1200"},"objectId":"","operateType":"READ","subItems":[]}],"requestType":"DIRECTIVE","seqId":"v22ylm_4pb52","serviceId":"md.jspay.notify.pay.success","source":""}
            int flagIndex = line.IndexOf("智能平台下发数据【md.jspay.notify.pay.success】");
            if (flagIndex > 0)
            {
                string strLogTime = line.Substring(0, 23);
                DateTime logTime = DateTime.ParseExact(strLogTime, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.CurrentCulture);
                //string threadName = line.Substring(line.IndexOf('[') + 1, line.IndexOf(']') - line.IndexOf('[') - 1);


                int orderNoStartIndex = line.IndexOf("outTradeNo\":\"") + "outTradeNo\":\"".Length;
                string orderNo = line.Substring(orderNoStartIndex, line.IndexOf('\"', orderNoStartIndex) - orderNoStartIndex);

                //int credentialNoStartIndex = line.LastIndexOf("VEHICLE_NO\":\"") + "VEHICLE_NO\":\"".Length;
                //string credentialNo = line.Substring(credentialNoStartIndex, line.IndexOf('\"', credentialNoStartIndex) - credentialNoStartIndex);


                int payTimeStartIndex = line.IndexOf("timeEnd\":\"") + "timeEnd\":\"".Length;
                string strPayTime = line.Substring(payTimeStartIndex, line.IndexOf('\"', payTimeStartIndex) - payTimeStartIndex);

                DateTime payTime = DateTime.Parse(strPayTime);

                JspayOrderRecord orderRecord = context.OrderRecords.LastOrDefault(x => x.OrderNo == orderNo);
                if (orderRecord != null)
                {
                    orderRecord.PayTime = payTime;
                    orderRecord.NotifyTime = logTime;
                }


            }
            return false;
        }
    }
}
