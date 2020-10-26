using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panuon.UI.Silver;
using Microsoft.Win32;
using PartialViewInterface.Utils;
using MySql.Data.MySqlClient;
using System.Data;
using PartialViewInterface;
using System.Threading;
using System.ComponentModel;
using System.Windows;

namespace PartialViewAutoCorrectionParkNumber
{
    partial class AutoCorrectParkNum : Window
	{
		/// <summary>
		/// 循环时间
		/// </summary>
		public static int LoopTime;

		/// <summary>
		/// 委托
		/// </summary>
		public delegate void DeleFun(string str);
		/// <summary>
		/// 命令=>调用
		/// </summary>
		/// 
		public static event DeleFun DeleEvent;


		BackgroundWorker bgw = new BackgroundWorker();

		public AutoCorrectParkNum()
		{
			bgw.DoWork += (StartCorrection);
		}

		/// <summary>
		/// 开始线程
		/// </summary>
		/// <param name="action"></param>
		public void DoWork(int action)
        {
			//bgw.WorkerReportsProgress = true;
			bgw.RunWorkerAsync();
		}

		/// <summary>
		/// 停止线程
		/// </summary>
		public void StopWork()
		{
			bgw.WorkerSupportsCancellation = true;
			bgw.CancelAsync();
			bgw.Dispose();
		}

		/// <summary>
		/// 矫正车位数
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void StartCorrection(object sender, DoWorkEventArgs e)
        {
			try
			{
				while (true)
				{
					//总车位数
					int TotalPlace = 0;
					//剩余车位数
					int RemainPlace = 0;
					//场内记录数
					int CountPlace = 0;
					string commandText = "select TotalParkingPlace,RemainParkingPlace from sys_area_parking_place";
					DataTable dataTable = MySqlHelper.ExecuteDataset(EnvironmentInfo.ConnectionString, commandText).Tables[0];
					if (dataTable != null & dataTable.Rows.Count > 0)
					{
						TotalPlace = int.Parse(dataTable.Rows[0]["TotalParkingPlace"].ToString());
						RemainPlace = int.Parse(dataTable.Rows[0]["RemainParkingPlace"].ToString());
					}
					//统计场内记录
					commandText = "select count(*) from box_enter_record where wasgone=0";
					object obj = MySqlHelper.ExecuteScalar(EnvironmentInfo.ConnectionString, commandText);
					if (!obj.Equals(DBNull.Value))
					{
						CountPlace = int.Parse(obj.ToString());
					}
					if (TotalPlace - RemainPlace != CountPlace)
					{
						int SetPlace = TotalPlace - CountPlace;
						if (SetPlace < 0)
						{
							SetPlace = 0;
						}
						//更新车位数
						commandText = string.Format("update sys_area_parking_place set RemainParkingPlace={0} ", SetPlace);
						int num = MySqlHelper.ExecuteNonQuery(EnvironmentInfo.ConnectionString, commandText, new MySqlParameter[0]);
						if (num > 0)
						{
							this.Dispatcher.Invoke(new Action(delegate
							{
								DeleEvent("更新车位数成功！");
							}));
						}
					}
					
					this.Dispatcher.Invoke(new Action(delegate
					{
						DeleEvent(string.Format("总车位数:{0} 剩余车位数:{1} 场内记录数:{2}", TotalPlace, RemainPlace, CountPlace));
					}));
					//测试使用
					Thread.Sleep(60000 * LoopTime);
					//Thread.Sleep(1000*LoopTime);
					if ((bgw.CancellationPending == true))
					{
						break;
					}
				}


				return ;
			}
			catch (Exception ex)
			{
				this.Dispatcher.Invoke(new Action(delegate
				{
					DeleEvent(ex.ToString());
					DeleEvent("设置车位数出现错误！");
				}));
				return ;

			}
		}
    }
}
