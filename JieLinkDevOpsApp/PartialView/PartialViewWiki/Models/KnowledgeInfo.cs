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
        public string Id { get; set; }

        public string Knowledge { get; set; }

        public string KeyWords { get; set; }

        public string Image { get; set; }

        public Visibility Visibility
        {
            get { return _visibility; }
            set { _visibility = value; NotifyPropertyChanged(); }
        }
        private Visibility _visibility = Visibility.Visible;
    }
}
