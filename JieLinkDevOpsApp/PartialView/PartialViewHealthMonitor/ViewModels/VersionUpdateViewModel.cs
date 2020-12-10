using PartialViewHealthMonitor.CheckUpdate;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Models;
using PartialViewInterface.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewHealthMonitor.ViewModels
{
    public class VersionUpdateViewModel : DependencyObject
    {
        public DelegateCommand CheckUpdateCommand { get; set; }
        public bool canExecute { get; set; }

        public VersionUpdateViewModel()
        {
            canExecute = false;
            BtnContent = "立即升级";
            CurrentVersion = EnvironmentInfo.CurrentVersion;

            CheckUpdateCommand = new DelegateCommand();
            CheckUpdateCommand.ExecuteAction = Update;
            CheckUpdateCommand.CanExecuteFunc = new Func<object, bool>((object parameter) => { return canExecute; });
        }


        private void Update(object parameter)
        {
           
            Task.Factory.StartNew(() =>
            {
                canExecute = false;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    BtnContent = "正在下载升级包，请等待...";
                }));
                UpdateRequest updateRequest = CheckUpdateHelper.GetUploadRequest();
                if (updateRequest != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (MessageBoxHelper.MessageBoxShowQuestion($"检测到新版本{updateRequest.Version}[当前版本{EnvironmentInfo.CurrentVersion}]，是否立即升级？") == MessageBoxResult.Yes)
                        {
                            CheckUpdateHelper.ExecuteUpdate(updateRequest);
                        }
                    }));
                }
                canExecute = true;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    BtnContent = "立即升级";
                }));
            });
        }


        public string CurrentVersion
        {
            get { return (string)GetValue(CurrentVersionProperty); }
            set { SetValue(CurrentVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVersionProperty =
            DependencyProperty.Register("CurrentVersion", typeof(string), typeof(VersionUpdateViewModel));



        public string LastVersion
        {
            get { return (string)GetValue(LastVersionProperty); }
            set { SetValue(LastVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastVersionProperty =
            DependencyProperty.Register("LastVersion", typeof(string), typeof(VersionUpdateViewModel));




        public string BtnContent
        {
            get { return (string)GetValue(BtnContentProperty); }
            set { SetValue(BtnContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BtnContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BtnContentProperty =
            DependencyProperty.Register("BtnContent", typeof(string), typeof(VersionUpdateViewModel));




    }
}
