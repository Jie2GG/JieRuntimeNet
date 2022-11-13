namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 表示 UDP 协议套接字数据事件数据的类
    /// </summary>
    public class SocketUdpDataEventArgs : SocketDataEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取当前事件的 UDP 状态
        /// </summary>
        public UdpState State { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="SocketUdpDataEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="state">相关的UDP状态</param>
        public SocketUdpDataEventArgs (UdpState state)
            : base (state.Data)
        {
            this.State = state;
        }
        #endregion
    }
}
