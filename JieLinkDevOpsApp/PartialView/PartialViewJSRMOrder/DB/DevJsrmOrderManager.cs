using PartialViewInterface;
using PartialViewInterface.Utils;
using PartialViewJSRMOrder.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewJSRMOrder.DB
{
    public class DevJsrmOrderManager
    {
        public Order GetOrder(string problemCode)
        {
            string error = "";
            DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select * from dev_jsrm_order where problemCode='{problemCode}';");
            Order order = null;
            foreach (DataRow dr in dataTable.Rows)
            {
                order = new Order();
                order.problemCode = dr["problemCode"].ToString();
                order.projectName = dr["projectName"].ToString();
                order.problemInfo = dr["problemInfo"].ToString();
                order.userName = dr["userName"].ToString();
                order.problemTime = dr["problemTime"].ToString();
                order.remoteAccount = dr["remoteAccount"].ToString();
                order.softVersion = dr["softVersion"].ToString();
                order.Dispatched = int.Parse(dr["Dispatched"].ToString());
                order.ReceiveTime = DateTime.Parse(dr["ReceiveTime"].ToString());
                order.ResponsiblePerson = dr["ResponsiblePerson"].ToString();
            }

            return order;
        }

        public void AddOrder(Order order)
        {
            EnvironmentInfo.SqliteHelper.ExecuteSql($"insert into dev_jsrm_order values('{order.problemCode}','{order.projectName}','{order.problemInfo}','{order.userName}','{order.problemTime}','{order.remoteAccount}','{order.softVersion}',{order.Dispatched},'{order.ReceiveTime.ToString("yyyy-MM-dd HH:mm:ss")}','{order.ResponsiblePerson}','{order.YanFaTime.ToString("yyyy-MM-dd HH:mm:ss")}',null);");
        }

        public List<Order> GetDispatchingOrderList()
        {
            string error = "";
            DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select * from dev_jsrm_order where Dispatched=0;");
            List<Order> orders = ConvertToOrderList(dataTable);
            return orders;
        }

        public List<Order> ConvertToOrderList(DataTable dataTable)
        {
            List<Order> orders = new List<Order>();
            foreach (DataRow dr in dataTable.Rows)
            {
                Order order = new Order();
                order.problemCode = dr["problemCode"].ToString();
                order.projectName = dr["projectName"].ToString();
                order.problemInfo = dr["problemInfo"].ToString();
                order.userName = dr["userName"].ToString();
                order.problemTime = dr["problemTime"].ToString();
                order.remoteAccount = dr["remoteAccount"].ToString();
                order.softVersion = dr["softVersion"].ToString();
                order.Dispatched = int.Parse(dr["Dispatched"].ToString());
                order.ReceiveTime = DateTime.Parse(dr["ReceiveTime"].ToString());
                order.ResponsiblePerson = dr["ResponsiblePerson"].ToString();
                if (dr["YanfaTime"].ToString() != "")
                    order.YanFaTime = DateTime.Parse(dr["YanfaTime"].ToString());
                else
                    order.YanFaTime = DateTime.MinValue;
                if (dr["FinishTime"].ToString() != "")
                    order.FinishTime = DateTime.Parse(dr["FinishTime"].ToString());
                else
                    order.FinishTime = DateTime.MinValue;

                orders.Add(order);
            }

            return orders;
        }

        public DataTable GetDispatchingOrderTable()
        {
            string error = "";
            return EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select * from dev_jsrm_order where ReceiveTime>'{DateTime.Now.ToString("yyyy-MM-dd")}';");
        }
        public DataTable GetYesterdayDispatchingOrderTable()
        {
            string error = "";
            return EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select * from dev_jsrm_order where ReceiveTime>'{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}' and ReceiveTime<'{DateTime.Now.ToString("yyyy-MM-dd")}';");
        }
        public DataTable GetDispatchingOrderTableForEmail()
        {
            string error = "";
            string sql = $"select problemCode as '工单号',projectName as '项目名称',problemInfo as '问题描述',softversion as '版本' ,problemtime as '提交时间',YanfaTime as '转到研发时间',finishtime as '完成时间',ResponsiblePerson as '责任人',dispatched from dev_jsrm_order where ReceiveTime>'{DateTime.Now.ToString("yyyy-MM-dd")}';";
            return EnvironmentInfo.SqliteHelper.GetDataTable(out error,sql);
        }
        public DataTable GetYesterdayDispatchingOrderTableForEmail()
        {
            string error = "";
            return EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select problemCode as '工单号',projectName as '项目名称',problemInfo as '问题描述',softversion as '版本' ,problemtime as '提交时间',YanfaTime as '转到研发时间',finishtime as '完成时间',ResponsiblePerson as '责任人' from dev_jsrm_order where ReceiveTime>'{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}' and ReceiveTime<'{DateTime.Now.ToString("yyyy-MM-dd")}';");
        }
        public void UpdateDispatch(string problemCode)
        {
            EnvironmentInfo.SqliteHelper.ExecuteSql($"update dev_jsrm_order set Dispatched=1 where problemCode='{problemCode}';");
        }
        public void UpdateFinsihTime(string problemCode,DateTime finishtime,string TrueResponsiblePerson)
        {
            EnvironmentInfo.SqliteHelper.ExecuteSql($"update dev_jsrm_order set ResponsiblePerson = '{TrueResponsiblePerson}' ,finishtime='{finishtime.ToString("yyyy-MM-dd HH:mm:ss")}' where problemCode='{problemCode}';");
        }
        /// <summary>
        /// 更新未完成并且未分配工单的接收时间，以便再分配
        /// </summary>
        public void UpdateYesterdayFinsihTime()
        {
            EnvironmentInfo.SqliteHelper.ExecuteSql($"update dev_jsrm_order set receivetime = '{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}' where dispatched = 0 and ReceiveTime>'{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}' and ReceiveTime<'{DateTime.Now.ToString("yyyy-MM-dd")}';");
        }
        public Dictionary<string, int> GetResponsiblePerson()
        {
            string error = "";
            DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select ResponsiblePerson,COUNT(*) as Count from dev_jsrm_order WHERE ReceiveTime>'{DateTime.Now.ToString("yyyy-MM-dd")}' GROUP BY ResponsiblePerson;");
            Dictionary<string, int> responsiblePersons = new Dictionary<string, int>();
            foreach (DataRow dr in dataTable.Rows)
            {
                responsiblePersons.Add(dr["ResponsiblePerson"].ToString(), int.Parse(dr["Count"].ToString()));
            }

            return responsiblePersons;
        }

        public List<string> GetIsDelay(int delaytime)
        {
            List<string> ret = new List<string>();
            try
            {
                string error = "";
                DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select projectName,YanfaTime from dev_jsrm_order where FinishTime is null");
                //Dictionary<string, DateTime> DelayJob = new Dictionary<string, DateTime>();
                foreach (DataRow dr in dataTable.Rows)
                {
                    if(!string.IsNullOrEmpty(dr["YanfaTime"].ToString()))
                    {
                        var yanfatime = (DateTime)dr["YanfaTime"];
                        if (yanfatime.AddHours(delaytime) < DateTime.Now)
                            ret.Add(dr["projectName"].ToString());
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                return ret;
            }
            
        }
    }
}
