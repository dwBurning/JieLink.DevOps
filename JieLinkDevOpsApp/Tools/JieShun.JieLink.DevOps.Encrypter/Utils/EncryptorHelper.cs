using JieShun.JieLink.DevOps.Encrypter.Models;
using JieShun.Udf.Core;
using MySql.Data.MySqlClient;
using PartialViewEncrypter.Models;
using PartialViewInterface.Utils;
using System;
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
        public async Task StartAsync(Action<int, string> callback, string[] dbs, Dictionary<string, string> connStrs, Dictionary<string, string> sqlFindColumns, EnumCMD cmd)
        {
            await Task.Factory.StartNew(() =>
            {
                Start(callback, dbs, connStrs, sqlFindColumns, cmd);
            });
        }


        public void Start(Action<int, string> callback, string[] dbs, Dictionary<string, string> connStrs, Dictionary<string, string> sqlFindColumns, EnumCMD cmd)
        {
            bool ok = false;
            string message = "升级成功";
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ok = ExecuteEncryptDatabase(callback, dbs, connStrs, sqlFindColumns, cmd);
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
                throw ex;
            }
            
        }


        public async Task StartAsync(Action<int, string> callback, string path, EnumCMD cmd, string connStr)
        {
            await Task.Factory.StartNew(() =>
            {
                Start(callback, path, cmd, connStr);
            });
        }

        public void Start(Action<int, string> callback, string path, EnumCMD cmd,string connStr)
        {
            bool ok = false;
            string message = "升级成功";
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ok = ExecuteEncryptFile(callback, path, cmd, connStr);
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
        bool ExecuteEncryptDatabase(Action<int, string> callback, string[] dbs, Dictionary<string, string> connStrs, Dictionary<string, string> sqlFindColumns, EnumCMD cmd)
        {
            int progress = 10;
            callback?.Invoke(progress, "开始查询待加密表和字段");
            LogHelper.CommLogger.Info("progress:{0},开始查询待加密表和字段", progress.ToString());
            foreach (var db in dbs)
            {
                callback?.Invoke(progress, $"开始加密数据库{db}");
                LogHelper.CommLogger.Info($"开始加密数据库{db}");
                var sqlFindTable = sqlFindColumns[db];
                LogHelper.CommLogger.Info("获取待加密表和字段：{0}", sqlFindTable);
                List<TableInfo> tables = new List<TableInfo>();
                using (DataTable dt = MySqlHelper.ExecuteDataset(connStrs[db], sqlFindTable).Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        TableInfo info = new TableInfo { table = dr["TABLE_NAME"].ToString(), column = dr["COLUMN_NAME"].ToString() };
                        tables.Add(info);
                        LogHelper.CommLogger.Info($"查询到待加密表：{info.table}，字段：{info.column}");
                    }
                }
                //把需要加密的字段按表名称分组，生成SQL可以一次性修改同一个表内的多个字段
                Dictionary<string, List<TableInfo>> pairs = new Dictionary<string, List<TableInfo>>();
                foreach (var item in tables)
                {
                    if (!pairs.ContainsKey(item.table))
                    {
                        var list = new List<TableInfo>();
                        list.Add(item);
                        pairs.Add(item.table, list);
                    }
                    else
                    {
                        pairs[item.table].Add(item);
                    }
                }
                callback?.Invoke(progress, "查询待加密表和字段完成");
                var path = GetScriptPath(db);
                int total = pairs.Count;
                int each = 90 / dbs.Length / total;
                foreach (var item in pairs)
                {
                    try
                    {
                        LogHelper.CommLogger.Info($"正在加密表{item.Key}");
                        callback?.Invoke(Math.Min(progress += each, 90), $"正在加密表{item.Key}");
                        Encrypt(item, connStrs[db], cmd, path);
                        LogHelper.CommLogger.Info($"加密表{item.Key}完成");
                    }
                    catch (Exception ex)
                    {
                        LogHelper.CommLogger.Error($"对数据库{db}加密出现异常：{ex.Message}");
                        callback?.Invoke(progress, ex.Message);
                    }
                    Thread.Sleep(200);
                }
                LogHelper.CommLogger.Info($"加密数据库{db}完成");
            }
            callback.Invoke(100, "加/解密完成");
            LogHelper.CommLogger.Info("所有数据库加密完成");
            return true;
        }

        /// <summary>
        /// 对单个表进行加密
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="connStr"></param>
        private void Encrypt(KeyValuePair<string, List<TableInfo>> pair, string connStr, EnumCMD cmd, string path)
        {
            List<string> list = new List<string>();
            foreach (var item in pair.Value)
            {
                list.Add(item.column);
            }
            var offset = 0;
            string[] columns = list.ToArray(); //待加密字段
            while (true)
            {
                try
                {
                    string sqlFindColumn = GetSqlFindColumn(pair.Key, columns, offset);
                    string sqlEmcryptColumn = string.Empty;
                    DataTable dt = MySqlHelper.ExecuteDataset(connStr, sqlFindColumn).Tables[0];
                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        LogHelper.CommLogger.Info($"查询到表{pair.Key}待加密数据条数：{dt.Rows.Count}");
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row != null)
                            {
                                string encryptSql = GetSqlEncryptColumn(pair.Key, columns, row, cmd);
                                LogHelper.CommLogger.Info(encryptSql);
                                if (cmd == EnumCMD.EncryptToDatabase || cmd == EnumCMD.DecryptToDataBase) //直接执行加密到数据库
                                {
                                    sqlEmcryptColumn += encryptSql;
                                }
                                WriteSql(encryptSql, cmd, path);
                            }
                        }
                    }
                    else
                    {
                        LogHelper.CommLogger.Info($"查询到表{pair.Key}待加密数据条数：0");
                        break;
                    }
                    if (cmd == EnumCMD.EncryptToDatabase || cmd == EnumCMD.DecryptToDataBase)
                    {
                        LogHelper.CommLogger.Info("开始执行到数据库……");
                        int result = MySqlHelper.ExecuteNonQuery(connStr, sqlEmcryptColumn);
                        LogHelper.CommLogger.Info($"执行到数据库成功：{result}");
                    }
                    offset += 100;
                }
                catch (Exception ex)
                {
                    LogHelper.CommLogger.Error($"对表{pair.Key}加密出现异常：{ex}");
                    throw ex;
                }

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
        bool ExecuteEncryptFile(Action<int, string> callback, string path, EnumCMD cmd, string connStr)
        {
            //加/解密单个文件
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
            //加/解密文件夹
            else if (cmd == EnumCMD.EncryptFolder || cmd == EnumCMD.DecryptFolder)
            {
                progress = 10;
                callback?.Invoke(progress, "开始查询待处理文件)");
                LogHelper.CommLogger.Info("开始查询待处理文件");
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jepg")).ToArray();
                //var files = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
                LogHelper.CommLogger.Info($"查询到待处理文件数量：{files.Length}");
                if (files.Length % 100 > 0)
                {
                    groupCount = (files.Length / 100) + 1;
                }
                else
                {
                    groupCount = files.Length / 100;
                }
                int each = 90 / groupCount == 0 ? 1 : 90 / groupCount;
                ThreadPool.SetMaxThreads(3,3);
                for (int i = 0; i < files.Length; i += 100)
                {
                    string[] paths = files.Skip(i).Take(100).ToArray();
                    EnumCMD cmd1 = cmd == EnumCMD.EncryptFolder ? EnumCMD.EncryptFile : EnumCMD.DecryptFile;
                    ThreadParam pa = new ThreadParam() { paths = paths, cmd = cmd1, each = each, callback = callback };

                    ThreadPool.QueueUserWorkItem(new WaitCallback(ImgEncrypt), pa);
                    //Task.Run(() => ImgEncrypt(paths, cmd == EnumCMD.EncryptFolder ? EnumCMD.EncryptFile : EnumCMD.DecryptFile, each, callback));
                    Thread.Sleep(200);
                }
            }
            //一键加/解密人脸
            else if (cmd == EnumCMD.EncryptFileOneKey || cmd == EnumCMD.DecryptFileOneKey)
            {
                progress = 10;
                callback?.Invoke(progress, "开始查询待处理文件)");
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
                int each = 90 / groupCount == 0 ? 1 : 90 / groupCount;
                ThreadPool.SetMaxThreads(3, 3);
                for (int i = 0; i < pathsArr.Length; i += 100)
                {
                    string[] paths = pathsArr.Skip(i).Take(100).ToArray();
                    EnumCMD cmd1 = cmd == EnumCMD.EncryptFolder ? EnumCMD.EncryptFile : EnumCMD.DecryptFile;
                    ThreadParam pa = new ThreadParam() { paths = paths, cmd = cmd1, each = each, callback = callback };

                    ThreadPool.QueueUserWorkItem(new WaitCallback(ImgEncrypt), pa);
                    //Task.Run(() => ImgEncrypt(paths, cmd == EnumCMD.EncryptFileOneKey ? EnumCMD.EncryptFile : EnumCMD.DecryptFile, each, callback));
                    Thread.Sleep(200);
                }
            }
            while (groupCount > 0)
            {
                Thread.Sleep(200);
            }
            LogHelper.CommLogger.Info("所有图片加密完成");
            callback.Invoke(100, "加/解密完成");
            return true;
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
                        result = JieShun.Udf.Core.UdfEncrypt.SM4EncryptBinary(fileContents);
                    }
                    else if (cmd == EnumCMD.DecryptFile || cmd == EnumCMD.DecryptFileOneKey)
                    {
                        result = JieShun.Udf.Core.UdfEncrypt.SM4DecryptBinary(fileContents);
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
                        result = JieShun.Udf.Core.UdfEncrypt.SM4EncryptBinary(fileContents);
                    }
                    else if (cmd == EnumCMD.DecryptFile || cmd == EnumCMD.DecryptFileOneKey)
                    {
                        result = JieShun.Udf.Core.UdfEncrypt.SM4DecryptBinary(fileContents);
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
        private string GetSqlFindColumn(string table,string[] cloumns,int offset)
        {
            //select * from table as a inner join (select id from table order by id limit m, n) as b on a.id = b.id order by a.id; //limit性能较差，可以尝试遍历id
            var sql = "select ID, ";
            foreach (var item in cloumns)
            {
                sql += $" {item},";
            }
            sql = sql.TrimEnd(',');
            sql += $" from {table} limit {offset},100"; 
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
        private string GetSqlEncryptColumn(string table, string[] cloumns, DataRow  dr,EnumCMD cmd)
        {
            var sql = $"update {table} set";
            try
            {
                int cloumnsIsNull = 0;
                for (int i = 0; i < cloumns.Length; i++)
                {
                    if (dr[cloumns[i]] == null)
                    {
                        cloumnsIsNull++;
                        continue;
                    }
                    sql += (i == 0) ? $" {cloumns[i]} = '" : $" , {cloumns[i]} = '";
                    if (cmd == EnumCMD.EncryptToSQL || cmd == EnumCMD.EncryptToDatabase)
                    {
                        sql += $"{ UdfEncrypt.SM4Encrypt(dr[cloumns[i]].ToString())}' ";
                    }
                    else if (cmd == EnumCMD.DecryptToSQL || cmd == EnumCMD.DecryptToDataBase)
                    {
                        sql += $"{ UdfEncrypt.SM4Decrypt(dr[cloumns[i]].ToString())}' ";
                    }
                }
                sql += $"where id = '{dr["ID"].ToString()}';";
                if (cloumnsIsNull == cloumns.Length) sql = string.Empty; //校验待加密字段是否全部位null
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
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script", $"{db}.sql");
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return path;
        }

        
        /// <summary>
        /// 生成脚本文件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cmd"></param>
        /// <param name="path"></param>
        public void WriteSql(string msg,EnumCMD cmd,string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Append,FileAccess.Write);
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
