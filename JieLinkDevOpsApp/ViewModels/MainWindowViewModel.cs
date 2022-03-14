﻿using JieShun.JieLink.DevOps.App.Models;
using Panuon.UI.Silver.Core;
using PartialViewInterface;
using PartialViewInterface.Utils;
using Quartz;
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
        public List<IStartup> startups = new List<IStartup>();
        public List<Type> jobs = new List<Type>();
        public static IDictionary<string, IPartialView> partialViewDic;
        public MainWindowViewModel()
        {
            Title = $"JieLink运维工具 {EnvironmentInfo.CurrentVersion}";

            partialViewDic = new Dictionary<string, IPartialView>();
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = BaseDirectoryPath + "plugs";
            List<string> plugs = FileHelper.GetAllFiles(path);

            foreach (var plug in plugs)
            {
                //解决样式不生效问题
                if (!plug.Contains("PartialView"))
                    continue;
                var asm = Assembly.LoadFile(plug);
                asm.GetTypes()
                .Where(t => typeof(IPartialView).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && t.IsClass && t.IsSubclassOf(typeof(UserControl)))
                .Select(t => (IPartialView)Activator.CreateInstance(t)).ToList() 
                .ForEach(x => partialViewDic.Add(x.TagName, x));
                //获取插件的Startup启动类
                startups.AddRange(asm.GetExportedTypes()
                .Where(x => typeof(IStartup).IsAssignableFrom(x)).ToList()
                .Select(x => (IStartup)Activator.CreateInstance(x)).ToList());
                startups.Sort((a, b) => b.Priority - a.Priority);
                //获取cron后台定时任务
                jobs.AddRange(asm.GetExportedTypes()
                .Where(x => typeof(IJob).IsAssignableFrom(x)).ToList());
            }

            var centerMenus = new ObservableCollection<TreeViewItemModel>();

            partialViewDic.Values.Where(x => x.MenuType == MenuType.Center).OrderBy(x => x.Order).ToList()
            .ForEach(x => centerMenus.Add(new TreeViewItemModel(x.MenuName, x.TagName)));

            var centerV3Menus = new ObservableCollection<TreeViewItemModel>();

            partialViewDic.Values.Where(x => x.MenuType == MenuType.CenterV3).OrderBy(x => x.Order).ToList()
            .ForEach(x => centerV3Menus.Add(new TreeViewItemModel(x.MenuName, x.TagName)));

            var commonMenus = new ObservableCollection<TreeViewItemModel>();

            partialViewDic.Values.Where(x => x.MenuType == MenuType.Common).OrderBy(x => x.Order).ToList()
            .ForEach(x => commonMenus.Add(new TreeViewItemModel(x.MenuName, x.TagName)));

            var boxMenus = new ObservableCollection<TreeViewItemModel>();
            partialViewDic.Values.Where(x => x.MenuType == MenuType.Box).OrderBy(x => x.Order).ToList()
            .ForEach(x => boxMenus.Add(new TreeViewItemModel(x.MenuName, x.TagName)));

            var doorServerMenus = new ObservableCollection<TreeViewItemModel>();
            partialViewDic.Values.Where(x => x.MenuType == MenuType.DoorServer).OrderBy(x => x.Order).ToList()
            .ForEach(x => doorServerMenus.Add(new TreeViewItemModel(x.MenuName, x.TagName)));

            MenuItems = new ObservableCollection<TreeViewItemModel>()
            {
                new TreeViewItemModel("设计","Information","\uf05a"){ IsSelected = true},
                new TreeViewItemModel("JieLink2.X","Center", "\uf17a")
                {
                    MenuItems = centerMenus,
                    IsExpanded = false
                },
                new TreeViewItemModel("JieLink3.X","CenterV3", "\uf17a")
                {
                    MenuItems = centerV3Menus,
                    IsExpanded = false
                },

                new TreeViewItemModel("公共","Common", "\uf109")
                {
                     MenuItems = commonMenus,
                     IsExpanded = false
                },

                //new TreeViewItemModel("盒子","Box", "\uf109")
                //{
                //     MenuItems = boxMenus,
                //     IsExpanded = false
                //},

                //new TreeViewItemModel("门禁","DoorServer", "\uf1ad")
                //{
                //     MenuItems = doorServerMenus,
                //     IsExpanded = false
                //},
                new TreeViewItemModel("百科","KnowledgeWiki","\uf266"),
                new TreeViewItemModel("设置","SystemSetting","\uf085"),
                new TreeViewItemModel("版本","VersionUpdate","\uf1d8"),
            };
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(); }
        }



        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; NotifyPropertyChanged(); OnSearchTextChanged(); }
        }


        private string _searchText;

        public ObservableCollection<TreeViewItemModel> MenuItems { get; private set; }

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