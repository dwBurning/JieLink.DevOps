using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using PartialViewClearCloudData.Models;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;

namespace PartialViewClearCloudData
{
    public class ClearCloudDeviceDataViewModel : DependencyObject
    {
        public DelegateCommand GenerateSqlCommand { get; set; }
        public DelegateCommand CloudDeviceDeleteCommand { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public static string sql = string.Empty;

        public ClearCloudDeviceDataViewModel()
        {
            GenerateSqlCommand = new DelegateCommand();
            GenerateSqlCommand.ExecuteAction = GenerateSql;

            CloudDeviceDeleteCommand = new DelegateCommand();
            CloudDeviceDeleteCommand.ExecuteAction = CloudDeviceDelete;

            TypeClassDataSource = new Dictionary<string, string>();
            TypeClassDataSource.Add("010301", "环境摄像机");
            TypeClassDataSource.Add("0207", "车场控制器");
            TypeClassDataSource.Add("0209", "门禁控制器");
            TypeClassDataSource.Add("0211", "无人值守卫士");
            TypeClassDataSource.Add("0201", "道闸");
            TypeClassDataSource.Add("0701", "岗亭盒子");
            TypeClassDataSource.Add("1111", "门禁服务");
            this.SelectTypeClass = "010301";
        }

        public void GenerateSql(object parameter)
        {
            sql = AnalysizeSql();
            if (!string.IsNullOrWhiteSpace(sql))
            {
                ShowMessage(string.Format("itemId={0}生成sql语句：", ItemId));
                ShowMessage(sql, false);
                LogHelper.CommLogger.Info(string.Format("itemId={0}生成sql语句：{1}", ItemId, sql));
            }
            else
            {
                ShowMessage(string.Format("itemId={0}生成sql语句：为空", ItemId));
            }
            return;
        }

        public string AnalysizeSql()
        {
            sql = string.Empty;
            if (string.IsNullOrWhiteSpace(ItemId))
            {
                ShowMessage("请输入待删除设备itemId");
                LogHelper.CommLogger.Info("请输入待删除设备itemId");
                return sql;
            }
            if (ItemId.TrimEnd(' ').Length != 32)
            {
                ShowMessage("输入的待删除设备itemId不符合要求，请确认");
                LogHelper.CommLogger.Info("输入的待删除设备itemId不符合要求，请确认，itemId=" + ItemId);
                return sql;
            }
            LogHelper.CommLogger.Info("即将删除设备，itemId=" + ItemId);
            BaseDataEquipmentModel baseDataEquipmentModel = new BaseDataEquipmentModel()
            {
                itemId = ItemId,
                typeClass = SelectTypeClass,
                code = "no used",
                name = "no used",
                type = "200",
                ipAddress = "192.168.12.12",
                isDeleted = true,
                ioType = 0,
                timestamp = (CommonHelper.GetTimeStamp()).ToString(),
                parkId = ConstantHelper.PARKNO,
                parentId = ""
            };
            string protocolData = JsonHelper.SerializeObject(baseDataEquipmentModel);
            sql = $"INSERT INTO sync_xmpp(ServiceType,DataType,ServiceId,SeqId,BusinessId,ProtocolData,SendNum,SendPriority,`Status`,AddTime,UpdateTime,Remark,RequestType) " +
                $"VALUE(0,0,'dm_equip_base_data_equipment','{Guid.NewGuid().ToString().Replace("-", "")}','{new Guid(ItemId)}','{protocolData}',0,9,0,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','子系统上传设备信息到云平台','DATA')";

            return sql;
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="parameter"></param>
        public void CloudDeviceDelete(object parameter)
        {
            sql = AnalysizeSql();
            if (!string.IsNullOrWhiteSpace(sql))
            {
                try
                {
                    MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql);
                    ShowMessage("设备删除成功，item=" + ItemId);
                    LogHelper.CommLogger.Info(string.Format("设备删除记录入库成功，itemId={0}，可查sync_xmpp或者sync_park_base_history确认", ItemId));
                }
                catch (Exception ex)
                {
                    ShowMessage(string.Format("设备删除记录入库异常，itemId={0}，异常信息：{1}", ItemId, ex.Message));
                    LogHelper.CommLogger.Info(string.Format("设备itemId删除记录入库异常，itemId={0}，异常信息：{1}", ItemId, ex.ToString()));
                }
            }
            else
            {
                ShowMessage(string.Format("itemId={0}生成sql语句：为空", ItemId));
            }
            sql = string.Empty;
            return;
        }

        public string ItemId
        {
            get { return (string)GetValue(ItemIdProperty); }
            set { SetValue(ItemIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemIdProperty =
            DependencyProperty.Register("ItemId", typeof(string), typeof(ClearCloudDeviceDataViewModel));

        public Dictionary<string, string> TypeClassDataSource
        {
            get { return (Dictionary<string, string>)GetValue(TypeClassDataSourceProperty); }
            set { SetValue(TypeClassDataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeClassDataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeClassDataSourceProperty =
            DependencyProperty.Register("TypeClassDataSource", typeof(Dictionary<string, string>), typeof(ClearCloudDeviceDataViewModel));


        public string SelectTypeClass
        {
            get { return (string)GetValue(SelectTypeClassProperty); }
            set { SetValue(SelectTypeClassProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectTypeClass.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectTypeClassProperty =
            DependencyProperty.Register("SelectTypeClass", typeof(string), typeof(ClearCloudDeviceDataViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ClearCloudDeviceDataViewModel));


        public void ShowMessage(string message, bool showTime = true)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Message != null && Message.Length > 5000)
                {
                    Message = string.Empty;
                }

                if (message.Length > 0)
                {
                    if (showTime)
                    {
                        Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} {message}{Environment.NewLine}";
                    }
                    else
                    {
                        Message += $"{message}{Environment.NewLine}";
                    }
                }
            }));
        }

    }
}
