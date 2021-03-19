using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Panuon.UI.Silver.Core;

namespace PartialViewCheckUpload.Info
{
    class SyncInfo : PropertyChangedBase
    {
        string _addtime;
        [DisplayName("生成时间")]
        [ReadOnlyColumn]
        [ColumnWidth("3*")]
        public string AddTime
        {
            get { return _addtime; }
            set { _addtime = value; NotifyPropertyChanged(); }
        }

        public string ProtocolData { get => _protocolData; set => _protocolData = value; }

        string _protocolData;

    }
}
