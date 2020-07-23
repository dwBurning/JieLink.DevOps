using PartialViewCheckUpdate.Views;
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

namespace PartialViewCheckUpdate
{
    /// <summary>
    /// CheckUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class CheckUpdate : UserControl, IPartialView
    {
        CheckFiles checkFiles;
        CheckScripts checkScripts;
        UpdateFilesStepByStep updateFilesStepByStep;
        public CheckUpdate()
        {
            InitializeComponent();
            checkFiles = new CheckFiles();
            checkFiles.UpdateFaildNotify += CheckFiles_UpdateFaildNotify;
            checkScripts = new CheckScripts();
            updateFilesStepByStep = new UpdateFilesStepByStep();

            ContentControl.Content = checkFiles;
        }

        private void CheckFiles_UpdateFaildNotify()
        {
            ContentControl.Content = updateFilesStepByStep;
        }

        public string MenuName
        {
            get { return "检查升级"; }
        }

        public string TagName
        {
            get { return "CheckUpdate"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        private void TvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            var selectedItem = TvMenu.SelectedItem as TreeViewItem;
            var tag = selectedItem.Tag.ToString();
            if (tag == "CheckFiles")
            {
                ContentControl.Content = checkFiles;
            }
            else
            {
                ContentControl.Content = checkScripts;
            }
        }
    }
}
