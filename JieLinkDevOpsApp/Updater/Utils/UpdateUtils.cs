using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JieShun.JieLink.DevOps.Updater.Models;
using Newtonsoft.Json;
namespace JieShun.JieLink.DevOps.Updater.Utils
{
    public class UpdateUtils
    {
        
        public static UpdateRequest ParseUpdateRequest(string[] args)
        {
            UpdateRequest updateRequest = null;
            if (args.Contains("-file="))
            {
                string requestFile = (args.FirstOrDefault(x => x.StartsWith("-file=")) ?? Guid.NewGuid().ToString()).Replace("-file=", "");
                return updateRequest=TryParsePackageInfo<UpdateRequest>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, requestFile));
            }
            string product = (args.FirstOrDefault(x => x.StartsWith("-product=")) ?? Guid.NewGuid().ToString()).Replace("-product=", "");
            string guid = (args.FirstOrDefault(x => x.StartsWith("-guid=")) ?? Guid.NewGuid().ToString()).Replace("-guid=", "");
            string rootPath = (args.FirstOrDefault(x => x.StartsWith("-root=")) ?? string.Empty).Replace("-root=", "");
            string packagePath = (args.FirstOrDefault(x => x.StartsWith("-package=")) ?? string.Empty).Replace("-package=", "");
            if (!string.IsNullOrEmpty(rootPath) && !string.IsNullOrEmpty(packagePath))
            {
                updateRequest = new UpdateRequest();
                updateRequest.Product = product;
                updateRequest.Guid = guid;
                updateRequest.RootPath = rootPath;
                updateRequest.PackagePath = packagePath;
                return updateRequest;
            }
            updateRequest = TryParsePackageInfo<UpdateRequest>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateRequest.json"));
            return updateRequest;
        }

        public static void ExtractZip(string zipFile,string dstDir)
        {
            Console.WriteLine("解压文件:{0}", zipFile);
            ZipHelper.UnzipFile(zipFile, dstDir);
        }
        public static T TryParsePackageInfo<T>(string filePath)
        {
            try
            {
                return ParsePackageInfo<T>(filePath);
            }
            catch(Exception ex)
            {
                Console.WriteLine("TryParsePackageInfo:" + ex.Message);
                return default(T);
            }
        }
        public static T ParsePackageInfo<T>(string filePath)
        {
            Console.WriteLine("解析升级包json文件:{0}", filePath);
            if (!File.Exists(filePath))
            {
                throw new Exception("文件不存在："+ filePath);
            }
            
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs);
                string json = sr.ReadToEnd();
                sr.Close();
                var ret= JsonConvert.DeserializeObject<T>(json);
                if(ret==null)
                {
                    throw new Exception("文件解析异常：" + filePath);
                }
                return ret;
            }
        }
        public static void KillProcess(List<ProgramInfo> processList,int waitSeconds)
        {
            foreach(var info in processList)
            {
                Console.WriteLine("结束进程:{0}", info.ProcessName);
                ProcessHelper.StopProcess(info.ProcessName);
            }
            Thread.Sleep(waitSeconds);
            foreach (var info in processList)
            {
                if (string.IsNullOrEmpty(info.ServiceName))
                {
                    //服务名为空就代表不是服务，就按进程的方式运行
                    if (ProcessHelper.IsProcessRunning(info.ProcessName))
                    {
                        throw new Exception(info.ProcessName + "关闭失败！");
                    }

                }
                else
                {
                    if (ProcessHelper.IsServiceRunning(info.ServiceName))
                    {
                        throw new Exception(info.ServiceName + "关闭失败！");
                    }
                }

            }
        }
        public static bool TryReplaceFile(string sourcePath, string dstPath, List<string> ignores)
        {
            try
            {
                ReplaceFile(sourcePath, dstPath, ignores);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("TryReplaceFile:" + ex.Message);
                return false;
            }
        }
        public static void ReplaceFile(string sourcePath,string dstPath,List<string> ignores)
        {
            //ignores为完整路径
            if (!Directory.Exists(dstPath))
                Directory.CreateDirectory(dstPath);
            var sourceFilePaths = Directory.GetFiles(sourcePath);
            foreach(var sourceFilePath in sourceFilePaths)
            {
                FileInfo fi = new FileInfo(sourceFilePath); 
                if (ignores.Any(x => IsFileMatch(fi.FullName,fi.Extension,x)))
                    continue;
                string dstFilePath = Path.Combine(dstPath,fi.Name);
                Console.WriteLine("复制文件:{0}", dstFilePath);
                File.Copy(sourceFilePath, dstFilePath,true);
            }
            //递归复制子文件夹
            var subDirs=Directory.GetDirectories(sourcePath);
            foreach(var subDir in subDirs)
            {
                if (ignores.Contains(subDir))
                    continue;
                ReplaceFile(subDir, Path.Combine(dstPath, new DirectoryInfo(subDir).Name), ignores);
            }
        }
        public static bool TryStartProcess(string root, List<ProgramInfo> processList)
        {
            try
            {
                StartProcess(root, processList);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("TryStartProcess:" + ex.Message);
                return false;
            }
        }
        public static void StartProcess(string root, List<ProgramInfo> processList)
        {
            foreach (var info in processList)
            {
                if(string.IsNullOrEmpty(info.ServiceName))
                {
                    //服务名为空就代表不是服务，就按进程的方式运行
                    if(!ProcessHelper.IsProcessRunning(info.ProcessName))
                    {
                        string filePath = info.ExecutablePath;
                        if (File.Exists(info.ExecutablePath))
                        {
                            filePath = info.ExecutablePath;
                        }
                        else
                        {
                            filePath = Path.Combine(root, info.ExecutablePath);
                        }
                        Console.WriteLine("启动进程:{0}", info.ProcessName);
                        ProcessHelper.StartProcess(filePath, info.Args);
                    }
                    
                }
                else
                {
                    if (!ProcessHelper.IsServiceRunning(info.ServiceName))
                    {
                        Console.WriteLine("启动服务:{0}", info.ServiceName);
                        ProcessHelper.StartService(info.ServiceName);
                    }
                }
                
            }
            //检查进程是否启动
            Thread.Sleep(1000);
            foreach (var info in processList)
            {
                if (string.IsNullOrEmpty(info.ServiceName))
                {
                    //服务名为空就代表不是服务，就按进程的方式运行
                    if (!ProcessHelper.IsProcessRunning(info.ProcessName))
                    {
                        throw new Exception(info.ProcessName + "启动失败！");
                    }

                }
                else
                {
                    if (!ProcessHelper.IsServiceRunning(info.ServiceName))
                    {
                        throw new Exception(info.ServiceName + "启动失败！");
                    }
                }

            }
        }
        private static bool IsFileMatch(string currentPath,string extension, string match)
        {
            if(string.Equals(currentPath,match,StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if(match.Contains("*."))
            {
                if(string.Equals(match.Substring(match.IndexOf("*.")+1), extension,StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            
            return false;
        }
        public static void TryDeleteDir(string dir)
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch
            {

            }
            
        }
    }
}
