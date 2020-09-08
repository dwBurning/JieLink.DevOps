using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models
{
    public class CloudSeatDownCommandParam
    {

        public string eventId { get; set; }


        public DateTime operateTime { get; set; }


        public string operateId { get; set; }


        public string operateName { get; set; }


        public long timestamp { get; set; }


        public string vehicleInfo { get; set; }


        public string inRecordId { get; set; }


        public DateTime inTime { get; set; }


        public string extStr1 { get; set; }


        public bool isShowLoadFeeMsg
        {
            get
            {
                return this._isShowLoadFeeMsg;
            }
            set
            {
                this._isShowLoadFeeMsg = value;
            }
        }

        private bool _isShowLoadFeeMsg = true;
    }
}
