using PartialViewInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public static class DevOpsAPI
    {
        public static void SendEvent(WarningMessage warning)
        {
            Console.WriteLine("发送报警事件，{0}", warning.Message);
            DevOpsEvent opsEvent = new DevOpsEvent();
            opsEvent.EventType = (int)warning.WarningType;
            opsEvent.OperatorDate = DateTime.Now;
            opsEvent.ProjectNo = EnvironmentInfo.ProjectNo ?? string.Empty;
            opsEvent.ProjectName = EnvironmentInfo.ProjectName ?? string.Empty;
            opsEvent.ProjectVersion = EnvironmentInfo.ProjectVersion ?? string.Empty;
            opsEvent.RemoteAccount = EnvironmentInfo.RemoteAccount ?? string.Empty;
            opsEvent.RemotePassword = EnvironmentInfo.RemotePassword ?? string.Empty;
            opsEvent.ContactPhone = EnvironmentInfo.ContactPhone ?? string.Empty;
            opsEvent.ContactName = EnvironmentInfo.ContactName ?? string.Empty;

            try
            {
                string url = string.Format("{0}/devops/reportDevOpsEvent", EnvironmentInfo.ServerUrl);
                var returnData = HttpHelper.Post<ReturnData>(url, opsEvent, 3000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static DevOpsProduct ReportVersion()
        {
            try
            {
                string url = string.Format("{0}/devops/reportProjectInfo", EnvironmentInfo.ServerUrl);
                ProjectInfo projectInfo = new ProjectInfo();
                projectInfo.ProjectNo = EnvironmentInfo.ProjectNo ?? "";
                projectInfo.ProjectName = EnvironmentInfo.ProjectName ?? "";
                projectInfo.ProjectVersion = EnvironmentInfo.ProjectVersion ?? "";
                projectInfo.DevopsVersion = EnvironmentInfo.CurrentVersion;
                projectInfo.ProductType = enumProductType.DevOps;
                return HttpHelper.Post<DevOpsProduct>(url, projectInfo, 3000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
