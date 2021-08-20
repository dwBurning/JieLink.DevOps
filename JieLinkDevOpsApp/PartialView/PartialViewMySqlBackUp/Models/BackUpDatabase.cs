using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewMySqlBackUp.Models
{
    public class BackUpDatabase : PropertyChangedBase
    {
        private string _databaseName;

        [DisplayName("数据库名")]
        [ReadOnlyColumn]
        [ColumnWidth("3*")]
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; NotifyPropertyChanged(); }
        }
    }
}
