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

namespace PartialViewDelRegeist
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class BoxDelRegist : UserControl, IPartialView
    {
        public BoxDelRegist()
        {
            InitializeComponent();
        }

        public string MenuName
        {
            get { return "删除盒子注册表"; }
        }

        public string TagName
        {
            get { return "BoxDelRegist"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order { get { return 1000; } }

        private void button_DelRegist_Click(object sender, RoutedEventArgs e)
        {
            SetAutoRunProject.Instance().DoWork(1);
        }

    }
}
