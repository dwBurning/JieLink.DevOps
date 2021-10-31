using PartialViewInterface.Utils;
using PartialViewJSRMOrder.DB;
using PartialViewJSRMOrder.ViewModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJSRMOrder.Monitor
{
    class DispatchJob : IJob
    {
        DevJsrmOrderManager devJsrmOrderManager;
        public DispatchJob()
        {
            devJsrmOrderManager = new DevJsrmOrderManager();
        }

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            string receive = data.GetString("ReceiveEmail");
            if (string.IsNullOrEmpty(receive))
            {
                OrderMonitorViewModel.Instance().ShowMessage("接收人的邮箱为空");
                return;
            }

            DataTable dataTable = devJsrmOrderManager.GetDispatchingOrderTable();

            string content = SendEmailHelper.HtmlBody(dataTable);
            
            SendEmailHelper.SendEmailAsync(receive, $"{DateTime.Now.ToString("yyyyMMddHH")}待处理捷服务工单", content, true);
            OrderMonitorViewModel.Instance().ShowMessage($"新增工单{dataTable.Rows.Count}单");

            foreach (DataRow dataRow in dataTable.Rows)
            {
                devJsrmOrderManager.UpdateDispatch(dataRow["problemCode"].ToString());
            }
        }
    }
}
