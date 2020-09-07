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

namespace PartialViewSyncTool.SyncToolViewModel
{
    public class DataCheckViewModel : DependencyObject
    {
        public DelegateCommand GetBoxConnStringCommand { get; set; }

        public DelegateCommand StartCheckCommand { get; set; }

        public DelegateCommand OutPutToNotepadCommand { get; set; }

        private bool canExecute = false;

        BoxConnConfig boxConnConfig;

        List<ControlVoucher> controlVouchers;

        Dictionary<string, string> dictBoxConnStr;

        bool isChecked = false;

        public DataCheckViewModel()
        {
            boxConnConfig = new BoxConnConfig();
            boxConnConfig.ShowMessage += BoxConnConfig_ShowMessage;

            GetBoxConnStringCommand = new DelegateCommand();
            GetBoxConnStringCommand.ExecuteAction = GetBoxConnString;

            StartCheckCommand = new DelegateCommand();
            StartCheckCommand.ExecuteAction = Start;
            StartCheckCommand.CanExecuteFunc = new Func<object, bool>((object obj) => { return canExecute; });

            OutPutToNotepadCommand = new DelegateCommand();
            OutPutToNotepadCommand.ExecuteAction = OutPut;
            OutPutToNotepadCommand.CanExecuteFunc = new Func<object, bool>((object obj) => { return canExecute; });
        }

        private void Start(object parameter)
        {
            canExecute = false;
            isChecked = IsChecked;
            controlVouchers = new List<ControlVoucher>();
            Task.Factory.StartNew(() =>
            {
                CheckVoucher();
            });
        }

        private void CheckVoucher()
        {
            try
            {
                string cmd = "select * from control_voucher";
                using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, cmd))
                {
                    while (reader.Read())
                    {
                        ControlVoucher voucher = new ControlVoucher();
                        voucher.VGUID = reader["guid"].ToString();
                        voucher.PGUID = reader["pguid"].ToString();
                        voucher.LGUID = reader["lguid"].ToString();
                        voucher.PersonNo = reader["personno"].ToString();
                        voucher.VoucherType = int.Parse(reader["vouchertype"].ToString());
                        voucher.VoucherNo = reader["voucherno"].ToString();
                        voucher.CardNum = reader["cardnum"].ToString();
                        voucher.Status = int.Parse(reader["Status"].ToString());
                        voucher.DeviceList = new List<string>();
                        if (!isChecked)
                        {
                            string sql = $"select * from control_voucher_device where VGuid='{voucher.VGUID}'";
                            DataTable table = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];
                            foreach (DataRow dr in table.Rows)
                            {
                                voucher.DeviceList.Add(dr["DeviceId"].ToString());
                            }
                        }
                        Compare(voucher);
                    }
                }

                OutPut(null);
                canExecute = true;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }
        }

        private void Compare(ControlVoucher voucher)
        {
            foreach (var ip in dictBoxConnStr.Keys)
            {
                ShowMessage($"比对盒子{ip}的数据");
                GetBoxData(dictBoxConnStr[ip], voucher);
            }
        }

        private void GetBoxData(string connStr, ControlVoucher voucher)
        {
            string sql = "";
            if (!isChecked)
            {
                sql = $"select * from crd_credential where `no`='{voucher.VoucherNo}' and `state`={voucher.Status} and personNo='{voucher.PersonNo}'";
            }
            else
            {
                sql = $"select * from crd_credential where `no`='{voucher.VoucherNo}' and `state`={voucher.Status}";
            }
            
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connStr, sql))
            {
                if (!reader.Read())
                {
                    ShowMessage($"凭证[{voucher.VoucherNo}]与中心不一致");
                    if (!isChecked)
                    {
                        RepairData(connStr, voucher);
                    }
                    else
                    {
                        controlVouchers.Add(voucher);
                    }
                }
            }
        }

        private void RepairData(string connStr, ControlVoucher voucher)
        {
            List<string> devices = new List<string>();
            string sql = "select deviceid from crd_device where parentdeviceid=0";
            DataTable dt = MySqlHelper.ExecuteDataset(connStr, sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                devices.Add(dr["deviceid"].ToString());
            }
            string deviceList = "";
            devices.ForEach((x) =>
            {
                int index = voucher.DeviceList.FindIndex(m => m == x);
                if (index >= 0)
                {
                    deviceList += x + ";";
                }
            });

            sql = $"select * from crd_credential where `no`='{voucher.VoucherNo}' and `state`={voucher.Status}";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connStr, sql))
            {
                if (reader.Read())
                {
                    string personNo = reader["personNo"].ToString();
                    if (personNo != voucher.PersonNo)
                    {
                        string cmd = $"update crd_credential set personNo='{voucher.PersonNo}',deviceidList='{deviceList.TrimEnd(';')}' where `no`='{voucher.VoucherNo}' and `state`={voucher.Status} ";
                        int result = MySqlHelper.ExecuteNonQuery(connStr, cmd);
                        ShowMessage($"更新凭证[{voucher.VoucherNo}]的人事No为[{voucher.PersonNo}]，权限列表为[{deviceList}]");
                    }
                }
                else
                {
                    controlVouchers.Add(voucher);
                }
            }
        }

        private void OutPut(object parameter)
        {
            if (controlVouchers.Count <= 0)
            {
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("以下凭证信息需要在中心凭证管理中执行挂失解挂操作：");
            foreach (var voucher in controlVouchers)
            {
                stringBuilder.AppendLine(voucher.VoucherNo);
            }

            File.WriteAllText("差异凭证信息.txt", stringBuilder.ToString());
            ProcessHelper.StartProcessV2("notepad.exe", "差异凭证信息.txt");
        }

        private void GetBoxConnString(object parameter)
        {
            dictBoxConnStr = boxConnConfig.GetBoxConnString();
            if (dictBoxConnStr.Count > 0)
            {
                canExecute = true;
            }
        }

        private void BoxConnConfig_ShowMessage(string message)
        {
            ShowMessage(message);
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



        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(DataCheckViewModel));





        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(DataCheckViewModel));





    }
}
