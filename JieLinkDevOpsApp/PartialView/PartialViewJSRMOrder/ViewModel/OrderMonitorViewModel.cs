using Panuon.UI.Silver;
using PartialViewInterface.Commands;
using PartialViewInterface.DB;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using PartialViewJSRMOrder.DB;
using PartialViewJSRMOrder.Model;
using PartialViewJSRMOrder.Monitor;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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


        private static OrderMonitorViewModel instance;
        private static readonly object locker = new object();

        public static OrderMonitorViewModel Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new OrderMonitorViewModel();
                    }
                }
            }
            return instance;
        }

        public DelegateCommand GetVerifyCodeCommand { get; set; }

        public DelegateCommand LoginCommand { get; set; }

        public DelegateCommand StartTaskCommand { get; set; }


        Operator user;

        KeyValueSettingManager keyValueSettingManager;

        DevJsrmOrderManager devJsrmOrderManager;

        public bool IsNeedLogin { get; set; }

        public bool IsCanLogin { get; set; }

        public OrderMonitorViewModel()
        {
            GetVerifyCodeCommand = new DelegateCommand();
            GetVerifyCodeCommand.ExecuteAction = GetVerifyCode;
            GetVerifyCodeCommand.CanExecuteFunc = new Func<object, bool>((object parameter) =>
            {
                return !string.IsNullOrEmpty(this.UserName);
            });

            IsCanLogin = !string.IsNullOrEmpty(this.VerifyCode) && !string.IsNullOrEmpty(this.UserName);
            LoginCommand = new DelegateCommand();
            LoginCommand.ExecuteAction = Login;
            LoginCommand.CanExecuteFunc = new Func<object, bool>((object parameter) =>
            {
                return IsCanLogin;
            });

            StartTaskCommand = new DelegateCommand();
            StartTaskCommand.ExecuteAction = StartTask;
            StartTaskCommand.CanExecuteFunc = new Func<object, bool>((object parameter) =>
            {
                return !IsNeedLogin;
            });

            keyValueSettingManager = new KeyValueSettingManager();
            devJsrmOrderManager = new DevJsrmOrderManager();

            UserName = "104542";
            user = JsonHelper.DeserializeObject<Operator>(keyValueSettingManager.ReadSetting("JSRMUserInfo")?.ValueText);
            if (user == null)
            {
                user = new Operator();
            }

            GetOrderJob = keyValueSettingManager.ReadSetting("GetOrderJob")?.ValueText;
            DispatchJob = keyValueSettingManager.ReadSetting("DispatchJob")?.ValueText;
            YesterdayReportJob = keyValueSettingManager.ReadSetting("YesterdayReportJob")?.ValueText;
            ReceiveEmail = keyValueSettingManager.ReadSetting("ReceiveEmail")?.ValueText;
        }


        public void TextChanaged(string verifyCode)
        {
            IsCanLogin = !string.IsNullOrEmpty(verifyCode) && !string.IsNullOrEmpty(this.UserName);
        }

        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        private void StartTask(object parameter)
        {
            keyValueSettingManager.WriteSetting(new KeyValueSetting() { KeyId = "GetOrderJob", ValueText = this.GetOrderJob });
            keyValueSettingManager.WriteSetting(new KeyValueSetting() { KeyId = "DispatchJob", ValueText = this.DispatchJob });
            keyValueSettingManager.WriteSetting(new KeyValueSetting() { KeyId = "YesterdayReportJob", ValueText = this.YesterdayReportJob });
            keyValueSettingManager.WriteSetting(new KeyValueSetting() { KeyId = "ReceiveEmail", ValueText = this.ReceiveEmail });

            string[] jobs = new string[] { "GetOrderJob", "DispatchJob", "YesterdayReportJob" };
            foreach (var jobKey in jobs)
            {
                string cron = string.Empty;
                Type jobType = null;
                switch (jobKey)
                {
                    case "GetOrderJob": cron = this.GetOrderJob; jobType = typeof(GetOrderJob); break;
                    case "DispatchJob": cron = this.DispatchJob; jobType = typeof(DispatchJob); break;
                    case "YesterdayReportJob": cron = this.YesterdayReportJob; jobType = typeof(YesterdayReportJob); break;
                }

                var triggerKey = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals("JSRM")).Where(x => x.Name == jobKey).FirstOrDefault();
                if (triggerKey != null)
                {
                    scheduler.UnscheduleJob(triggerKey);
                }
                string requestArgs = JsonHelper.SerializeObject(GetHttpRequestArgsHelper.GetHttpRequestArgs(this.UserName, queryAuthProblemList, user.token, user.userId));
                var job = JobBuilder.Create(jobType)
                                        .WithIdentity(jobKey, "JSRM")
                                        .UsingJobData("HttpRequestArgs", requestArgs)
                                        .UsingJobData("ReceiveEmail", this.ReceiveEmail)
                                        .Build();
                var trigger = TriggerBuilder.Create()
                                .StartNow()
                                .WithIdentity(jobKey, "JSRM")
                                .WithCronSchedule(cron)
                                .Build();
                scheduler.ScheduleJob(job, trigger);
            }
            ShowMessage("定时任务已启动");
            Notice.Show("定时任务已启动", "通知", 3, MessageBoxIcon.Success);
        }

        /// <summary>
        /// 根据工单号查询转研发时间或者完成时间和完成人
        /// </summary>
        /// <param name="GD"></param>
        public DateTime GetTimePointByGDAsync(string GD,bool isFinishTime,out string responsibleperson)
        {
            responsibleperson = "";
            HttpHelper.HttpRequestArgs requestArgs = null;
            OrderMonitorViewModel.Instance().Dispatcher.Invoke(() =>
            {
                requestArgs = GetHttpRequestArgsHelper.GetHttpRequestArgs(this.UserName, queryProblemDisposeList, user.token, user.userId, GD);
            });
            var result = JsonHelper.DeserializeObject<DisposeReturnMsg>(HttpHelper.Post(requestArgs));
            if (result != null)
            {
                if (isFinishTime == false)
                { 
                    foreach(var slice in result.respData)
                    {
                        if(slice.remark.Contains("转派到【研发】节点，处理部门：产品研发五部"))
                        {
                            return Convert.ToDateTime(slice.createTime);
                        }
                    }
                }
                else
                {
                    int YanfaId = -1;
                    int DispathId = -1;

                    foreach (var slice in result.respData)
                    {
                        if (slice.remark.Contains("转派到【研发】节点，处理部门：产品研发五部")|| slice.remark.Contains("验证退回，打回到【研发】节点"))
                        {
                            if (slice.id > YanfaId)
                                YanfaId = slice.id;
                        }
                        if (slice.remark.Contains("处理问题，解决方案为") || slice.remark.Contains("驳回到【总部节点】节点，原因"))
                        {
                            if (slice.id > DispathId)
                                DispathId = slice.id;
                            //responsibleperson = slice.userName;
                            //return Convert.ToDateTime(slice.createTime);
                        }
                    }
                    //研发节点已处理但是再次转入研发节点的工单被误认为已经处理的问题

                    //研发节点小于解决节点，确实已经解决
                    if(YanfaId < DispathId)
                    {
                        var temp = result.respData.Where(x => x.id == DispathId).First();
                        responsibleperson = temp.userName;
                        return Convert.ToDateTime(temp.createTime);
                    }

                }
            }
            return DateTime.MinValue;
        }



        private void GetVerifyCode(object parameter)
        {
            Operator user = new Operator();
            user.username = this.UserName;
            ReturnMsg<object> returnMsg;
            try
            {
                returnMsg = Post<ReturnMsg<object>>(GetHttpRequestArgsHelper.GetHttpRequestArgs(getVerifyCode, user));
            }
            catch (Exception)
            {
                returnMsg = new ReturnMsg<object>() { success = false };
            }

            if (returnMsg.success)
            {
                ShowMessage(returnMsg.respMsg);
                Notice.Show(returnMsg.respMsg, "通知", 3, MessageBoxIcon.Success);
            }
            else
            { MessageBoxHelper.MessageBoxShowWarning(returnMsg.respMsg); }
        }

        public async void Load()
        {
            if (string.IsNullOrEmpty(user.token))
            {
                IsNeedLogin = true;
                ShowMessage("请重新登录");
                return;
            }

            ReturnMsg<PageOrder> returnMsg = await ExecuteGetOrderJob.GetOrder(this.UserName, queryAuthProblemList, user.token, user.userId);
            if (returnMsg.success)
            {
                IsNeedLogin = false;
                ShowMessage("Token有效，已自动登陆");
                ExecuteGetOrderJob.AddOrder(returnMsg.respData.data);
                StartTask(null);
            }
            else
            {
                IsNeedLogin = true;
                ShowMessage("Token失效，请重新登录");
            }
        }


        public async void Login(object parameter)
        {
            this.Password = (string)parameter;
            string password = GetMD5(this.Password);

            user.username = this.UserName;
            user.password = password;
            user.verifyCode = this.VerifyCode;
            ReturnMsg<Token> returnMsg;
            try
            {
                returnMsg = Post<ReturnMsg<Token>>(GetHttpRequestArgsHelper.GetHttpRequestArgs(getToken, user));
            }
            catch (Exception)
            {
                returnMsg = new ReturnMsg<Token>() { success = false };
            }

            if (returnMsg.success)
            {
                IsNeedLogin = false;
                ShowMessage(returnMsg.respMsg);

                user.token = returnMsg.respData.token;
                user.userId = returnMsg.respData.userId;
                user.password = this.Password;

                keyValueSettingManager.WriteSetting(new KeyValueSetting() { KeyId = "JSRMUserInfo", ValueText = JsonHelper.SerializeObject(user) });

                ReturnMsg<PageOrder> returnMsg1 = await ExecuteGetOrderJob.GetOrder(this.UserName, queryAuthProblemList, user.token, user.userId);
                if (returnMsg1.success)
                {
                    ExecuteGetOrderJob.AddOrder(returnMsg1.respData.data);
                }
            }
            else
            {
                Notice.Show(returnMsg.respMsg, "通知", 3, MessageBoxIcon.Warning);
            }
        }




        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="Text">要加密的字符串</param>
        /// <returns>密文</returns>
        public string GetMD5(string password)
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

        public void ShowMessage(string message)
        {
            LogHelper.CommLogger.Info(message);
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
            set
            { SetValue(VerifyCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerifyCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerifyCodeProperty =
            DependencyProperty.Register("VerifyCode", typeof(string), typeof(OrderMonitorViewModel));




        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(OrderMonitorViewModel));




        public string GetOrderJob
        {
            get { return (string)GetValue(GetOrderJobProperty); }
            set { SetValue(GetOrderJobProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GetOrderJob.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GetOrderJobProperty =
            DependencyProperty.Register("GetOrderJob", typeof(string), typeof(OrderMonitorViewModel));




        public string DispatchJob
        {
            get { return (string)GetValue(DispatchJobProperty); }
            set { SetValue(DispatchJobProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DispatchJob.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DispatchJobProperty =
            DependencyProperty.Register("DispatchJob", typeof(string), typeof(OrderMonitorViewModel));

        public string YesterdayReportJob
        {
            get { return (string)GetValue(YesterdayReportJobProperty); }
            set { SetValue(YesterdayReportJobProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YesterdayReportJob.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YesterdayReportJobProperty =
            DependencyProperty.Register("YesterdayReportJob", typeof(string), typeof(OrderMonitorViewModel));

        public string ReceiveEmail
        {
            get { return (string)GetValue(ReceiveEmailProperty); }
            set { SetValue(ReceiveEmailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReceiveEmail.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReceiveEmailProperty =
            DependencyProperty.Register("ReceiveEmail", typeof(string), typeof(OrderMonitorViewModel));



    }
}
