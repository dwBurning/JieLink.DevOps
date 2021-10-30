using Panuon.UI.Silver;
using PartialViewInterface.Commands;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using PartialViewJSRMOrder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static PartialViewInterface.Utils.HttpHelper;

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
            ReturnMsg<object> returnMsg = Post<ReturnMsg<object>>(getVerifyCode, user);
            Notice.Show(returnMsg.respMsg, "通知", 3, MessageBoxIcon.Warning);
        }

        Token token = new Token();
        private void Login(object parameter)
        {
            this.Password = (string)parameter;
            string password = getMD5(this.Password);

            Operator user = new Operator();
            user.username = this.UserName;
            user.password = password;
            user.verifyCode = this.VerifyCode;

            ReturnMsg<Token> returnMsg = Post<ReturnMsg<Token>>(getToken, user);
            if (returnMsg.success)
            {
                token.token = returnMsg.respData.token;
                token.userId = returnMsg.respData.userId;

                getOrder();
            }
        }

        private void getOrder()
        {
            HttpRequestArgs httpRequestArgs = new HttpRequestArgs();
            httpRequestArgs.Url = queryAuthProblemList;
            httpRequestArgs.Heads.Add("userId", token.userId);
            httpRequestArgs.Heads.Add("X-Token", token.userId);
            ReturnMsg<List<Order>> returnMsg = Post<ReturnMsg<List<Order>>>(httpRequestArgs);
        }


        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="Text">要加密的字符串</param>
        /// <returns>密文</returns>
        public string getMD5(string password)
        {
            MD5 algorithm = MD5.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
            string md5 = "";
            for (int i = 0; i < data.Length; i++)
            {
                md5 += data[i].ToString("x2");
            }
            return md5;
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
