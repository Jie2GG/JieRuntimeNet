namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 表示包含套接字客户端传输的事件数据的类
    /// </summary>
    public class SocketClientDataEventArgs : SocketClientEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取套接字传输的数据
        /// </summary>
        public byte[] Data { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="SocketDataEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="client">引发此事件的客户端</param>
        /// <param name="data">传输的数据</param>
        public SocketClientDataEventArgs (SocketClient client, byte[] data)
            : base (client)
        {
            this.Data = data;
        }
        #endregion
    }
}
