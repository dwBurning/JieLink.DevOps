using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace PartialViewInterface.Utils
{
	/// <summary>
    /// 数据库连接帮助类
    /// </summary>
	public static class MSSqlHelper
	{
		/// <summary>
        /// 测试数据库
        /// </summary>
		public static bool TestDbConnection(string strConn)
		{
			try
			{
			    bool retConn = false;
				using (SqlConnection connection = new SqlConnection(strConn))
				{
					using (BackgroundWorker bgw = new BackgroundWorker())
					{
						bgw.WorkerSupportsCancellation = true;
						bgw.DoWork += delegate(object o, DoWorkEventArgs e)
						{
							try
							{
								MSSqlHelper.ConnectionStr = strConn;
								connection.Open();
								if (connection.State == ConnectionState.Open)
								{
									retConn = true;
								}
							}
							catch (Exception ex2)
							{
								retConn = false;
							}
						};
						bgw.RunWorkerAsync();
						int overtime = 30;
						for (int i = 0; i < overtime; i++)
						{
							if (retConn)
							{
								break;
							}
							Thread.Sleep(100);
						}
						bgw.CancelAsync();
						bgw.Dispose();
					}
				}
				return retConn;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			
		}

        //// <summary>
        /// 查询语句
        /// </summary>
		public static int ExecuteNonQuery(string cmdText, params SqlParameter[] commandParameters)
		{
			int result = -52;
			using (SqlCommand sqlCommand = new SqlCommand())
			{
				using (SqlConnection sqlConnection = new SqlConnection(MSSqlHelper.ConnectionStr))
				{
					try
					{
						MSSqlHelper.getCmd(sqlCommand, sqlConnection, CommandType.StoredProcedure, cmdText, commandParameters);
						result = sqlCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						throw ex;
					}
					finally
					{
						sqlCommand.Parameters.Clear();
					}
				}
			}
			return result;
		}

	
		private static void getCmd(SqlCommand cmd, SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
		{
			try
			{
				conn.Open();
				cmd.Connection = conn;
				cmd.CommandTimeout = 300;
				cmd.CommandText = cmdText;
				cmd.CommandType = CommandType.StoredProcedure;
				if (cmdParms != null)
				{
					foreach (SqlParameter value in cmdParms)
					{
						cmd.Parameters.Add(value);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		
		public static int ExecuteNonQuery(string cmd)
		{
			int result;
			try
			{
				int ret = -1;
				using (SqlConnection connection = new SqlConnection(MSSqlHelper.ConnectionStr))
				{
					connection.Open();
					using (SqlCommand sqlcmd = new SqlCommand(cmd, connection))
					{
						sqlcmd.CommandText = cmd;
						ret = sqlcmd.ExecuteNonQuery();
					}
				}
				result = ret;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}

		
		public static DataTable ExecuteQuery(string strSql, SqlParameter[] sp)
		{
			DataTable result;
			try
			{
				using (SqlConnection connection = new SqlConnection(MSSqlHelper.ConnectionStr))
				{
					SqlCommand comm = new SqlCommand
					{
						Connection = connection,
						CommandType = CommandType.Text,
						CommandText = strSql
					};
					connection.Open();
					using (SqlDataAdapter sda = new SqlDataAdapter(comm))
					{
						if (sp != null)
						{
							comm.Parameters.AddRange(sp);
						}
						DataTable dt = new DataTable();
						sda.Fill(dt);
						comm.Parameters.Clear();
						result = dt;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}

		
		public static DataTable GetDataTable(string cmdText, params SqlParameter[] commandParameters)
		{
			DataTable dataTable = new DataTable();
			DataTable result;
			using (SqlCommand sqlCommand = new SqlCommand())
			{
				using (SqlConnection sqlConnection = new SqlConnection(MSSqlHelper.ConnectionStr))
				{
					try
					{
						MSSqlHelper.getCmd(sqlCommand, sqlConnection, CommandType.StoredProcedure, cmdText, commandParameters);
						SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
						sqlDataAdapter.SelectCommand = sqlCommand;
						dataTable.TableName = "Table" + DateTime.Now.Millisecond.ToString();
						sqlDataAdapter.Fill(dataTable);
					}
					catch (Exception ex)
					{
						throw ex;
					}
					finally
					{
						sqlCommand.Parameters.Clear();
					}
				}
				result = dataTable;
			}
			return result;
		}

		/// <summary>
        /// 连接字符串
        /// </summary>
		public static string ConnectionStr;
	}
}
