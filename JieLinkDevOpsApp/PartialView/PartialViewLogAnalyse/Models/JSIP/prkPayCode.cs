using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models.JSIP
{
    public class prkPayCode
    {
        public int chargeType { get; set; }


        public string payCode { get; set; }


        public string signature { get; set; }


        public string transactionId { get; set; }

        //// Token: 0x170001C5 RID: 453
        //// (get) Token: 0x06000467 RID: 1127 RVA: 0x0000A08E File Offset: 0x0000828E
        //// (set) Token: 0x06000468 RID: 1128 RVA: 0x0000A096 File Offset: 0x00008296
        //[DataMember]
        //public prkCashInfo cashInfo { get; set; }


        public uint deviceId { get; set; }
    }
}
