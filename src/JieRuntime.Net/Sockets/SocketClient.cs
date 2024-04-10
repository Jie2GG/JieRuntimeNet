using System;
using System.Net;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 提供网络套接字客户端的类, 该类是抽象的
    /// </summary>
    public abstract class SocketClient : IDisposable
    {
        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否正在运行
        /// </summary>
        public abstract bool IsRunning { get; }

        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否已连接到远程主机
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// 获取当前客户端的本地端点
        /// </summary>
        public abstract IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// 获取当前客户端连接的远程端点
        /// </summary>
        public abstract IPEndPoint RemoteEndPoint { get; }
        #endregion

        #region --事件--
        /// <summary>
        /// 客户端连接远程主机事件
        /// </summary>
        public abstract event EventHandler<SocketEventArgs> Connected;

        /// <summary>
        /// 客户端断开远程主机连接事件
        /// </summary>
        public abstract event EventHandler<SocketEventArgs> Disconnected;

        /// <summary>
        /// 客户端出现异常的事件
        /// </summary>
        public abstract event EventHandler<SocketExceptionEventArgs> Exception;

        /// <summary>
        /// 客户端接收到远程主机数据的事件
        /// </summary>
        public abstract event EventHandler<SocketDataEventArgs> Received;

        /// <summary>
        /// 客户端发送数据到远程主机的事件
        /// </summary>
        public abstract event EventHandler<SocketDataEventArgs> Sending;
        #endregion

        #region --公开方法--
        /// <summary>
        /// 开始对远程主机连接
        /// </summary>
        /// <param name="remoteEP">表示远程设备的 <see cref="IPEndPoint"/></param>
        public abstract void Connect (IPEndPoint remoteEP);

        /// <summary>
        /// 断开远程主机连接
        /// </summary>
        /// <param name="reuseSocket">是否需要重用套接字</param>
        public abstract void Disconnect (bool reuseSocket);

        /// <summary>
        /// 向已连接的远程主机发送数据
        /// </summary>
        /// <param name="data"><see cref="byte"/> 类型的数组，包含要发送的数据</param>
        public abstract void Send (byte[] data);

        /// <summary>
        /// 释放当前实例所占用的资源
        /// </summary>
        public abstract void Dispose ();
        #endregion
    }
}
