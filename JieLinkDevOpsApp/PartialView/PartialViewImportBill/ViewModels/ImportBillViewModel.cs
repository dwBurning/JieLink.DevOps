using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewImportBill.ViewModels
{
    public class ImportBillViewModel : DependencyObject
    {
        public DelegateCommand ImportBillCommand { get; set; }

        public bool canExecute = false;

        public ImportBillViewModel()
        {
            ImportBillCommand = new DelegateCommand();
            ImportBillCommand.ExecuteAction = ImportBill;
            ImportBillCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });
        }

        private void ImportBill(object parameter)
        {
            string filePath = this.FilePath;
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
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            Guid bGuid = Guid.NewGuid();
                            string orderId = dr["订单号"].ToString();
                            string inTime = dr["服务开始时间"].ToString();
                            string feesTime = dr["服务结束时间"].ToString();
                            string fees = dr["收费金额"].ToString().Replace("元", "");
                            string accountReceivable = dr["收费金额"].ToString().Replace("元", "");
                            string actualPaid = dr["收费金额"].ToString().Replace("元", "");
                            string payTime = dr["支付时间"].ToString();
                            int payTypeID = dr["支付方式"].ToString() == "微信" ? 2 : dr["支付方式"].ToString() == "支付宝" ? 1 : dr["支付方式"].ToString() == "捷顺金科" ? 22 : 0;
                            string createTime = DateTime.Now.ToString();
                            string money = dr["应收金额"].ToString().Replace("元", "");
                            string credentialNO = dr["车牌"].ToString().Replace("-", "").Trim();
                            string plate = dr["车牌"].ToString().Replace("-", "").Trim();
                            string payTypeName = dr["支付方式"].ToString();
                            string benefit = dr["优惠金额"].ToString().Replace("元", "");
                            int paid = 0;
                            int derate = 0;
                            int exchange = 0;
                            int smallChange = 0;

                            ShowMessage($"-- {orderId}");

                            string selSql = $"select EnterRecordID into @enterRecordId from box_enter_record where plate='{plate}' and EnterTime='{inTime}';";
                            ShowMessage(selSql);

                            string insertSql = $"INSERT INTO `box_bill` (`BGUID`,`OrderId`,`InTime`,`FeesTime`,`Fees`,`Benefit`,`Derate`,`AccountReceivable`,`Paid`,`ActualPaid`,`Exchange`,`SmallChange`,`Cashier`,`PayTime`,`discountPicturePath`,`PayTypeID`,`ChargeType`,`ChargeDeviceID`,`OperatorID`,`OperatorName`,`CloudID`,`EnterRecordID`,`CreateTime`,`OrderType`,`Money`,`Status`,`SealTypeId`,`SealTypeName`,`Remark`,`ReplaceDeduct`,`AppUserId`,`TrusteeFlag`,`EventType`,`PayFrom`,`DeviceID`,`CredentialNO`,`CredentialType`,`CashierName`,`PersonNo`,`PersonName`,`discounts`,`ChargeDeviceName`,`FreeMoney`,`UpLoadFlag`,`Plate`,`OnlineExchange`,`PayTypeName`,`ExtStr1`,`ExtStr2`,`ExtStr3`,`ExtStr4`,`ExtStr5`,`ExtInt1`,`ExtInt2`,`ExtInt3`,`CashTotal`,`ParkNo`) VALUES ('{bGuid}', '{orderId}', '{inTime}', '{feesTime}', '{fees}', '{benefit}', '{derate}', '{accountReceivable}', '{paid}', '{accountReceivable}', '{exchange}', '{smallChange}', '9999', '{payTime}', '', '{payTypeID}', '0', '', '9999', '超级管理员', '{orderId}', @enterRecordId, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '1', '{fees}', '1', '54', '临时用户A', '人工补录', '0', '', '0', '1', 'jieshun', null, '{credentialNO}', '163', '超级管理员', '', '', '', null, '0.00', '1', '{plate}', '0.00', '{payTypeName}',null, null, null, null, null, '0', '0', '0','0.00', '00000000-0000-0000-0000-000000000000');";

                            ShowMessage(insertSql);
                        }
                    }
                    else
                    {
                        ShowMessage("导入文件无数据!");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("异常明细:" + ex.ToString());
                }
            });
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
                    Message += $"{message}{Environment.NewLine}";
                }
            }));
        }


        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImportBillViewModel));



        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ImportBillViewModel));



    }
}
