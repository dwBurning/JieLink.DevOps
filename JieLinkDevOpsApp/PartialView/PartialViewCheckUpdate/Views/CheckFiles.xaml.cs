using PartialViewCheckUpdate.Models.Enum;
using PartialViewCheckUpdate.ViewModels;
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

namespace PartialViewCheckUpdate.Views
{
    /// <summary>
    /// CheckFiles.xaml 的交互逻辑
    /// </summary>
    public partial class CheckFiles : UserControl
    {
        public event Action<string> UpdateFaildNotify;
        private CheckFilesViewModel viewModel { get; set; }

        public CheckFiles()
        {
            InitializeComponent();
            viewModel = new CheckFilesViewModel();
            viewModel.UpdateFaildNotify += ViewModel_UpdateFaildNotify;
            DataContext = viewModel;
        }

        private void ViewModel_UpdateFaildNotify(string action)
        {
            UpdateFaildNotify?.Invoke(action);
        }

        private void btnChooseInstallPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.InstallPath = folderBrowserDialog.SelectedPath.Trim();
            }
        }

        private void btnChooseSetUpPackagePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.SetUpPackagePath = folderBrowserDialog.SelectedPath.Trim();
            }
        }

        private void btnUpdateStepByStep_Click(object sender, RoutedEventArgs e)
        {
            UpdateFaildNotify?.Invoke("CheckFiles");
        }
    }
}
