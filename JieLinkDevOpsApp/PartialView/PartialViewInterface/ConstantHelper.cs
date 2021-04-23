using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface
{
    public class ConstantHelper
    {
        /// <summary>
        /// jsds区域root节点
        /// </summary>
        public const string JSDSDISTRICTROOT = "root";

        public const string G3DEPTNO = "00000000";
        /// <summary>
        /// jsds区域EXTERIOR节点
        /// </summary>
        public const string JSDSDISTRICTEXTERIOR = "EXTERIOR";
        /// <summary>
        /// 根节点的父Id
        /// </summary>
        public const string ROOTPARENTID = "00000000-0000-0000-0000-000000000000";
        /// <summary>
        /// JSDS默认操作员
        /// </summary>
        public const string JSDSDEFAULTPERSON = "DEFAULTPERSON";
        /// <summary>
        /// 默认车场编号
        /// </summary>
        public const string PARKNO = "00000000-0000-0000-0000-000000000000";
        /// <summary>
        /// 老Y08
        /// </summary>
        public const string JSMJY08 = "JSMJY08";
        /// <summary>
        /// 老Y08读卡器：jsds权限 服务+通道+读卡器（不是到Y08本身）——转到jielink的话直接用Y08本身
        /// </summary>
        public const string JSMJY08_Reader = "JSMJY08_Reader";
        /// <summary>
        /// 老Y08门锁
        /// </summary>
        public const string JSMJY08_Locker = "JSMJY08_Locker";
        /// <summary>
        /// 老Y08按钮：jsds权限 服务+通道+按钮（不是到Y08本身）——转到jielink的话直接用Y08本身
        /// </summary>
        public const string JSMJY08_OpenDoorButton = "JSMJY08_OpenDoorButton";
        public const string JSDSPARAMMAC = "MAC";
        public const string JSDSPARAMDEVID = "DevID";
        public const string JSDSPARAMGATEWAY = "gateway";
        public const string JSDSPARAMMASK = "MASK";
        public const string JSDSPARAMIP = "ip";
        public const string JSDSPARAMMACNO = "MacNO";
        /// <summary>
        /// 领御III型二门
        /// </summary>
        public const string JSMJK0220A = "JSMJK0220A";
        /// <summary>
        /// 读卡器：649781404/8208，显示领御III型二门的读卡器是
        /// 读卡器当门，有读卡器权限即有该门权限
        /// </summary>
        public const string JSMJK022040A_Reader = "JSMJK022040A_Reader";
        /// <summary>
        /// 按钮
        /// </summary>
        public const string JSMJK022FK_OpenDoorButton = "JSMJK02FK_OpenDoorButton";
        /// <summary>
        /// 门锁
        /// </summary>
        public const string JSMJK022FK_Locker = "JSMJK02FK_Locker";
        /// <summary>
        /// 报警器
        /// </summary>
        public const string JSMJK022FK_Warner = "JSMJK02FK_Warner";
        /// <summary>
        /// 读卡器？
        /// </summary>
        public const string JSMJK022FK_Reader = "JSMJK02FK_Reader";
        /// <summary>
        /// 领御III型四门
        /// </summary>
        public const string JSMJK0240A = "JSMJK0240A";
        /// <summary>
        /// JieLink门的MAC
        /// </summary>
        public const string JIELINKDOORMAC = "00:00:00:00:00:00";
        /// <summary>
        /// JieLink门的MAC
        /// </summary>
        public const int JIELINKDOORTYPE = 699;
        /// <summary>
        /// 领御III型门禁20
        /// </summary>
        public const int JIELINK_JSMJK02_20 = 116;
        /// <summary>
        /// 领御III型门禁40
        /// </summary>
        public const int JIELINK_JSMJK02_40 = 100;
        /// <summary>
        /// 领御III型门禁40
        /// </summary>
        public const int JIELINK_JSMJY08A_OLD = 252;
        /// <summary>
        /// 速通：对应JieLink的速通II型停车场（JSTC1801-01）  设备类型为JSTC1801_01
        /// </summary>
        public const int JIELINK_JSTC1801_01 = 22;

        /// <summary>
        /// 速通：对应JieLink的速通IV（JSTC2801）  设备类型为JSTC2801
        /// </summary>
        public const int JIELINK_JSTC2801 = 73;
        /// <summary>
        /// 车场速通
        /// </summary>
        public const string JSC8ST = "JSC8ST";
        /// <summary>
        /// 速通道闸
        /// </summary>
        public const string JSC8ST_Gate = "JSC8ST_Gate";
        /// <summary>
        /// 显示屏
        /// </summary>
        public const string JSC8ST_LedScreen = "JSC8ST_LedScreen";
        /// <summary>
        /// 对应JieLink的速通II型停车场（JSTC1801-01）  设备类型为JSTC1801_01
        /// </summary>
        public const string JSKT6037B = "JSKT6037B";
        /// <summary>
        /// 对应JieLink的速通II型停车场（JSTC1801-01）  设备类型为JSTC1801_01
        /// </summary>
        public const string JSKT6037C = "JSKT6037C";
        /// <summary>
        /// 对应JieLink的速通II型停车场（JSTC1801-01）  设备类型为JSTC1801_01
        /// </summary>
        public const string JSKT6030B_L = "JSKT6030B-L";
        /// <summary>
        /// 与JieLink的速通IV一样
        /// </summary>
        public const string JSTC2801 = "JSTC2801";
        /// <summary>
        /// 月租用户A
        /// </summary>
        public const int MONTH_A = 50;
        /// <summary>
        /// 预付费用户A
        /// </summary>
        public const int PREPAYMENT_A = 51;
    }
}
