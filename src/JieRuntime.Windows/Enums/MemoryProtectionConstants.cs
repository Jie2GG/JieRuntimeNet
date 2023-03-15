using System;

namespace JieRuntime.Windows.Enums
{
    /// <summary>
    /// 内存保护常量. 有关详细信息, 请参阅使用 <a href="https://learn.microsoft.com/zh-cn/windows/win32/Memory/memory-protection-constants">内存保护常量</a>
    /// </summary>
    [Flags]
    public enum MemoryProtectionConstants
    {
        /// <summary>
        /// 启用对已提交页面区域的执行访问. 尝试写入已提交的区域会导致访问冲突
        /// </summary>
        Execute = 0x10,
        /// <summary>
        /// 启用对页面已提交区域的执行或只读访问. 尝试写入已提交的区域会导致访问冲突
        /// </summary>
        ExecuteRead = 0x20,
        /// <summary>
        /// 启用对页面已提交区域的执行、只读或读/写访问权限
        /// </summary>
        ExecuteReadWrite = 0x40,
        /// <summary>
        /// 启用对文件映射对象的映射视图执行、只读或复制写入访问. 尝试写入到提交的写入页上的副本会导致为进程创建页面的专用副本
        /// </summary>
        ExecuteWriteCopy = 0x80,
        /// <summary>
        /// 禁用对页面已提交区域的所有访问. 尝试读取、写入或执行已提交的区域会导致访问冲突
        /// </summary>
        Noaccess = 0x01,
        /// <summary>
        /// 启用对页面已提交区域的只读访问. 尝试写入已提交的区域会导致访问冲突
        /// </summary>
        Readonly = 0x02,
        /// <summary>
        /// 启用对已提交页面区域的只读或读/写访问权限
        /// </summary>
        ReadWrite = 0x04,
        /// <summary>
        /// 启用对文件映射对象的映射视图的只读或复制写入访问权限. 尝试写入到提交的写入页上的副本会导致为进程创建页面的专用副本. 专用页标记为 PAGE_READWRITE, 并将更改写入新页面
        /// </summary>
        WriteCopy = 0x08,
        /// <summary>
        /// 将页面中的所有位置设置为 CFG 的无效目标
        /// </summary>
        TargestInvalid = 0x40000000,
        /// <summary>
        /// 当 VirtualProtect 的保护更改时, 区域中的页面不会更新其 CFG 信息
        /// </summary>
        TargestNoUpdate = 0x40000000,
    }
}