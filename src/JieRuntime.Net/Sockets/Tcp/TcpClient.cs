using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using JieRuntime.Extensions;
using JieRuntime.Utils;

namespace JieRuntime.Net.Sockets.Tcp
{
    /// <summary>
    /// 提供基于 TCP 协议的网络套接字客户端
    /// </summary>
    public class TcpClient : SocketClient
    {
        #region --字段--
        private readonly TcpOptions options;
        private readonly TcpCache cache;
        private Socket client;
        private bool isRunning;
        private bool isConncted;
        private IPEndPoint localEndPoint;
        private IPEndPoint remoteEndPoint;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否正在运行
        /// </summary>
        public override bool IsRunning => this.isRunning;

        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否已连接到远程主机
        /// </summary>
        public override bool IsConnected => this.isConncted;

        /// <summary>
        /// 获取当前客户端的本地端点
        /// </summary>
        public override IPEndPoint LocalEndPoint => this.localEndPoint;

        /// <summary>
        /// 获取当前客户端连接的远程端点
        /// </summary>
        public override IPEndPoint RemoteEndPoint => this.remoteEndPoint;
        #endregion

        #region --事件--
        /// <summary>
        /// 客户端连接远程主机事件
        /// </summary>
        public override event EventHandler<SocketEventArgs> Connected;

        /// <summary>
        /// 客户端断开远程主机连接事件
        /// </summary>
        public override event EventHandler<SocketEventArgs> Disconnected;

        /// <summary>
        /// 客户端出现异常的事件
        /// </summary>
        public override event EventHandler<SocketExceptionEventArgs> Exception;

        /// <summary>
        /// 客户端接收到远程主机数据的事件
        /// </summary>
        public override event EventHandler<SocketDataEventArgs> Received;

        /// <summary>
        /// 客户端发送数据到远程主机的事件
        /// </summary>
        public override event EventHandler<SocketDataEventArgs> Sending;
        #endregion

        #region --构造函数--
        /// <summary>
        ///  初始化 <see cref="TcpClient"/> 类的新实例
        /// </summary>
        public TcpClient ()
            : this (new TcpOptions ())
        { }

        /// <summary>
        /// 使用 TCP 协议选项来初始化 <see cref="TcpClient"/> 类的新实例
        /// </summary>
        /// <param name="options">TCP 协议选项</param>
        /// <exception cref="ArgumentNullException">参数: <paramref name="options"/> 是 <see langword="null"/></exception>
        public TcpClient (TcpOptions options)
        {
            this.options = options ?? throw new ArgumentNullException (nameof (options));

            this.cache = new TcpCache (this.options.PacketHeaderBytesSize);
        }

        /// <summary>
        /// 使用已初始化的 <see cref="Socket"/> 来初始化 <see cref="TcpClient"/> 类的新实例
        /// </summary>
        /// <param name="socket">已初始化的 <see cref="Socket"/></param>
        /// <param name="options">TCP 协议选项</param>
        /// <exception cref="ArgumentNullException"><paramref name="socket"/> 不能为 <see langword="null"/></exception>
        public TcpClient (Socket socket, TcpOptions options)
            : this (options)
        {
            this.client = socket ?? throw new ArgumentNullException (nameof (socket));
            this.options = options ?? throw new ArgumentNullException (nameof (options));

            if (this.client.SocketType != SocketType.Stream || this.client.ProtocolType != ProtocolType.Tcp)
            {
                throw new SocketException ((int)SocketError.ProtocolType);
            }

            if (this.client.Connected)
            {
                this.isRunning = true;
                this.isConncted = true;
                this.localEndPoint = this.client.LocalEndPoint as IPEndPoint;
                this.remoteEndPoint = ReflectionUtils.GetFieldValue (this.client, "_rightEndPoint") as IPEndPoint;

                //开始接收数据
                byte[] buffer = new byte[this.options.PacketMaxSize];
                this.client.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, this.SocketReceiveAsyncCallback, buffer);
            }
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 开始对远程主机连接
        /// </summary>
        /// <param name="remoteEP">表示远程设备的 <see cref="IPEndPoint"/></param>
        public override void Connect (IPEndPoint remoteEP)
        {
            if (remoteEP is null)
            {
                throw new ArgumentNullException (nameof (remoteEP));
            }

            if (!this.isRunning)
            {
                this.isRunning = true;
                try
                {
                    this.remoteEndPoint = remoteEP;
                    this.client ??= new Socket (remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    // 开始连接远程主机
                    this.client.BeginConnect (remoteEP, this.SocketConnectAsyncCallback, this.client);
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }

        /// <summary>
        /// 断开远程主机连接
        /// </summary>
        /// <param name="reuseSocket">是否需要重用套接字</param>
        public override void Disconnect (bool reuseSocket)
        {
            if (this.isRunning)
            {
                this.isRunning = false;

                try
                {
                    this.client?.Close ();
                    this.isConncted = false;

                    // 触发事件
                    Task.Run (() => this.InvokeDisconnectedEvent ());
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
                finally
                {
                    // 释放并重置客户端
                    this.Dispose ();
                    if (reuseSocket)
                    {
                        this.client = null;
                    }
                }
            }
        }

        /// <summary>
        /// 向已连接的远程主机发送数据
        /// </summary>
        /// <param name="data"><see cref="byte"/> 类型的数组，包含要发送的数据</param>
        public override void Send (byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException (nameof (data));
            }

            if (data.Length > this.options.PacketMaxSize - this.options.PacketHeaderBytesSize)
            {
                throw new ArgumentOutOfRangeException ($"数据大小超过设定的上限: {this.options.PacketMaxSize - this.options.PacketHeaderBytesSize}", nameof (data));
            }

            if (this.isRunning && this.isConncted)
            {
                try
                {
                    // 计算封包包头
                    byte[] lenBytes = BinaryConvert.GetBytes (data.Length + this.options.PacketHeaderBytesSize, true);

                    // 填充包头
                    byte[] buf = lenBytes
                        .Right (this.options.PacketHeaderBytesSize)    // 从右取指定长度的包头
                        .Concat (data);                                 // 连接要发送的数据

                    // 发送数据
                    this.client?.BeginSend (buf, 0, buf.Length, SocketFlags.None, this.SocketSendAsyncCallback, data);
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }

        /// <summary>
        /// 释放当前实例所占用的资源
        /// </summary>
        public override void Dispose ()
        {
            this.Disconnect (false);
            this.client.Dispose ();
        }
        #endregion

        #region --私有方法--
        /// <summary>
        /// 调用连接成功事件
        /// </summary>
        private void InvokeConnectedEvent ()
        {
            this.Connected?.Invoke (this, new SocketEventArgs ());
        }

        /// <summary>
        /// 调用断开连接事件
        /// </summary>
        private void InvokeDisconnectedEvent ()
        {
            this.Disconnected?.Invoke (this, new SocketEventArgs ());
        }

        /// <summary>
        /// 调用异常事件
        /// </summary>
        private void InvokeExceptionEvent (Exception exception)
        {
            this.Exception?.Invoke (this, new SocketExceptionEventArgs (exception));
        }

        /// <summary>
        /// 调用收到数据事件
        /// </summary>
        private void InvokeReceivedEvent (byte[] data)
        {
            this.Received?.Invoke (this, new SocketDataEventArgs (data));
        }

        /// <summary>
        /// 调用数据发送事件
        /// </summary>
        private void InvokeSendingEvent (byte[] data)
        {
            this.Sending?.Invoke (this, new SocketDataEventArgs (data));
        }

        private void SocketConnectAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.isRunning)
            {
                try
                {
                    this.client?.EndConnect (ar);
                    this.isConncted = true;
                    this.localEndPoint = this.client.LocalEndPoint as IPEndPoint;

                    // 触发事件
                    Task.Run (() => this.InvokeConnectedEvent ());

                    // 开始接收数据
                    if (this.isRunning && this.isConncted)
                    {
                        byte[] buffer = new byte[this.options.PacketMaxSize];
                        this.client?.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, this.SocketReceiveAsyncCallback, buffer);
                    }
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }

        private void SocketReceiveAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.isRunning && this.isConncted)
            {
                byte[] buffer = ar.AsyncState as byte[];

                try
                {
                    // 完成数据接收
                    int len = this.client?.EndReceive (ar) ?? -1;

                    // 判断结果
                    if (this.isRunning && this.isConncted && len > 0)
                    {
                        // 将数据复制到临时缓存
                        byte[] data = buffer.Left (len);

                        if (this.options.PacketHeaderBytesSize > 0)
                        {
                            // 利用缓存处理封包
                            this.cache.Push (data);
                            while (true)
                            {
                                byte[] temp = this.cache.Pull ();
                                if (temp is null)
                                {
                                    break;
                                }
                                Task.Run (() => this.InvokeReceivedEvent (temp));
                            }

                        }
                        else
                        {
                            // 如果没有包头长度, 则不启动粘包处理
                            Task.Run (() => this.InvokeReceivedEvent (data));
                        }
                    }
                    else
                    {
                        this.Disconnect (true);
                    }

                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
                finally
                {
                    // 继续接收
                    if (this.isRunning && this.isConncted)
                    {
                        buffer.Initialize ();
                        this.client?.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, this.SocketReceiveAsyncCallback, buffer);
                    }
                    else
                    {
                        this.Disconnect (true);
                    }
                }
            }
        }

        private void SocketSendAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.isRunning && this.isConncted)
            {
                try
                {
                    // 获取数据
                    byte[] data = ar.AsyncState as byte[];

                    // 获取发送结果
                    int len = this.client?.EndSend (ar) ?? -1;
                    if (len > 0)
                    {
                        // 触发数据送达事件
                        Task.Run (() => this.InvokeSendingEvent (data.Left (len)));
                    }
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }
        #endregion
    }
}
