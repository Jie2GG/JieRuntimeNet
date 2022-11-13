using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JieRuntime.Ini
{
    /// <summary>
    /// 表示 Initialization 配置项执行期间发生的错误
    /// </summary>
    public class IniException : Exception
    {
        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="IniException"/> 类的新实例
        /// </summary>
        public IniException ()
        { }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="IniException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public IniException (string message)
            : base (message)
        { }

        /// <summary>
        /// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="IniException"/> 类的新实例
        /// </summary>
        /// <param name="message">解释异常原因的错误消息</param>
        /// <param name="innerException">导致当前异常的异常，如果没有指定内部异常，则为 <see langword="null"/> (在Visual Basic中为 <see langword="Nothing"/>)</param>
        public IniException (string message, Exception innerException)
            : base (message, innerException)
        { }

        /// <summary>
        /// 使用序列化数据初始化 <see cref="IniException"/> 类的新实例
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>，它保存关于所抛出异常的序列化对象数据</param>
        /// <param name="context">包含关于源或目的地的上下文信息的 <see cref="StreamingContext"/></param>
        protected IniException (SerializationInfo info, StreamingContext context)
            : base (info, context)
        { }
        #endregion
    }
}
