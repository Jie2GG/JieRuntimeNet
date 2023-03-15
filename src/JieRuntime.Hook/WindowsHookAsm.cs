using JieRuntime.Windows.Enums;

namespace JieRuntime.Hook
{
    /// <summary>
    /// 表示 Windows 平台进程挂钩的汇编数据
    /// </summary>
    public class WindowsHookAsm
    {
        /// <summary>
        /// 获取内存保护标志
        /// </summary>
        public MemoryProtectionConstants MemoryProtectionConstants { get; internal set; }

        /// <summary>
        /// 原始函数汇编码
        /// </summary>
        public byte[] OriginalFunctionAsm { get; internal set; }

        /// <summary>
        /// 挂钩函数汇编码
        /// </summary>
        public byte[] HookFunctionAsm { get; internal set; }
    }
}
