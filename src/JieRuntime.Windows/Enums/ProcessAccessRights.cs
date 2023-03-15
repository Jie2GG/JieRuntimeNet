using System;

namespace JieRuntime.Windows.Enums
{
    /// <summary>
    /// 进程安全和访问权限 . 有关详细信息, 请参阅使用 <a href="https://learn.microsoft.com/zh-cn/windows/win32/procthread/process-security-and-access-rights?redirectedfrom=MSDN">处理安全和访问权限</a>
    /// </summary>
    [Flags]
    public enum ProcessAccessRights : uint
    {
        /// <summary>
        /// 组合值为全部访问
        /// </summary>
        All = 0x001F0FFF,
        /// <summary>
        /// 在 TerminateProcess 函数中使用进程句柄来终止进程
        /// </summary>
        Terminate = 0x00000001,
        /// <summary>
        /// 允许使用 CreateRemoteThread 函数中的进程句柄在进程中创建线程
        /// </summary>
        CreateThread = 0x00000002,
        /// <summary>
        /// 允许在 VirtualProtectEx 和 WriteProcessMemory 函数中使用进程句柄来修改进程的虚拟内存
        /// </summary>
        VirtualMemoryOperation = 0x00000008,
        /// <summary>
        /// 允许使用 ReadProcessMemory 函数中的进程句柄从进程的虚拟内存中读取
        /// </summary>
        VirtualMemoryRead = 0x00000010,
        /// <summary>
        /// 允许使用 WriteProcessMemory 函数中的进程句柄写入进程的虚拟内存
        /// </summary>
        VirtualMemoryWrite = 0x00000020,
        /// <summary>
        /// 允许在 DuplicateHandle 函数中使用进程句柄作为源进程或目标进程来复制句柄
        /// </summary>
        DuplicateHandle = 0x00000040,
        /// <summary>
        /// 允许使用 SetPriorityClass 函数中的进程句柄来设置进程的优先级类
        /// </summary>
        SetInformation = 0x00000200,
        /// <summary>
        /// 允许在 GetExitCodeProcess 和 GetPriorityClass 函数中使用流程句柄从流程对象中读取信息
        /// </summary>
        QueryInformation = 0x00000400,
        /// <summary>
        /// 需要检索有关进程的某些信息 (参见 GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName). 有 PROCESS_QUERY_INFORMATION 访问权限的句柄被自动授予 PROCESS_QUERY_LIMITED_INFORMATION. Windows Server 2003 和 Windows XP 不支持此访问权限
        /// </summary>
        QueryLimitedInformation = 0x00000400,
        /// <summary>
        /// 使用该对象进行同步的权限, 这使得线程可以等待对象处于有信号状态
        /// </summary>
        Synchronize = 0x00100000
    }
}