using PartialViewEncrypter.ViewModels;
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

namespace PartialViewEncrypter
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class Encrypter : UserControl, IPartialView
    {
        EncrypterViewModel viewModel;
        public Encrypter()
        {
            InitializeComponent();
            viewModel = new EncrypterViewModel();
            DataContext = viewModel;
        }


        public string MenuName
        {
            get { return "加/解密"; }
        }

        public string TagName
        {
            get { return "Encrypter"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.CenterV3; }
        }

        public int Order
        {
            get { return 800; }
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
