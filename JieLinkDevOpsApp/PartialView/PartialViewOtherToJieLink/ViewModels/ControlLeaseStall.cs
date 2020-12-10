using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJsdsOneClickUpgradeToJieLink.ViewModels
{
    public class ControlLeaseStall
    {
        // Token: 0x170012D9 RID: 4825
        // (get) Token: 0x0600292D RID: 10541 RVA: 0x00023274 File Offset: 0x00021474
        // (set) Token: 0x0600292E RID: 10542 RVA: 0x0002327C File Offset: 0x0002147C
        public string LGUID { get; set; }

        // Token: 0x170012DA RID: 4826
        // (get) Token: 0x0600292F RID: 10543 RVA: 0x00023285 File Offset: 0x00021485
        // (set) Token: 0x06002930 RID: 10544 RVA: 0x0002328D File Offset: 0x0002148D
        public string PGUID { get; set; }

        // Token: 0x170012DB RID: 4827
        // (get) Token: 0x06002931 RID: 10545 RVA: 0x00023296 File Offset: 0x00021496
        // (set) Token: 0x06002932 RID: 10546 RVA: 0x0002329E File Offset: 0x0002149E
        public string PersonName { get; set; }

        // Token: 0x170012DC RID: 4828
        // (get) Token: 0x06002933 RID: 10547 RVA: 0x000232A7 File Offset: 0x000214A7
        // (set) Token: 0x06002934 RID: 10548 RVA: 0x000232AF File Offset: 0x000214AF
        public string PersonNo { get; set; }

        // Token: 0x170012DD RID: 4829
        // (get) Token: 0x06002935 RID: 10549 RVA: 0x000232B8 File Offset: 0x000214B8
        // (set) Token: 0x06002936 RID: 10550 RVA: 0x000232C0 File Offset: 0x000214C0
        public string MGUID { get; set; }

        // Token: 0x170012DE RID: 4830
        // (get) Token: 0x06002937 RID: 10551 RVA: 0x000232C9 File Offset: 0x000214C9
        // (set) Token: 0x06002938 RID: 10552 RVA: 0x000232D1 File Offset: 0x000214D1
        public int SetmealNo { get; set; }

        // Token: 0x170012DF RID: 4831
        // (get) Token: 0x06002939 RID: 10553 RVA: 0x000232DA File Offset: 0x000214DA
        // (set) Token: 0x0600293A RID: 10554 RVA: 0x000232E2 File Offset: 0x000214E2
        public DateTime StartTime { get; set; }

        // Token: 0x170012E0 RID: 4832
        // (get) Token: 0x0600293B RID: 10555 RVA: 0x000232EB File Offset: 0x000214EB
        // (set) Token: 0x0600293C RID: 10556 RVA: 0x000232F3 File Offset: 0x000214F3
        public DateTime EndTime { get; set; }

        // Token: 0x170012E1 RID: 4833
        // (get) Token: 0x0600293D RID: 10557 RVA: 0x000232FC File Offset: 0x000214FC
        // (set) Token: 0x0600293E RID: 10558 RVA: 0x00023304 File Offset: 0x00021504
        public string OperatorNO { get; set; }

        // Token: 0x170012E2 RID: 4834
        // (get) Token: 0x0600293F RID: 10559 RVA: 0x0002330D File Offset: 0x0002150D
        // (set) Token: 0x06002940 RID: 10560 RVA: 0x00023315 File Offset: 0x00021515
        public string OperateTime { get; set; }

        // Token: 0x170012E3 RID: 4835
        // (get) Token: 0x06002941 RID: 10561 RVA: 0x0002331E File Offset: 0x0002151E
        // (set) Token: 0x06002942 RID: 10562 RVA: 0x00023326 File Offset: 0x00021526
        public int Status { get; set; }

        // Token: 0x170012E4 RID: 4836
        // (get) Token: 0x06002943 RID: 10563 RVA: 0x0002332F File Offset: 0x0002152F
        // (set) Token: 0x06002944 RID: 10564 RVA: 0x00023337 File Offset: 0x00021537
        public string NisspId { get; set; }

        // Token: 0x170012E5 RID: 4837
        // (get) Token: 0x06002945 RID: 10565 RVA: 0x00023340 File Offset: 0x00021540
        // (set) Token: 0x06002946 RID: 10566 RVA: 0x00023348 File Offset: 0x00021548
        public int CarNumber { get; set; }

        // Token: 0x170012E6 RID: 4838
        // (get) Token: 0x06002947 RID: 10567 RVA: 0x00023351 File Offset: 0x00021551
        // (set) Token: 0x06002948 RID: 10568 RVA: 0x00023359 File Offset: 0x00021559
        [Obsolete]
        public DateTime StopServiceTime { get; set; }

        // Token: 0x170012E8 RID: 4840
        // (get) Token: 0x0600294B RID: 10571 RVA: 0x00023373 File Offset: 0x00021573
        // (set) Token: 0x0600294C RID: 10572 RVA: 0x0002337B File Offset: 0x0002157B
        public string UniqueServiceNo { get; set; }
    }
}
