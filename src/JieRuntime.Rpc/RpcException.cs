using System;
using System.Runtime.Serialization;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示远程调用服务执行期间发生的错误
    /// </summary>
    public class RpcException : Exception
    {
        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcException"/> 类的新实例
        /// </summary>
        public RpcException ()
        { }

        /// <summary>
        /// 使用指定的错误消息初始化 <see cref="RpcException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public RpcException (string message)
            : base (message)
        { }

        /// <summary>
        /// 使用指定的错误消息和对导致此异常的内部异常的引用初始化 <see cref="RpcException"/> 类的新实例
        /// </summary>
        /// <param name="message">解释异常原因的错误消息</param>
        /// <param name="innerException">导致当前异常的异常，如果没有指定内部异常，则为 <see langword="null"/> (在Visual Basic中为 <see langword="Nothing"/>)</param>
        public RpcException (string message, Exception innerException)
            : base (message, innerException)
        { }

        /// <summary>
        /// 使用序列化数据初始化 <see cref="RpcException"/> 类的新实例
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>，它保存关于所抛出异常的序列化对象数据</param>
        /// <param name="context">包含关于源或目的地的上下文信息的 <see cref="StreamingContext"/></param>
        protected RpcException (SerializationInfo info, StreamingContext context)
            : base (info, context)
        { }
        #endregion
    }
}
