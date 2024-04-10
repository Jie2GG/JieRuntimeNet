using System;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 表示包含套接字客户端异常事件数据的类
    /// </summary>
    public class SocketClientExceptionEventArgs : SocketClientEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取套接字的异常
        /// </summary>
        public Exception Exception { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="SocketClientExceptionEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="client">套接字客户端</param>
        /// <param name="exception">引发此事件的异常</param>
        /// <exception cref="ArgumentNullException">参数: <paramref name="exception"/> 不能为 <see langword="null"/></exception>
        public SocketClientExceptionEventArgs (SocketClient client, Exception exception)
            : base (client)
        {
            this.Exception = exception ?? throw new ArgumentNullException (nameof (exception));
        }
        #endregion
    }
}
