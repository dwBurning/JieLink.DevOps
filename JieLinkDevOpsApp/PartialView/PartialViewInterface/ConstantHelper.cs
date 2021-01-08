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
        /// 组织根节点
        /// </summary>
        public const string GROUPROOTPARENTID = "00000000-0000-0000-0000-000000000000";
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
    }
}
