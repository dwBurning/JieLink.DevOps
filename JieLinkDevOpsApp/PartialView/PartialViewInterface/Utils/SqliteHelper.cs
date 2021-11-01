using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class SqliteHelper : IDisposable
    {
        public SQLiteConnection conn;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (conn != null)
                {
                    conn.Dispose();
                    conn = null;
                }
        }

        ~SqliteHelper()
        {
            Dispose(false);
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="dataBaseName">数据库名</param>
        public SqliteHelper(string dataBaseName)
        {
            string connString = string.Format(@"Data Source={0}", dataBaseName);
            conn = new SQLiteConnection(connString);
            conn.Open();
        }

        /// <summary>
        /// 手动打开数据库。
        /// </summary>
        public void SqliteOpen()
        {
            if (conn != null && conn.State == ConnectionState.Closed)
                conn.Open();
        }

        /// <summary>
        /// 通过执行SQL语句，获取表中数据。
        /// </summary>
        /// <param name="sError">错误信息</param>
        /// <param name="sSQL">执行的SQL语句</param>
        public DataTable GetDataTable(out string sError, string sSQL)
        {
            DataTable dt = null;
            sError = string.Empty;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand() { CommandText = sSQL, Connection = conn };
                SQLiteDataAdapter dao = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                dao.Fill(dt);
            }
            catch (Exception e)
            {
                sError = e.Message;
            }
            return dt;
        }

        /// <summary>
        /// 通过执行SQL语句，获取表中数据个数。
        /// </summary>
        /// <param name="sError">错误信息</param>
        /// <param name="sSQL">执行的SQL语句</param>
        public int GetDataCount(out string sError, string sSQL)
        {
            DataTable dt = new DataTable();
            sError = string.Empty;
            SQLiteCommand cmd = new SQLiteCommand() { CommandText = sSQL, Connection = conn };
            try
            {
                SQLiteDataAdapter dao = new SQLiteDataAdapter(cmd);
                dao.Fill(dt);
                cmd.Dispose();
            }
            catch (Exception e)
            {
                sError = e.Message;
            }
            finally { cmd.Dispose(); }
            return int.Parse(dt.Rows[0][0].ToString());
        }

        /// <summary>
        /// 通过执行SQL语句，执行insert,update,delete 动作，也可以使用事务。
        /// </summary>
        /// <param name="sError">错误信息</param>
        /// <param name="sSQL">执行的SQL语句</param>
        /// <param name="bUseTransaction">是否使用事务</param>
        public bool UpdateData(out string sError, string sSQL, bool bUseTransaction = false)
        {
            bool iResult = false;
            sError = string.Empty;
            if (!bUseTransaction)
            {
                try
                {
                    SQLiteCommand comm = new SQLiteCommand(conn) { CommandText = sSQL };
                    iResult = comm.ExecuteNonQuery() > 0;
                    comm.Dispose();
                }
                catch (Exception ex)
                {
                    sError = ex.Message;
                }
            }
            else// 使用事务
            {
                DbTransaction trans = null;
                trans = conn.BeginTransaction();
                SQLiteCommand comm = new SQLiteCommand(conn) { CommandText = sSQL };
                try
                {
                    iResult = comm.ExecuteNonQuery() > 0;
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    sError = ex.Message;
                    iResult = false;
                    trans.Rollback();
                }
                finally { comm.Dispose(); trans.Dispose(); }
            }
            return iResult;
        }

        /// <summary>
        /// 使用事务执行多条相同的带参数的SQL语句。
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <param name="sqLiteParameters">每次SQL执行的参数</param>
        public void ExecuteSqlTran(string sqlString, object[][] sqLiteParameters)
        {
            if (sqLiteParameters.Length == 0)
                return;
            using (DbTransaction trans = conn.BeginTransaction())
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;
                try
                {
                    for (int i = 0; i < sqLiteParameters[0].Length; i++)
                    {
                        cmd.Parameters.Add(cmd.CreateParameter());
                    }
                    //循环
                    foreach (object[] sqlParameters in sqLiteParameters)
                    {
                        ExecuteSqlNonQuery(cmd, sqlString, sqlParameters);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    cmd.Dispose(); trans.Dispose();
                }
            }
        }

        /// <summary>
        /// 不使用事务执行一条带参数的SQL语句。
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <param name="sqLiteParameters">SQL执行的参数</param>
        public void ExecuteSql(string sqlString, params object[] sqLiteParameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlString;
            try
            {
                for (int i = 0; i < sqLiteParameters.Length; i++)
                {
                    cmd.Parameters.Add(cmd.CreateParameter());
                    cmd.Parameters[i].Value = sqLiteParameters[i];
                }
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cmd.Dispose();
            }
        }

        private void ExecuteSqlNonQuery(SQLiteCommand cmd, string cmdText, object[] cmdParms)
        {
            cmd.CommandText = cmdText;
            if (cmdParms != null)
            {
                for (int i = 0; i < cmdParms.Length; i++)
                {
                    cmd.Parameters[i].Value = cmdParms[i];
                }
            }
            cmd.ExecuteNonQuery();
        }
    }
}
