using MySql.Data.MySqlClient;
using PartialViewCheckUpload.TaskInfos;
using PartialViewInterface;
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
        public List<TaskInfo> TaskInfos { get; set; }

        private CheckUploadViewModels()
        {
            TaskInfos = new List<TaskInfo>();

            GetTaskInfos();
        }

        public void GetTaskInfos()
        {
            try
            {
                string sql = $"select table_name from information_schema.`TABLES` where TABLE_SCHEMA='{EnvironmentInfo.DbConnEntity.DbName}';";
                DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];//获取所有的表
                foreach (DataRow dr in dt.Rows)
                { 
                    TaskInfo taskinfo = new TaskInfo { ServiceName = dr["ServiceName"].ToString(),ServiceId = dr["ServiceID"].ToString(), IsChecked = true }
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
