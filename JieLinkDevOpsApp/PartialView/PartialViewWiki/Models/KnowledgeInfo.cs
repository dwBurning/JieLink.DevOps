using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewWiki.Models
{
    public class KnowledgeInfo : PropertyChangedBase
    {
        public ActionType Type { get; set; }

        public string Id { get; set; }

        public string Knowledge { get; set; }

        public string KeyWords { get; set; }

        public string Image { get; set; }

        public string Solution { get; set; }

        public Visibility Visibility
        {
            get { return _visibility; }
            set { _visibility = value; NotifyPropertyChanged(); }
        }
        private Visibility _visibility = Visibility.Visible;
    }

    public enum ActionType
    {
        //动图 演示 操作步骤 文件名取自Id字段
        Gif = 0,

        //文本 描述 操作步骤 文本的值取自Solution字段 使用|换行
        Text = 1,

        //程序 操作 解决问题 程序执行的方法 需要在SoftExecute类中添加 方法名称与Id相同
        Soft = 2,

        //Url 跳转链接 url地址取自Solution字段
        Link = 3
    }
}
