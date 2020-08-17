using JieShun.JieLink.DevOps.App.Models;
using JieShun.JieLink.DevOps.App.ViewModels;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using PartialViewInterface;
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json;
using PartialViewInterface.Utils;
using PartialViewInterface.Models;
using Quartz;
using Quartz.Impl;
using System.Windows.Forms;

namespace JieShun.JieLink.DevOps.App
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX, IComponentConnector
    {
        BackgroundWorker backgroundWorker = new BackgroundWorker();

        #region Property
        private MainWindowViewModel viewModel;

        public string Text { get; set; }

        private NotifyIcon notifyIcon;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            ContentControl.Content = MainWindowViewModel.partialViewDic["Information"];//加载介绍窗口


            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Text = "JieLink运维工具持续工作中...";
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            //打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("打开");
            open.Click += Show;
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += Exit;
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left) this.Show(o, e);
            });

        }

        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        bool isExit = false;
        private void Exit(object sender, EventArgs e)
        {
            this.Show(sender, e);
            if (MessageBoxHelper.MessageBoxShowQuestion("退出后将无法实时监控JieLink软件的运行状态，确定退出么？") == MessageBoxResult.Yes)
            {
                isExit = true;
                StdSchedulerFactory.GetDefaultScheduler().Shutdown();
                foreach (var startup in viewModel.startups)
                {
                    startup.Exit();
                }
                this.Close();
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProjectInfoWindow windowX = new ProjectInfoWindow();
            this.IsMaskVisible = true;
            windowX.ShowDialog();
            this.IsMaskVisible = false;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            backgroundWorker.ReportProgress(1);
        }

        #region EventHandler
        private void TvMenu_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            var selectedItem = TvMenu.SelectedItem as TreeViewItemModel;
            var tag = selectedItem.Tag;
            if (tag.IsNullOrEmpty())
                return;

            if (MainWindowViewModel.partialViewDic.ContainsKey(tag))
            { ContentControl.Content = MainWindowViewModel.partialViewDic[tag]; }
            else
            { ContentControl.Content = null; }
        }
        #endregion

        private void WindowX_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isExit)
            {
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                this.notifyIcon.ShowBalloonTip(3, "提示", "运维工具已最小化到系统托盘", ToolTipIcon.Info);
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            string url = ConfigHelper.ReadAppConfig("ServerUrl");
            EnvironmentInfo.ServerUrl = url;
            string projectInfoConfig = ConfigHelper.ReadAppConfig("ProjectInfo");
            if (string.IsNullOrEmpty(projectInfoConfig))
            {
                if (!backgroundWorker.IsBusy)
                {
                    backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                ProjectInfo projectInfo = JsonConvert.DeserializeObject<ProjectInfo>(projectInfoConfig);
                EnvironmentInfo.ProjectNo = projectInfo.ProjectNo;
                EnvironmentInfo.RemoteAccount = projectInfo.RemoteAccount;
                EnvironmentInfo.RemotePassword = projectInfo.RemotePassword;
                EnvironmentInfo.ContactName = projectInfo.ContactName;
                EnvironmentInfo.ContactPhone = projectInfo.ContactPhone;
            }
            //运行插件的启动方法
            foreach (var startup in viewModel.startups)
            {
                startup.Start();
            }
            //运行后台任务
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            foreach (var jobType in viewModel.jobs)
            {
                string cron = ConfigHelper.GetValue<string>(jobType.Name, "");
                if (string.IsNullOrEmpty(cron))
                    continue;
                var job = JobBuilder.Create(jobType)
                                .Build();
                var trigger = TriggerBuilder.Create()
                                .StartNow()
                                .WithCronSchedule(cron)
                                .Build();
                scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}
