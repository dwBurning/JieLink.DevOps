using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewMySqlBackUp.Models
{
    public class BackUpTable
    {
        [DisplayName("数据表名")]
        [ReadOnlyColumn]
        [ColumnWidth("3*")]
        public string TableName { get; set; }

        [DisplayName("备份")]
        [ColumnWidth("*")]
        public bool IsChecked { get; set; }
    }
}
