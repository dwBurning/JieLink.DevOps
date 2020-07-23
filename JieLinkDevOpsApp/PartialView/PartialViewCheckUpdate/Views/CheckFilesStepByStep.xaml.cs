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
    /// UpdateFilesStepByStep.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateFilesStepByStep : UserControl
    {
        public UpdateFilesStepByStep()
        {
            InitializeComponent();
            List<string> messages = new List<string>();
            messages.Add(@"1.手工替换文件的试升级中心
                            \n1.1 升级SmartCenter
                            \n1.1.1 找到SmartCenter安装目录
                            \n在任务管理中找到“SmartCenter.Host.exe”中右键 - 选择“打开文件位置”。");

            step1.Text = messages[0];
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

        }
    }
}
