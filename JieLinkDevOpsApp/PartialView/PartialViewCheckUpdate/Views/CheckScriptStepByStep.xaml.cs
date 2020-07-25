using Panuon.UI.Silver;
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
    /// CheckScriptStepByStep.xaml 的交互逻辑
    /// </summary>
    public partial class CheckScriptStepByStep : UserControl
    {
        public CheckScriptStepByStep()
        {
            InitializeComponent();
        }

        private void btnDec_Click(object sender, RoutedEventArgs e)
        {
            CarouselImg.Index--;
        }

        private void btnInc_Click(object sender, RoutedEventArgs e)
        {
            CarouselImg.Index++;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                CheckUpdateContext.ShowDemonstrateWindow("CheckScripts","检查脚本");
            }
        }
    }
}
