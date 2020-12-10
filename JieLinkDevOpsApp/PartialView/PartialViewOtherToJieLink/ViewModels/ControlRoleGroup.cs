using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJsdsOneClickUpgradeToJieLink.ViewModels
{
    public class ControlRoleGroup
    {
        // Token: 0x17000F77 RID: 3959
        // (get) Token: 0x06002213 RID: 8723 RVA: 0x0001DC25 File Offset: 0x0001BE25
        // (set) Token: 0x06002214 RID: 8724 RVA: 0x0001DC2D File Offset: 0x0001BE2D
        public int ID { get; set; }

        // Token: 0x17000F78 RID: 3960
        // (get) Token: 0x06002215 RID: 8725 RVA: 0x0001DC36 File Offset: 0x0001BE36
        // (set) Token: 0x06002216 RID: 8726 RVA: 0x0001DC3E File Offset: 0x0001BE3E
        public string RGGUID { get; set; }

        // Token: 0x17000F79 RID: 3961
        // (get) Token: 0x06002217 RID: 8727 RVA: 0x0001DC47 File Offset: 0x0001BE47
        // (set) Token: 0x06002218 RID: 8728 RVA: 0x0001DC4F File Offset: 0x0001BE4F
        public string RGName { get; set; }

        // Token: 0x17000F7A RID: 3962
        // (get) Token: 0x06002219 RID: 8729 RVA: 0x0001DC58 File Offset: 0x0001BE58
        // (set) Token: 0x0600221A RID: 8730 RVA: 0x0001DC60 File Offset: 0x0001BE60
        public string RGCode { get; set; }

        // Token: 0x17000F7B RID: 3963
        // (get) Token: 0x0600221B RID: 8731 RVA: 0x0001DC69 File Offset: 0x0001BE69
        // (set) Token: 0x0600221C RID: 8732 RVA: 0x0001DC71 File Offset: 0x0001BE71
        public string ParentId { get; set; }

        // Token: 0x17000F7C RID: 3964
        // (get) Token: 0x0600221D RID: 8733 RVA: 0x0001DC7A File Offset: 0x0001BE7A
        // (set) Token: 0x0600221E RID: 8734 RVA: 0x0001DC82 File Offset: 0x0001BE82
        public int RGType { get; set; }

        // Token: 0x17000F7D RID: 3965
        // (get) Token: 0x0600221F RID: 8735 RVA: 0x0001DC8B File Offset: 0x0001BE8B
        // (set) Token: 0x06002220 RID: 8736 RVA: 0x0001DC93 File Offset: 0x0001BE93
        public int Status { get; set; }

        // Token: 0x17000F7E RID: 3966
        // (get) Token: 0x06002221 RID: 8737 RVA: 0x0001DC9C File Offset: 0x0001BE9C
        // (set) Token: 0x06002222 RID: 8738 RVA: 0x0001DCA4 File Offset: 0x0001BEA4
        public DateTime CreatedOnUtc { get; set; }

        // Token: 0x17000F7F RID: 3967
        // (get) Token: 0x06002223 RID: 8739 RVA: 0x0001DCAD File Offset: 0x0001BEAD
        // (set) Token: 0x06002224 RID: 8740 RVA: 0x0001DCB5 File Offset: 0x0001BEB5
        public string Remark { get; set; }

        public string RGFullPath { get; set; }
    }
}
