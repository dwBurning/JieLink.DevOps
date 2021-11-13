using PartialViewInterface.Utils;
using PartialViewJSRMOrder.DB;
using PartialViewJSRMOrder.Model;
using PartialViewJSRMOrder.ViewModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PartialViewInterface.Utils.HttpHelper;

namespace PartialViewJSRMOrder.Monitor
{
    class GetOrderJob : IJob
    {
        DevJsrmOrderManager devJsrmOrderManager;

        public GetOrderJob()
        {
            devJsrmOrderManager = new DevJsrmOrderManager();
        }

        public async void Execute(IJobExecutionContext context)
        {
            //18点后工单仍然写入数据库，但是不再分配。避免18点后转到研发并且完成的工单不会被计数
            //if (DateTime.Now.Hour >= 18)
            //{
            //    OrderMonitorViewModel.Instance().ShowMessage("18点之后的工单，隔天处理");
            //    return;
            //}


            JobDataMap data = context.JobDetail.JobDataMap;
            string requestArgs = data.GetString("HttpRequestArgs");
            if (string.IsNullOrEmpty(requestArgs))
            {
                OrderMonitorViewModel.Instance().ShowMessage("请求参数为空");
                return;
            }

            HttpRequestArgs httpRequestArgs = JsonHelper.DeserializeObject<HttpRequestArgs>(requestArgs);
            ReturnMsg<PageOrder> returnMsg = await PostAsync<ReturnMsg<PageOrder>>(httpRequestArgs);
            if (returnMsg.success)
            {
                OrderMonitorViewModel.Instance().ShowMessage("工单请求成功");
                ExecuteGetOrderJob.AddOrder(returnMsg.respData.data);
            }
            else
            {
                OrderMonitorViewModel.Instance().ShowMessage(returnMsg.respMsg);
            }
        }
    }
}
