using System;

namespace JieRuntime.Hook.Exceptions
{
    /// <summary>
    /// 表示进程已经退出的异常
    /// </summary>
    [Serializable]
    public class ProcessExitedException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="ProcessExitedException"/> 类的新实例
        /// </summary>
        public ProcessExitedException ()
            : base ("进程已退出")
        { }
    }
}