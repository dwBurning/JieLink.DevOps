using PartialViewInterface.Utils;
using PartialViewJSRMOrder.DB;
using PartialViewJSRMOrder.Model;
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
    class YesterdayReportJob : IJob
    {
        DevJsrmOrderManager devJsrmOrderManager;
        public YesterdayReportJob()
        {
            devJsrmOrderManager = new DevJsrmOrderManager();
        }

        /// <summary>
        /// 发送昨日报表
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            string receive = data.GetString("ReceiveEmail");
            if (string.IsNullOrEmpty(receive))
            {
                OrderMonitorViewModel.Instance().ShowMessage("接收人的邮箱为空");
                return;
            }

            DataTable dataTable = devJsrmOrderManager.GetYesterdayDispatchingOrderTable();
            
            List<Order> orders = devJsrmOrderManager.ConvertToOrderList(dataTable);

            //获取已处理工单的完成时间
            orders.Where(x => x.FinishTime == DateTime.MinValue).ToList().ForEach(x =>
            {
                string TrueResponsiblePerson = "";
                string SolutionInfo = "";
                x.FinishTime = OrderMonitorViewModel.Instance().GetTimePointByGDAsync(x.problemCode, true,out TrueResponsiblePerson, out SolutionInfo);
                if(x.FinishTime != DateTime.MinValue)
                    devJsrmOrderManager.UpdateFinsihTime(x.problemCode, x.FinishTime, TrueResponsiblePerson, SolutionInfo);
            });
            DataTable dataTableForEmail = devJsrmOrderManager.GetYesterdayDispatchingOrderTableForEmail();

            int count = orders.Count();
            if (count <= 0)
            {
                OrderMonitorViewModel.Instance().ShowMessage("无昨日工单");
                return;
            }

            string content = SendEmailHelper.HtmlBody(dataTableForEmail);

            SendEmailHelper.SendEmailAsync(receive, $"{DateTime.Now.AddDays(-1).ToString("yyyyMMdd")}日捷服务处理报表", content, true);
            OrderMonitorViewModel.Instance().ShowMessage($"已发送昨日报表邮件");

            orders.Where(x => x.Dispatched == 0).ToList().ForEach(x =>
            {
                devJsrmOrderManager.UpdateDispatch(x.problemCode);
            });
        }
    }
}
