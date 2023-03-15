using System;

namespace JieRuntime.Windows.Enums
{
    /// <summary>
    /// 内存分配类型
    /// </summary>
    [Flags]
    public enum MemoryAlocationTypes : uint
    {
        /// <summary>
        /// 为指定的保留内存页分配内存费用, (磁盘上的总内存大小和分页文件). 该函数还保证当调用方稍后最初访问内存时, 内容将为零. 除非/直到实际访问虚拟地址, 否则不会分配实际物理页
        /// </summary>
        Commit = 0x00001000,
        /// <summary>
        /// 保留进程的虚拟地址空间范围, 而无需在内存或磁盘上的分页文件中分配任何实际物理存储
        /// </summary>
        Reserve = 0x00002000,
        /// <summary>
        /// 指示 lpAddress 和 dwSize 指定的内存区域中的数据不再感兴趣. 不应从页读取或写入到分页文件. 但是, 内存块稍后将再次使用, 因此不应将其取消提交. 此值不能与任何其他值一起使用
        /// </summary>
        Reset = 0x00080000,
        /// <summary>
        /// MEM_RESET_UNDO 只应在之前成功应用 MEM_RESET 的地址范围上调用. 它表示 lpAddress 和 dwSize 指定的指定内存范围内的数据对调用方感兴趣, 并尝试扭转 MEM_RESET 的影响. 如果函数成功, 则表示指定地址范围中的所有数据都完好无损. 如果函数失败, 则地址范围中的至少一些数据已替换为零
        /// </summary>
        ResetUndo = 0x1000000,
        /// <summary>
        /// 使用大型页面支持分配内存
        /// <br />
        /// 大小和对齐方式必须是大页最小值的倍数. 若要获取此值, 请使用 GetLargePageMinimum 函数
        /// </summary>
        LargePages = 0x20000000,
        /// <summary>
        /// 保留可用于映射地址 窗口扩展 (AWE) 页的地址范围
        /// <br />
        /// 此值必须与 MEM_RESERVE 一起使用, 而没有其他值
        /// </summary>
        Physical = 0x00400000,
        /// <summary>
        /// 在可能的最高地址分配内存.  这比常规分配慢, 尤其是在有许多分配时
        /// </summary>
        TopDown = 0x00100000,
    }
}