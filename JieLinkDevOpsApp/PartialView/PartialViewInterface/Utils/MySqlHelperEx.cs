using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class MySqlHelperEx
    {
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="commandText">Sql语句</param>
        /// <param name="commandTimeout">超时时间，单位秒，默认3600秒</param>
        /// <returns></returns>
        public static int ExecuteNonQueryEx(string connectionString, string commandText, int commandTimeout = 3600)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = commandTimeout;
                try
                {
                    cmd.CommandText = commandText;
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}
