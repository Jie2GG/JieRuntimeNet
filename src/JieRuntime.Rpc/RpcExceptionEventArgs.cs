using JieRuntime.Net.Sockets;
using System;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示包含远程调用异常事件数据的类
    /// </summary>
    public class RpcExceptionEventArgs : RpcEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取套接字的异常
        /// </summary>
        public Exception Exception { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcExceptionEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="exception">引发此事件的异常</param>
        /// <exception cref="ArgumentNullException">参数: <paramref name="exception"/> 不能为 <see langword="null"/></exception>
        public RpcExceptionEventArgs (Exception exception)
        {
            this.Exception = exception ?? throw new ArgumentNullException (nameof (exception));
        }
        #endregion
    }
}
