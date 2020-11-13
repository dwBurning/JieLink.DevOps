using MySql.Data.MySqlClient;
using PartialViewTmTools.Models;
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

namespace PartialViewTmTools.ViewModels
{
    public class TmToolsViewModel : DependencyObject
    {
        public DelegateCommand CheckDataCommand { get; set; }

        public DelegateCommand ImportJKCommand { get; set; }

        private bool canExecute = false;
        List<BoxBill> boxBillList = new List<BoxBill>();

        public TmToolsViewModel()
        {
            CheckDataCommand = new DelegateCommand();
            CheckDataCommand.ExecuteAction = CheckData;

            ImportJKCommand = new DelegateCommand();
            ImportJKCommand.ExecuteAction = ImportJK;
            ImportJKCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });

           Message = "注意：在用工具导入之前，按模板文件夹中的金科补单模板录入数据。\r\n";

        }


       
        private void CheckData(object parameter)
        {
          
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请先指定文件路径！");
                return;
            }


            canExecute = true;
            ShowMessage("校验通过！");
        }

            private void ImportJK(object parameter)
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
                    int yesCount = 0;
                    int noCount = 0;
                   
                    DataTable dt = NPOIExcelHelper.ExcelToDataTable(filePath, true);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            Guid bGuid = Guid.NewGuid();
                            string orderId = dr["订单号"].ToString();
                            string inTime = dr["服务开始时间"].ToString();
                            string feesTime = dr["服务结束时间"].ToString();
                            string fees = dr["收费金额"].ToString();
                            string accountReceivable = dr["收费金额"].ToString();
                            string actualPaid = dr["收费金额"].ToString();
                            string payTime = dr["支付时间"].ToString();
                            int payTypeID = dr["支付方式"].ToString() == "微信" ? 2 : dr["支付方式"].ToString() == "支付宝" ? 1 : dr["支付方式"].ToString() == "捷顺金科" ? 22 : 0;
                            string createTime = DateTime.Now.ToString();
                            string money = dr["应收金额"].ToString();
                            string credentialNO = dr["车牌"].ToString().Replace('-', ' ').Trim();
                            string plate = dr["车牌"].ToString().Replace('-', ' ').Trim();
                            string payTypeName = dr["支付方式"].ToString();
                            string benefit = dr["优惠金额"].ToString();
                            string paid = dr["收费金额"].ToString();

                            string selSql = $"select EnterRecordID from box_enter_record where plate='" + plate + "' and EnterTime='" + inTime + "'";
                            DataSet ds = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, selSql);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                string enterRecordId = ds.Tables[0].Rows[0][0].ToString();
                                string insertSql = $"insert into box_bill(BGUID, OrderId, EnterRecordId, InTime, FeesTime, Fees, AccountReceivable, ActualPaid, PayTime, PayTypeID, CreateTime, OrderType, Money, Status, Remark, CredentialNO, CredentialType, Plate, PayTypeName,Benefit,paid,sealTypeId,SealTypeName) values('{bGuid}','{orderId}', '{enterRecordId}', '{inTime}', '{feesTime}', '{fees}','{accountReceivable}','{actualPaid}','{payTime}', '{payTypeID}', '{createTime}', 1, '{money}', 1, '手动补充', '{credentialNO}', 163, '{plate}', '{payTypeName}','{benefit}','{paid}',54,'临时套餐A');";
                                int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, insertSql);
                                if (result > 0)
                                {
                                    yesCount++;
                                }
                            }
                            else
                            {
                                noCount++;
                                BoxBill boxbill = new BoxBill();
                                boxbill.Plate = plate;
                                boxbill.OrderId = orderId;
                                boxBillList.Add(boxbill);
                            }

                        }
                        ShowMessage($"共补单成功记录{yesCount}条,异常记录{noCount}条");
                    }
                    else
                    {
                        ShowMessage("导入文件无数据!");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("异常明细:"+ex.ToString());
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
                    Message += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }

        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="fileName">文件路径</param>
        /// <returns>文件的编码类型</returns>

        public static Encoding GetType(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(FileStream fs)
        {
            Encoding reVal = Encoding.Default;
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }


        public List<BoxBill> NoCountList
        {
            get { return (List<BoxBill>)GetValue(NoCountListProperty); }
            set { SetValue(NoCountListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoCountListProperty =
            DependencyProperty.Register("NoCountList", typeof(List<BoxBill>), typeof(TmToolsViewModel));


        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(TmToolsViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(TmToolsViewModel));

    }
}
