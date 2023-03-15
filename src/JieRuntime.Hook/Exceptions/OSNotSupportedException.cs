using System;

namespace JieRuntime.Hook.Exceptions
{
    /// <summary>
    /// 表示操作系统不受支持的异常
    /// </summary>
    public class OSNotSupportedException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="OSNotSupportedException"/> 类的新实例
        /// </summary>
        public OSNotSupportedException ()
            : base ("当前操作系统暂不支持被 Hook")
        { }
    }
}
