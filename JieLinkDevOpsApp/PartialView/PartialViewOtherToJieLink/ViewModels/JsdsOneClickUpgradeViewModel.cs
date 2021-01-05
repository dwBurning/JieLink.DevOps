using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PartialViewOtherToJieLink.Models;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class JsdsOneClickUpgradeViewModel : DependencyObject
    {
        private static JsdsOneClickUpgradeViewModel instance;
        private static readonly object locker = new object();

        JsdsUpgradePolicy policy = new JsdsUpgradePolicy();

        public static JsdsOneClickUpgradeViewModel Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new JsdsOneClickUpgradeViewModel();
                    }
                }
            }
            return instance;
        }

        public string UpgradeResult
        {
            get { return (string)GetValue(UpgradeResultProperty); }
            set { SetValue(UpgradeResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckResult.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpgradeResultProperty =
            DependencyProperty.Register("UpgradeResult", typeof(string), typeof(JsdsOneClickUpgradeViewModel));

        public void ShowMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (UpgradeResult != null && UpgradeResult.Length > 5000)
                {
                    UpgradeResult = string.Empty;
                }
                if (!string.IsNullOrWhiteSpace(message))
                {
                    UpgradeResult += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
                }
            }));
        }

        /// <summary>
        /// 是否可以继续点击一键升级按钮：避免连续点击
        /// </summary>
        public bool EnableContinueToClickUpgradeButton;
        
        /// <summary>
        /// 入场记录
        /// </summary>

        public bool EnterRecord
        {
            get { return (bool)GetValue(EnterRecordProperty); }
            set { SetValue(EnterRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnterRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnterRecordProperty =
            DependencyProperty.Register("EnterRecord", typeof(bool), typeof(JsdsOneClickUpgradeViewModel));

        /// <summary>
        /// 入场记录
        /// </summary>

        public bool OutRecord
        {
            get { return (bool)GetValue(OutRecordProperty); }
            set { SetValue(OutRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutRecordProperty =
            DependencyProperty.Register("OutRecord", typeof(bool), typeof(JsdsOneClickUpgradeViewModel));
        /// <summary>
        /// 入场记录
        /// </summary>

        public bool BillRecord
        {
            get { return (bool)GetValue(BillRecordProperty); }
            set { SetValue(BillRecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BillRecord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BillRecordProperty =
            DependencyProperty.Register("BillRecord", typeof(bool), typeof(JsdsOneClickUpgradeViewModel));
    }
}
