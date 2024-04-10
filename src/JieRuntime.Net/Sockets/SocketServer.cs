using System;
using System.Collections.Generic;
using System.Net;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 提供网络套接字服务端的类, 该类是抽象的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SocketServer<T>
        where T : SocketClient
    {
        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前服务端是否正在运行
        /// </summary>
        public abstract bool IsRunning { get; }

        /// <summary>
        /// 获取当前服务端监听的端点
        /// </summary>
        public IPEndPoint ListenerPoint { get; }

        /// <summary>
        /// 获取当前服务端的客户端列表
        /// </summary>
        public abstract IReadOnlyCollection<T> Clients { get; }
        #endregion

        #region --事件--
        /// <summary>
        /// 服务端启动事件
        /// </summary>
        public abstract event EventHandler<SocketEventArgs> Started;

        /// <summary>
        /// 服务端停止事件
        /// </summary>
        public abstract event EventHandler<SocketEventArgs> Stopped;

        /// <summary>
        /// 服务端出现异常的事件
        /// </summary>
        public abstract event EventHandler<SocketExceptionEventArgs> Exception;

        /// <summary>
        /// 服务端和远程主机建立连接的事件
        /// </summary>
        public abstract event EventHandler<SocketClientEventArgs> ClientConnected;

        /// <summary>
        /// 服务端断开和远程主机连接的事件
        /// </summary>
        public abstract event EventHandler<SocketClientEventArgs> ClientDisconnected;

        /// <summary>
        /// 服务端收到远程主机数据的事件
        /// </summary>
        public abstract event EventHandler<SocketClientDataEventArgs> ClientReceived;

        /// <summary>
        /// 服务端向远程主机发送数据的事件
        /// </summary>
        public abstract event EventHandler<SocketClientDataEventArgs> ClientSending;

        /// <summary>
        /// 服务端连接的远程主机出现异常的事件
        /// </summary>
        public abstract event EventHandler<SocketClientExceptionEventArgs> ClientException;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 使用指定的 IP 地址和端口初始化 <see cref="SocketServer{T}"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <exception cref="ArgumentNullException"><paramref name="localaddr"/> 是 <see langword="null"/></exception>
        protected SocketServer (IPAddress localaddr, int port)
        {
            if (localaddr is null)
            {
                throw new ArgumentNullException (nameof (localaddr));
            }

            // 监听地址
            this.ListenerPoint = new IPEndPoint (localaddr, port);
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 服务端开始监听
        /// </summary>
        public abstract void Start ();

        /// <summary>
        /// 服务端停止监听
        /// </summary>
        public abstract void Stop ();
        #endregion
    }
}
