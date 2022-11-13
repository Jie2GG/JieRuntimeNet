namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 表示包含套接字客户端信息事件数据的类
    /// </summary>
    public class SocketClientInfoEventArgs : SocketEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取当前事件的客户端
        /// </summary>
        public SocketClient Client { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="SocketClientInfoEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="client">套接字客户端</param>
        public SocketClientInfoEventArgs (SocketClient client)
        {
            this.Client = client;
        }
        #endregion
    }
}
