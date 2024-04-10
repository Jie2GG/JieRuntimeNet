using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using JieRuntime.Extensions;

namespace JieRuntime.Net.Sockets.Udp
{
    /// <summary>
    /// 提供基于 UDP 协议的网络套接字客户端
    /// </summary>
    public class UdpClient : SocketClient
    {
        #region --字段--
        private readonly UdpOptions options;
        private Socket client;
        private bool isRunning;
        private IPEndPoint localEndPoint;
        private EndPoint remoteEndPoint;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否正在运行
        /// </summary>
        public override bool IsRunning => this.isRunning;

        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否已连接到远程主机
        /// </summary>
        public override bool IsConnected => false;

        /// <summary>
        /// 获取当前客户端的本地端点
        /// </summary>
        public override IPEndPoint LocalEndPoint => this.localEndPoint;

        /// <summary>
        /// 获取当前客户端连接的远程端点
        /// </summary>
        public override IPEndPoint RemoteEndPoint => this.remoteEndPoint as IPEndPoint;
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
        /// 初始化 <see cref="UdpClient"/> 类的新实例
        /// </summary>
        public UdpClient ()
            : this (new UdpOptions ())
        { }

        /// <summary>
        /// 初始化 <see cref="UdpClient"/> 类的新实例
        /// </summary>
        /// <param name="options">UDP 协议选项</param>
        public UdpClient (UdpOptions options)
        {
            this.options = options ?? throw new ArgumentNullException (nameof (options));
        }

        internal UdpClient (Socket socket, IPEndPoint localEP, IPEndPoint remoteEP, UdpOptions options)
            : this (options)
        {
            this.client = socket;
            this.localEndPoint = localEP;
            this.remoteEndPoint = remoteEP;
            this.isRunning = true;

            // 开始接收数据
            byte[] buffer = new byte[this.options.PacketMaxSize];
            this.client?.BeginReceiveFrom (buffer, 0, buffer.Length, SocketFlags.None, ref this.remoteEndPoint, this.SocketReceiveFromAsyncCallback, buffer);
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
                    // 如果有必要, 初始化客户端
                    this.client ??= new Socket (remoteEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                    // 设置客户端状态
                    this.remoteEndPoint = remoteEP;

                    // 开始连接远程客户端
                    this.client?.BeginConnect (remoteEP, this.ConnectAsyncCallback, null);
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

            if (this.isRunning)
            {
                try
                {
                    // 发送数据
                    this.client?.BeginSend (data, 0, data.Length, SocketFlags.None, this.SocketSendAsyncCallback, data);
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

        private void ConnectAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.isRunning)
            {
                try
                {
                    this.client?.EndConnect (ar);
                    this.localEndPoint = this.client.LocalEndPoint as IPEndPoint;

                    // 触发事件
                    Task.Run (() => this.InvokeConnectedEvent ());

                    // 开始接收数据
                    if (this.isRunning)
                    {
                        byte[] buffer = new byte[this.options.PacketMaxSize];
                        this.client?.BeginReceiveFrom (buffer, 0, buffer.Length, SocketFlags.None, ref this.remoteEndPoint, this.SocketReceiveFromAsyncCallback, buffer);
                    }
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }

        private void SocketReceiveFromAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.isRunning)
            {
                byte[] buffer = ar.AsyncState as byte[];
                try
                {
                    int len = this.client?.EndReceiveFrom (ar, ref this.remoteEndPoint) ?? -1;
                    // 判断结果
                    if (this.isRunning && len > 0)
                    {
                        // 将数据复制到临时缓存
                        byte[] data = buffer.Left (len);
                        Task.Run (() => this.InvokeReceivedEvent (data));
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
                    if (this.isRunning)
                    {
                        buffer.Initialize ();
                        this.client?.BeginReceiveFrom (buffer, 0, buffer.Length, SocketFlags.None, ref this.remoteEndPoint, this.SocketReceiveFromAsyncCallback, buffer);
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
            if (ar.IsCompleted && this.isRunning)
            {
                try
                {
                    // 获取数据
                    byte[] buf = ar.AsyncState as byte[];

                    // 获取发送结果
                    int len = this.client?.EndSend (ar) ?? -1;
                    if (len > 0)
                    {
                        // 触发数据送达事件
                        this.InvokeSendingEvent (buf.Left (len));
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
