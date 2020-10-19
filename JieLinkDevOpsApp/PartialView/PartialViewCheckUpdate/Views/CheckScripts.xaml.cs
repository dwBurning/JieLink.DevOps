using Panuon.UI.Silver;
using PartialViewCheckUpdate.ViewModels;
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

namespace PartialViewCheckUpdate.Views
{
    /// <summary>
    /// CheckScripts.xaml 的交互逻辑
    /// </summary>
    public partial class CheckScripts : UserControl
    {
        public event Action<string> UpdateFaildNotify;

        private CheckScriptViewModel viewModel;

        public CheckScripts()
        {
            InitializeComponent();
            viewModel = new CheckScriptViewModel();
            viewModel.UpdateFaildNotify += ViewModel_UpdateFaildNotify;
            DataContext = viewModel;
            
        }

        private void ViewModel_UpdateFaildNotify(string obj)
        {
            UpdateFaildNotify?.Invoke("CheckScripts");
        }

        private void btnExecuteStepByStep_Click(object sender, RoutedEventArgs e)
        {
            UpdateFaildNotify?.Invoke("CheckScripts");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EnvironmentInfo.ConnectionString))
            {
                viewModel.CenterDb = EnvironmentInfo.DbConnEntity.DbName;
                viewModel.CenterDbPort = EnvironmentInfo.DbConnEntity.Port.ToString();
                viewModel.CenterDbPwd = EnvironmentInfo.DbConnEntity.Password;
                viewModel.CenterDbUser = EnvironmentInfo.DbConnEntity.UserName;
                viewModel.CenterIp = EnvironmentInfo.DbConnEntity.Ip;
            }
        }
    }
}
