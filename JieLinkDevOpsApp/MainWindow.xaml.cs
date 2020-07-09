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

namespace JieShun.JieLink.DevOps.App
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX, IComponentConnector
    {
        #region Identity
        private static IDictionary<string, Type> _partialViewDic;
        #endregion

        #region Property
        public MainWindowViewModel ViewModel { get; set; }

        public string Text { get; set; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
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

            if (_partialViewDic.ContainsKey(tag))
                ContentControl.Content = Activator.CreateInstance(_partialViewDic[tag]);
            else
                ContentControl.Content = null;
        }
        #endregion

        private void WindowX_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
