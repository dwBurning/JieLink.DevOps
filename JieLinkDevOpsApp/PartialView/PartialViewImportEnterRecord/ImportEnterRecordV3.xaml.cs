using PartialViewImportEnterRecord.ViewModels;
using PartialViewInterface;
using PartialViewInterface.Utils;
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

namespace PartialViewImportEnterRecord
{
    /// <summary>
    /// ImportEnterRecordV3.xaml 的交互逻辑
    /// </summary>
    public partial class ImportEnterRecordV3 : UserControl, IPartialView
    {
        ImportEnterRecordViewModel3 viewModel;
        public ImportEnterRecordV3()
        {
            InitializeComponent();
            viewModel = new ImportEnterRecordViewModel3();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "导入记录"; }
        }

        public string TagName
        {
            get { return "ImportEnterRecordV3"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.CenterV3; }
        }

        public int Order
        {
            get { return 800; }
        }

        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.FilePath = fileDialog.FileName.Trim();
                viewModel.canExecute = true;
            }
        }

        private void OpenTemplate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "Template");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Global.ValidV3(new Action<string, bool>((message, result) =>
            {
                if (!result)
                {
                    MessageBoxHelper.MessageBoxShowWarning(message);
                }

                this.IsEnabled = result;
            }));
        }
    }
}
