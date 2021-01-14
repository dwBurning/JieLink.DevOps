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
using PartialViewExportFacePic.ViewModels;

namespace PartialViewExportFacePic
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ExportFacePic : UserControl, IPartialView
    {
        ExportFacePicOptViewModel viewModel;
        public ExportFacePic()
        {
            InitializeComponent();
            viewModel = new ExportFacePicOptViewModel();
            DataContext = viewModel;
        }

        public string MenuName
        {
            get { return "人脸导出"; }
        }

        public string TagName
        {
            get { return "ExportPicBackUp"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.DoorServer; }
        }

        private void RichTextBox_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            RichTextBox_Text.ScrollToEnd();
        }

        /// <summary>
        /// 右击菜单清屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox_Text.Document.Blocks.Clear();
        }
    }
}
