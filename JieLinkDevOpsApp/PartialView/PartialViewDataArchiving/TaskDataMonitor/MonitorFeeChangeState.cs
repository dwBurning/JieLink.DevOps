using MySql.Data.MySqlClient;
using PartialViewInterface;
using PartialViewInterface.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.TaskDataMonitor
{
    /// <summary>
    /// 检查dic_detail中有没有插入dm_park_event_feechangestate
    /// 监控dm_park_event_feechangestate在xmpp表中是否出现了积累
    /// </summary>
    public class MonitorFeeChangeState : IMonitor
    {
        public void Monitor()
        {
            string dm_park_data_handOpen = @"INSERT INTO `dic_detail` (`DicTypeId`, `DicDetailId`, `DicDetailName`, `DicEnDetailName`, `NisspCode`, `NisspName`, `Seq`, `Valid`) select
1024, 90, '子系统上传手动开闸详情到云平台', 'dm_park_data_handOpen', 'park/handOpen', '子系统上传手动开闸详情到云平台', 90, 1 from dual where not exists
(select DicEnDetailName from dic_detail where DicTypeId = 1024 and DicEnDetailName = 'dm_park_data_handOpen'); ";
            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, dm_park_data_handOpen);

            string dm_park_event_feeChangeState = @"INSERT INTO `dic_detail` (`DicTypeId`, `DicDetailId`, `DicDetailName`, `DicEnDetailName`, `NisspCode`, `NisspName`, `Seq`, `Valid`) select
1024, 91, '子系统上传停车费稽核事件到云平台', 'dm_park_event_feeChangeState', 'park/event/feeChangeState', '子系统上传停车费稽核事件到云平台', 91, 1 from dual where not exists
(select DicEnDetailName from dic_detail where DicTypeId = 1024 and DicEnDetailName = 'dm_park_event_feeChangeState'); ";
            MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, dm_park_event_feeChangeState);

            string query = "select count(*) from sync_xmpp where ServiceId in('dm_park_data_handOpen','dm_park_event_feeChangeState');";
            object result = MySqlHelper.ExecuteScalar(EnvironmentInfo.ConnectionString, query);
            if (int.Parse(result.ToString()) > 100)
            {
                string delete = "delete from sync_xmpp where ServiceId in('dm_park_data_handOpen','dm_park_event_feeChangeState');";
                LogHelper.CommLogger.Info(delete);
                MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString,delete);
            }
        }
    }
}
