using Panuon.UI.Silver;
using PartialViewInterface.Commands;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using PartialViewJSRMOrder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewJSRMOrder.ViewModel
{
    public class OrderMonitorViewModel : DependencyObject
    {
        private readonly string getVerifyCode = "http://jsrm.jslife.net/api/auth/jwt/getVerifyCode";
        private readonly string getToken = "http://jsrm.jslife.net/api/auth/jwt/getToken";
        private readonly string queryAuthProblemList = "http://jsrm.jslife.net/api/jsrm-workorder/workorder/problem/queryAuthProblemList";
        private readonly string queryProblemDisposeList = "http://jsrm.jslife.net/api/jsrm-workorder/workorder/problemDispose/queryProblemDisposeList";


        public DelegateCommand GetVerifyCodeCommand { get; set; }

        public DelegateCommand LoginCommand { get; set; }

        public OrderMonitorViewModel()
        {
            GetVerifyCodeCommand = new DelegateCommand();
            GetVerifyCodeCommand.ExecuteAction = GetVerifyCode;

            LoginCommand = new DelegateCommand();
            LoginCommand.ExecuteAction = Login;

            UserName = "104542";
        }

        private void GetVerifyCode(object parameter)
        {
            Operator user = new Operator();
            user.username = this.UserName;
            //ReturnMsg returnMsg = HttpHelper.Post<ReturnMsg>(getVerifyCode, user);
            //Notice.Show(returnMsg.respMsg, "通知", 3, MessageBoxIcon.Warning);
        }

        private void Login(object parameter)
        {
            this.Password = (string)parameter;
            string miweng = MD5(this.Password);

            Operator user = new Operator();
            user.username = this.UserName;
            user.password = this.Password;
            user.verifyCode = this.VerifyCode;
            //HttpHelper.Post<ReturnMsg>(getToken, user);
        }


        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="Text">要加密的字符串</param>
        /// <returns>密文</returns>
        public string MD5(string Text)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Text, "MD5").ToLower();
        }


        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(OrderMonitorViewModel));



        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(OrderMonitorViewModel));



        public string VerifyCode
        {
            get { return (string)GetValue(VerifyCodeProperty); }
            set { SetValue(VerifyCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerifyCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerifyCodeProperty =
            DependencyProperty.Register("VerifyCode", typeof(string), typeof(OrderMonitorViewModel));

    }
}
