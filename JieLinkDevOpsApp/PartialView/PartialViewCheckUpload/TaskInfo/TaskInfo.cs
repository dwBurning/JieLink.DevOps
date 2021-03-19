using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panuon.UI.Silver.Core;

namespace PartialViewCheckUpload.TaskInfos
{
    class TaskInfo : PropertyChangedBase
    {
        private string _serviceName;

        [DisplayName("任务名")]
        [ReadOnlyColumn]
        [ColumnWidth("3*")]
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; NotifyPropertyChanged(); }
        }


        private string _serviceId;

        [DisplayName("服务ID")]
        [ReadOnlyColumn]
        [ColumnWidth("3*")]
        public string ServiceId
        {
            get { return ServiceId; }
            set { ServiceId = value; NotifyPropertyChanged(); }
        }


        private bool _isChecked;

        [DisplayName("查询")]
        [ColumnWidth("*")]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyPropertyChanged(); }
        }
    }
}
