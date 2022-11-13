using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 提供基于 TCP 协议的网络服务端
    /// </summary>
    public class TcpServer
    {
        #region --字段--
        private Socket server;
        private readonly Collection<SocketClient> clients;
        private volatile bool isRunning;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取基础网络套接字 <see cref="Socket"/>
        /// </summary>
        public Socket Server => this.server;

        /// <summary>
        /// 获取当前网络服务端监听的 IP 地址
        /// </summary>
        public IPEndPoint ListenerEndPoint { get; }

        /// <summary>
		/// 获取当前服务端是否正在运行
		/// </summary>
        public bool IsRunning => this.isRunning;

        /// <summary>
		/// 获取当前连接到服务端的客户端
		/// </summary>
        public IReadOnlyCollection<SocketClient> Clients => this.clients;

        /// <summary>
        /// 获取或设置当前服务端挂起连接队列的最大长度
        /// </summary>
        public int ListenBacklog { get; set; } = int.MaxValue;
        #endregion

        #region --事件--
        /// <summary>
        /// 表示服务端启动的事件
        /// </summary>
        public event EventHandler<SocketEventArgs> Started;

        /// <summary>
        /// 表示服务端停止的事件
        /// </summary>
        public event EventHandler<SocketEventArgs> Stopped;

        /// <summary>
        /// 表示服务端异常的事件
        /// </summary>
        public event EventHandler<SocketExceptionEventArgs> Exception;

        /// <summary>
        /// 表示有客户端连接到服务端的事件
        /// </summary>
        public event EventHandler<SocketClientInfoEventArgs> ClientConnected;

        /// <summary>
        /// 表示客户端断开连接服务端的事件
        /// </summary>
        public event EventHandler<SocketClientInfoEventArgs> ClientDisconnected;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="TcpServer"/> 类的新实例, 网络服务端将监听回环地址
        /// </summary>
        /// <param name="port">服务端使用的端口</param>
        public TcpServer (int port)
            : this (IPAddress.Any, port)
        { }

        /// <summary>
        /// 使用指定的 IP 地址和端口初始化 <see cref="TcpServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <exception cref="ArgumentNullException">localaddr 是 <see langword="null"/></exception>
        public TcpServer (IPAddress localaddr, int port)
        {
            if (localaddr is null)
            {
                throw new ArgumentNullException (nameof (localaddr));
            }

            // 监听地址
            this.ListenerEndPoint = new IPEndPoint (localaddr, port);

            // 创建客户端列表
            this.clients = new Collection<SocketClient> ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
		/// 启动服务端
		/// </summary>
        public void Start ()
        {
            if (!this.IsRunning)
            {
                this.isRunning = true;

                try
                {
                    // 创建套接字监听器
                    this.server ??= new Socket (SocketType.Stream, ProtocolType.Tcp);

                    // 绑定监听地址
                    this.Server.Bind (this.ListenerEndPoint);

                    // 设置监听队列
                    this.Server.Listen (this.ListenBacklog);

                    // 开始监听
                    this.Server.BeginAccept (this.AcceptAsyncCallback, null);

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
        /// 停止服务端
        /// </summary>
        public void Stop ()
        {
            if (this.IsRunning)
            {
                this.isRunning = false;

                try
                {
                    // 关闭套接字
                    this.Server.Close ();
                    this.server.Dispose ();

                    // 释放所有客户端
                    foreach (SocketClient client in this.clients)
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
                    this.server = null;
                }
            }
        }

        /// <summary>
        /// 释放当前实例所占用的资源
        /// </summary>
        public void Dispose ()
        {
            this.Stop ();
            GC.SuppressFinalize (this);
        }

        /// <summary>
        /// 服务端启动
        /// </summary>
        /// <param name="e">包含服务端启动的事件参数</param>
        protected virtual void OnStarted (SocketEventArgs e)
        { }

        /// <summary>
        /// 服务端停止
        /// </summary>
        /// <param name="e">包含服务端停止的事件参数</param>
        protected virtual void OnStopped (SocketEventArgs e)
        { }

        /// <summary>
        /// 服务端异常
        /// </summary>
        /// <param name="e">包含服务端异常的事件参数</param>
        protected virtual void OnException (SocketExceptionEventArgs e)
        { }

        /// <summary>
        /// 有客户端连接到服务端
        /// </summary>
        /// <param name="e">包含客户端的事件参数</param>
        protected virtual void OnClientConnected (SocketClientInfoEventArgs e)
        { }

        /// <summary>
        /// 客户端断开连接服务端
        /// </summary>
        /// <param name="e">包含客户端的事件参数</param>
        protected virtual void OnClientDisconnected (SocketClientInfoEventArgs e)
        { }
        #endregion

        #region --私有方法--
        private void InvokeStartedEvent ()
        {
            SocketEventArgs e = new ();
            this.OnStarted (e);
            this.Started?.Invoke (this, e);
        }

        private void InvokeStoppedEvent ()
        {
            SocketEventArgs e = new ();
            this.OnStopped (e);
            this.Stopped?.Invoke (this, e);
        }

        private void InvokeExceptionEvent (Exception exception)
        {
            SocketExceptionEventArgs e = new (exception);
            this.OnException (e);
            this.Exception?.Invoke (this, e);
        }

        private void InvokeClientConnectedEvent (SocketClient client)
        {
            SocketClientInfoEventArgs e = new (client);
            this.OnClientConnected (e);
            this.ClientConnected?.Invoke (this, e);
        }

        private void InvokeClientDisconnectedEvent (SocketClient client)
        {
            SocketClientInfoEventArgs e = new (client);
            this.OnClientDisconnected (e);
            this.ClientDisconnected?.Invoke (this, e);
        }

        // Socket异步监听回调函数
        private void AcceptAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.IsRunning)
            {
                try
                {
                    Socket socket = this.Server?.EndAccept (ar);

                    if (socket != null)
                    {
                        // 托管客户端Socket
                        TcpClient client = new (socket);
                        client.Disconnected += this.ClientDisconnectedEventHandler;

                        // 加入托管队列
                        this.clients.Add (client);

                        // 调用连接事件
                        this.InvokeClientConnectedEvent (client);
                    }
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
                finally
                {
                    // 继续等待连接
                    if (this.IsRunning)
                    {
                        this.Server?.BeginAccept (this.AcceptAsyncCallback, null);
                    }
                }
            }
        }

        // Socket客户端断开连接事件处理函数
        private void ClientDisconnectedEventHandler (object sender, SocketEventArgs e)
        {
            if (sender is SocketClient client)
            {
                // 移除事件处理函数
                client.Disconnected -= this.ClientDisconnectedEventHandler;

                // 移除客户端
                this.clients.Remove (client);

                // 触发事件
                this.InvokeClientDisconnectedEvent (client);
            }
        }
        #endregion
    }
}
