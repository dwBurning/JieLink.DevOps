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

namespace PartialViewInformation
{
    /// <summary>
    /// Information.xaml 的交互逻辑
    /// </summary>
    public partial class Information : UserControl, IPartialView
    {
        public Information()
        {
            InitializeComponent();
        }

        public string MenuName
        {
            get
            { return "介绍"; }
        }

        public string TagName
        {
            get
            { return "Information"; }
        }

        public MenuType MenuType
        {
            get
            { return MenuType.None; }
        }
    }
}
