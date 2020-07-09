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

namespace PartialViewTest
{
    /// <summary>
    /// BoxPartialView.xaml 的交互逻辑
    /// </summary>
    public partial class BoxPartialView : UserControl,IPartialView
    {
        public BoxPartialView()
        {
            InitializeComponent();
        }

        public string MenuName
        {
            get { return "测试"; }
        }

        public string Tag
        {
            get { return "Box"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Box; }
        }
    }
}
