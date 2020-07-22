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

namespace PartialViewSetting
{
    /// <summary>
    /// SystemSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting : UserControl,IPartialView
    {
        public SystemSetting()
        {
            InitializeComponent();
        }

        public string MenuName
        {
            get { return "设置"; }
        }

        public string TagName
        {
            get { return "SystemSetting"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.None; }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
