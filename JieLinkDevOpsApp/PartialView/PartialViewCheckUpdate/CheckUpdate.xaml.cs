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
        CheckFilesStepByStep checkFilesStepByStep;
        CheckScriptStepByStep checkScriptStepByStep;
        public CheckUpdate()
        {
            InitializeComponent();
            checkFiles = new CheckFiles();
            checkFiles.UpdateFaildNotify += CheckFiles_UpdateFaildNotify;
            checkScripts = new CheckScripts();
            checkScripts.UpdateFaildNotify += CheckFiles_UpdateFaildNotify;
            checkFilesStepByStep = new CheckFilesStepByStep();
            checkFilesStepByStep.BackToCheckFile += CheckFilesStepByStep_BackToCheckFile;
            checkScriptStepByStep = new CheckScriptStepByStep();
            checkScriptStepByStep.BackToCheckScript += CheckScriptStepByStep_BackToCheckScript;
            ContentControl.Content = checkFiles;
        }

        private void CheckScriptStepByStep_BackToCheckScript()
        {
            ContentControl.Content = checkScripts;
        }

        private void CheckFilesStepByStep_BackToCheckFile()
        {
            ContentControl.Content = checkFiles;
        }

        private void CheckFiles_UpdateFaildNotify(string action)
        {
            if (action == "CheckFiles")
            {
                ContentControl.Content = checkFilesStepByStep;
            }
            else
            {
                ContentControl.Content = checkScriptStepByStep;
            }
            
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
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
