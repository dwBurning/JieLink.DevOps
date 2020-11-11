using PartialViewCheckUpdate.ViewModels;
using PartialViewInterface;
using System.Windows;
using System.Windows.Controls;


namespace PartialViewCheckUpdate
{
    /// <summary>
    /// CheckUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class CheckUpdate : UserControl, IPartialView
    {
        CheckUpdateViewModel viewModel;

        public CheckUpdate()
        {
            InitializeComponent();
            viewModel = new CheckUpdateViewModel();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "升级辅助"; }
        }

        public string TagName
        {
            get { return "CheckUpdate"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
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
                viewModel.PackagePath = folderBrowserDialog.SelectedPath.Trim();
            }
        }
    }
}
