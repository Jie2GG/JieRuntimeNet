using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

using JieRuntime.Extensions;

namespace JieRuntime.Net.Sockets.Udp
{
    /// <summary>
    /// 提供基于 UDP 协议的网络套接字服务端
    /// </summary>
    public class UdpServer : SocketServer<UdpClient>
    {
        #region --字段--
        private readonly UdpOptions options;
        private Socket server = null;
        private bool isRunning;
        private readonly Collection<UdpClient> clients;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前服务端是否正在运行
        /// </summary>
        public override bool IsRunning => this.isRunning;

        /// <summary>
        /// 获取当前服务端的客户端列表
        /// </summary>
        public override IReadOnlyCollection<UdpClient> Clients => this.clients;
        #endregion

        #region --事件--
        /// <summary>
        /// 服务端启动事件
        /// </summary>
        public override event EventHandler<SocketEventArgs> Started;

        /// <summary>
        /// 服务端停止事件
        /// </summary>
        public override event EventHandler<SocketEventArgs> Stopped;

        /// <summary>
        /// 服务端出现异常的事件
        /// </summary>
        public override event EventHandler<SocketExceptionEventArgs> Exception;

        /// <summary>
        /// 服务端和远程主机建立连接的事件
        /// </summary>
        public override event EventHandler<SocketClientEventArgs> ClientConnected;

        /// <summary>
        /// 服务端断开和远程主机连接的事件
        /// </summary>
        public override event EventHandler<SocketClientEventArgs> ClientDisconnected;

        /// <summary>
        /// 服务端收到远程主机数据的事件
        /// </summary>
        public override event EventHandler<SocketClientDataEventArgs> ClientReceived;

        /// <summary>
        /// 服务端向远程主机发送数据的事件
        /// </summary>
        public override event EventHandler<SocketClientDataEventArgs> ClientSending;

        /// <summary>
        /// 服务端连接的远程主机出现异常的事件
        /// </summary>
        public override event EventHandler<SocketClientExceptionEventArgs> ClientException;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 使用指定的端口号初始化 <see cref="UdpServer"/> 类的新实例
        /// </summary>
        /// <param name="port">服务端使用的端口号</param>
        public UdpServer (int port)
            : this (IPAddress.Any, port)
        { }

        /// <summary>
        /// 使用指定的 IP 地址和端口号初始化 <see cref="UdpServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <exception cref="ArgumentNullException"><paramref name="localaddr"/> 是 <see langword="null"/></exception>
        public UdpServer (IPAddress localaddr, int port)
            : this (localaddr, port, new UdpOptions ())
        { }

        /// <summary>
        /// 使用指定的 IP 地址和端口号初始化 <see cref="UdpServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <param name="options">TCP 协议选项</param>
        /// <exception cref="ArgumentNullException"><paramref name="localaddr"/> 或 <paramref name="options"/> 是 <see langword="null"/></exception>
        public UdpServer (IPAddress localaddr, int port, UdpOptions options)
            : base (localaddr, port)
        {
            this.options = options ?? throw new ArgumentNullException (nameof (options));
            this.clients = new Collection<UdpClient> ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 服务端开始监听
        /// </summary>
        public override void Start ()
        {
            if (!this.isRunning)
            {
                this.isRunning = true;

                try
                {
                    // 创建监听套接字
                    this.server ??= new Socket (SocketType.Dgram, ProtocolType.Udp);

                    // 设置套接字选项
                    this.server.Bind (this.ListenerPoint);

                    // 开始监听
                    byte[] buffer = new byte[this.options.PacketMaxSize];
                    EndPoint remoteEP = this.ListenerPoint;
                    this.server.BeginReceiveFrom (buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP, this.SocketReceiveFromAsyncCallback, buffer);

                    // 触发事件
                    this.InvokeStartedEvent ();
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }

        /// <summary>
        /// 服务端停止监听
        /// </summary>
        public override void Stop ()
        {
            if (this.isRunning)
            {
                this.isRunning = false;

                try
                {
                    foreach (UdpClient client in this.clients)
                    {
                        client.Dispose ();
                    }
                    this.clients.Clear ();

                    // 触发事件
                    this.InvokeStoppedEvent ();
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
                finally
                {
                    // 关闭监听套接字
                    this.server?.Close ();
                    this.server?.Dispose ();
                    this.server = null;
                }
            }
        }
        #endregion

        #region --私有方法--
        /// <summary>
        /// 调用服务端启动事件
        /// </summary>
        private void InvokeStartedEvent ()
        {
            this.Started?.Invoke (this, new SocketEventArgs ());
        }

        /// <summary>
        /// 调用服务端停止事件
        /// </summary>
        private void InvokeStoppedEvent ()
        {
            this.Stopped?.Invoke (this, new SocketEventArgs ());
        }

        /// <summary>
        /// 调用服务端异常事件
        /// </summary>
        private void InvokeExceptionEvent (Exception exception)
        {
            this.Exception?.Invoke (this, new SocketExceptionEventArgs (exception));
        }

        /// <summary>
        /// 调用服务端收到远程主机数据的事件
        /// </summary>
        private void InvokeClientReceivedEvent (UdpClient client, byte[] data)
        {
            this.ClientReceived?.Invoke (this, new SocketClientDataEventArgs (client, data));
        }

        /// <summary>
        /// 调用服务端向远程主机发送数据的事件
        /// </summary>
        private void InvokeClientSendingEvent (UdpClient client, byte[] data)
        {
            this.ClientSending?.Invoke (this, new SocketClientDataEventArgs (client, data));
        }

        /// <summary>
        /// 调用服务端连接的远程主机出现异常的事件
        /// </summary>
        private void InvokeClientExceptionEvent (UdpClient client, Exception exception)
        {
            this.ClientException?.Invoke (this, new SocketClientExceptionEventArgs (client, exception));
        }

        private void SocketReceiveFromAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.isRunning)
            {
                byte[] buffer = ar.AsyncState as byte[];

                EndPoint remoteEP = new IPEndPoint (this.ListenerPoint.Address, this.ListenerPoint.Port);
                int len = this.server?.EndReceiveFrom (ar, ref remoteEP) ?? -1;
                if (len > 0)
                {
                    // 将数据复制到临时缓存
                    byte[] data = buffer.Left (len);

                    // 创建新客户端
                    UdpClient client = new (this.server, this.ListenerPoint, (IPEndPoint)remoteEP, this.options);
                    client.Received += this.SocketReceivedEventHandler;
                    client.Sending += this.SocketSendingEventHandler;
                    client.Exception += this.SocketExceptionEventHandler;
                    this.clients.Add (client);

                    // 触发事件
                    this.InvokeClientReceivedEvent (client, data);
                }
            }
        }

        private void SocketReceivedEventHandler (object sender, SocketDataEventArgs e)
        {
            if (sender is UdpClient client)
            {
                this.InvokeClientReceivedEvent (client, e.Data);
            }
        }

        private void SocketSendingEventHandler (object sender, SocketDataEventArgs e)
        {
            if (sender is UdpClient client)
            {
                this.InvokeClientSendingEvent (client, e.Data);
            }
        }

        private void SocketExceptionEventHandler (object sender, SocketExceptionEventArgs e)
        {
            if (sender is UdpClient client)
            {
                this.InvokeClientExceptionEvent (client, e.Exception);
            }
        }
        #endregion
    }
}
