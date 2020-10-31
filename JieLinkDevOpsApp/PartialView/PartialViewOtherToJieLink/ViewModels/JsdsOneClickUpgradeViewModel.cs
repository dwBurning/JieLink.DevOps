using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewJsdsOneClickUpgradeToJieLink.ViewModels
{
    public class JsdsOneClickUpgradeViewModel : DependencyObject
    {
        private static JsdsOneClickUpgradeViewModel instance;
        private static readonly object locker = new object();

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

                UpgradeResult += $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {message}{Environment.NewLine}";
            }));
        }

        /// <summary>
        /// 是否可以继续点击一键升级按钮：避免连续点击
        /// </summary>
        public bool EnableContinueToClickUpgradeButton;
    }
}
