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
            EnvironmentInfo.SqliteHelper.ExecuteSql($"insert into dev_jsrm_order values('{order.problemCode}','{order.projectName}','{order.problemInfo}','{order.userName}','{order.problemTime}','{order.remoteAccount}','{order.softVersion}',{order.Dispatched},'{order.ReceiveTime.ToString("yyyy-MM-dd HH:mm:ss")}','{order.ResponsiblePerson}');");
        }

        public List<Order> GetDispatchingOrderList()
        {
            string error = "";
            DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select * from dev_jsrm_order where Dispatched=0;");
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
                orders.Add(order);
            }
            return orders;
        }

        public DataTable GetDispatchingOrderTable()
        {
            string error = "";
            return  EnvironmentInfo.SqliteHelper.GetDataTable(out error, $"select * from dev_jsrm_order where Dispatched=0;");
        }

        public void UpdateDispatch(string problemCode)
        {
            EnvironmentInfo.SqliteHelper.ExecuteSql($"update dev_jsrm_order set Dispatched=1 where problemCode='{problemCode}';");
        }
    }
}
