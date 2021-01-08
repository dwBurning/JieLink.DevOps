using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.ViewModels
{
    public class ControlDevices
    {
        // Token: 0x170008FE RID: 2302
        // (get) Token: 0x06001435 RID: 5173 RVA: 0x0001B7A3 File Offset: 0x000199A3
        // (set) Token: 0x06001436 RID: 5174 RVA: 0x0001B7AB File Offset: 0x000199AB
        public int ID { get; set; }

        // Token: 0x170008FF RID: 2303
        // (get) Token: 0x06001437 RID: 5175 RVA: 0x0001B7B4 File Offset: 0x000199B4
        // (set) Token: 0x06001438 RID: 5176 RVA: 0x0001B7BC File Offset: 0x000199BC
        public string DGUID { get; set; }

        // Token: 0x17000900 RID: 2304
        // (get) Token: 0x06001439 RID: 5177 RVA: 0x0001B7C5 File Offset: 0x000199C5
        // (set) Token: 0x0600143A RID: 5178 RVA: 0x0001B7CD File Offset: 0x000199CD
        public int DeviceType { get; set; }

        // Token: 0x17000901 RID: 2305
        // (get) Token: 0x0600143B RID: 5179 RVA: 0x0001B7D6 File Offset: 0x000199D6
        // (set) Token: 0x0600143C RID: 5180 RVA: 0x0001B7DE File Offset: 0x000199DE
        public int IoType { get; set; }

        // Token: 0x17000902 RID: 2306
        // (get) Token: 0x0600143D RID: 5181 RVA: 0x0001B7E7 File Offset: 0x000199E7
        // (set) Token: 0x0600143E RID: 5182 RVA: 0x0001B7EF File Offset: 0x000199EF
        public string DeviceName { get; set; }

        // Token: 0x17000903 RID: 2307
        // (get) Token: 0x0600143F RID: 5183 RVA: 0x0001B7F8 File Offset: 0x000199F8
        // (set) Token: 0x06001440 RID: 5184 RVA: 0x0001B800 File Offset: 0x00019A00
        public int DeviceStatus { get; set; }

        // Token: 0x17000904 RID: 2308
        // (get) Token: 0x06001441 RID: 5185 RVA: 0x0001B809 File Offset: 0x00019A09
        // (set) Token: 0x06001442 RID: 5186 RVA: 0x0001B811 File Offset: 0x00019A11
        public string Remark { get; set; }

        // Token: 0x17000905 RID: 2309
        // (get) Token: 0x06001443 RID: 5187 RVA: 0x0001B81A File Offset: 0x00019A1A
        // (set) Token: 0x06001444 RID: 5188 RVA: 0x0001B822 File Offset: 0x00019A22
        public string IP { get; set; }

        // Token: 0x17000906 RID: 2310
        // (get) Token: 0x06001445 RID: 5189 RVA: 0x0001B82B File Offset: 0x00019A2B
        // (set) Token: 0x06001446 RID: 5190 RVA: 0x0001B833 File Offset: 0x00019A33
        public string Company { get; set; }

        // Token: 0x17000907 RID: 2311
        // (get) Token: 0x06001447 RID: 5191 RVA: 0x0001B83C File Offset: 0x00019A3C
        // (set) Token: 0x06001448 RID: 5192 RVA: 0x0001B844 File Offset: 0x00019A44
        public int DeviceClass { get; set; }

        // Token: 0x17000908 RID: 2312
        // (get) Token: 0x06001449 RID: 5193 RVA: 0x0001B84D File Offset: 0x00019A4D
        // (set) Token: 0x0600144A RID: 5194 RVA: 0x0001B855 File Offset: 0x00019A55
        public string Gateway_IP { get; set; }

        // Token: 0x17000909 RID: 2313
        // (get) Token: 0x0600144B RID: 5195 RVA: 0x0001B85E File Offset: 0x00019A5E
        // (set) Token: 0x0600144C RID: 5196 RVA: 0x0001B866 File Offset: 0x00019A66
        public string Hardware_Version { get; set; }

        // Token: 0x1700090A RID: 2314
        // (get) Token: 0x0600144D RID: 5197 RVA: 0x0001B86F File Offset: 0x00019A6F
        // (set) Token: 0x0600144E RID: 5198 RVA: 0x0001B877 File Offset: 0x00019A77
        public string Mac { get; set; }

        // Token: 0x1700090B RID: 2315
        // (get) Token: 0x0600144F RID: 5199 RVA: 0x0001B880 File Offset: 0x00019A80
        // (set) Token: 0x06001450 RID: 5200 RVA: 0x0001B888 File Offset: 0x00019A88
        public string CpuID { get; set; }

        // Token: 0x1700090C RID: 2316
        // (get) Token: 0x06001451 RID: 5201 RVA: 0x0001B891 File Offset: 0x00019A91
        // (set) Token: 0x06001452 RID: 5202 RVA: 0x0001B899 File Offset: 0x00019A99
        public string Manufacturer { get; set; }

        // Token: 0x1700090D RID: 2317
        // (get) Token: 0x06001453 RID: 5203 RVA: 0x0001B8A2 File Offset: 0x00019AA2
        // (set) Token: 0x06001454 RID: 5204 RVA: 0x0001B8AA File Offset: 0x00019AAA
        public string Product_Date { get; set; }

        // Token: 0x1700090E RID: 2318
        // (get) Token: 0x06001455 RID: 5205 RVA: 0x0001B8B3 File Offset: 0x00019AB3
        // (set) Token: 0x06001456 RID: 5206 RVA: 0x0001B8BB File Offset: 0x00019ABB
        public string Model { get; set; }

        // Token: 0x1700090F RID: 2319
        // (get) Token: 0x06001457 RID: 5207 RVA: 0x0001B8C4 File Offset: 0x00019AC4
        // (set) Token: 0x06001458 RID: 5208 RVA: 0x0001B8CC File Offset: 0x00019ACC
        public string Net_Mask { get; set; }

        // Token: 0x17000910 RID: 2320
        // (get) Token: 0x06001459 RID: 5209 RVA: 0x0001B8D5 File Offset: 0x00019AD5
        // (set) Token: 0x0600145A RID: 5210 RVA: 0x0001B8DD File Offset: 0x00019ADD
        public string SN { get; set; }

        // Token: 0x17000911 RID: 2321
        // (get) Token: 0x0600145B RID: 5211 RVA: 0x0001B8E6 File Offset: 0x00019AE6
        // (set) Token: 0x0600145C RID: 5212 RVA: 0x0001B8EE File Offset: 0x00019AEE
        public string Software_Version { get; set; }

        // Token: 0x17000912 RID: 2322
        // (get) Token: 0x0600145D RID: 5213 RVA: 0x0001B8F7 File Offset: 0x00019AF7
        // (set) Token: 0x0600145E RID: 5214 RVA: 0x0001B8FF File Offset: 0x00019AFF
        public string ParentID { get; set; }

        // Token: 0x17000914 RID: 2324
        // (get) Token: 0x06001461 RID: 5217 RVA: 0x0001B919 File Offset: 0x00019B19
        // (set) Token: 0x06001462 RID: 5218 RVA: 0x0001B921 File Offset: 0x00019B21
        public int IsDebugModel { get; set; }

        // Token: 0x17000915 RID: 2325
        // (get) Token: 0x06001463 RID: 5219 RVA: 0x0001B92A File Offset: 0x00019B2A
        // (set) Token: 0x06001464 RID: 5220 RVA: 0x0001B932 File Offset: 0x00019B32
        public string DeviceID { get; set; }

        // Token: 0x17000916 RID: 2326
        // (get) Token: 0x06001465 RID: 5221 RVA: 0x0001B93B File Offset: 0x00019B3B
        // (set) Token: 0x06001466 RID: 5222 RVA: 0x0001B943 File Offset: 0x00019B43
        public string Summary { get; set; }

        // Token: 0x17000917 RID: 2327
        // (get) Token: 0x06001467 RID: 5223 RVA: 0x0001B94C File Offset: 0x00019B4C
        // (set) Token: 0x06001468 RID: 5224 RVA: 0x0001B954 File Offset: 0x00019B54
        public string Mac2 { get; set; }

        // Token: 0x17000918 RID: 2328
        // (get) Token: 0x06001469 RID: 5225 RVA: 0x0001B95D File Offset: 0x00019B5D
        // (set) Token: 0x0600146A RID: 5226 RVA: 0x0001B965 File Offset: 0x00019B65
        public int SelfPayFilePort { get; set; }

        // Token: 0x17000919 RID: 2329
        // (get) Token: 0x0600146B RID: 5227 RVA: 0x0001B96E File Offset: 0x00019B6E
        // (set) Token: 0x0600146C RID: 5228 RVA: 0x0001B976 File Offset: 0x00019B76
        public string SubDeviceNet { get; set; }

        // Token: 0x1700091A RID: 2330
        // (get) Token: 0x0600146D RID: 5229 RVA: 0x0001B97F File Offset: 0x00019B7F
        // (set) Token: 0x0600146E RID: 5230 RVA: 0x0001B987 File Offset: 0x00019B87
        public string NamePlateDna { get; set; }

        // Token: 0x1700091B RID: 2331
        // (get) Token: 0x0600146F RID: 5231 RVA: 0x0001B990 File Offset: 0x00019B90
        // (set) Token: 0x06001470 RID: 5232 RVA: 0x0001B998 File Offset: 0x00019B98
        public int GetNum { get; set; }

        // Token: 0x1700091C RID: 2332
        // (get) Token: 0x06001471 RID: 5233 RVA: 0x0001B9A1 File Offset: 0x00019BA1
        // (set) Token: 0x06001472 RID: 5234 RVA: 0x0001B9A9 File Offset: 0x00019BA9
        public DateTime GetNumTime { get; set; }

        // Token: 0x1700091D RID: 2333
        // (get) Token: 0x06001473 RID: 5235 RVA: 0x0001B9B2 File Offset: 0x00019BB2
        // (set) Token: 0x06001474 RID: 5236 RVA: 0x0001B9BA File Offset: 0x00019BBA
        public string InTime { get; set; }

        // Token: 0x1700091E RID: 2334
        // (get) Token: 0x06001475 RID: 5237 RVA: 0x0001B9C3 File Offset: 0x00019BC3
        // (set) Token: 0x06001476 RID: 5238 RVA: 0x0001B9CB File Offset: 0x00019BCB
        public string UpdateTime { get; set; }

        // Token: 0x1700091F RID: 2335
        // (get) Token: 0x06001477 RID: 5239 RVA: 0x0001B9D4 File Offset: 0x00019BD4
        // (set) Token: 0x06001478 RID: 5240 RVA: 0x0001B9DC File Offset: 0x00019BDC
        public string MasterIp { get; set; }

        // Token: 0x17000920 RID: 2336
        // (get) Token: 0x06001479 RID: 5241 RVA: 0x0001B9E5 File Offset: 0x00019BE5
        // (set) Token: 0x0600147A RID: 5242 RVA: 0x0001B9ED File Offset: 0x00019BED
        public string BluetoothAddress { get; set; }

        // Token: 0x17000921 RID: 2337
        // (get) Token: 0x0600147B RID: 5243 RVA: 0x0001B9F6 File Offset: 0x00019BF6
        // (set) Token: 0x0600147C RID: 5244 RVA: 0x0001B9FE File Offset: 0x00019BFE
        public int AuthStatus { get; set; }

        // Token: 0x17000922 RID: 2338
        // (get) Token: 0x0600147D RID: 5245 RVA: 0x0001BA07 File Offset: 0x00019C07
        // (set) Token: 0x0600147E RID: 5246 RVA: 0x0001BA0F File Offset: 0x00019C0F
        public int SelfPayMonitorPort { get; set; }

        // Token: 0x17000923 RID: 2339
        // (get) Token: 0x0600147F RID: 5247 RVA: 0x0001BA18 File Offset: 0x00019C18
        // (set) Token: 0x06001480 RID: 5248 RVA: 0x0001BA20 File Offset: 0x00019C20
        public string MonitorPathSuffix { get; set; }

        // Token: 0x17000924 RID: 2340
        // (get) Token: 0x06001481 RID: 5249 RVA: 0x0001BA29 File Offset: 0x00019C29
        // (set) Token: 0x06001482 RID: 5250 RVA: 0x0001BA31 File Offset: 0x00019C31
        public string MacNo { get; set; }

        // Token: 0x17000925 RID: 2341
        // (get) Token: 0x06001483 RID: 5251 RVA: 0x0001BA3A File Offset: 0x00019C3A
        // (set) Token: 0x06001484 RID: 5252 RVA: 0x0001BA42 File Offset: 0x00019C42
        public string QrCodeLink { get; set; }

        // Token: 0x17000926 RID: 2342
        // (get) Token: 0x06001486 RID: 5254 RVA: 0x0001BA54 File Offset: 0x00019C54
        // (set) Token: 0x06001485 RID: 5253 RVA: 0x0001BA4B File Offset: 0x00019C4B
        public int SpeakTecType
        {
            get
            {
                return this._speakTecType;
            }
            set
            {
                this._speakTecType = value;
            }
        }

        // Token: 0x17000927 RID: 2343
        // (get) Token: 0x06001488 RID: 5256 RVA: 0x0001BA65 File Offset: 0x00019C65
        // (set) Token: 0x06001487 RID: 5255 RVA: 0x0001BA5C File Offset: 0x00019C5C
        public int SpeakVideoType
        {
            get
            {
                return this._speakVideoType;
            }
            set
            {
                this._speakVideoType = value;
            }
        }

        // Token: 0x17000928 RID: 2344
        // (get) Token: 0x06001489 RID: 5257 RVA: 0x0001BA6D File Offset: 0x00019C6D
        // (set) Token: 0x0600148A RID: 5258 RVA: 0x0001BA75 File Offset: 0x00019C75
        public string[] SubMacDevices { get; set; }

        // Token: 0x17000929 RID: 2345
        // (get) Token: 0x0600148B RID: 5259 RVA: 0x0001BA7E File Offset: 0x00019C7E
        // (set) Token: 0x0600148C RID: 5260 RVA: 0x0001BA86 File Offset: 0x00019C86
        public int OnOrOff { get; set; }

        // Token: 0x1700092C RID: 2348
        // (get) Token: 0x06001490 RID: 5264 RVA: 0x0001BB23 File Offset: 0x00019D23
        // (set) Token: 0x06001491 RID: 5265 RVA: 0x0001BB2B File Offset: 0x00019D2B
        public int udpSendFlag { get; set; }

        // Token: 0x1700092D RID: 2349
        // (get) Token: 0x06001492 RID: 5266 RVA: 0x0001BB34 File Offset: 0x00019D34
        // (set) Token: 0x06001493 RID: 5267 RVA: 0x0001BB3C File Offset: 0x00019D3C
        public string PassgateId { get; set; }

        // Token: 0x1700092E RID: 2350
        // (get) Token: 0x06001494 RID: 5268 RVA: 0x0001BB45 File Offset: 0x00019D45
        // (set) Token: 0x06001495 RID: 5269 RVA: 0x0001BB4D File Offset: 0x00019D4D
        public bool IsDeleted { get; set; }

        // Token: 0x04000AD2 RID: 2770
        private int _speakTecType = 1;

        // Token: 0x04000AD3 RID: 2771
        private int _speakVideoType = 1;

    }
}

