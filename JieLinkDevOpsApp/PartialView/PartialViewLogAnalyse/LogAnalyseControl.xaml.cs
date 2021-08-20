using PartialViewInterface;
using PartialViewLogAnalyse.ViewModels;
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

namespace PartialViewLogAnalyse
{
    /// <summary>
    /// LogAnalyseControl.xaml 的交互逻辑
    /// </summary>
    public partial class LogAnalyseControl : UserControl, IPartialView
    {
        public LogAnalyseControl()
        {
            InitializeComponent();
            DataContext = new LogAnalyseViewModel();
        }

        public string MenuName
        {
            get { return "日志分析"; }
        }

        public string TagName
        {
            get { return "LogAnalyse"; }
        }

        public MenuType MenuType
        {
            get { return MenuType.Center; }
        }

        public int Order
        {
            get { return 800; }
        }
    }
}
