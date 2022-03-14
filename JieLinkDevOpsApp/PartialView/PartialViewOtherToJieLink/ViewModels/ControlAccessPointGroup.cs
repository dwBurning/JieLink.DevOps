using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class ControlAccessPointGroup
    {
        // Token: 0x17001E36 RID: 7734
        // (get) Token: 0x06004225 RID: 16933 RVA: 0x00039C58 File Offset: 0x00037E58
        // (set) Token: 0x06004226 RID: 16934 RVA: 0x00039C60 File Offset: 0x00037E60
        public int Id { get; set; }

        // Token: 0x17001E37 RID: 7735
        // (get) Token: 0x06004227 RID: 16935 RVA: 0x00039C69 File Offset: 0x00037E69
        // (set) Token: 0x06004228 RID: 16936 RVA: 0x00039C71 File Offset: 0x00037E71
        public string APGUID { get; set; }

        // Token: 0x17001E38 RID: 7736
        // (get) Token: 0x06004229 RID: 16937 RVA: 0x00039C7A File Offset: 0x00037E7A
        // (set) Token: 0x0600422A RID: 16938 RVA: 0x00039C82 File Offset: 0x00037E82
        public string APName { get; set; }

        // Token: 0x17001E39 RID: 7737
        // (get) Token: 0x0600422B RID: 16939 RVA: 0x00039C8B File Offset: 0x00037E8B
        // (set) Token: 0x0600422C RID: 16940 RVA: 0x00039C93 File Offset: 0x00037E93
        public string ParkNo { get; set; }

        // Token: 0x17001E3A RID: 7738
        // (get) Token: 0x0600422D RID: 16941 RVA: 0x00039C9C File Offset: 0x00037E9C
        // (set) Token: 0x0600422E RID: 16942 RVA: 0x00039CA4 File Offset: 0x00037EA4
        public string AreaCode { get; set; }

        // Token: 0x17001E3B RID: 7739
        // (get) Token: 0x0600422F RID: 16943 RVA: 0x00039CAD File Offset: 0x00037EAD
        // (set) Token: 0x06004230 RID: 16944 RVA: 0x00039CB5 File Offset: 0x00037EB5
        public int DefaultType { get; set; }

        // Token: 0x17001E3C RID: 7740
        // (get) Token: 0x06004231 RID: 16945 RVA: 0x00039CBE File Offset: 0x00037EBE
        // (set) Token: 0x06004232 RID: 16946 RVA: 0x00039CC6 File Offset: 0x00037EC6
        public int APType { get; set; }

        // Token: 0x17001E3D RID: 7741
        // (get) Token: 0x06004233 RID: 16947 RVA: 0x00039CCF File Offset: 0x00037ECF
        // (set) Token: 0x06004234 RID: 16948 RVA: 0x00039CD7 File Offset: 0x00037ED7
        public string ParentId { get; set; }

        // Token: 0x17001E3E RID: 7742
        // (get) Token: 0x06004235 RID: 16949 RVA: 0x00039CE0 File Offset: 0x00037EE0
        // (set) Token: 0x06004236 RID: 16950 RVA: 0x00039CE8 File Offset: 0x00037EE8
        public int Status { get; set; }

        // Token: 0x17001E3F RID: 7743
        // (get) Token: 0x06004237 RID: 16951 RVA: 0x00039CF1 File Offset: 0x00037EF1
        // (set) Token: 0x06004238 RID: 16952 RVA: 0x00039CF9 File Offset: 0x00037EF9
        public DateTime CreatedOnUtc { get; set; }

        // Token: 0x17001E40 RID: 7744
        // (get) Token: 0x06004239 RID: 16953 RVA: 0x00039D02 File Offset: 0x00037F02
        // (set) Token: 0x0600423A RID: 16954 RVA: 0x00039D0A File Offset: 0x00037F0A
        public string Remark { get; set; }
    }
}
