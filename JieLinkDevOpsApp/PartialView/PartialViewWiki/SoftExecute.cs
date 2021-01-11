using MySql.Data.MySqlClient;
using Panuon.UI.Silver;
using PartialViewInterface;
using PartialViewWiki.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PartialViewWiki
{
    public class SoftExecute
    {
        public void ChangeDevice()
        {
            ChangeDeviceWindow window = new ChangeDeviceWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            (Application.Current.MainWindow as WindowX).IsMaskVisible = true;
            window.ShowDialog();
            (Application.Current.MainWindow as WindowX).IsMaskVisible = false;
        }

        public void ImportPersonFaild()
        {
            string sql = "update person_import set EnterTime=null,Birthday=null;";
            int result = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, sql);
            if (result > 0)
            {
                Notice.Show("数据已修订，请重新执行入库！", "通知", 3, MessageBoxIcon.Success);
            }
            else
            {
                Notice.Show("请确认person_import表中是否有数据？", "通知", 3, MessageBoxIcon.Warning);
            }
        }


        private void RepairEnterDeviceId()
        {
            // 查询场内记录中 当前设备列表不存在设备ID，和设备名称
            string sql = @"select DISTINCT EnterDeviceID,EnterDeviceName from box_enter_record where WasGone=0 and EnterDeviceID not in (
select DeviceID from control_devices where DeviceType in(select DicDetailId from dic_detail where DicTypeId = 1000 and NisspCode = '0207')); ";

            DataTable dt = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, sql).Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                string cmd = $"select * from control_devices where DeviceName='{dr["EnterDeviceName"]}' and DeviceType in(select DicDetailId from dic_detail where DicTypeId=1000 and NisspCode='0207');";
            }

        }

    }
}
