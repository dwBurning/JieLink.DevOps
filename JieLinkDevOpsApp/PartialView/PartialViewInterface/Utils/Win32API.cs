using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.Kernel32;
using static PInvoke.AdvApi32;
using static PInvoke.Userenv;

namespace PartialViewInterface.Utils
{
    public class Win32API
    {
        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        #region P/Invoke  APIs
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSSendMessage(IntPtr hServer, int SessionId, String pTitle, int TitleLength, String pMessage, int MessageLength, int Style, int Timeout, out int pResponse, bool bWait);

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] UInt32 Reserved,
            [MarshalAs(UnmanagedType.U4)] UInt32 Version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref UInt32 pSessionInfoCount
            );

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);


        [DllImport("advapi32", CallingConvention = CallingConvention.Winapi, SetLastError = true, EntryPoint = "LookupPrivilegeValueA")]
        static extern bool LookupPrivilegeValue([MarshalAs(UnmanagedType.LPStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPStr)] string lpName, ref LUID lpLuid);
        [DllImport("advapi32", CallingConvention = CallingConvention.Winapi, SetLastError = true, EntryPoint = "AdjustTokenPrivileges")]
        static extern bool AdjustTokenPrivileges(SafeObjectHandle handle, bool disableAllPrivileges, ref TOKEN_PRIVILEGES newState, int bufferLength, IntPtr previousState, IntPtr teturnLength);
        [DllImport("advapi32", CallingConvention = CallingConvention.Winapi, SetLastError = true, EntryPoint = "CreateProcessAsUser")]
        static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, CreateProcessFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFOA lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        #endregion

        #region Structs
        /// <summary> 
        /// Struct, Enum and P/Invoke Declarations for CreateProcessAsUser. 
        /// </summary> 
        ///  

        private enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WTS_SESSION_INFO
        {
            public UInt32 SessionID;
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public uint Attributes;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct STARTUPINFOA
        {

            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;


        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }
        /// <summary>
        /// 以当前登录的windows用户(角色权限)运行指定程序进程
        /// </summary>
        /// <param name="hToken"></param>
        /// <param name="lpApplicationName">指定程序(全路径)</param>
        /// <param name="lpCommandLine">参数</param>
        /// <param name="lpProcessAttributes">进程属性</param>
        /// <param name="lpThreadAttributes">线程属性</param>
        /// <param name="bInheritHandles"></param>
        /// <param name="dwCreationFlags"></param>
        /// <param name="lpEnvironment"></param>
        /// <param name="lpCurrentDirectory"></param>
        /// <param name="lpStartupInfo">程序启动属性</param>
        /// <param name="lpProcessInformation">最后返回的进程信息</param>
        /// <returns>是否调用成功</returns>
        [DllImport("ADVAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes,
                                                      bool bInheritHandles, uint dwCreationFlags, string lpEnvironment, string lpCurrentDirectory,
                                                      ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("KERNEL32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CloseHandle(IntPtr hHandle);
        [DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX mem);
        #endregion
        /// <summary>
        /// 服务程序执行消息提示,前台MessageBox.Show
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        public static void ShowServiceMessage(string message, string title)
        {
            int resp = 0;
            WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, WTSGetActiveConsoleSessionId(), title, title.Length, message, message.Length, 0, 0, out resp, false);
        }
        /// <summary>
        /// 以当前登录系统的用户角色权限启动指定的进程
        /// </summary>
        /// <param name="processName">指定的进程(全路径)</param>
        public static bool CreateProcessV1(string processName, string cmdLine)
        {
            bool ok = false;
            IntPtr ppSessionInfo = IntPtr.Zero;
            UInt32 SessionCount = 0;
            if (WTSEnumerateSessions(
                                    (IntPtr)WTS_CURRENT_SERVER_HANDLE,  // Current RD Session Host Server handle would be zero. 
                                    0,  // This reserved parameter must be zero. 
                                    1,  // The version of the enumeration request must be 1. 
                                    ref ppSessionInfo, // This would point to an array of session info. 
                                    ref SessionCount  // This would indicate the length of the above array.
                                    ))
            {
                for (int nCount = 0; nCount < SessionCount; nCount++)
                {
                    WTS_SESSION_INFO tSessionInfo = (WTS_SESSION_INFO)Marshal.PtrToStructure(ppSessionInfo + nCount * Marshal.SizeOf(typeof(WTS_SESSION_INFO)), typeof(WTS_SESSION_INFO));
                    if (WTS_CONNECTSTATE_CLASS.WTSActive == tSessionInfo.State)
                    {
                        IntPtr hToken = IntPtr.Zero;
                        if (WTSQueryUserToken(tSessionInfo.SessionID, out hToken))
                        {
                            PROCESS_INFORMATION tProcessInfo;
                            STARTUPINFO tStartUpInfo = new STARTUPINFO();
                            tStartUpInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                            bool ChildProcStarted = CreateProcessAsUser(
                                                                        hToken,             // Token of the logged-on user. 
                                                                        processName,      // Name of the process to be started. 
                                                                        cmdLine,               // Any command line arguments to be passed. 
                                                                        IntPtr.Zero,        // Default Process' attributes. 
                                                                        IntPtr.Zero,        // Default Thread's attributes. 
                                                                        false,              // Does NOT inherit parent's handles. 
                                                                        0,                  // No any specific creation flag. 
                                                                        null,               // Default environment path. 
                                                                        null,               // Default current directory. 
                                                                        ref tStartUpInfo,   // Process Startup Info.  
                                                                        out tProcessInfo    // Process information to be returned. 
                                                     );
                            if (ChildProcStarted)
                            {
                                ok = true;
                                CloseHandle(tProcessInfo.hThread);
                                CloseHandle(tProcessInfo.hProcess);
                            }
                            else
                            {
                                ShowServiceMessage("CreateProcessAsUser失败", "CreateProcess");
                            }
                            CloseHandle(hToken);
                            break;
                        }
                        else
                        {
                            //var errorCode=PInvoke.Kernel32.GetLastError();
                        }
                    }
                }
                WTSFreeMemory(ppSessionInfo);

            }
            return ok;
        }
        public static bool CreateProcessV2(string processName, string cmdLine)
        {
            try
            {
                using (SafeObjectHandle handle = DuplicateToken())
                {
                    CreateProcessAsUser(handle, processName, cmdLine);
                }
            }
            catch
            {
                CreateProcessAsUser(new SafeObjectHandle(), processName, cmdLine);
            }
            //不报异常就是OK
            return true;
        }
        public static SafeObjectHandle DuplicateToken()
        {
            int pid = GetPid("winlogon.exe");
            if (pid == 0)
            {
                throw new Exception("无法获取pid");
            }

            var hProcess = OpenProcess((int)ACCESS_MASK.SpecialRight.MAXIMUM_ALLOWED, false, pid);
            if (hProcess.IsInvalid)
            {
                throw new Exception("winlogon.exe OpenProcess失败");
            }
            SafeObjectHandle hPToken;
            if (!OpenProcessToken(hProcess.DangerousGetHandle(), (int)ACCESS_MASK.SpecialRight.MAXIMUM_ALLOWED, out hPToken))
            {
                hProcess.Close();
                throw new Exception("winlogon.exe OpenProcessToken失败");
            }
            hProcess.Close();
            SafeObjectHandle hNewPToken;
            if (!DuplicateTokenEx(hPToken, (int)ACCESS_MASK.SpecialRight.MAXIMUM_ALLOWED, IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, TOKEN_TYPE.TokenPrimary, out hNewPToken))
            {
                throw new Exception("DuplicateTokenEx 失败");
            }

            const string SE_DEBUG_NAME = "SeDebugPrivilege";
            const string SE_ASSIGNPRIMARYTOKEN_NAME = "SeAssignPrimaryTokenPrivilege";
            const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
            var ft = new LUID();

            if (!LookupPrivilegeValue(null, SE_DEBUG_NAME, ref ft))
            {
                throw new Exception("LookupPrivilegeValue 失败");
            }

            const uint SE_PRIVILEGE_ENABLED = 2;
            TOKEN_PRIVILEGES tp;
            tp.PrivilegeCount = 1;
            tp.Privileges = new LUID_AND_ATTRIBUTES[1];
            tp.Privileges[0].Luid = ft;
            tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
            if (!AdjustTokenPrivileges(hNewPToken, false, ref tp, Marshal.SizeOf(tp), IntPtr.Zero, IntPtr.Zero))
            {
                throw new Exception("AdjustTokenPrivileges 失败");
            }
            PInvoke.Win32ErrorCode errorCode = GetLastError();
            if (errorCode == PInvoke.Win32ErrorCode.ERROR_NOT_ALL_ASSIGNED)
            {
                throw new Exception("AdjustTokenPrivileges ERROR_NOT_ALL_ASSIGNED");
            }

            return hNewPToken;
        }
        public static void CreateProcessAsUser(SafeObjectHandle handle, string exePath, string cmdLine)
        {
            if (!string.IsNullOrEmpty(cmdLine))
                cmdLine = " " + cmdLine;
            IntPtr pEnv = IntPtr.Zero;
            const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
            const int NORMAL_PRIORITY_CLASS = 0x00000020;
            const int CREATE_NEW_CONSOLE = 0x00000010;
            int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE | CREATE_UNICODE_ENVIRONMENT;
            if (!handle.IsInvalid)
                CreateEnvironmentBlock(out pEnv, handle, true);

            PROCESS_INFORMATION pi;
            STARTUPINFOA si = new STARTUPINFOA();
            si.cb = (uint)Marshal.SizeOf(si);
            //si.lpDesktop = Marshal.StringToHGlobalAnsi("winsta0\\default");
            si.lpDesktop = "winsta0\\default";
            IntPtr pHandle = IntPtr.Zero;
            if (!handle.IsInvalid)
                pHandle = handle.DangerousGetHandle();
            if (!CreateProcessAsUser(pHandle
                , exePath
                , cmdLine
                , IntPtr.Zero //进程安全属性
                , IntPtr.Zero  // 线程安全属性
                , false // 句柄不可继承
                , (CreateProcessFlags)dwCreationFlags // 创建标识
                , pEnv// 环境信息 
                , null // 当前路径
                , ref si
                , out pi))
            {
                var errorCode = GetLastError();
                throw new Exception("CreateProcessAsUser 失败");
            }

        }
        public static int GetPid(string processName)
        {
            int pid = 0;//待获取

            var hSnapshot = CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS, 0);
            var entry = new PROCESSENTRY32();
            entry.dwSize = Marshal.SizeOf(entry);
            bool hasMore = Process32First(hSnapshot, ref entry);
            while (hasMore)
            {
                //Console.WriteLine(entry.szExeFile);
                if (string.Equals(processName, entry.ExeFile, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("找到目标进程{0},pid={1}", entry.ExeFile, entry.th32ProcessID);
                    pid = entry.th32ProcessID;
                    break;
                }
                hasMore = Process32Next(hSnapshot, ref entry);
            }
            hSnapshot.Close();
            return pid;
        }
    }
}
