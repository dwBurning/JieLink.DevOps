using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.ViewModels
{
    public class ArchiveTable : PropertyChangedBase
    {
        [IgnoreColumn]
        public int Id { get; set; }

        private string _tableName;

        [DisplayName("数据表名")]
        [ReadOnlyColumn]
        [ColumnWidth("2*")]
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; NotifyPropertyChanged(); }
        }

        private string _dateFiled;

        [DisplayName("时间字段")]
        [ColumnWidth("*")]
        public string DateField
        {
            get { return _dateFiled; }
            set { _dateFiled = value; NotifyPropertyChanged(); }
        }
       
        private string _where;

        [DisplayName("附件条件")]
        [ColumnWidth("2*")]
        public string Where
        {
            get { return _where; }
            set { _where = value; NotifyPropertyChanged(); }
        }
    }
}
