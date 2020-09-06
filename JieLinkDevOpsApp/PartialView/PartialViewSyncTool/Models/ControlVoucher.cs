using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewSyncTool
{
    public class ControlVoucher
    {
        public string VGUID { get; set; }

        public string PGUID { get; set; }

        public string LGUID { get; set; }

        public string PersonNo { get; set; }

        public int VoucherType { get; set; }

        public string VoucherNo { get; set; }

        public string CardNum { get; set; }

        public string AddOperatorNo { get; set; }

        public string AddTime { get; set; }

        public int Status { get; set; }

        public string LastTime { get; set; }

        public string Remark { get; set; }

        public int StatusFromPerson { get; set; }
    }
}
