using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewMySqlBackUp.Models
{
    public class BackUpTable : PropertyChangedBase
    {
        private string _tableName;

        [DisplayName("数据表名")]
        [ReadOnlyColumn]
        [ColumnWidth("2*")]
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; NotifyPropertyChanged(); }
        }

        private bool _isChecked;

        [DisplayName("忽略备份")]
        [ColumnWidth("*")]
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; NotifyPropertyChanged(); }
        }
    }
}
