using PartialViewInterface;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

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

        public int Order
        {
            get { return 800; }
        }

        private void Hyperlink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            // 激活的是当前默认的浏览器
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }
    }
}
