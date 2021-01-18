using PartialViewImportBill.ViewModels;
using PartialViewInterface;
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

namespace PartialViewImportBill
{
    /// <summary>
    /// ImportBill.xaml 的交互逻辑
    /// </summary>
    public partial class ImportBill : UserControl, IPartialView
    {
        ImportBillViewModel viewModel;

        public ImportBill()
        {
            InitializeComponent();
            viewModel = new ImportBillViewModel();
            this.DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "补录订单"; }
        }

        public string TagName
        {
            get { return "ImportBill"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        private void btnChooseInstallPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.FilePath = fileDialog.FileName.Trim();
                viewModel.canExecute = true;
            }
        }

        private void CleanText_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Message = "";
        }
    }
}
