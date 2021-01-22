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


        

    }
}
