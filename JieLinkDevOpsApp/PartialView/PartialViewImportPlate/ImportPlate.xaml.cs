using MySql.Data.MySqlClient;
using PartialViewImportPlate.ViewModels;
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

namespace PartialViewImportPlate
{
    /// <summary>
    /// ImportPlate.xaml 的交互逻辑
    /// </summary>
    public partial class ImportPlate : UserControl, IPartialView
    {
        ImportPlateViewModel viewModel;

        public ImportPlate()
        {
            InitializeComponent();
            viewModel = new ImportPlateViewModel();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "导入车牌"; }
        }

        public string TagName
        {
            get { return "ImportPlate"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, "select * from sys_user limit 1");
                this.IsEnabled = true;
            }
            catch (Exception)
            {
                MessageBoxHelper.MessageBoxShowWarning("请先在【设置】菜单中配置数据库连接");
                this.IsEnabled = false;
            }
        }

        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                viewModel.FilePath = fileDialog.FileName.Trim();
            }
        }
    }
}
