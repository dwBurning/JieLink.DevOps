using PartialViewInterface;
using System.Windows.Controls;


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
