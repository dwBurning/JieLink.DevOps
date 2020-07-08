using JieShun.JieLink.DevOps.App.Models;
using Panuon.UI.Silver.Core;
using System.Collections.ObjectModel;

namespace JieShun.JieLink.DevOps.App.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public MainWindowViewModel()
        {
            MenuItems = new ObservableCollection<TreeViewItemModel>()
            {
                new TreeViewItemModel("Introduction","Introduction", "\uf05a"),
                new TreeViewItemModel("Overview","Overview", "\uf0eb"),
                new TreeViewItemModel("NativeControls","NativeControls", "\uf17a")
                {
                     MenuItems = new ObservableCollection<TreeViewItemModel>()
                     {
                         new TreeViewItemModel("Button","Button"),
                         new TreeViewItemModel("TextBox","TextBox"),
                     }
                }
            };
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; NotifyPropertyChanged(); OnSearchTextChanged(); }
        }


        private string _searchText;

        public ObservableCollection<TreeViewItemModel> MenuItems { get; } = new ObservableCollection<TreeViewItemModel>();

        #region Event
        private void OnSearchTextChanged()
        {
            foreach (var item in MenuItems)
            {
                ChangeItemVisibility(item);
            }
        }

        private bool ChangeItemVisibility(TreeViewItemModel model)
        {
            var result = false;

            if (model.Header.ToLower().Contains(SearchText.ToLower()))
                result = true;

            if (model.MenuItems.Count != 0)
            {
                foreach (var item in model.MenuItems)
                {
                    var inner = ChangeItemVisibility(item);
                    result = result ? true : inner;
                }
            }

            model.Visibility = result ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            model.IsExpanded = result;
            return result;
        }

        #endregion

    }
}