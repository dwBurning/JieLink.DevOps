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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PartialViewInterface;
using System.Reflection;
using System.ComponentModel;
using System.Threading;

namespace JieShun.JieLink.DevOps.App
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX, IComponentConnector
    {
        BackgroundWorker backgroundWorker = new BackgroundWorker();

        #region Property
        public MainWindowViewModel ViewModel { get; set; }

        public string Text { get; set; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            ContentControl.Content = MainWindowViewModel.partialViewDic["Information"];//加载介绍窗口

            
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            
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
            foreach (var startup in ViewModel.startups)
            {
                startup.Exit();
            }
            Application.Current.Shutdown();
        }

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var startup in ViewModel.startups)
            {
                startup.Start();
            }
        }
    }
}
