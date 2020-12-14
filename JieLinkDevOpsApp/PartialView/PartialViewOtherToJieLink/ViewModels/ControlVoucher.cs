using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class ControlVoucher
	{
		// Token: 0x0600310E RID: 12558 RVA: 0x00027C01 File Offset: 0x00025E01
		public ControlVoucher()
		{
			this.AddTime = DateTime.Now;
		}

		// Token: 0x17001698 RID: 5784
		// (get) Token: 0x0600310F RID: 12559 RVA: 0x00027C14 File Offset: 0x00025E14
		// (set) Token: 0x06003110 RID: 12560 RVA: 0x00027C1C File Offset: 0x00025E1C
		public string Guid { get; set; }

		// Token: 0x17001699 RID: 5785
		// (get) Token: 0x06003111 RID: 12561 RVA: 0x00027C25 File Offset: 0x00025E25
		// (set) Token: 0x06003112 RID: 12562 RVA: 0x00027C2D File Offset: 0x00025E2D
		public string PGuid { get; set; }

		// Token: 0x1700169A RID: 5786
		// (get) Token: 0x06003113 RID: 12563 RVA: 0x00027C36 File Offset: 0x00025E36
		// (set) Token: 0x06003114 RID: 12564 RVA: 0x00027C3E File Offset: 0x00025E3E
		public string LGuid { get; set; }

		// Token: 0x1700169B RID: 5787
		// (get) Token: 0x06003115 RID: 12565 RVA: 0x00027C47 File Offset: 0x00025E47
		// (set) Token: 0x06003116 RID: 12566 RVA: 0x00027C4F File Offset: 0x00025E4F
		public string NisspLGuid { get; set; }

		// Token: 0x1700169C RID: 5788
		// (get) Token: 0x06003117 RID: 12567 RVA: 0x00027C58 File Offset: 0x00025E58
		// (set) Token: 0x06003118 RID: 12568 RVA: 0x00027C60 File Offset: 0x00025E60
		public string PersonNo { get; set; }

		// Token: 0x1700169D RID: 5789
		// (get) Token: 0x06003119 RID: 12569 RVA: 0x00027C69 File Offset: 0x00025E69
		// (set) Token: 0x0600311A RID: 12570 RVA: 0x00027C71 File Offset: 0x00025E71
		public string PersonName { get; set; }

		// Token: 0x1700169E RID: 5790
		// (get) Token: 0x0600311B RID: 12571 RVA: 0x00027C7A File Offset: 0x00025E7A
		// (set) Token: 0x0600311C RID: 12572 RVA: 0x00027C82 File Offset: 0x00025E82
		public string Mobile { get; set; }

		// Token: 0x1700169F RID: 5791
		// (get) Token: 0x0600311D RID: 12573 RVA: 0x00027C8B File Offset: 0x00025E8B
		// (set) Token: 0x0600311E RID: 12574 RVA: 0x00027C93 File Offset: 0x00025E93
		public int VoucherType { get; set; }

		// Token: 0x170016A1 RID: 5793
		// (get) Token: 0x06003120 RID: 12576 RVA: 0x00027CAE File Offset: 0x00025EAE
		// (set) Token: 0x06003121 RID: 12577 RVA: 0x00027CB6 File Offset: 0x00025EB6
		public string VoucherNo { get; set; }

		// Token: 0x170016A2 RID: 5794
		// (get) Token: 0x06003122 RID: 12578 RVA: 0x00027CBF File Offset: 0x00025EBF
		// (set) Token: 0x06003123 RID: 12579 RVA: 0x00027CC7 File Offset: 0x00025EC7
		public string FaceFeature { get; set; }

		// Token: 0x170016A3 RID: 5795
		// (get) Token: 0x06003124 RID: 12580 RVA: 0x00027CD0 File Offset: 0x00025ED0
		// (set) Token: 0x06003125 RID: 12581 RVA: 0x00027CD8 File Offset: 0x00025ED8
		public ushort FaceFeatureVersion { get; set; }

		// Token: 0x170016A4 RID: 5796
		// (get) Token: 0x06003126 RID: 12582 RVA: 0x00027CE1 File Offset: 0x00025EE1
		// (set) Token: 0x06003127 RID: 12583 RVA: 0x00027CE9 File Offset: 0x00025EE9
		public string RelativeUrl { get; set; }

		// Token: 0x170016A5 RID: 5797
		// (get) Token: 0x06003128 RID: 12584 RVA: 0x00027CF2 File Offset: 0x00025EF2
		// (set) Token: 0x06003129 RID: 12585 RVA: 0x00027CFA File Offset: 0x00025EFA
		public string CardNum { get; set; }

		// Token: 0x170016A6 RID: 5798
		// (get) Token: 0x0600312A RID: 12586 RVA: 0x00027D03 File Offset: 0x00025F03
		// (set) Token: 0x0600312B RID: 12587 RVA: 0x00027D0B File Offset: 0x00025F0B
		public string PhysicsNum { get; set; }

		// Token: 0x170016A9 RID: 5801
		// (get) Token: 0x06003130 RID: 12592 RVA: 0x00027D36 File Offset: 0x00025F36
		// (set) Token: 0x06003131 RID: 12593 RVA: 0x00027D3E File Offset: 0x00025F3E
		public string CardKey { get; set; }

		// Token: 0x170016AA RID: 5802
		// (get) Token: 0x06003132 RID: 12594 RVA: 0x00027D47 File Offset: 0x00025F47
		// (set) Token: 0x06003133 RID: 12595 RVA: 0x00027D4F File Offset: 0x00025F4F
		public int Status { get; set; }

		// Token: 0x170016AB RID: 5803
		// (get) Token: 0x06003134 RID: 12596 RVA: 0x00027D58 File Offset: 0x00025F58
		// (set) Token: 0x06003135 RID: 12597 RVA: 0x00027D60 File Offset: 0x00025F60
		public DateTime? LastTime { get; set; }

		// Token: 0x170016AC RID: 5804
		// (get) Token: 0x06003136 RID: 12598 RVA: 0x00027D69 File Offset: 0x00025F69
		// (set) Token: 0x06003137 RID: 12599 RVA: 0x00027D71 File Offset: 0x00025F71
		public DateTime AddTime { get; set; }

		// Token: 0x170016AD RID: 5805
		// (get) Token: 0x06003138 RID: 12600 RVA: 0x00027D7A File Offset: 0x00025F7A
		// (set) Token: 0x06003139 RID: 12601 RVA: 0x00027D82 File Offset: 0x00025F82
		public string Remark { get; set; }

		// Token: 0x170016AE RID: 5806
		// (get) Token: 0x0600313A RID: 12602 RVA: 0x00027D8B File Offset: 0x00025F8B
		// (set) Token: 0x0600313B RID: 12603 RVA: 0x00027D93 File Offset: 0x00025F93
		public string AddOperatorNo { get; set; }

		// Token: 0x170016AF RID: 5807
		// (get) Token: 0x0600313C RID: 12604 RVA: 0x00027D9C File Offset: 0x00025F9C
		// (set) Token: 0x0600313D RID: 12605 RVA: 0x00027DA4 File Offset: 0x00025FA4
		public string AddOperatorName { get; set; }

		// Token: 0x170016B0 RID: 5808
		// (get) Token: 0x0600313E RID: 12606 RVA: 0x00027DAD File Offset: 0x00025FAD
		// (set) Token: 0x0600313F RID: 12607 RVA: 0x00027DB5 File Offset: 0x00025FB5
		public string UpdateOperatorNo { get; set; }

		// Token: 0x170016B1 RID: 5809
		// (get) Token: 0x06003140 RID: 12608 RVA: 0x00027DBE File Offset: 0x00025FBE
		// (set) Token: 0x06003141 RID: 12609 RVA: 0x00027DC6 File Offset: 0x00025FC6
		public string UpdateOperatorName { get; set; }

		// Token: 0x170016B2 RID: 5810
		// (get) Token: 0x06003142 RID: 12610 RVA: 0x00027DCF File Offset: 0x00025FCF
		// (set) Token: 0x06003143 RID: 12611 RVA: 0x00027DD7 File Offset: 0x00025FD7
		public int StatusFromPerson { get; set; }

		// Token: 0x170016B3 RID: 5811
		// (get) Token: 0x06003144 RID: 12612 RVA: 0x00027DE0 File Offset: 0x00025FE0
		// (set) Token: 0x06003145 RID: 12613 RVA: 0x00027DE8 File Offset: 0x00025FE8
		public string StatusName { get; set; }
	}
}
