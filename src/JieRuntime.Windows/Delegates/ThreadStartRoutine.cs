using System;
using System.Runtime.InteropServices;

namespace JieRuntime.Windows.Delegates
{
    /// <summary>
    /// 指向一个函数, 该函数通知主机线程已开始执行
    /// </summary>
    /// <param name="lpThreadParameter">指向已经开始执行的代码的指针</param>
    /// <returns>返回值表示该函数的成功或失败, 返回值永远不应该设置为 STILL_ACTIVE(259), 如 GetExitCodeThread 中所述</returns>
    [UnmanagedFunctionPointer (CallingConvention.Winapi)]
    public delegate uint ThreadStartRoutine (IntPtr lpThreadParameter);
}
