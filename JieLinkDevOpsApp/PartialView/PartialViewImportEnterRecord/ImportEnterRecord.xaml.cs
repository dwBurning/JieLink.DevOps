using MySql.Data.MySqlClient;
using PartialViewImportEnterRecord.ViewModels;
using PartialViewInterface;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// ImportEnterRecord.xaml 的交互逻辑
    /// </summary>
    public partial class ImportEnterRecord : UserControl, IPartialView
    {
        ImportEnterRecordViewModel viewModel;

        public ImportEnterRecord()
        {
            InitializeComponent();
            viewModel = new ImportEnterRecordViewModel();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "导入记录"; }
        }

        public string TagName
        {
            get { return "ImportEnterRecord"; }
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
    }
}
