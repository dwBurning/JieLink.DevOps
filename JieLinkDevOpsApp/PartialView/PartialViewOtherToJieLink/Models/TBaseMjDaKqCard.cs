using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.Models
{
    /// <summary>
    /// G3门禁卡数据
    /// </summary>
    public class TBaseMjDaKqCard
    {
        public int User_Card_Serial_No { get; set; }

        public int Mc_Cardinfo_Id { get; set; }

        public int CardTypeId { get; set; }

        public string Card_No { get; set; }

        public int Card_state { get; set; }

        /// <summary>
        /// 门禁卡的开始时间
        /// </summary>
        public DateTime Start_Date { get; set; }

        /// <summary>
        /// 门禁卡的结束时间
        /// </summary>
        public DateTime End_Date { get; set; }

        public int User_Card_Type { get; set; }

        public string User_PassWord { get; set; }

        public string MaxNoEx { get; set; }

        public string MaxNo { get; set; }

        public int MachineType { get; set; }
    }
}
