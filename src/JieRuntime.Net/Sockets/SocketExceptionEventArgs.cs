using System;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 表示套接字异常事件数据的类
    /// </summary>
    public class SocketExceptionEventArgs : SocketEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取当前事件的异常
        /// </summary>
        public Exception Exception { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="SocketExceptionEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="e">异常信息</param>
        public SocketExceptionEventArgs (Exception e)
        {
            this.Exception = e;
        }
        #endregion
    }
}
