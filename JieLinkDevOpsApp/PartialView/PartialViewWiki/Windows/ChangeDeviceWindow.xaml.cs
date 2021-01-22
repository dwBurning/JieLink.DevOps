using Panuon.UI.Silver;
using PartialViewWiki.ViewModels;
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
using System.Windows.Shapes;

namespace PartialViewWiki.Windows
{
    /// <summary>
    /// ChangeDevice.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeDeviceWindow : WindowX
    {
        ChangeDeviceViewModel viewModel;

        public ChangeDeviceWindow()
        {
            InitializeComponent();
            viewModel = new ChangeDeviceViewModel();
            this.DataContext = viewModel;
        }

        private void cmbDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SelectChangeDevice.SelectedDevice = viewModel.SelectDevice;
        }
    }
}
