using System;
using System.Net;
using System.Net.Sockets;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 提供套接字客户端服务的类, 该类是抽象的
    /// </summary>
    public abstract class SocketClient : IDisposable
    {
        #region --属性--
        /// <summary>
        /// 获取基础网络套接字 <see cref="Socket"/>
        /// </summary>
        public abstract Socket Client { get; }

        /// <summary>
		/// 获取当前网络客户端的本地 IP 地址
		/// </summary>
		public abstract IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// 获取当前网络客户端的远程 IP 地址
        /// </summary>
        public abstract IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 获取当前客户端是否正在运行
        /// </summary>
        public abstract bool IsRunning { get; }

        /// <summary>
        /// 获取当前客户端是否已连接到远程服务器
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// 获取或设置当前客户端接收或发送数据时的封包大小
        /// </summary>
        public abstract int PacketSize { get; set; }

        /// <summary>
        /// 获取当前客户端接收或发送数据时的封包头占用字节树
        /// </summary>
        public abstract byte PacketHeaderLength { get; }
        #endregion

        #region --事件--
        /// <summary>
        /// 表示客户端成功连接到远程服务器的事件
        /// </summary>
        public abstract event EventHandler<SocketEventArgs> Connected;

        /// <summary>
        /// 表示客户端断开与远程服务器连接的事件
        /// </summary>
        public abstract event EventHandler<SocketEventArgs> Disconnected;

        /// <summary>
        /// 表示客户端收到远程服务器数据的事件
        /// </summary>
        public abstract event EventHandler<SocketDataEventArgs> ReceiveData;

        /// <summary>
        /// 表示客户端发送数据到远程服务器的事件
        /// </summary>
        public abstract event EventHandler<SocketDataEventArgs> SendData;

        /// <summary>
        /// 表示客户端出现异常的事件
        /// </summary>
        public abstract event EventHandler<SocketExceptionEventArgs> Exception;
        #endregion

        #region --公开方法--
        /// <summary>
        /// 连接远程客户端
        /// </summary>
        /// <param name="remoteEP">远程服务端点</param>
        /// <exception cref="ArgumentNullException">remoteEP 是 <see langword="null"/></exception>
        public abstract void Connect (IPEndPoint remoteEP);

        /// <summary>
        /// 断开连接远程客户端
        /// </summary>
        /// <param name="reuseClient">是否需要复用当前客户端</param>
        public abstract void Disconnect (bool reuseClient);

        /// <summary>
        /// 发送数据到远程客户端
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <exception cref="ArgumentNullException">data 是 <see langword="null"/></exception>
        public abstract void Send (byte[] data);

        /// <summary>
        /// 释放当前实例所占用的资源
        /// </summary>
        public abstract void Dispose ();

        /// <summary>
		/// 客户端连接成功
		/// </summary>
		/// <param name="e">包含客户端连接成功的事件参数</param>
		protected virtual void OnConnected (SocketEventArgs e)
        { }

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="e">包含客户端断开连接的事件参数</param>
        protected virtual void OnDisconnected (SocketEventArgs e)
        { }

        /// <summary>
        /// 客户端收到数据
        /// </summary>
        /// <param name="e">包含客户端收到数据的事件参数</param>
        protected virtual void OnReceiveData (SocketDataEventArgs e)
        { }

        /// <summary>
        /// 客户端发送数据
        /// </summary>
        /// <param name="e">包含客户端发送数据的事件参数</param>
        protected virtual void OnSendData (SocketDataEventArgs e)
        { }

        /// <summary>
        /// 客户端异常
        /// </summary>
        /// <param name="e">包含客户端异常信息的事件参数</param>
        protected virtual void OnException (SocketExceptionEventArgs e)
        { }
        #endregion
    }
}
