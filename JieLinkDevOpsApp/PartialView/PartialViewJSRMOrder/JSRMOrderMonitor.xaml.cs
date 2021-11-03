using PartialViewInterface;
using PartialViewJSRMOrder.ViewModel;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PartialViewJSRMOrder
{
    /// <summary>
    /// JSRMOrderMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class JSRMOrderMonitor : UserControl, IPartialView
    {
        OrderMonitorViewModel viewModel;

        public JSRMOrderMonitor()
        {
            InitializeComponent();
            viewModel = OrderMonitorViewModel.Instance();
            this.gridOder.DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "运维工单"; }
        }

        public string TagName
        {
            get { return "JSRMOrderMonitor"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 800; }
        }

        bool isLoad = false;
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoad)
            {
                viewModel.Load();//避免每次点进来 都触发
            }

            isLoad = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.TextChanaged(txtVerifyCode.Text);
        }
    }
}
