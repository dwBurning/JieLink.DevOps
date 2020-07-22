using Panuon.UI.Silver;
using PartialViewCheckUpdate.ViewModels;
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

namespace PartialViewCheckUpdate.Views
{
    /// <summary>
    /// CheckUpdatePartialView2.xaml 的交互逻辑
    /// </summary>
    public partial class CheckUpdatePartialView2 : UserControl, IPartialView
    {
        public CheckUpdatePartialView2()
        {
            InitializeComponent();
            CarouselText.Index = 0;
            CarouselImg.Index = 0;
            CarouselText2.Index = 0;
            CarouselImg2.Index = 0;
            this.DataContext = new CheckUpdateViewModel();
        }

        public string MenuName
        {
            get { return "检查升级"; }
        }

        public string TagName
        {
            get { return "update"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            string imgSource = string.Empty;
            try
            {
                switch (img.Tag.ToString())
                {
                    case "0":
                        imgSource = "..\\Pic\\0.png";
                        break;
                    case "1":
                        imgSource = "..\\Pic\\1.png";
                        break;
                    case "2":
                        imgSource = "..\\Pic\\2.png";
                        break;
                    case "3":
                        imgSource = "..\\Pic\\3.png";
                        break;
                    case "4":
                        imgSource = "..\\Pic\\4.png";
                        break;
                    case "5":
                        imgSource = "..\\Pic\\5.png";
                        break;
                    case "6":
                        imgSource = "..\\Pic\\6.png";
                        break;
                    case "7":
                        imgSource = "..\\Pic\\7.png";
                        break;
                    case "8":
                        imgSource = "..\\Pic\\8.png";
                        break;
                    case "9":
                        imgSource = "..\\Pic\\9.png";
                        break;
                    case "00":
                        imgSource = "..\\Pic\\00.png";
                        break;
                    case "01":
                        imgSource = "..\\Pic\\01.png";
                        break;
                    case "02":
                        imgSource = "..\\Pic\\02.png";
                        break;
                    case "03":
                        imgSource = "..\\Pic\\03.png";
                        break;
                    case "04":
                        imgSource = "..\\Pic\\04.png";
                        break;
                    case "05":
                        imgSource = "..\\Pic\\05.png";
                        break;
                    case "06":
                        imgSource = "..\\Pic\\06.png";
                        break;
                    case "07":
                        imgSource = "..\\Pic\\07.png";
                        break;
                    case "08":
                        imgSource = "..\\Pic\\08.png";
                        break;
                    case "09":
                        imgSource = "..\\Pic\\09.png";
                        break;
                    case "010":
                        imgSource = "..\\Pic\\010.png";
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(imgSource))
                {
                    return;
                }
                Window window = null;
                window = new ImgWindow(imgSource);
                window.Show();
            }
            catch (Exception)
            {
                return;
            } 
        }

        private void BtnDec_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnDec":
                    CarouselText.Index--;
                    CarouselImg.Index--;
                    break;
                case "btnDec2":
                    CarouselText2.Index--;
                    CarouselImg2.Index--;
                    break;
                
                default:
                    break;
            }
            
        }

        private void BtnInc_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "btnInc":
                    CarouselText.Index++;
                    CarouselImg.Index++;
                    break;
                case "btnInc2":
                    CarouselText2.Index++;
                    CarouselImg2.Index++;
                    break;

                default:
                    break;
            }
        }
        
    }
}
