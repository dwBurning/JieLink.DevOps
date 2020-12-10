using Newtonsoft.Json;
using Panuon.UI.Silver.Core;
using PartialViewWiki.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewWiki.ViewModels
{
    public class KnowledgeWikiViewModel : PropertyChangedBase
    {
        public ObservableCollection<KnowledgeInfo> Knowledges { get; set; }

        public KnowledgeWikiViewModel()
        {
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = BaseDirectoryPath + "plugs\\Knowledges.json";
            string jsonData = File.ReadAllText(path);
            Knowledges = JsonConvert.DeserializeObject<ObservableCollection<KnowledgeInfo>>(jsonData);
        }

        public string KnowledgeKey
        {
            get { return _knowledgeKey; }
            set { _knowledgeKey = value; NotifyPropertyChanged(); OnSearchTextChanged(); }
        }


        private string _knowledgeKey;

        private void OnSearchTextChanged()
        {
            foreach (var knowledge in Knowledges)
            {
                ChangeItemVisibility(knowledge);
            }
        }

        private bool ChangeItemVisibility(KnowledgeInfo knowledgeInfo)
        {
            var result = false;

            if (knowledgeInfo.KeyWords.ToLower().Contains(KnowledgeKey.ToLower()))
                result = true;

            knowledgeInfo.Visibility = result ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            return result;
        }
    }
}
