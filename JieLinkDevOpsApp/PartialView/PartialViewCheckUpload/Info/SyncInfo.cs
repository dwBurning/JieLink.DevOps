using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Panuon.UI.Silver.Core;
using System.Collections.ObjectModel;

namespace PartialViewCheckUpload.Info
{
    class SyncInfo : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _addtime;

        [DisplayName("生成时间")]
        [ReadOnlyColumn]
        [ColumnWidth("3*")]
        public string AddTime
        {
            get { return _addtime; }
            set { 
                _addtime = value;
                OnPropertyChanged("AddTime");
            }
        }

        private string _protocolData;

        [DisplayName("ProtocolData")]
        [ReadOnlyColumn]
        [ColumnWidth("1*")]
        public string ProtocolData
        {
            get { return _protocolData; }
            set { _protocolData = value; OnPropertyChanged("ProtocolData"); }
        }

        public string status;

        public string remark;

        public string failmessage;

        public string updatetime;

    }
}
