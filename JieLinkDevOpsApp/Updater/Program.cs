using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static PInvoke.Kernel32;
using static PInvoke.AdvApi32;
using static PInvoke.Userenv;
using Newtonsoft.Json;
using System.Reflection;
using System.Diagnostics;
using JieShun.JieLink.DevOps.Updater.Models;
using JieShun.JieLink.DevOps.Updater.Utils;
using System.Security.Principal;
using System.Windows;

namespace JieShun.JieLink.DevOps.Updater
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //判断有无管理员权限
            if (!CheckIsAdministrator(args))
            {
                Console.WriteLine("程序不是以管理员权限运行，程序退出...");
                return;
            }
            AttachConsole(-1);

            Console.WriteLine("命令行参数:{0}", String.Join(" ", args));

            //CreateUpdateRequestForTest();
            //CreatePackageInfoForTest();

            UpdateRequest request = UpdateUtils.ParseUpdateRequest(args);
            if (request != null)
            {
                if (CheckNeedUpdate(args, request))
                {
                    if (args.Contains("-c"))
                    {
                        RunAsConsole(args, request);
                    }
                    else
                    {
                        RunAsWpfApplication(args, request);
                    }

                }
            }
            else
            {
                if (args.Contains("-c"))
                {
                    Console.WriteLine("缺少升级所需的参数");
                    Console.WriteLine("程序退出...");
                    //Console.WriteLine("按任意键继续...");
                    //Console.Read();
                }
                else
                {
                    RunAsWpfApplication(args, request);
                }
            }
        }
        static void RunAsWpfApplication(string[] args, UpdateRequest request)
        {
            App app = new App();
            app.InitializeComponent();
            MainWindow windows = new MainWindow();
            MainWindow.UpdateRequest = request;
            app.MainWindow = windows;
            app.Run();
        }
        static void RunAsConsole(string[] args, UpdateRequest request)
        {
            UniversalUpdater updater = new UniversalUpdater(request);
            try
            {
                updater.Start(UpdateProgress);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        static bool CheckIsAdministrator(string[] args)
        {
            //返回true不是有权限，只是界面上提示他无权限
            if (Debugger.IsAttached)
            {
                return true;
            }
            else if (args.Contains("-runas"))//加上这个判断，防止无限循环启动
            {
                return true;
            }

            //获得当前登录的Windows用户标示
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            //判断当前登录用户是否为管理员
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                return true;
            }
            else
            {
                //MessageBox.Show("当前进程无管理员权限!");
                var newArgs = args.ToList();
                newArgs.Add("-runas");
                //创建启动对象
                ProcessStartInfo startInfo = new ProcessStartInfo();
                //设置运行文件
                startInfo.FileName = Assembly.GetCallingAssembly().Location;
                //设置启动参数
                startInfo.Arguments = String.Join(" ", newArgs);
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                //如果不是管理员，则启动UAC
                Process.Start(startInfo);
                return false;

            }
        }
        static bool CheckNeedUpdate(string[] args, UpdateRequest request)
        {
            if (Debugger.IsAttached)
            {
                return true;
            }
            if (args.Contains("-runnow=1"))
            {
                return true;
            }
            string programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Updater\\V1.0.0");
            if (!AppDomain.CurrentDomain.BaseDirectory.StartsWith(programDataPath))
            {
                //不在program data下执行，就不执行升级操作
                Console.WriteLine("正在复制文件到：" + programDataPath);
                UpdateUtils.TryReplaceFile(AppDomain.CurrentDomain.BaseDirectory, programDataPath, new List<string> {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Temp"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Logs")
                    });
                FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
                string executePath = Path.Combine(programDataPath, fi.Name);
                //重新组织命令行参数
                var newArgs = args.ToList();
                newArgs.RemoveAll(x => x.StartsWith("-r") || x.StartsWith("-p"));
                newArgs.Add("-r=\"" + request.RootPath + "\"");
                newArgs.Add("-p=\"" + request.PackagePath + "\"");
                ProcessHelper.StartProcess(executePath, String.Join(" ", newArgs));
                return false;
            }
            return true;
        }
        static void UpdateProgress(int progress, string message)
        {
            Console.Write("{0}({1}%)", message, progress);
        }
        static void CreateUpdateRequestForTest()
        {
            //测试
            UpdateRequest updateRequest = new UpdateRequest();
            updateRequest.Guid = Guid.NewGuid().ToString();
            updateRequest.Product = "JSOCT2016";
            updateRequest.RootPath = @"D:\Program Files (x86)\Jielink";
            updateRequest.PackagePath = @"D:\迅雷下载\JSOCT2016 V2.6.2 Jielink+智能终端操作平台安装包\obj\JSOCT2016-V2.6.2.zip";

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateRequest.json");
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            var json = JsonConvert.SerializeObject(updateRequest, Formatting.Indented);
            using (FileStream fs = new FileStream(path, FileMode.Truncate, System.IO.FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
        }

        static void CreatePackageInfoForTest()
        {
            //测试
            ProgramInfo iis = new ProgramInfo();
            iis.ServiceName = "W3SVC";
            ProgramInfo smartCenter = new ProgramInfo();
            smartCenter.ExecutablePath = "SmartCenter\\SmartCenter.Host.exe";
            smartCenter.ProcessName = "SmartCenter.Host.exe";
            smartCenter.ServiceName = "JsstJieLinkSmartCenter";
            ProgramInfo smartCenterDaemon = new ProgramInfo();
            smartCenterDaemon.ExecutablePath = "守护进程\\JSST.JieLink.Daemon.exe";
            smartCenterDaemon.ProcessName = "JSST.JieLink.Daemon.exe";
            smartCenterDaemon.ServiceName = "JSST.JieLink.Daemon";

            SubPackage smartCenterPackage = new SubPackage();
            smartCenterPackage.SubPath = "programfiles\\SmartCenter";
            smartCenterPackage.TargetPath = "SmartCenter";
            smartCenterPackage.ExcludeList = new List<string>() {
                "Config",
                "update",
                "Agent.dat",
                "Logs",
                "*.config",
            };
            SubPackage smartWebPackage = new SubPackage();
            smartWebPackage.SubPath = "programfiles\\SmartWeb";
            smartWebPackage.TargetPath = "SmartWeb";
            smartWebPackage.ExcludeList = new List<string>() {
                "Config",
                "Web.config",
            };
            SubPackage smartApiPackage = new SubPackage();
            smartApiPackage.SubPath = "programfiles\\SmartApi";
            smartApiPackage.TargetPath = "SmartApi";
            smartApiPackage.ExcludeList = new List<string>() {
                "Config",
                "Web.config",
            };
            SubPackage smartFileDownPackage = new SubPackage();
            smartFileDownPackage.SubPath = "programfiles\\SmartFile\\down";
            smartFileDownPackage.TargetPath = "SmartFile\\down";
            smartFileDownPackage.ExcludeList = new List<string>() {
                "Config",
                "Web.config",
            };
            SubPackage smartFileUploadPackage = new SubPackage();
            smartFileUploadPackage.SubPath = "programfiles\\SmartFile\\upload";
            smartFileUploadPackage.TargetPath = "SmartFile\\upload";
            smartFileUploadPackage.ExcludeList = new List<string>() {
                "Config",
                "Web.config",
            };
            PackageInfo packageInfo = new PackageInfo();
            packageInfo.KillProcessList = new List<ProgramInfo>() { iis, smartCenterDaemon, smartCenter };
            packageInfo.RunProcessList = new List<ProgramInfo>() { iis, smartCenter, smartCenterDaemon };
            packageInfo.SubPackages = new List<SubPackage>() { smartCenterPackage, smartWebPackage, smartApiPackage, smartFileDownPackage, smartFileUploadPackage };
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSOCT2016.json");
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            var json = JsonConvert.SerializeObject(packageInfo, Formatting.Indented);
            using (FileStream fs = new FileStream(path, FileMode.Truncate, System.IO.FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(json);
                sw.Close();
            }
        }

    }
}
