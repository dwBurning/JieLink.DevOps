using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewOtherToJieLink.Models
{
    public class JsdsUpgradePolicy : DependencyObject
    {
        /// <summary>
        /// 入场
        /// </summary>
        public bool EnterRecordSelect { get; set; }

        /// <summary>
        /// 出场
        /// </summary>

        public bool OutRecordSelect { get; set; }
        /// <summary>
        /// 收费
        /// </summary>

        public bool BillRecordSelect { get; set; }

        public int SelectedTimeIndex { get; set; }
    }
}
