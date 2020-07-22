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
using System.Windows.Shapes;

namespace PartialViewCheckUpdate.Views
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class ImgWindow : WindowX
    {
        public ImgWindow()
        {
            InitializeComponent();
        }
        public ImgWindow(string imgSource)
        {
            InitializeComponent();
            img.Source = new BitmapImage(new Uri(imgSource, UriKind.Relative));
        }

        private void Img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ForceClose();
        }
    }
}
