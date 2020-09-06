using MySql.Data.MySqlClient;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
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
            controlVouchers = new List<ControlVoucher>();
            Task.Factory.StartNew(() =>
            {
                CheckVoucher();
            });
        }

        private void CheckVoucher()
        {
            string cmd = "select * from control_voucher";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(EnvironmentInfo.ConnectionString, cmd))
            {
                while (reader.Read())
                {
                    ControlVoucher voucher = new ControlVoucher();
                    voucher.PersonNo = reader["personno"].ToString();
                    voucher.VoucherNo = reader["voucherno"].ToString();
                    voucher.Status = int.Parse(reader["Status"].ToString());
                    Compare(voucher);
                }
            }
            canExecute = true;
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
            string sql = $"select * from crd_credential where `no`='{voucher.VoucherNo}' and `state`={voucher.Status} and personNo='{voucher.PersonNo}'";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(connStr, sql))
            {
                if (!reader.Read())
                {
                    controlVouchers.Add(voucher);
                    ShowMessage($"凭证[{voucher.VoucherNo}]与中心不一致");
                }
            }
        }

        private void OutPut(object parameter)
        {
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



    }
}
