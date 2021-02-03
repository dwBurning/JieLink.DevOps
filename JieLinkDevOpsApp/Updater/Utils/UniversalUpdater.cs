using JieShun.JieLink.DevOps.Updater.Models;
using JieShun.JieLink.DevOps.Updater.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JieShun.JieLink.DevOps.Updater.Utils
{
    public class UniversalUpdater
    {
        #region 字段
        /// <summary>
        /// guid
        /// </summary>
        string guid;
        /// <summary>
        /// 升级包信息
        /// </summary>
        PackageInfo packageInfo;
        string product;
        /// <summary>
        /// 根路径
        /// </summary>
        string rootDir;
        /// <summary>
        /// 升级包解压后的根目录
        /// </summary>
        string packageDir;
        /// <summary>
        /// 升级包所在的路径
        /// </summary>
        string packageTempDir;
        /// <summary>
        /// 升级包完整的路径
        /// </summary>
        string packageTempFullPath;
        #endregion
        public UniversalUpdater(UpdateRequest updateRequest)
        {
            guid = updateRequest.Guid;
            product = updateRequest.Product;
            rootDir = updateRequest.RootPath;
            packageTempFullPath = updateRequest.PackagePath;
            FileInfo fi = new FileInfo(updateRequest.PackagePath);
            packageTempDir = fi.DirectoryName;
            packageDir = Path.Combine(fi.DirectoryName, fi.Name.Replace(fi.Extension, ""));

        }
        public async Task StartAsync(Action<int, string> callback)
        {
            await Task.Factory.StartNew(() =>
            {
                Start(callback);
            });
        }
        public void Start(Action<int, string> callback)
        {
            bool ok = false;
            string message = "升级成功";
            try
            {
                ok = ExecuteUpdate(callback);
            }
            catch (Exception ex)
            {
                ok = false;
                message = ex.Message;
                Console.WriteLine(ex.Message);
                callback?.Invoke(100, ex.Message);
                throw ex;
            }
            finally
            {
                //写升级结果
                UpdateResult result = new UpdateResult();
                result.Code = ok ? 0 : 1;
                result.Message = message;
                WriteUpdateResult(result);
            }


        }
        bool ExecuteUpdate(Action<int, string> callback)
        {
            int progress = 0;
            //解压
            progress = 10;
            callback?.Invoke(progress, "正在解压文件");
            UpdateUtils.ExtractZip(packageTempFullPath, packageTempDir);

            //解析包的json文件（必须按格式来写）
            progress = 20;
            callback?.Invoke(progress, "解析升级包");
            packageInfo = UpdateUtils.TryParsePackageInfo<PackageInfo>(Path.Combine(packageDir, "package.json"));
            if (packageInfo == null && !string.IsNullOrEmpty(product))
                packageInfo = UpdateUtils.ParsePackageInfo<PackageInfo>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs\\" + product + ".json"));

            try
            {
                //结束目标进程
                progress = 30;
                callback?.Invoke(progress, "结束目标进程");
                UpdateUtils.KillProcess(packageInfo.KillProcessList, 1000);

                //替换文件
                foreach (var subPackage in packageInfo.SubPackages)
                {
                    if (subPackage.ZipList != null)
                    {
                        foreach (var zipFile in subPackage.ZipList)
                        {
                            string zipFileFullPath = Path.Combine(packageDir, zipFile);
                            string zipFileDir = new FileInfo(zipFileFullPath).DirectoryName;
                            UpdateUtils.ExtractZip(zipFileFullPath, zipFileDir);
                        }
                    }
                    progress = Math.Min(progress + 10, 80);
                    callback?.Invoke(progress, "替换" + subPackage.TargetPath);

                    string sourceDir = Path.Combine(packageDir, subPackage.SubPath.Trim(new char[] { '/', '\\' }));
                    string targetDir = Path.Combine(rootDir, subPackage.TargetPath.Trim(new char[] { '/', '\\' }));
                    List<string> excludeList = subPackage.ExcludeList.Select(x => Path.Combine(sourceDir, x)).ToList();
                    UpdateUtils.ReplaceFile(sourceDir, targetDir, excludeList);
                }
                //更新配置文件
                foreach (var programInfo in packageInfo.RunProcessList)
                {
                    if (!string.IsNullOrEmpty(programInfo.ExecutablePath) && programInfo.ConfigToUpdate != null)
                    {
                        string filePath = programInfo.ExecutablePath;
                        if (File.Exists(programInfo.ExecutablePath))
                        {
                            filePath = programInfo.ExecutablePath;
                        }
                        else
                        {
                            filePath = Path.Combine(rootDir, programInfo.ExecutablePath);
                        }
                        foreach (var kvp in programInfo.ConfigToUpdate)
                        {
                            UpdateUtils.WriterAppConfig(filePath, kvp.Key, kvp.Value);
                        }
                    }

                }
                //启动进程
                progress = 90;
                callback?.Invoke(progress, "启动进程");
                UpdateUtils.StartProcess(rootDir, packageInfo.RunProcessList);

                //删除解压的文件
                progress = 95;
                callback?.Invoke(progress, "删除解压的文件");
                UpdateUtils.TryDeleteDir(packageDir);

                //完成
                progress = 100;
                callback?.Invoke(progress, "升级完成！");
            }
            catch (Exception ex)
            {
                UpdateUtils.StartProcess(rootDir, packageInfo.RunProcessList);
                throw ex;
            }
            return true;
        }
        void WriteUpdateResult(UpdateResult result)
        {
            string fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, guid + ".json");
            if (!File.Exists(fullName))
            {
                File.Create(fullName).Close();
            }
            var json = JsonConvert.SerializeObject(result);
            Console.WriteLine("写升级结果:{0}", json);
            using (FileStream fs = new FileStream(fullName, FileMode.Truncate, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
        }
    }
}
