using MySql.Data.MySqlClient;
using PartialViewCheckUpload.Info;
using PartialViewInterface;
using PartialViewInterface.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewCheckUpload.ViewModels
{
    class CheckUploadViewModels : DependencyObject
    {
        /// <summary>
        /// 从sys_taskinfo表里获取所有任务
        /// </summary>
        public List<TaskInfo> TaskInfos { get; set; }

        public List<SyncInfo> SyncInfos { get; set; }

        /// <summary>
        /// 临时测试用
        /// </summary>
        public DelegateCommand SelectSyncCommand { get; set; }
        public CheckUploadViewModels()
        {
            TaskInfos = new List<TaskInfo>();


            SelectSyncCommand = new DelegateCommand();
            SelectSyncCommand.ExecuteAction = SelectSync;

            GetTaskInfos();
        }

        private void SelectSync(object parameter)
        {
            try
            {
                string sql = $"SELECT AddTime,protocoldata from sync_xmpp_history ORDER BY id desc;";
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];//获取所有的表
                foreach (DataRow dr in dt.Rows)
                {
                    SyncInfo syncinfo = new SyncInfo { AddTime = dr["AddTime"].ToString(), ProtocolData = dr["protocoldata"].ToString()};
                    SyncInfos.Add(syncinfo);
                }
            }
            catch (Exception)
            {

                throw;
            }           
        }

        public void GetTaskInfos()
        {
            try
            {
                string sql = $"SELECT ServiceName,ServiceID from sys_taskinfo WHERE PlatType=0 AND IsSyncData=1 AND Enabled=1 ORDER BY SendPriority asc;";
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];//获取所有的表
                foreach (DataRow dr in dt.Rows)
                {
                    TaskInfo taskinfo = new TaskInfo { ServiceName = dr["ServiceName"].ToString(), ServiceId = dr["ServiceID"].ToString(), IsChecked = true };
                    TaskInfos.Add(taskinfo);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
