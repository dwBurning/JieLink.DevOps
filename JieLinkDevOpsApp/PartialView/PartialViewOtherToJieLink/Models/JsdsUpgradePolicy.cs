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
        /// <summary>
        /// 迁移时间
        /// </summary>
        public int SelectedTimeIndex { get; set; }
        /// <summary>
        /// 车场
        /// </summary>
        public bool ParkSelect { get; set; }
        /// <summary>
        /// 门禁
        /// </summary>
        public bool DoorSelect { get; set; }
    }
}
