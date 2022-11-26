using JieShun.JieLink.DevOps.Encrypter.Models;
using MySql.Data.MySqlClient;
using PartialViewEncrypter.Models;
using PartialViewInterface.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JieShun.JieLink.DevOps.Encrypter.Utils
{
    public class EncryptorHelper
    {
        private int progress;
        private int groupCount;
        private object locker = new object();

        public Action<int, string> callback;
        public string db;
        public string connStr;
        public string sqlFindColumns;
        public EnumCMD cmd;
        public string path;

        public EncryptorHelper(Action<int, string> callback, string db, string connStr, string sqlFindColumns, EnumCMD cmd, string path)
        {
            this.callback = callback;
            this.db = db;
            this.connStr = connStr;
            this.sqlFindColumns = sqlFindColumns;
            this.cmd = cmd;
            this.path = path;
        }

        public async Task StartAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                if (cmd == EnumCMD.EncryptToSQL || cmd == EnumCMD.EncryptToDatabase || cmd == EnumCMD.DecryptToSQL || cmd == EnumCMD.DecryptToDataBase)
                {
                    StartDB();
                }
                else
                {
                    StartFile();
                }
            });
        }


        public void StartDB()
        {
            bool ok = false;
            string message = "升级成功";
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ok = ExecuteEncryptDatabase();
                sw.Stop();
                LogHelper.CommLogger.Info($"加密总耗时：{sw.ElapsedMilliseconds}");
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                ok = false;
                message = ex.Message;
                Console.WriteLine(ex.Message);
                callback?.Invoke(100, ex.Message);
                Thread.Sleep(500);
                throw ex;
            }

        }

        public void StartFile()
        {
            bool ok = false;
            string message = "升级成功";
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ok = ExecuteEncryptFile();
                sw.Stop();
                LogHelper.CommLogger.Info($"加密总耗时：{sw.ElapsedMilliseconds}");
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                ok = false;
                message = ex.Message;
                Console.WriteLine(ex.Message);
                callback?.Invoke(100, ex.Message);
                Thread.Sleep(500);
                throw ex;
            }

        }

        /// <summary>
        /// 加密数据库
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="dbs">操作数据库</param>
        /// <param name="connStrs">连接字符串</param>
        /// <param name="sqlFindColumns">查找语句</param>
        /// <param name="cmd">命令</param>
        /// <returns></returns>
        public bool ExecuteEncryptDatabase()
        {
            #region 查询待加密表和字段

            int progress = 10;
            callback?.Invoke(progress, "开始查询待加密表和字段");
            LogHelper.CommLogger.Info("progress:{0},开始查询待加密表和字段", progress.ToString());
            callback?.Invoke(progress, $"开始加密数据库{db}");
            LogHelper.CommLogger.Info($"开始加密数据库{db}");
            LogHelper.CommLogger.Info("获取待加密表和字段：{0}", sqlFindColumns);

            Dictionary<string, List<string>> tableColumns = new Dictionary<string, List<string>>();  //把需要加密的字段按表名称分组，生成SQL可以一次性修改同一个表内的多个字段
            DataTable dt = MySqlHelper.ExecuteDataset(connStr, sqlFindColumns).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                var table = dr["TABLE_NAME"].ToString();
                var column = dr["COLUMN_NAME"].ToString();

                if (!tableColumns.ContainsKey(table))
                {
                    var list = new List<string>();
                    list.Add(column);
                    tableColumns.Add(table, list);
                }
                else
                {
                    tableColumns[table].Add(column);
                }
                LogHelper.CommLogger.Info($"查询到待加密表：{table}，字段：{column}");
            }
           
            callback?.Invoke(progress, "查询待加密表和字段完成");

            #endregion

            #region 对每个表进行加密

            path = GetScriptPath(db); 
            int each = 90 / tableColumns.Count; //每个表代表的 进度
            foreach (var item in tableColumns)
            {
                try
                {
                    LogHelper.CommLogger.Info($"正在加密表{item.Key}");
                    callback?.Invoke(Math.Min(progress += each, 90), $"正在加密表{item.Key}，请勿关闭窗口……");
                    Encrypt(item);
                    LogHelper.CommLogger.Info($"加密表{item.Key}完成");
                }
                catch (Exception ex)
                {
                    LogHelper.CommLogger.Error($"对数据库{db}加密出现异常：{ex.Message}");
                    callback?.Invoke(progress, ex.Message);
                }
                Thread.Sleep(100);
            }

            #endregion

            LogHelper.CommLogger.Info($"加密数据库{db}完成");
            callback.Invoke(100, "加/解密完成");
            return true;
        }

        /// <summary>
        /// 对单个表进行加密
        /// </summary>
        /// <param name="table"></param>
        /// <param name="connStr"></param>
        private void Encrypt(KeyValuePair<string, List<string>> table)
        {
            List<string> list = table.Value;
            var offset = 0;
            var num = 5000;  //每次加密数据条数 num
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                string sqlEmcryptColumn = string.Empty;
                try
                {
                    sw.Restart();
                    var count = 0;
                    string sqlFindColumn = GetSqlFindColumn(table.Key, list, offset, num);  //获取待加密数据
                    DataTable dt = MySqlHelper.ExecuteDataset(connStr, sqlFindColumn).Tables[0];
                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        count = dt.Rows.Count;
                        LogHelper.CommLogger.Info($"查询到表{table.Key}待加密数据条数：{count}");
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row != null)
                            {
                                string encryptSql = GetSqlEncryptColumn(table.Key, list, row);
                                if (string.IsNullOrEmpty(encryptSql))
                                {
                                    continue;
                                }
                                LogHelper.CommLogger.Info(encryptSql);
                                sqlEmcryptColumn += encryptSql;
                                if (cmd == EnumCMD.EncryptToSQL || cmd == EnumCMD.DecryptToSQL)
                                {
                                    sqlEmcryptColumn += Environment.NewLine;
                                }
                            }
                        }
                    }
                    else
                    {
                        LogHelper.CommLogger.Info($"查询到表{table.Key}待加密数据条数：0");
                        break;
                    }
                    LogHelper.CommLogger.Info($"表{table.Key}加密数据条数：{count}，耗时：{sw.ElapsedMilliseconds}");
                    ExecuteSql(sqlEmcryptColumn);
                    offset += num;
                }
                catch (Exception ex)
                {
                    LogHelper.CommLogger.Error($"对表{table.Key}加密出现异常，重新执行：{ex}");
                    ExecuteSql(sqlEmcryptColumn);  //重新执行一次，处理测试出现执行sql超时情况
                    offset += num;
                }
            }
            sw.Stop();
        }

        /// <summary>
        /// 处理加密的sql：执行到数据库or生成脚本
        /// </summary>
        /// <param name="sqlEmcryptColumn"></param>
        private void ExecuteSql(string sqlEmcryptColumn)
        {
            if (string.IsNullOrWhiteSpace(sqlEmcryptColumn))
            {
                return;
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                string sql = "begin;" + sqlEmcryptColumn + "commit;";
                if (cmd == EnumCMD.EncryptToDatabase || cmd == EnumCMD.DecryptToDataBase)
                {
                    LogHelper.CommLogger.Info("开始执行到数据库……");
                    int result = MySqlHelper.ExecuteNonQuery(connStr, sql);
                    LogHelper.CommLogger.Info($"执行到数据库成功：{result}，耗时：{sw.ElapsedMilliseconds}");
                }
                else
                {
                    LogHelper.CommLogger.Info("开始写入到脚本……");
                    WriteSql(sql, cmd, path);
                    LogHelper.CommLogger.Info($"执行写入到脚本完成，耗时：{sw.ElapsedMilliseconds}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Error($"ExecuteSql出现错误：{ex}");
            }
            finally
            {
                sw.Stop();
            }
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="path"></param>
        /// <param name="cmd"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        bool ExecuteEncryptFile()
        {
            #region 加/解密单个文件
            if (cmd == EnumCMD.EncryptFile || cmd == EnumCMD.DecryptFile)
            {
                progress = 10;
                callback?.Invoke(progress, "开始查询待处理文件)");
                LogHelper.CommLogger.Info("开始查询待处理文件");
                string[] paths = new string[] { path };
                try
                {
                    ImgEncrypt(paths, cmd);
                }
                catch (Exception ex)
                {
                    LogHelper.CommLogger.Error($"加/解密单个文件出现异常：{ex.Message}");
                    callback?.Invoke(100, ex.Message);
                    return false;
                }
            }
            #endregion

            #region 加/解密文件夹
            else if (cmd == EnumCMD.EncryptFolder || cmd == EnumCMD.DecryptFolder)
            {
                StartBackgroundWork();
                progress = 10;
                callback?.Invoke(progress, "开始查询待处理文件");
                LogHelper.CommLogger.Info("开始查询待处理文件");
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);//.Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jepg")).ToArray();
                files = files.Where(s => !s.EndsWith(".db")).ToArray();
                callback?.Invoke(progress, $"查询到待处理文件数量：{files.Length}");
                LogHelper.CommLogger.Info($"查询到待处理文件数量：{files.Length}");
                if (files.Length % 100 > 0)
                {
                    groupCount = (files.Length / 100) + 1;
                }
                else
                {
                    groupCount = files.Length / 100;
                }
                callback?.Invoke(progress, $"开始加密，剩余待加密{groupCount}组图片");
                int each = 90 / groupCount == 0 ? 1 : 90 / groupCount;
                for (int i = 0; i < files.Length; i += 100)
                {
                    string[] paths = files.Skip(i).Take(100).ToArray();
                    EnumCMD cmd1 = cmd == EnumCMD.EncryptFolder ? EnumCMD.EncryptFile : EnumCMD.DecryptFile;
                    ThreadParam pa = new ThreadParam() { paths = paths, cmd = cmd1, each = each, callback = callback };

                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ImgEncrypt), pa);
                    TaskQueue.Enqueue(pa);
                    LogHelper.CommLogger.Info($"待处理图片入队，待处理组数：{TaskQueue.Count}");
                    Thread.Sleep(50);
                }
            }
            #endregion

            #region 一键加/解密人脸
            else if (cmd == EnumCMD.EncryptFileOneKey || cmd == EnumCMD.DecryptFileOneKey)
            {
                StartBackgroundWork();
                progress = 10;
                callback?.Invoke(progress, "开始查询待处理文件");
                LogHelper.CommLogger.Info("开始查询待处理文件");
                List<string> pathsStr = new List<string>();
                using (DataTable dt = MySqlHelper.ExecuteDataset(connStr, GetSqlFindHead()).Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["PhotoPath"] != null)
                        {
                            var ps = dr["PhotoPath"].ToString().Split('/');
                            if (ps.Length == 7)
                            {
                                var p = Path.Combine(path, ps[3], ps[4], ps[5], ps[6]);
                                pathsStr.Add(p);
                            }
                        }
                        if (dr["DataPath"] != null)
                        {
                            var ps = dr["DataPath"].ToString().Split('/');
                            if (ps.Length == 8)
                            {
                                var s1 = ps[4].Substring(0, 6);
                                var s2 = ps[4].Substring(6, 2);
                                var p = Path.Combine(path, ps[3], s1, s2, ps[5], ps[6], ps[7]);
                                pathsStr.Add(p);
                            }
                        }
                    }
                }
                callback?.Invoke(progress, $"查询到待处理文件数量：{pathsStr.Count}");
                LogHelper.CommLogger.Info($"查询到待处理文件数量：{pathsStr.Count}");
                string[] pathsArr = pathsStr.ToArray();
                if (pathsArr.Length % 100 > 0)
                {
                    groupCount = (pathsArr.Length / 100) + 1;
                }
                else
                {
                    groupCount = pathsArr.Length / 100;
                }
                callback?.Invoke(progress, $"开始加密，剩余待加密{groupCount}组图片");
                int each = 90 / groupCount == 0 ? 1 : 90 / groupCount;
                for (int i = 0; i < pathsArr.Length; i += 100)
                {
                    string[] paths = pathsArr.Skip(i).Take(100).ToArray();
                    EnumCMD cmd1 = cmd == EnumCMD.EncryptFolder ? EnumCMD.EncryptFile : EnumCMD.DecryptFile;
                    ThreadParam pa = new ThreadParam() { paths = paths, cmd = cmd1, each = each, callback = callback };

                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ImgEncrypt), pa);
                    //Task.Run(() => ImgEncrypt(paths, cmd == EnumCMD.EncryptFileOneKey ? EnumCMD.EncryptFile : EnumCMD.DecryptFile, each, callback));
                    //Thread.Sleep(200);
                    TaskQueue.Enqueue(pa);
                    LogHelper.CommLogger.Info($"待处理图片入队，待处理组数：{TaskQueue.Count}");
                    Thread.Sleep(50);
                }
            }
            #endregion

            while (groupCount > 0)
            {
                Thread.Sleep(200);
            }
            LogHelper.CommLogger.Info("所有图片加密完成");
            callback.Invoke(100, "加/解密完成");
            return true;
        }

        protected static List<Thread> ThreadList = new List<Thread>();
        protected ConcurrentQueue<object> TaskQueue = new ConcurrentQueue<object>();

        private void StartBackgroundWork()
        {
            var threadCount = 4;
            lock (ThreadList)
            {
                if (!ThreadList.Any())
                {
                    for (int i = 0; i < threadCount; i++)
                    {
                        Thread thread = new Thread(TaskWorker_DoWork);
                        thread.IsBackground = true;
                        thread.Name = "图片处理线程" + i;
                        ThreadList.Add(thread);
                        thread.Start();
                        LogHelper.CommLogger.Info("启动：" + thread.Name);
                    }
                }
            }
        }

        private void TaskWorker_DoWork()
        {
            while (true)
            {
                try
                {
                    object param = null;
                    var hasTask = false;
                    lock (TaskQueue)
                    {
                        if (TaskQueue.Count>0)
                        {
                            if (TaskQueue.TryDequeue(out param))
                            {
                                hasTask = true;
                                LogHelper.CommLogger.Info($"待处理图片出队，待处理组数：{TaskQueue.Count}");
                            }
                        }
                    }
                    if (hasTask && param != null)
                    {
                        ImgEncrypt(param);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.CommLogger.Error($"图片加密出现异常：{ex}");
                }
            }
        }

        /// <summary>
        /// 图片加密
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="cmd"></param>
        /// <param name="each"></param>
        /// <param name="callback"></param>
        public void ImgEncrypt(object o)
        {
            ThreadParam pa = o as ThreadParam;
            var paths = pa.paths;
            var cmd = pa.cmd;
            var callback = pa.callback;
            var each = pa.each;
            foreach (var path in paths)
            {
                byte[] fileContents = new byte[0];
                var result = new byte[0];
                try
                {
                    LogHelper.CommLogger.Info($"开始加密图片：{path}");
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        fileContents = new byte[fs.Length];
                        fs.Read(fileContents, 0, fileContents.Length);
                    }
                    if (cmd == EnumCMD.EncryptFile || cmd == EnumCMD.EncryptFileOneKey)
                    {
                        result = JieShunSM4EncryptHelper.SM4EncryptBinary(fileContents);
                    }
                    else if (cmd == EnumCMD.DecryptFile || cmd == EnumCMD.DecryptFileOneKey)
                    {
                        result = JieShunSM4EncryptHelper.SM4DecryptBinary(fileContents);
                    }
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                    {
                        fs.Write(result, 0, result.Length);
                    }
                    LogHelper.CommLogger.Info($"加密图片完成：{path}");
                }
                catch (Exception ex)
                {
                    LogHelper.CommLogger.Error($"图片加密出现异常：{ex}");
                    callback?.Invoke(progress, ex.Message);
                }
            }
            lock (locker)
            {
                groupCount--;
                progress = Math.Min(progress + each, 100);
                callback?.Invoke(progress, $"正在加密，剩余待加密{groupCount}组图片");
            }
        }

        /// <summary>
        /// 图片加密
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="cmd"></param>
        /// <param name="each"></param>
        /// <param name="callback"></param>
        public void ImgEncrypt(string[] paths, EnumCMD cmd, int each = 90, Action<int, string> callback = null)
        {
            foreach (var path in paths)
            {
                byte[] fileContents = new byte[0];
                var result = new byte[0];
                try
                {
                    LogHelper.CommLogger.Info($"开始加密图片：{path}");
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        fileContents = new byte[fs.Length];
                        fs.Read(fileContents, 0, fileContents.Length);
                    }
                    if (cmd == EnumCMD.EncryptFile || cmd == EnumCMD.EncryptFileOneKey)
                    {
                        result = JieShunSM4EncryptHelper.SM4EncryptBinary(fileContents);
                    }
                    else if (cmd == EnumCMD.DecryptFile || cmd == EnumCMD.DecryptFileOneKey)
                    {
                        result = JieShunSM4EncryptHelper.SM4DecryptBinary(fileContents);
                    }
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                    {
                        fs.Write(result, 0, result.Length);
                    }
                    LogHelper.CommLogger.Info($"加密图片完成：{path}");
                }
                catch (Exception ex)
                {
                    LogHelper.CommLogger.Error($"图片加密出现异常：{ex}");
                    callback?.Invoke(progress, ex.Message);
                }
            }
            lock (locker)
            {
                groupCount--;
                progress = Math.Min(progress + each, 100);
            }
            callback?.Invoke(progress, $"已加密{progress}组图片,剩余待加密{groupCount}组图片");
        }

        /// <summary>
        /// 获取待加密字段值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cloumns"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private string GetSqlFindColumn(string table, List<string> cloumns, int offset, int num)
        {
            //select * from table as a inner join (select id from table order by id limit m, n) as b on a.id = b.id order by a.id; //limit性能较差，可以尝试遍历id
            var sql = "select ID, ";
            foreach (var item in cloumns)
            {
                sql += $" {item},";
            }
            sql = sql.TrimEnd(',');
            sql += $" from {table} limit {offset},{num}";
            return sql;
        }

        /// <summary>
        /// 获取待加密表
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private string GetSqlFindTables(string dbName)
        {
            return $"SELECT TABLE_NAME, COLUMN_NAME FROM information_schema.`COLUMNS` WHERE TABLE_SCHEMA = '{dbName}' AND (" +
                                $"COLUMN_NAME LIKE '%mobile%' OR " +
                                "COLUMN_NAME LIKE '%phone%' OR " +
                                "COLUMN_NAME LIKE '%idnumber%' OR " +
                                "COLUMN_NAME LIKE '%tel%' );";
        }

        /// <summary>
        /// 获取加密sql
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cloumns"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private string GetSqlEncryptColumn(string table, List<string> cloumns, DataRow dr)
        {
            var sql = $"update {table} set";
            try
            {
                int cloumnsIsNull = 0;  //计数无需加密字段
                for (int i = 0; i < cloumns.Count; i++)
                {
                    if (dr[cloumns[i]] == null || string.IsNullOrEmpty(dr[cloumns[i]].ToString()))
                    {
                        cloumnsIsNull++;
                        continue;
                    }
                    sql += $" {cloumns[i]} = '";
                    if (cmd == EnumCMD.EncryptToSQL || cmd == EnumCMD.EncryptToDatabase)
                    {
                        sql += $"{ JieShunSM4EncryptHelper.SM4Encrypt(dr[cloumns[i]].ToString())}' ,";
                    }
                    else if (cmd == EnumCMD.DecryptToSQL || cmd == EnumCMD.DecryptToDataBase)
                    {
                        sql += $"{ JieShunSM4EncryptHelper.SM4Decrypt(dr[cloumns[i]].ToString())}' ,";
                    }
                }
                sql = sql.TrimEnd(',');
                sql += $"where id = '{dr["ID"].ToString()}';";
                if (cloumnsIsNull == cloumns.Count) sql = string.Empty; //校验待加密字段 == 无需加密字段
            }
            catch (Exception ex)
            {
                throw;
            }
            return sql;
        }

        /// <summary>
        /// 获取待加密人脸文件
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cloumns"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private string GetSqlFindHead()
        {
            return "select PhotoPath, DataPath from vsp_auth_certificate; ";
        }

        /// <summary>
        /// 获取加解密脚本生成路径
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetScriptPath(string db)
        {
            if (cmd == EnumCMD.EncryptToSQL || cmd == EnumCMD.DecryptToSQL)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script", $"{db}.sql");
                //var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //if (!Directory.Exists(dir))
                //{
                //    Directory.CreateDirectory(dir);
                //}
                return path;
            }
            return ""; 
        }


        /// <summary>
        /// 生成脚本文件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cmd"></param>
        /// <param name="path"></param>
        public void WriteSql(string msg, EnumCMD cmd, string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                LogHelper.CommLogger.Info($"生成脚本文件出错：{ex.Message}");
            }

        }

    }

    public class ThreadParam
    {
        public string[] paths;
        public EnumCMD cmd;
        public int each = 90;
        public Action<int, string> callback = null;
    }

}
