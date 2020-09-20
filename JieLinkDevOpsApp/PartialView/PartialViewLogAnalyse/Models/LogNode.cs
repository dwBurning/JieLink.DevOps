using System;
using System.Collections.Generic;
using System.Text;

namespace PartialViewLogAnalyse.Models
{
    public enum LogNodeType
    {
        Begin,
        ManualEnterOut,
        RequestPass,
        MatchEnterRecord,
        AddToDeviceQueue,
        ProcessOnlineBegin,
        ShowMessage,
        ConfirmOpenGateEnter,
        ConfirmOpenGateOut,
        SuspendFlowBegin,
        SuspendFlowEnd,
        RemoteEvent,
        ReceiveFacePay,
        FacePayBegin,
        FacePayTimeOut,
        FacePayReturn,
        FacePayEnd,
        BillChanged,
        OpenGateBegin,
        OpenGateSuccess,
        OpenGateFailed,
        SaveBill,
        SaveRecord,
        ParseVehicleBack,
        EndDeviceFlowProcess,
        CloudSeatBegin,
        CloudSeatAdjust,
        CloudSeatAdjustIn,
        CloudRemoteOpenGate,
        NoPlateScanCode,
        GetOrder,
        NotifyPay
    }
    public class LogNode
    {
        public LogNodeType LogNodeType { get; set; }
        public string ThreadName { get; set; }
        public DateTime LogTime { get; set; }
        public string Message { get; set; }
    }
}
