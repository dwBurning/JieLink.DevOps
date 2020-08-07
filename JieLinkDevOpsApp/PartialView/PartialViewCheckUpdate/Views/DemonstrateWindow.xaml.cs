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
using WpfAnimatedGif;

namespace PartialViewCheckUpdate.Views
{
    /// <summary>
    /// DemonstrateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DemonstrateWindow : WindowX
    {
        public DemonstrateWindow(string imagePath, string title)
        {
            InitializeComponent();

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(imagePath);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(gifImage, image);
            this.Title = title;
        }
    }
}
