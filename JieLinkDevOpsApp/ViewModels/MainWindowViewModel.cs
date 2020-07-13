using JieShun.JieLink.DevOps.App.Models;
using Panuon.UI.Silver.Core;
using PartialViewInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace JieShun.JieLink.DevOps.App.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public static IDictionary<string, IPartialView> partialViewDic;
        public MainWindowViewModel()
        {
            partialViewDic = new Dictionary<string, IPartialView>();
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = BaseDirectoryPath + "plugs";
            List<string> plugs = FileHelper.GetAllFiles(path);

            foreach (var plug in plugs)
            {
                //解决样式不生效问题
                if (plug.Contains("Panuon.UI.Silver.dll"))
                    continue;

                Assembly.LoadFile(plug).GetTypes()
               .Where(t => typeof(IPartialView).IsAssignableFrom(t)) //获取间接或直接继承t的所有类型
               .Where(t => !t.IsAbstract && t.IsClass && t.IsSubclassOf(typeof(UserControl))) //获取非抽象类 排除接口继承
               .Select(t => (IPartialView)Activator.CreateInstance(t)).ToList() //创造实例，并返回结果（项目需求，可删除）
               .ForEach(x => partialViewDic.Add(x.Tag, x));
            }

            var centerMenus = new ObservableCollection<TreeViewItemModel>();

            partialViewDic.Values.Where(x => x.MenuType == MenuType.Center).ToList()
            .ForEach(x => centerMenus.Add(new TreeViewItemModel(x.MenuName, x.Tag)));

            var boxMenus = new ObservableCollection<TreeViewItemModel>();
            partialViewDic.Values.Where(x => x.MenuType == MenuType.Box).ToList()
            .ForEach(x => boxMenus.Add(new TreeViewItemModel(x.MenuName, x.Tag)));

            MenuItems = new ObservableCollection<TreeViewItemModel>()
            {
                new TreeViewItemModel("中心","Center", "\uf17a"){
                MenuItems = centerMenus
                },
                new TreeViewItemModel("盒子","Box", "\uf03d")
                {
                     MenuItems = boxMenus
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