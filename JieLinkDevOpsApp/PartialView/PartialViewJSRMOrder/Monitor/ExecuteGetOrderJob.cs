using Panuon.UI.Silver;
using PartialViewInterface.Utils;
using PartialViewJSRMOrder.DB;
using PartialViewJSRMOrder.Model;
using PartialViewJSRMOrder.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PartialViewInterface.Utils.HttpHelper;

namespace PartialViewJSRMOrder.Monitor
{
    public class ExecuteGetOrderJob
    {
        static DevJsrmOrderManager devJsrmOrderManager;
        static ExecuteGetOrderJob()
        {
            devJsrmOrderManager = new DevJsrmOrderManager();
        }

        public static async Task<ReturnMsg<PageOrder>> GetOrder(string userName, string url, string token, string userId)
        {
            try
            {
                HttpRequestArgs httpRequestArgs = GetHttpRequestArgsHelper.GetHttpRequestArgs(userName, url, token, userId);
                return await PostAsync<ReturnMsg<PageOrder>>(httpRequestArgs);
            }
            catch (Exception)
            {
                return new ReturnMsg<PageOrder>() { success = false };
            }
        }


        public static void AddOrder(List<Order> orders)
        {
            //18点后工单仍然写入数据库，但是不再分配。避免18点后转到研发并且完成的工单不会被计数
            //if (DateTime.Now.Hour > 18)
            //{
            //    OrderMonitorViewModel.Instance().ShowMessage("18点之后的工单，隔天处理");
            //    return;
            //}

            TaskHelper.Start(() =>
            {
                foreach (var x in orders)
                {
                    GetResponsiblePerson(x);
                    GetYanfaTime(x);
                    GetOverTime(x);
                    Order order = devJsrmOrderManager.GetOrder(x.problemCode);
                    if (order != null)
                    {
                        continue;
                    }
                    x.ReceiveTime = DateTime.Now;

                    OrderMonitorViewModel.Instance().ShowMessage($"新增加工单 {x.problemCode}");
                    OrderMonitorViewModel.Instance().Dispatcher.Invoke(() =>
                    {
                        Notice.Show($"新增加工单 {x.problemCode}", "通知", 3, MessageBoxIcon.Success);
                    });
                    //
                    if (x.problemInfo.Contains('\''))
                        x.problemInfo = x.problemInfo.Replace('\'', ' ');

                    devJsrmOrderManager.AddOrder(x);
                }
            });
        }

        private static void GetYanfaTime(Order order)
        {
            string str = "";
            string str1 = "";
            order.YanFaTime = OrderMonitorViewModel.Instance().GetTimePointByGDAsync(order.problemCode,false,out str, out str1);
        }

        /// <summary>
        /// 计算最后处理时间(超时时间)
        /// </summary>
        /// <param name="order"></param>
        private static void GetOverTime(Order order)
        {
            if (order.YanFaTime == DateTime.MinValue)
            {
                order.OverTime1 = DateTime.MinValue;
                return;
            }
            if (order.YanFaTime.DayOfWeek != DayOfWeek.Friday || order.YanFaTime.DayOfWeek != DayOfWeek.Saturday || order.YanFaTime.DayOfWeek != DayOfWeek.Sunday)
            {
                if (order.YanFaTime.Hour < 19 && order.YanFaTime.Hour >= 9)
                {
                    order.OverTime1 = order.YanFaTime.AddHours(8);
                }
                else
                { 
                    order.OverTime1 = GetOverTime(order.YanFaTime, 1);
                }
            }
            else if (order.YanFaTime.DayOfWeek == DayOfWeek.Friday)
            {
                if (order.YanFaTime.Hour < 19 && order.YanFaTime.Hour > 9)
                {
                    order.OverTime1 = order.YanFaTime.AddHours(8);
                }
                else
                {
                    order.OverTime1 = GetOverTime(order.YanFaTime, 3);
                }
            }
            else if (order.YanFaTime.DayOfWeek == DayOfWeek.Saturday)
            {
                order.OverTime1 = GetOverTime(order.YanFaTime, 2);
            }
            else if (order.YanFaTime.DayOfWeek == DayOfWeek.Sunday)
            {
                order.OverTime1 = GetOverTime(order.YanFaTime, 1);
            }
        }

        private static DateTime GetOverTime(DateTime YanFaTime, int addDays)
        {
            DateTime overTime = new DateTime(1900, 1, 1);
            if (YanFaTime.Month == 12)
            {
                if ((YanFaTime.Day + addDays) > 31)
                {
                    overTime = new DateTime(YanFaTime.Year + 1, 1, YanFaTime.Day + addDays - 31, 17, 0, 0);
                }
                else
                {
                    overTime = new DateTime(YanFaTime.Year, YanFaTime.Month, YanFaTime.Day + addDays, 17, 0, 0);
                }
            }
            else if (YanFaTime.Month == 1 || YanFaTime.Month == 3 || YanFaTime.Month == 5 || YanFaTime.Month == 7 || YanFaTime.Month == 8 || YanFaTime.Month == 10)
            {
                if ((YanFaTime.Day + addDays) > 31)
                {
                    overTime = new DateTime(YanFaTime.Year, YanFaTime.Month + 1, YanFaTime.Day + addDays - 31, 17, 0, 0);
                }
                else
                {
                    overTime = new DateTime(YanFaTime.Year, YanFaTime.Month, YanFaTime.Day + addDays, 17, 0, 0);
                }
            }
            else if (YanFaTime.Month == 4 || YanFaTime.Month == 6 || YanFaTime.Month == 9 || YanFaTime.Month == 11)
            {
                if ((YanFaTime.Day + addDays) > 30)
                {
                    overTime = new DateTime(YanFaTime.Year, YanFaTime.Month + 1, YanFaTime.Day + addDays - 30, 17, 0, 0);
                }
                else
                {
                    overTime = new DateTime(YanFaTime.Year, YanFaTime.Month, YanFaTime.Day + addDays, 17, 0, 0);
                }
            }
            else
            {
                if ((YanFaTime.Day + addDays) > 28)
                {
                    overTime = new DateTime(YanFaTime.Year, YanFaTime.Month + 1, YanFaTime.Day + addDays - 28, 17, 0, 0);
                }
                else
                {
                    overTime = new DateTime(YanFaTime.Year, YanFaTime.Month, YanFaTime.Day + addDays, 17, 0, 0);
                }
            }
            return overTime;
        }

        private static void GetResponsiblePerson(Order order)
        {
            if (order.softVersion.Contains("速通"))
            {
                order.ResponsiblePerson = "江坚";
                return;
            }

            if (order.softVersion.Contains("融合") || order.softVersion.Contains("1.0."))
            {
                order.ResponsiblePerson = "丁小永";
                return;
            }

            if (order.problemInfo.Contains("天启"))
            {
                order.ResponsiblePerson = "邱大发";
                return;
            }

            if (order.problemInfo.Contains("车位引导") || order.softVersion.ToLower().Contains("jsrj11") || order.softVersion.Contains("车位引导"))
            {
                var responsiblePersons_ = devJsrmOrderManager.GetResponsiblePerson();
                bool isExist2 = responsiblePersons_.ContainsKey("李志魁");
                bool isExist3 = responsiblePersons_.ContainsKey("黄其省");
                if (!isExist2)
                {
                    order.ResponsiblePerson = "李志魁";
                }
                else if (!isExist3)
                {
                    order.ResponsiblePerson = "黄其省";
                }
                else
                {
                    var person = responsiblePersons_.OrderBy(p => p.Value).Where(p =>  p.Key == "李志魁"
                    || p.Key == "黄其省").Select(p => p.Key).FirstOrDefault();

                    order.ResponsiblePerson = person;
                }

                return;
            }

            if (order.problemInfo.Contains("访客机"))
            {
                order.ResponsiblePerson = "王浩东";
                return;
            }

            var responsiblePersons = devJsrmOrderManager.GetResponsiblePerson();
            if (order.problemInfo.Contains("门禁"))
            {
                bool isExist1 = responsiblePersons.ContainsKey("王浩东");
                bool isExist2 = responsiblePersons.ContainsKey("马成杰");
                bool isExist3 = responsiblePersons.ContainsKey("胡敏");
                if (!isExist1)
                {
                    order.ResponsiblePerson = "王浩东";
                }
                else if (!isExist2)
                {
                    order.ResponsiblePerson = "马成杰";
                }
                else if (!isExist3)
                {
                    order.ResponsiblePerson = "胡敏";
                }
                else
                {
                    var person = responsiblePersons.OrderBy(p => p.Value).Where(p => p.Key == "王浩东"
                    || p.Key == "马成杰"
                    || p.Key == "胡敏").Select(p => p.Key).FirstOrDefault();

                    order.ResponsiblePerson = person;
                }

                return;
            }

            if (order.userName == "檀峰")//云托管的问题
            {
                bool isExist1 = responsiblePersons.ContainsKey("丁小永");
                bool isExist2 = responsiblePersons.ContainsKey("钟峰");

                if (!isExist1)
                {
                    order.ResponsiblePerson = "丁小永";
                }
                else if (!isExist2)
                {
                    order.ResponsiblePerson = "钟峰";
                }
                else
                {
                    var person = responsiblePersons.OrderBy(p => p.Value).Where(p => p.Key == "丁小永"
                        || p.Key == "钟峰").Select(p => p.Key).FirstOrDefault();

                    order.ResponsiblePerson = person;
                }
            }

            if (order.softVersion.ToLower().Contains("link3"))
            {
                bool isExist1 = responsiblePersons.ContainsKey("梁哲");
                bool isExist2 = responsiblePersons.ContainsKey("文安");
                bool isExist3 = responsiblePersons.ContainsKey("舒兴祥");

                if (!isExist1)
                {
                    order.ResponsiblePerson = "梁哲";
                }
                else if (!isExist2)
                {
                    order.ResponsiblePerson = "文安";
                }
                else if (!isExist3)
                {
                    order.ResponsiblePerson = "舒兴祥";
                }
                else
                {
                    var person = responsiblePersons.OrderBy(p => p.Value).Where(p => p.Key == "梁哲"
                    || p.Key == "文安"
                    || p.Key == "舒兴祥").Select(p => p.Key).FirstOrDefault();

                    order.ResponsiblePerson = person;
                }
            }
            else
            {
                bool isExist1 = responsiblePersons.ContainsKey("丁小永");
                bool isExist2 = responsiblePersons.ContainsKey("段扬扬");
                bool isExist3 = responsiblePersons.ContainsKey("马成杰");
                bool isExist4 = responsiblePersons.ContainsKey("邱大发");
                bool isExist5 = responsiblePersons.ContainsKey("钟峰");

                if (!isExist1)
                {
                    order.ResponsiblePerson = "丁小永";
                }
                else if (!isExist2)
                {
                    order.ResponsiblePerson = "段扬扬";
                }
                else if (!isExist3)
                {
                    order.ResponsiblePerson = "马成杰";
                }
                else if (!isExist4)
                {
                    order.ResponsiblePerson = "邱大发";
                }
                else if (!isExist5)
                {
                    order.ResponsiblePerson = "钟峰";
                }
                else
                {
                    var person = responsiblePersons.OrderBy(p => p.Value).Where(p => p.Key == "丁小永"
                    || p.Key == "段扬扬"
                    || p.Key == "马成杰"
                    || p.Key == "邱大发"
                    || p.Key == "钟峰").Select(p => p.Key).FirstOrDefault();

                    order.ResponsiblePerson = person;
                }
            }
        }
    }
}
