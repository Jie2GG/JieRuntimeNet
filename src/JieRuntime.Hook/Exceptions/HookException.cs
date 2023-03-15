using System;
using System.ComponentModel;

namespace JieRuntime.Hook.Exceptions
{
    /// <summary>
    /// 表示挂钩程序运行期间的错误
    /// </summary>
    [Serializable]
    public class HookException : Win32Exception
    {
        /// <summary>
        /// 初始化 <see cref="HookException"/> 类的新实例
        /// </summary>
        public HookException ()
            : base ()
        { }

        /// <summary>
        /// 使用指定的错误消息来初始化 <see cref="HookException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息字符串</param>
        public HookException (string message)
            : base (message)
        { }
    }
}
