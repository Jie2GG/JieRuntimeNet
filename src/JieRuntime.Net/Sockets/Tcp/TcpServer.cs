using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace JieRuntime.Net.Sockets.Tcp
{
    /// <summary>
    /// 提供基于 TCP 协议的网络套接字服务端
    /// </summary>
    public class TcpServer : SocketServer<TcpClient>
    {
        #region --字段--
        private readonly TcpOptions options;
        private Socket server;
        private bool isRunning;
        private readonly Collection<TcpClient> clients;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前服务端是否正在运行
        /// </summary>
        public override bool IsRunning => this.isRunning;

        /// <summary>
        /// 获取当前服务端的客户端列表
        /// </summary>
        public override IReadOnlyCollection<TcpClient> Clients => this.clients;
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
        /// 使用指定的端口号初始化 <see cref="TcpServer"/> 类的新实例
        /// </summary>
        /// <param name="port">服务端使用的端口号</param>
        public TcpServer (int port)
            : this (IPAddress.Any, port)
        { }

        /// <summary>
        /// 使用指定的 IP 地址和端口号初始化 <see cref="TcpServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <exception cref="ArgumentNullException"><paramref name="localaddr"/> 是 <see langword="null"/></exception>
        public TcpServer (IPAddress localaddr, int port)
            : this (localaddr, port, new TcpOptions ())
        { }

        /// <summary>
        /// 使用指定的 IP 地址和端口号初始化 <see cref="TcpServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <param name="options">TCP 协议选项</param>
        /// <exception cref="ArgumentNullException"><paramref name="localaddr"/> 或 <paramref name="options"/> 是 <see langword="null"/></exception>
        public TcpServer (IPAddress localaddr, int port, TcpOptions options)
            : base (localaddr, port)
        {
            if (options is null)
            {
                throw new ArgumentNullException (nameof (options));
            }

            // 创建客户端列表
            this.clients = new Collection<TcpClient> ();
            this.options = options;
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
                    this.server ??= new Socket (SocketType.Stream, ProtocolType.Tcp);

                    // 设置套接字选项
                    this.server.Bind (this.ListenerPoint);
                    this.server.Listen (this.options.ListenSize);

                    // 开始监听
                    this.server.BeginAccept (this.SocketAcceptAsyncCallback, null);

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
            if (!this.isRunning)
            {
                this.isRunning = false;
            }

            try
            {
                // 关闭所有客户端套接字
                foreach (TcpClient client in this.clients)
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
        /// 调用服务端和远程主机建立连接事件
        /// </summary>
        private void InvokeClientConnectedEvent (TcpClient client)
        {
            this.ClientConnected?.Invoke (this, new SocketClientEventArgs (client));
        }

        /// <summary>
        /// 调用服务端断开和远程主机连接事件
        /// </summary>
        private void InvokeClientDisconnectedEvent (TcpClient client)
        {
            this.ClientDisconnected?.Invoke (this, new SocketClientEventArgs (client));
        }

        /// <summary>
        /// 调用服务端收到远程主机数据的事件
        /// </summary>
        private void InvokeClientReceivedEvent (TcpClient client, byte[] data)
        {
            this.ClientReceived?.Invoke (this, new SocketClientDataEventArgs (client, data));
        }

        /// <summary>
        /// 调用服务端向远程主机发送数据的事件
        /// </summary>
        private void InvokeClientSendingEvent (TcpClient client, byte[] data)
        {
            this.ClientSending?.Invoke (this, new SocketClientDataEventArgs (client, data));
        }

        /// <summary>
        /// 调用服务端连接的远程主机出现异常的事件
        /// </summary>
        private void InvokeClientExceptionEvent (TcpClient client, Exception exception)
        {
            this.ClientException?.Invoke (this, new SocketClientExceptionEventArgs (client, exception));
        }

        private void SocketAcceptAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.isRunning)
            {
                try
                {
                    // 接受客户端连接请求
                    Socket clientSocket = this.server?.EndAccept (ar);

                    if (clientSocket != null)
                    {
                        TcpClient client = new (clientSocket, this.options);
                        // 绑定事件
                        client.Disconnected += this.ClientDisconnectedEventhandler;
                        client.Received += this.ClientReceivedEventHandler;
                        client.Sending += this.ClientSendingEventHandler;
                        client.Exception += this.ClientExceptionEventHandler;

                        // 添加到客户端列表中
                        this.clients.Add (client);
                        // 触发事件
                        Task.Run (() => this.InvokeClientConnectedEvent (client));
                    }
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
                finally
                {
                    // 继续监听
                    if (this.isRunning)
                    {
                        this.server?.BeginAccept (this.SocketAcceptAsyncCallback, null);
                    }
                }
            }
        }

        private void ClientDisconnectedEventhandler (object sender, SocketEventArgs e)
        {
            if (sender is TcpClient client)
            {
                // 移除事件处理函数
                client.Disconnected -= this.ClientDisconnectedEventhandler;
                client.Received -= this.ClientReceivedEventHandler;
                client.Sending -= this.ClientSendingEventHandler;
                client.Exception -= this.ClientExceptionEventHandler;

                // 移除客户端
                this.clients.Remove (client);

                // 触发事件
                Task.Run (() => this.InvokeClientDisconnectedEvent (client));
            }
        }

        private void ClientReceivedEventHandler (object sender, SocketDataEventArgs e)
        {
            if (sender is TcpClient client)
            {
                this.InvokeClientReceivedEvent (client, e.Data);
            }
        }

        private void ClientSendingEventHandler (object sender, SocketDataEventArgs e)
        {
            if (sender is TcpClient client)
            {
                this.InvokeClientSendingEvent (client, e.Data);
            }
        }

        private void ClientExceptionEventHandler (object sender, SocketExceptionEventArgs e)
        {
            if (sender is TcpClient client)
            {
                this.InvokeClientExceptionEvent (client, e.Exception);
            }
        }
        #endregion
    }
}
