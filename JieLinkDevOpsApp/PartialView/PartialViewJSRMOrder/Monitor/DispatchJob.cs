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
    class DispatchJob : IJob
    {
        DevJsrmOrderManager devJsrmOrderManager;
        public DispatchJob()
        {
            devJsrmOrderManager = new DevJsrmOrderManager();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //过了晚上十二点，把前一天未完成的 并且未分配的工单的接收时间设为今日
                if (DateTime.Now.Hour == 1)
                {
                    devJsrmOrderManager.UpdateYesterdayFinsihTime();
                    OrderMonitorViewModel.Instance().ShowMessage("更新前一晚数据");
                }

                if (DateTime.Now.Hour >= 18 || DateTime.Now.Hour < 8)
                {
                    OrderMonitorViewModel.Instance().ShowMessage("18点之后的工单，隔天处理");
                    return;
                }

                JobDataMap data = context.JobDetail.JobDataMap;
                string receive = data.GetString("ReceiveEmail");
                if (string.IsNullOrEmpty(receive))
                {
                    OrderMonitorViewModel.Instance().ShowMessage("接收人的邮箱为空");
                    return;
                }
                
                DataTable dataTable = devJsrmOrderManager.GetDispatchingOrderTable();
            
                List<Order> orders = devJsrmOrderManager.ConvertToOrderList(dataTable);

                //获取已处理工单的完成时间
                orders.Where(x => x.FinishTime == DateTime.MinValue).ToList().ForEach(x =>
                {
                    string TrueResponsiblePerson = "";
                    x.FinishTime = OrderMonitorViewModel.Instance().GetTimePointByGDAsync(x.problemCode, true,out TrueResponsiblePerson);
                    if(x.FinishTime != DateTime.MinValue)
                        devJsrmOrderManager.UpdateFinsihTime(x.problemCode, x.FinishTime, TrueResponsiblePerson);
                });
                DataTable dataTableForEmail = devJsrmOrderManager.GetDispatchingOrderTableForEmail();

                int count = orders.Where(x => x.Dispatched == 0).Count();

                var isDelay = devJsrmOrderManager.GetIsDelay(7);

                if (count <= 0 && isDelay.Count == 0)
                {
                    OrderMonitorViewModel.Instance().ShowMessage("没有新增的工单");
                    return;
                }

                //if (!(count >= 2 || DateTime.Now.Hour > 16))
                //{
                //    OrderMonitorViewModel.Instance().ShowMessage($"新增工单{count}单，暂不发送邮件");
                //    return;
                //}

                string content = SendEmailHelper.HtmlBody(dataTableForEmail);

                //仅在上班时间报这个警
                if(isDelay.Count > 0 && DateTime.Now.Hour >= 9 && DateTime.Now.Hour < 18)
                {
                    string title = isDelay[0] + "等" + isDelay.Count +  "个工单即将超时，需优先处理";
                    SendEmailHelper.SendEmailAsync(receive, title, content, true);
                    OrderMonitorViewModel.Instance().ShowMessage(title);
                }
                else if(count > 0)
                {
                    SendEmailHelper.SendEmailAsync(receive, $"{DateTime.Now.ToString("yyyyMMddHH")}待处理捷服务工单", content, true);
                    OrderMonitorViewModel.Instance().ShowMessage($"新增工单{count}单，已发送邮件");
                }

                orders.Where(x => x.Dispatched == 0).ToList().ForEach(x =>
                {
                    devJsrmOrderManager.UpdateDispatch(x.problemCode);
                });
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error(ex.ToString());
            }
        }
    }
}
