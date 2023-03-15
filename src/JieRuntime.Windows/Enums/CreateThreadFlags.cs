using System;

namespace JieRuntime.Windows.Enums
{
    /// <summary>
    /// 创建线程标志
    /// </summary>
    [Flags]
    public enum CreateThreadFlags
    {
        /// <summary>
        /// 创建后, 线程会立即运行
        /// </summary>
        None = 0,
        /// <summary>
        /// 线程以挂起状态创建, 在调用 ResumeThread 函数之前不会运行
        /// </summary>
        CreateSuspended = 0x00000004,
        /// <summary>
        /// dwStackSize 参数指定堆栈的初始保留大小. 如果未指定此标志, dwStackSize 将指定提交大小
        /// </summary>
        StackSizeParamIsAReservation = 0x00010000,
    }
}