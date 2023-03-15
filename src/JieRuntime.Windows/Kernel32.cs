using System;
using System.Runtime.InteropServices;
using System.Text;

using JieRuntime.Windows.Delegates;
using JieRuntime.Windows.Enums;
using JieRuntime.Windows.Structs;

namespace JieRuntime.Windows
{
    /// <summary>
    /// 提供 WindowsNT 核心接口 kernel32.dll 的托管方法集
    /// </summary>
    public static class Kernel32
    {
        #region --常量--
        /// <summary>
        /// 表示无效句柄值的常量
        /// </summary>
        public static readonly IntPtr INVALID_HANDLE_VALUE = new (-1);
        #endregion

        /// <summary>
        /// 打开一个已存在的进程对象, 并返回进程的句柄. 有关详细信息, 请参阅使用 <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess">OpenProcess</a>
        /// </summary>
        /// <param name="dwDesiredAccess">进程安全和访问权限</param>
        /// <param name="bInheritHandle">如果该值为 <see langword="true"/>, 由该进程创建的进程将继承该句柄. 否则, 进程不继承此句柄</param>
        /// <param name="dwProcessId">要打开的本地进程的标识符. 如果指定的过程是系统过程 (0x00000000), 则函数失败, 最后一个错误代码是 ERROR_INVALID_PARAMETER. 如果指定的进程是空闲进程或 CSRSS 进程,那么这个函数失败, 最后一个错误代码是 ERROR_INVALID_PARAMETER, 因为它们的访问限制阻止了用户级代码打开它们</param>
        /// <returns>如果函数执行成功, 返回值是指定进程的句柄</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "OpenProcess", SetLastError = true)]
        public static extern ObjectSafeHandle OpenProcess (ProcessAccessRights dwDesiredAccess, [MarshalAs (UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        /// <summary>
        /// 在指定进程的虚拟地址空间中保留、提交或更改内存区域的状态. 该函数初始化它分配给零的内存. 有关详细信息, 请参阅使用 <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/memoryapi/nf-memoryapi-virtualallocex">VirtualAllocEx</a>
        /// </summary>
        /// <param name="hProcess">进程的句柄. 该函数在此过程的虚拟地址空间中分配内存</param>
        /// <param name="lpAddress">指定要分配的页面区域的所需起始地址的指针. (如果要保留内存，函数会将此地址舍入到分配粒度中最近的倍数)</param>
        /// <param name="dwSize">要分配的内存区域的大小 (以字节为单位)</param>
        /// <param name="flAllocationType">内存分配的类型</param>
        /// <param name="flProtect">要分配的页面区域的内存保护</param>
        /// <returns>如果函数成功，则返回值是页面分配区域的基址</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "VirtualAllocEx", SetLastError = true)]
        public static extern ObjectSafeHandle VirtualAllocEx (IntPtr hProcess, IntPtr lpAddress, SizeT dwSize, MemoryAlocationTypes flAllocationType, MemoryProtectionConstants flProtect);

        /// <summary>
        /// 将数据写入指定进程中的内存区域. 要写入的整个区域必须是可访问的, 否则操作将失败. 有关详细信息, 请参阅使用 <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/memoryapi/nf-memoryapi-writeprocessmemory">WriteProcessMemory</a>
        /// </summary>
        /// <param name="hProcess">要修改的进程内存的句柄. 句柄必须对进程具有 PROCESS_VM_WRITE 和 PROCESS_VM_OPERATION 访问权</param>
        /// <param name="lpBaseAddress">指向指定进程中要写入数据的基址的指针. 在数据传输之前, 系统会验证指定大小的基址和内存中的所有数据是否都可以进行写访问, 如果不能进行写访问, 则写入失败</param>
        /// <param name="lpBuffer">指向缓冲区的指针, 该缓冲区包含要写入指定进程地址空间的数据</param>
        /// <param name="nSize">要写入指定进程的字节数</param>
        /// <param name="lpNumberOfBytesWritten">指向一个变量的指针, 该变量接收传输到指定进程的字节数. 可选参数。如果 lpNumberOfBytesWritten 为 <see langword="null"/>, 则忽略该参数</param>
        /// <returns>如果函数成功，则返回值为非零</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "WriteProcessMemory", SetLastError = true)]
        [return: MarshalAs (UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory (IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, SizeT nSize, out int lpNumberOfBytesWritten);

        /// <summary>
        /// 创建在另一个进程的虚拟地址空间中运行的线程.有关详细信息, 请参阅使用<a href="https://learn.microsoft.com/zh-cn/windows/win32/api/processthreadsapi/nf-processthreadsapi-createremotethread?source=recommendations">CreateRemoteThread</a>
        /// </summary>
        /// <param name="hProcess">要在其中创建线程的进程句柄. 句柄必须具有PROCESS_CREATE_THREAD、PROCESS_QUERY_INFORMATION、PROCESS_VM_OPERATION、PROCESS_VM_WRITE和PROCESS_VM_READ访问权限, 并且在某些平台上没有这些权限可能会失败</param>
        /// <param name="lpThreadAttributes">指向 <see cref="SecurityAttributes"/> 结构的指针, 该结构指定新线程的安全描述符, 并确定子进程是否可以继承返回的句柄. 如果 lpThreadAttributes 为 NULL, 则线程将获取默认的安全描述符, 并且无法继承句柄. 访问控制列出了线程的默认安全描述符中的 ACL () 来自创建者的主要令牌</param>
        /// <param name="dwStackSize">堆栈的初始大小 (以字节为单位). 系统将此值舍入到最近的页面. 如果此参数为 0 (零), 则新线程将使用可执行文件的默认大小</param>
        /// <param name="lpStartAddress">指向由线程执行的 <see cref="ThreadStartRoutine"/> 类型的应用程序定义函数的指针, 表示远程进程中线程的起始地址. 函数必须存在于远程进程中</param>
        /// <param name="lpParameter">指向要传递给线程函数的变量的指针</param>
        /// <param name="dwCreationFlags">控制线程创建的标志</param>
        /// <param name="lpThreadId">指向接收线程标识符的变量的指针</param>
        /// <returns>如果函数成功，则返回值是新线程的句柄</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "CreateRemoteThread", SetLastError = true)]
        public static extern ObjectSafeHandle CreateRemoteThread (IntPtr hProcess, SecurityAttributes lpThreadAttributes, SizeT dwStackSize, ThreadStartRoutine lpStartAddress, IntPtr lpParameter, CreateThreadFlags dwCreationFlags, out int lpThreadId);

        /// <summary>
        /// 关闭一个打开的对象句柄
        /// </summary>
        /// <param name="hObject">打开对象的有效句柄</param>
        /// <returns>如果函数成功，则返回值为非零</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "CloseHandle", SetLastError = true)]
        [return: MarshalAs (UnmanagedType.Bool)]
        public static extern bool CloseHandle (IntPtr hObject);

        /// <summary>
        /// 更改调用过程的虚拟地址空间中已提交页面区域的保护
        /// </summary>
        /// <param name="lpAddress">要更改其访问保护属性的页面区域的起始页的地址
        /// <br />
        /// 指定区域中的所有页面都必须位于使用 MEM_RESERVE 调用 VirtualAlloc 或 VirtualAllocEx 函数时分配的相同保留区域。 这些页面不能跨使用MEM_RESERVE通过对 VirtualAlloc 或 VirtualAllocEx 的单独调用分配的相邻保留区域
        /// </param>
        /// <param name="dwSize">要更改其访问保护属性的区域的大小 (以字节为单位). 受影响的页面区域包括从 lpAddress 参数到 (lpAddress+dwSize) 的范围中包含一个或多个字节的所有页面. 这意味着, 跨页边界的 2 字节范围会导致这两个页面的保护属性发生更改</param>
        /// <param name="flNewProtect">内存保护选项。 此参数可以是内存保护常量 <see cref="MemoryProtectionConstants"/> 之一</param>
        /// <param name="lpflOldProtect">指向一个变量的指针, 该变量接收页面指定区域中第一页的上一个访问保护值. 如果此参数为 NULL 或未指向有效变量, 则该函数将失败</param>
        /// <returns>如果该函数成功, 则返回值为非零值</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "VirtualProtect", SetLastError = true)]
        [return: MarshalAs (UnmanagedType.Bool)]
        public static extern bool VirtualProtect (IntPtr lpAddress, SizeT dwSize, MemoryProtectionConstants flNewProtect, out MemoryProtectionConstants lpflOldProtect);

        /// <summary>
        /// 更改指定进程的虚拟地址空间中已提交的页面区域的保护
        /// </summary>
        /// <param name="hProcess">要更改其内存保护的进程句柄. 句柄必须具有 PROCESS_VM_OPERATION 访问权限</param>
        /// <param name="lpAddress">要更改其访问保护属性的页面区域的起始页的地址
        /// <br />
        /// 指定区域中的所有页面都必须位于使用 MEM_RESERVE 调用 VirtualAlloc 或 VirtualAllocEx 函数时分配的相同保留区域。 这些页面不能跨使用MEM_RESERVE通过对 VirtualAlloc 或 VirtualAllocEx 的单独调用分配的相邻保留区域
        /// </param>
        /// <param name="dwSize">要更改其访问保护属性的区域的大小 (以字节为单位). 受影响的页面区域包括从 lpAddress 参数到 (lpAddress+dwSize) 的范围中包含一个或多个字节的所有页面. 这意味着, 跨页边界的 2 字节范围会导致这两个页面的保护属性发生更改</param>
        /// <param name="flNewProtect">内存保护选项。 此参数可以是内存保护常量 <see cref="MemoryProtectionConstants"/> 之一</param>
        /// <param name="lpflOldProtect">指向一个变量的指针, 该变量接收页面指定区域中第一页的上一个访问保护值. 如果此参数为 NULL 或未指向有效变量, 则该函数将失败</param>
        /// <returns>如果该函数成功, 则返回值为非零值</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "VirtualProtectEx", SetLastError = true)]
        [return: MarshalAs (UnmanagedType.Bool)]
        public static extern bool VirtualProtectEx (IntPtr hProcess, IntPtr lpAddress, SizeT dwSize, MemoryProtectionConstants flNewProtect, out MemoryProtectionConstants lpflOldProtect);

        /// <summary>
        /// 检索指定模块的模块句柄. 该模块必须由调用进程加载
        /// </summary>
        /// <param name="lpModuleName">加载的模块的名称 (.dll或.exe文件). 如果省略文件扩展名, 则会追加默认库扩展名.dll. 文件名字符串可以包含尾随点字符 (.), 以指示模块名称没有扩展名. 字符串不必指定路径. 指定路径时, 请确保使用反斜杠 (\), 而不是 (/) 正斜杠. 将名称单独 (大小写) 与当前映射到调用进程的地址空间的模块的名称进行比较</param>
        /// <returns>如果函数成功, 则返回值是指定模块的句柄</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "GetModuleHandle", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle (string lpModuleName);

        /// <summary>
        /// 从指定的动态链接库 (DLL) 检索导出函数 (也称为过程) 或变量
        /// </summary>
        /// <param name="hModule">包含函数或变量的 DLL 模块的句柄. LoadLibrary、LoadLibraryEx、LoadPackagedLibrary 或 GetModuleHandle 函数返回此句柄</param>
        /// <param name="lpProcName">函数或变量名称</param>
        /// <returns>如果函数成功, 则返回值是导出的函数或变量的地址</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "GetProcAddress", SetLastError = true)]
        public static extern IntPtr GetProcAddress (IntPtr hModule, string lpProcName);

        /// <summary>
        /// 释放加载的动态链接库 (DLL) 模块, 并在必要时递减其引用计数. 当引用计数达到零时, 将从调用进程的地址空间卸载模块, 并且句柄不再有效
        /// </summary>
        /// <param name="hModule">已加载库模块的句柄. LoadLibrary、LoadLibraryEx、GetModuleHandle 或 GetModuleHandleEx 函数返回此句柄</param>
        /// <returns>如果该函数成功, 则返回值为非零值</returns>
        [DllImport (nameof (Kernel32), EntryPoint = "FreeLibrary", SetLastError = true)]
        [return: MarshalAs (UnmanagedType.Bool)]
        public static extern bool FreeLibrary (IntPtr hModule);
    }
}
