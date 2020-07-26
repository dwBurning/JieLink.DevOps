using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JieShun.JieLink.DevOps.Updater.Models
{
    public class PackageInfo
    {
        /// <summary>
        /// 需要结束运行的程序名
        /// </summary>
        public List<ProgramInfo> KillProcessList { get; set; }
        public List<SubPackage> SubPackages { get; set; }
        public List<ProgramInfo> RunProcessList { get; set; }

    }
    public class SubPackage
    {
        /// <summary>
        ///要替换文件的根目录（相对于压缩包根目录）
        /// </summary>
        public string SubPath { get; set; }
        /// <summary>
        /// 目标路径（相对于程序安装目录）
        /// </summary>
        public string TargetPath { get; set; }
        /// <summary>
        /// 排除项列表
        /// </summary>
        public List<string> ExcludeList { get; set; }

    }
    public class ProgramInfo
    {
        /// <summary>
        /// 进程名
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 服务名，如果是服务的话
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 可执行文件的路径（相对或者绝对路径，程序要做到能自动识别）
        /// </summary>
        public string ExecutablePath { get; set; }
        /// <summary>
        /// 命令行参数
        /// </summary>
        public string Args { get; set; }
    }
}
