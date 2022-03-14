using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewInterface.Commands;
using PartialViewInterface.Utils;
using PartialViewOtherToJieLink.JSDSViewModels;
using PartialViewOtherToJieLink.Models;
using PartialViewOtherToJieLink.ViewModels;

namespace PartialViewOtherToJieLink
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class JsdsOneClickUpgradeToJieLink : UserControl, IPartialView
    {
        public string MenuName
        {
            get { return "改造升级"; }
        }

        public string TagName
        {
            get { return "PartialViewOtherToJieLink"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 800; }
        }

        JSDSToJieLinkViewModel viewModelJSDS;

        JSRJ1116ToJieLinkViewModel viewModelRJ1116;

        JSOCT2017ToJieLinkViewModel viewModelJSOCT2017;

        public JsdsOneClickUpgradeToJieLink()
        {
            InitializeComponent();
            viewModelJSDS = new JSDSToJieLinkViewModel();
            this.gridJSDS.DataContext = viewModelJSDS;

            viewModelRJ1116 = new JSRJ1116ToJieLinkViewModel();
            this.gridJSRJ1116.DataContext = viewModelRJ1116;

            viewModelJSOCT2017 = new JSOCT2017ToJieLinkViewModel();
            this.gridJSOCT2017.DataContext = viewModelJSOCT2017;
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
    }
}