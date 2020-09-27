using MySql.Data.MySqlClient;
using PartialViewImportPlate.Models;
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

namespace PartialViewImportPlate.ViewModels
{
    public class ImportPlateViewModel : DependencyObject
    {
        public DelegateCommand CheckDataCommand { get; set; }

        public DelegateCommand ImportPlateCommand { get; set; }

        private bool canExecute = false;

        public ImportPlateViewModel()
        {
            CheckDataCommand = new DelegateCommand();
            CheckDataCommand.ExecuteAction = CheckData;

            ImportPlateCommand = new DelegateCommand();
            ImportPlateCommand.ExecuteAction = ImportPlate;
            ImportPlateCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });

           Message = "说明：本工具是为共享汽车导入凭证而制作作，在用工具导入之前，至少要通过界面发行一个凭证信息。\r\n1)将需要导入的车牌粘贴到txt文件中，比如plates.txt\r\n2)将记事本文件另存为UTF8编码，必须为UTF8，否则汉字会乱码\r\n";

        }


        ControlVoucher voucher = new ControlVoucher();
        private void CheckData(object parameter)
        {
            if (string.IsNullOrEmpty(PersonNo))
            {
                MessageBoxHelper.MessageBoxShowWarning("请先输入人事编号！");
                return;
            }

            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBoxHelper.MessageBoxShowWarning("请先指定文件路径！");
                return;
            }


            bool result = false;
            string cmd = "select * from control_voucher where personno='{0}' limit 1";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, string.Format(cmd, PersonNo)))
            {
                if (reader.Read())
                {
                    voucher.VGUID = reader["guid"].ToString();
                    voucher.PGUID = reader["pguid"].ToString();
                    voucher.LGUID = reader["lguid"].ToString();
                    voucher.PersonNo = reader["personno"].ToString();
                    voucher.VoucherType = int.Parse(reader["vouchertype"].ToString());
                    voucher.VoucherNo = reader["voucherno"].ToString();
                    voucher.CardNum = reader["cardnum"].ToString();
                    voucher.AddOperatorNo = reader["addoperatorno"].ToString();
                    voucher.AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    voucher.Status = 1;
                    voucher.LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    voucher.Remark = "add by program";
                    voucher.StatusFromPerson = 1;
                    result = true;
                }
                else
                {
                    ShowMessage("没有查询到该人事相关的凭证信息，请先至少发行一个凭证！");
                    result = false;
                    return;
                }
            }

            if (result)
            {
                if (!File.Exists(FilePath))
                {
                    ShowMessage("文件不存在！");
                    return;
                }

                if (!FilePath.EndsWith(".txt"))
                {
                    ShowMessage("非文本文件！");
                    return;
                }

                if (GetType(FilePath) != System.Text.Encoding.UTF8)
                {
                    ShowMessage("非UTF8编码！");
                    return;
                }
            }

            canExecute = true;
            ShowMessage("校验通过！");
        }

        private void ImportPlate(object parameter)
        {
            string[] arryPlate = File.ReadAllLines(FilePath);
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < arryPlate.Length; i++)
                {
                    voucher.VoucherNo = arryPlate[i].Trim();
                    voucher.CardNum = arryPlate[i].Trim();
                    voucher.AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    voucher.LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string vguid = Guid.NewGuid().ToString();

                    string command = string.Format(@"insert into control_voucher(guid,pguid,lguid,personno,vouchertype,voucherno,cardnum,addoperatorno,addtime,`status`,lasttime,remark,statusfromperson)
            values('{12}','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},'{9}','{10}',{11})", voucher.PGUID, voucher.LGUID, voucher.PersonNo, voucher.VoucherType, voucher.VoucherNo, voucher.CardNum, voucher.AddOperatorNo, voucher.AddTime, voucher.Status, voucher.LastTime, voucher.Remark, voucher.StatusFromPerson, vguid);

                    int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, command);
                    if (result > 0)
                    {
                        ShowMessage(string.Format("凭证 {0} 添加成功！", voucher.VoucherNo));
                        //开始插入权限
                        string sql = string.Format("select * from control_voucher_device where VGuid='{0}'", voucher.VGUID);
                        DataTable table = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
                        foreach (DataRow dr in table.Rows)
                        {
                            string rights = string.Format("insert into control_voucher_device values(UUID(),'{0}','{1}','{2}',0)", vguid, dr["DGuid"].ToString(), dr["DeviceId"].ToString());
                            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, rights);
                        }

                        ShowMessage(string.Format("凭证 {0} 授权成功！", voucher.VoucherNo));
                    }
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

        public string PersonNo
        {
            get { return (string)GetValue(PersonNoProperty); }
            set { SetValue(PersonNoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PersonNo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PersonNoProperty =
            DependencyProperty.Register("PersonNo", typeof(string), typeof(ImportPlateViewModel));


        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImportPlateViewModel));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ImportPlateViewModel));

    }
}
