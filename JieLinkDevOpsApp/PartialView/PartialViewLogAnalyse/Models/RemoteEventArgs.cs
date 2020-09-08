using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models
{
    public enum EnumFlowClientEventID
    {
        //[EnumDescription(Text = "收费框放行操作")]
        ConfirmOpenGate,

        //[EnumDescription(Text = "收费框纠正车牌")]
        ChangePlate,

        //[EnumDescription(Text = "收费框修改套餐")]
        ChangeSetmeal,

        //[EnumDescription(Text = "图片墙相关操作")]
        ConfirmPictureWall,

        //[EnumDescription(Text = "对话框回调")]
        MessageBoxCallback,

        //[EnumDescription(Text = "对话框回调(Winform)")]
        MessageBoxCallbackFromWinform,

        //[EnumDescription(Text = "有车位通知")]
        ParkLotAvailable,

        //[EnumDescription(Text = "控制机当面付")]
        ControllerFacePay,

        //[EnumDescription(Text = "无牌车扫码入出场")]
        NoPlateScanQrCode,

        //[EnumDescription(Text = "控制机流程优化确认通行")]
        FlowOptimizeConfrimPass,

        //[EnumDescription(Text = "ETC支付")]
        ControllerEtcPay
    }
    public class RemoteEventArgs
    {

        public string TransactionId { get; set; }


        public EnumFlowClientEventID EventType { get; set; }


        public object Data { get; set; }


        public string DataJson { get; set; }
    }
    public class ConfirmOpenGateArgs
    {
        public int Action { get; set; }


        public int PayTypeId { get; set; }


        public string Remark { get; set; }


        public int CancelReason { get; set; }


        public string OperatorNo { get; set; }


        public string OperatorID { get; set; }


        public string OperatorName { get; set; }


        //public prkDiscount Discount { get; set; }


        public string CapturePath { get; set; }


        public int EventType { get; set; }


        public Dictionary<string, string> Extensions { get; set; }
    }
    public class ConfirmPictureWallArgs
    {
        public int Action { get; set; }


        public string NewPlate { get; set; }


        public int NewPlateColor { get; set; }


        public string EnterRecordId { get; set; }


        public DateTime SelectEnterTime { get; set; }
    }
    public class ChangeSetMealArgs
    {

        public int SetMealNo { get; set; }
    }
    public class ChangePlateArgs
    {

        public string NewPlate { get; set; }


        public int PlateColor { get; set; }


        public string Remark { get; set; }
    }
}
