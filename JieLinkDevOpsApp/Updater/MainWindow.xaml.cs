using JieShun.JieLink.DevOps.Updater.Models;
using JieShun.JieLink.DevOps.Updater.Utils;
using JieShun.JieLink.DevOps.Updater.ViewModels;
using Panuon.UI.Silver;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JieShun.JieLink.DevOps.Updater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        public static UpdateRequest UpdateRequest { get; set; }
        MainWindowViewModel viewModel;
        ConsoleRedirect consoleRedirect;
        public MainWindow()
        {
            InitializeComponent();
            consoleRedirect = new ConsoleRedirect(WriteLineSafely);
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnClose();
        }

        private async void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            Console.SetOut(consoleRedirect);
            if (UpdateRequest != null)
            {
                //获得当前登录的Windows用户标示
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                //判断当前登录用户是否为管理员
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    UniversalUpdater updater = new UniversalUpdater(UpdateRequest);
                    try
                    {
                        await updater.StartAsync(UpdateProgressSafely);
                    }
                    catch (Exception ex)
                    {
                        viewModel.UpdateMessage = ex.Message + ",程序即将退出！";
                    }
                }
                else
                {
                    viewModel.UpdateMessage = "当前程序无管理员权限,程序即将退出！";
                }
            }
            else
            {
                viewModel.UpdateMessage = "缺少升级所需的参数UpdateRequest.json,程序即将退出！";
            }
            await Task.Delay(5000);
            OnClose();
        }
        void UpdateProgressSafely(int progress, string message)
        {
            LogHelper.CommLogger.Info(message);
            Dispatcher.Invoke(() =>{
                UpdateProgress(progress, message);
            });
        }
        void UpdateProgress(int progress, string message)
        {
            viewModel.UpdateProgress = progress;
            viewModel.UpdateMessage = message;
        }
        void WriteLineSafely(string message)
        {
            Dispatcher.Invoke(() =>
            {
                WriteLine(message);
            });
        }
        void WriteLine(string message)
        {
            viewModel.Message = message;
        }
        void OnClose()
        {
            Application.Current.Shutdown();
        }

    }
}
