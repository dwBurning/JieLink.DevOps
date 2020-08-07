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
using SetWasgoneForSmartBox;

namespace PartialViewRegeist
{
    /// <summary>
    /// BoxPartialRegist.xaml 的交互逻辑
    /// </summary>

    public partial class BoxPartialRegist : UserControl, IPartialView
    {
        public BoxPartialRegist()
        {
            InitializeComponent();
        }

        public string MenuName
        {
            get { return "设置开机启动项"; }
        }

        public string TagName
        {
            get { return "BoxPartialRegist"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Box; }
        }

        private void button_RegistOpenBox_Click(object sender, RoutedEventArgs e)
        {
            SetAutoRunProject.Instance().DoWork(0);
        }

        private void button_RegistOpenExplorer_Click(object sender, RoutedEventArgs e)
        {
            SetAutoRunProject.Instance().DoWork(1);
        }
    }
}
