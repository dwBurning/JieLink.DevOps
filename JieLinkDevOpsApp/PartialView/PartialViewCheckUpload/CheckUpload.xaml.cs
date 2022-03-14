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

using PartialViewInterface;
using PartialViewInterface.Utils;
using MySql.Data.MySqlClient;
using PartialViewCheckUpload.ViewModels;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;

namespace PartialViewCheckUpload
{
    public partial class CheckUpload : UserControl, IPartialView
    {
        CheckUploadViewModels viewModel;
        public CheckUpload()
        {
            InitializeComponent();
            viewModel = new CheckUploadViewModels();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "上传检测"; }
        }

        public string TagName
        {
            get { return "CheckUpload"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 800; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Global.ValidV2(new Action<string, bool>((message, result) =>
            {
                if (!result)
                {
                    MessageBoxHelper.MessageBoxShowWarning(message);
                }

                this.IsEnabled = result;
            }));
        }

        private void dgresultTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.ChooseChange(((System.Windows.Controls.DataGrid)(sender)).SelectedIndex);
        }

        private void StartDateTimeChanged(object sender, SelectedDateTimeChangedEventArgs e)
        {
            viewModel.StartDateChange(((DateTimePicker)sender).SelectedDateTime);
        }

        private void EndDateTimeChanged(object sender, SelectedDateTimeChangedEventArgs e)
        {
            viewModel.EndDateChange(((DateTimePicker)sender).SelectedDateTime);
        }
    }
}
