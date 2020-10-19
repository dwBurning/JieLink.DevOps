using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models.JSIP
{
    public class comUtcTime
    {
        // Token: 0x060000D0 RID: 208 RVA: 0x0000380C File Offset: 0x00001A0C
        public comUtcTime()
        {
            this.time = DateTime.Now.ToString();
        }

        // Token: 0x060000D1 RID: 209 RVA: 0x00003838 File Offset: 0x00001A38
        public comUtcTime(string strTime)
        {
            this.time = strTime;
        }


        public string time { get; set; }

        public int millisecond { get; set; }




    }
}
