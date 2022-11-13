using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using JieRuntime.Extensions;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 提供基于 UDP 协议的网络服务端
    /// </summary>
    public partial class UdpServer
    {
        #region --常量--
        /// <summary>
        /// 表示数据报最大数据字节数
        /// </summary>
        public const int MaxPacketSize = 65507;
        #endregion

        #region --字段--
        private volatile Socket server;
        private volatile bool isRunning;
        private volatile int packetSize;
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
        /// 获取或设置当前客户端接受或发送数据时的封包大小
        /// </summary>
        public int PacketSize
        {
            get => this.packetSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException (nameof (value), value, "数据报大小不能小于 1");
                }

                if (value > MaxPacketSize)
                {
                    throw new ArgumentOutOfRangeException (nameof (value), value, $"数据报大小不能超过 65507");
                }
                this.packetSize = value;
            }
        }
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
        /// 表示服务端收到数据的事件
        /// </summary>
        public event EventHandler<SocketUdpDataEventArgs> ReceiveData;

        /// <summary>
        /// 表示服务端发送数据的事件
        /// </summary>
        public event EventHandler<SocketUdpDataEventArgs> SendData;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="UdpServer"/> 类的新实例, 网络服务端将监听回环地址
        /// </summary>
        /// <param name="port">服务端使用的端口</param>
        public UdpServer (int port)
            : this (IPAddress.Loopback, port)
        { }

        /// <summary>
        /// 使用指定的 IP 地址和端口初始化 <see cref="UdpServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <exception cref="ArgumentNullException">localaddr 是 <see langword="null"/></exception>
        public UdpServer (IPAddress localaddr, int port)
        {
            if (localaddr is null)
            {
                throw new ArgumentNullException (nameof (localaddr));
            }

            // 监听地址
            this.ListenerEndPoint = new IPEndPoint (localaddr, port);
            this.PacketSize = MaxPacketSize;
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
                    this.server ??= new Socket (SocketType.Dgram, ProtocolType.Udp);

                    // 绑定监听地址
                    this.Server?.Bind (this.ListenerEndPoint);

                    // 开始接收数据
                    UdpState state = UdpState.Create (this.PacketSize);
                    this.Server?.BeginReceiveFrom (state.Data, 0, state.Data.Length, SocketFlags.None, ref state.RemoteEndPoint, this.ReceiveFromAsyncCallback, state);

                    // 调用事件
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
        /// 向客户端发送数据
        /// </summary>
        /// <param name="remote">指定数据送达的客户端端口</param>
        /// <param name="data">要发送的数据</param>
        /// <exception cref="ArgumentNullException"><paramref name="remote"/> 和 <paramref name="data"/> 不能为 <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> 的长度超过 <see cref="PacketSize"/></exception>
        public void Send (EndPoint remote, byte[] data)
        {
            if (this.IsRunning)
            {
                if (remote is null)
                {
                    throw new ArgumentNullException (nameof (remote));
                }

                if (data is null)
                {
                    throw new ArgumentNullException (nameof (data));
                }

                if (data.Length > this.PacketSize)
                {
                    throw new ArgumentException ($"要发送的数据包大小超过了上限: {this.PacketSize}", nameof (data));
                }

                try
                {
                    UdpState state = UdpState.Create (remote, data);

                    // 发送数据
                    this.Server?.BeginSendTo (state.Data, 0, state.Data.Length, SocketFlags.None, state.RemoteEndPoint, this.SendToAsyncCallback, state);
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
        /// 服务端收到数据
        /// </summary>
        /// <param name="e">包含服务端收到数据的事件参数</param>
        protected virtual void OnReceiveData (SocketUdpDataEventArgs e)
        { }

        /// <summary>
        /// 服务端发送数据
        /// </summary>
        /// <param name="e">包含服务端发送数据的事件参数</param>
        protected virtual void OnSendData (SocketUdpDataEventArgs e)
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

        private void InvokeReceiveDataEvent (UdpState state)
        {
            SocketUdpDataEventArgs e = new (state);
            this.OnReceiveData (e);
            this.ReceiveData?.Invoke (this, e);
        }

        private void InvokeSendDataEvent (UdpState state)
        {
            SocketUdpDataEventArgs e = new (state);
            this.OnSendData (e);
            this.SendData?.Invoke (this, e);
        }

        // Socket异步接收数据回调函数
        private void ReceiveFromAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.IsRunning)
            {
                if (ar.AsyncState is UdpState state)
                {
                    try
                    {
                        int? len = this.Server?.EndReceiveFrom (ar, ref state.RemoteEndPoint);
                        if (this.IsRunning && len != null && len > 0)
                        {
                            // 处理数据
                            state.Data = state.Data
                                .Left (len.Value);

                            // 异步启动数据接收事件
                            Task.Factory.StartNew ((data) =>
                            {
                                if (data is UdpState udpState)
                                {
                                    this.InvokeReceiveDataEvent (udpState);
                                }
                            }, state);
                        }
                    }
                    catch (Exception e)
                    {
                        this.InvokeExceptionEvent (e);
                    }
                    finally
                    {
                        if (this.IsRunning)
                        {
                            UdpState nextState = UdpState.Create (this.PacketSize);
                            this.Server?.BeginReceiveFrom (nextState.Data, 0, nextState.Data.Length, SocketFlags.None, ref nextState.RemoteEndPoint, this.ReceiveFromAsyncCallback, nextState);
                        }
                    }
                }
            }
        }

        // Socket异步发送数据回调函数
        private void SendToAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.IsRunning)
            {
                try
                {
                    if (ar.AsyncState is UdpState state)
                    {
                        int? len = this.Server?.EndSendTo (ar);
                        if (len != null && len > 0)
                        {
                            // 触发事件
                            this.InvokeSendDataEvent (state);
                        }
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
