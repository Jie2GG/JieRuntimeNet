using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using JieRuntime.Extensions;
using JieRuntime.Utils;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 提供基于 TCP 协议的网络客户端
    /// </summary>
    public class TcpClient : SocketClient
    {
        #region --字段--
        private volatile Socket client;
        private volatile IPEndPoint localEndPoint;
        private volatile IPEndPoint remoteEndPoint;
        private volatile bool isRunning;
        private volatile bool isConnected;
        private volatile int packetSize;
        private volatile byte packetHeaderLength;
        private readonly TcpCache cache;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取基础网络套接字 <see cref="Socket"/>
        /// </summary>
        public override Socket Client => this.client;

        /// <summary>
		/// 获取当前 TCP 客户端的本地 IP 地址
		/// </summary>
        public override IPEndPoint LocalEndPoint => this.localEndPoint;

        /// <summary>
        /// 获取当前 TCP 客户端的远程 IP 地址
        /// </summary>
        public override IPEndPoint RemoteEndPoint => this.remoteEndPoint;

        /// <summary>
        /// 获取当前客户端是否正在运行
        /// </summary>
        public override bool IsRunning => this.isRunning;

        /// <summary>
        /// 获取当前客户端是否已连接到远程服务器
        /// </summary>
        public override bool IsConnected => this.isConnected;

        /// <summary>
        /// 获取或设置当前客户端接受或发送数据时的封包大小
        /// </summary>
        /// <exception cref="OutOfMemoryException">value 的值不能小于 1</exception>
        public override int PacketSize
        {
            get => this.packetSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException (nameof (value), value, "封包大小不能小于 1");
                }

                this.packetSize = value;

                long len = this.packetSize;
                byte size = 0;
                do
                {
                    len >>= 8;
                    size++;
                } while (len > 0);
                this.packetHeaderLength = size;
                if (this.cache != null)
                {
                    this.cache.PacketHeaderLength = this.PacketHeaderLength;
                }
            }
        }

        /// <summary>
        /// 获取当前客户端接收或发送数据时的封包头占用字节数
        /// </summary>
        public override byte PacketHeaderLength => this.packetHeaderLength;
        #endregion

        #region --事件--
        /// <summary>
        /// 表示客户端成功连接到远程客户端的事件
        /// </summary>
        public override event EventHandler<SocketEventArgs> Connected;

        /// <summary>
        /// 表示客户端断开与远程客户端连接的事件
        /// </summary>
        public override event EventHandler<SocketEventArgs> Disconnected;

        /// <summary>
        /// 表示客户端收到远程客户端数据的事件
        /// </summary>
        public override event EventHandler<SocketDataEventArgs> ReceiveData;

        /// <summary>
        /// 表示客户端发送数据到远程客户端的事件
        /// </summary>
        public override event EventHandler<SocketDataEventArgs> SendData;

        /// <summary>
        /// 表示客户端出现异常的事件
        /// </summary>
        public override event EventHandler<SocketExceptionEventArgs> Exception;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="TcpClient"/> 类的新实例
        /// </summary>
        public TcpClient ()
        {
            this.PacketSize = ushort.MaxValue;
            this.cache = new TcpCache (this.PacketHeaderLength);
        }

        /// <summary>
        /// 使用已初始化的 <see cref="Socket"/> 来初始化 <see cref="TcpClient"/> 类的新实例
        /// </summary>
        /// <param name="socket">已初始化的 <see cref="Socket"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="socket"/> 不能为 <see langword="null"/></exception>
        internal TcpClient (Socket socket)
            : this ()
        {
            if (socket is null)
            {
                throw new ArgumentNullException (nameof (socket));
            }

            this.client = socket;
            if (this.client.SocketType != SocketType.Stream || this.client.ProtocolType != ProtocolType.Tcp)
            {
                throw new SocketException ((int)SocketError.ProtocolType);
            }
            if (this.client.Connected)
            {
                // 设置当前客户端的状态
                this.isRunning = true;
                this.isConnected = true;
                this.localEndPoint = this.client.LocalEndPoint as IPEndPoint;
                this.remoteEndPoint = ReflectionUtils.GetFieldValue (this.client, "_rightEndPoint") as IPEndPoint;

                // 开始接收数据
                byte[] buffer = new byte[this.PacketSize];
                this.client.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, this.ReceiveAsyncCallback, buffer);
            }
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 连接远程客户端
        /// </summary>
        /// <param name="remoteEP">远程服务端点</param>
        /// <exception cref="ArgumentNullException"><paramref name="remoteEP"/> 是 <see langword="null"/></exception>
        public override void Connect (IPEndPoint remoteEP)
        {
            if (remoteEP is null)
            {
                throw new ArgumentNullException (nameof (remoteEP));
            }

            if (!this.IsRunning)
            {
                this.isRunning = true;
                try
                {
                    // 如果有必要, 初始化客户端
                    this.client ??= new Socket (remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    // 开始连接远程客户端
                    this.client.BeginConnect (remoteEP, this.ConnectAsyncCallback, null);
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }

        /// <summary>
        /// 断开连接远程客户端
        /// </summary>
        /// <param name="reuseClient">是否需要复用当前客户端</param>
        public override void Disconnect (bool reuseClient)
        {
            if (this.IsRunning)
            {
                this.isRunning = false;

                try
                {
                    // 关闭套接字
                    this.client?.Close ();

                    // 设置客户端状态
                    this.isConnected = false;

                    // 触发客户端断开事件
                    this.InvokeDisconnectedEvent ();
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
                finally
                {
                    // 释放套接字资源
                    this.client.Dispose ();

                    // 如果需要, 重置客户端
                    if (reuseClient)
                    {
                        this.client = null;
                    }
                }

            }
        }

        /// <summary>
        /// 发送数据到远程客户端
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> 是 <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> 的长度超过 <see cref="PacketSize"/></exception>
        public override void Send (byte[] data)
        {
            if (this.IsRunning && this.IsConnected)
            {
                if (data is null)
                {
                    throw new ArgumentNullException (nameof (data));
                }

                if (data.LongLength > (this.PacketSize - this.PacketHeaderLength))
                {
                    throw new ArgumentOutOfRangeException ($"要发送的数据包大小超过了上限: {this.PacketSize - this.PacketHeaderLength}", nameof (data));
                }

                try
                {
                    // 计算封包包头
                    byte[] lenBytes = BinaryConvert.GetBytes (data.LongLength + this.PacketHeaderLength, true);

                    // 填充包头
                    byte[] buf = lenBytes
                        .Right (this.PacketHeaderLength)    // 从右取指定长度的包头
                        .Concat (data);                      // 连接要发送的数据

                    // 发送数据
                    this.client?.BeginSend (buf, 0, buf.Length, SocketFlags.None, this.SendAsyncCallback, data);
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
            if (this.client != null)
            {
                this.client.Dispose ();
                GC.SuppressFinalize (this);
            }
        }
        #endregion

        #region --私有方法--
        private void InvokeConnectedEvent ()
        {
            SocketEventArgs e = new ();
            this.OnConnected (e);
            Connected?.Invoke (this, e);
        }

        private void InvokeDisconnectedEvent ()
        {
            SocketEventArgs e = new ();
            this.OnDisconnected (e);
            Disconnected?.Invoke (this, e);
        }

        private void InvokeReceiveDataEvent (byte[] data)
        {
            SocketDataEventArgs e = new (data);
            this.OnReceiveData (e);
            ReceiveData?.Invoke (this, e);
        }

        private void InvokeSendDataEvent (byte[] data)
        {
            SocketDataEventArgs e = new (data);
            this.OnSendData (e);
            SendData?.Invoke (this, e);
        }

        private void InvokeExceptionEvent (Exception exception)
        {
            SocketExceptionEventArgs e = new (exception);
            this.OnException (e);
            Exception?.Invoke (this, e);
        }

        // Socket异步连接回调函数
        private void ConnectAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.IsRunning)
            {
                try
                {
                    this.client?.EndConnect (ar);

                    // 修改客户端信息
                    this.isConnected = true;
                    this.localEndPoint = this.client.LocalEndPoint as IPEndPoint;
                    this.remoteEndPoint = ReflectionUtils.GetFieldValue (this.client, "_rightEndPoint") as IPEndPoint;

                    // 开始接收
                    if (this.IsRunning && this.IsConnected)
                    {
                        // 开始接收数据
                        byte[] buffer = new byte[this.PacketSize];
                        this.client?.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, this.ReceiveAsyncCallback, buffer);
                    }

                    // 触发事件
                    this.InvokeConnectedEvent ();
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }

        // Socket异步接收数据回调函数
        private void ReceiveAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.IsRunning && this.IsConnected)
            {
                byte[] buffer = ar.AsyncState as byte[];

                try
                {
                    // 接收远端数据
                    int? len = this.client?.EndReceive (ar, out SocketError errorCode);

                    // 对接收结果进行判断
                    if (this.IsRunning && this.IsConnected && len != null && len > 0)
                    {
                        // 复制数据到临时缓存
                        byte[] data = buffer.Left (len.Value);

                        // 封包处理
                        this.cache.Push (data);
                        while (this.cache.TryPull (out data))
                        {
                            // 异步启动数据接收事件
                            Task.Factory.StartNew ((data) => this.InvokeReceiveDataEvent (data as byte[]), data);
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
                    if (this.IsRunning && this.IsConnected)
                    {
                        buffer.Initialize ();
                        this.client?.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, this.ReceiveAsyncCallback, buffer);
                    }
                    else
                    {
                        this.Disconnect (true);
                    }
                }
            }
        }

        // Socket异步发送数据回调函数
        private void SendAsyncCallback (IAsyncResult ar)
        {
            if (ar.IsCompleted && this.IsRunning && this.IsConnected)
            {
                try
                {
                    // 获取数据
                    byte[] buf = ar.AsyncState as byte[];

                    // 获取发送结果
                    int? len = this.client?.EndSend (ar, out SocketError errorCode);
                    if (len != null && len > 0)
                    {
                        // 触发数据送达事件
                        this.InvokeSendDataEvent (buf);
                    }
                }
                catch (Exception e)
                {
                    this.InvokeExceptionEvent (e);
                }
            }
        }
        #endregion

        #region --内部类--
        /// <summary>
        /// 提供 TCP 协议网络客户端使用的缓冲区
        /// </summary>
        private class TcpCache
        {
            #region --字段--
            private readonly List<byte> data;
            #endregion

            #region --属性--
            public int PacketHeaderLength { get; set; }
            #endregion

            #region --构造函数--
            /// <summary>
            /// 初始化 <see cref="TcpCache"/> 类的新实例
            /// </summary>
            /// <param name="packetHeaderLength">指定封包包头长度</param>
            public TcpCache (int packetHeaderLength)
            {
                this.data = new List<byte> ();
                this.PacketHeaderLength = packetHeaderLength;
            }
            #endregion

            #region --公开方法--
            /// <summary>
            /// 将数据推入缓冲区
            /// </summary>
            /// <param name="data">要推入的数据</param>
            public void Push (byte[] data)
            {
                this.data.AddRange (data);
            }

            /// <summary>
            /// 尝试拉取缓冲区中的完整数据包, 如果缓冲区中的数据可以形成完整数据包, 则返回完整的数据包
            /// </summary>
            /// <param name="data">存放拉取的数据包</param>
            /// <returns>如果缓冲区的数据可以形成完整的数据包, 则为 <see langword="true"/>; 否则为 <see langword="false"/></returns>
            public bool TryPull (out byte[] data)
            {
                data = null;

                if (this.data.Count >= this.PacketHeaderLength)
                {
                    // 读取封包包头
                    byte[] lenBytes = new byte[this.PacketHeaderLength];
                    for (int i = 0; i < this.PacketHeaderLength; i++)
                    {
                        lenBytes[i] = this.data[i];
                    }
                    int len = BinaryConvert.ToInt32 (lenBytes, true);

                    // 读取数据
                    if (this.data.Count >= len)
                    {
                        // 去除自身长度
                        len -= this.PacketHeaderLength;

                        // 组合数据包
                        data = new byte[len];
                        this.data.CopyTo (this.PacketHeaderLength, data, 0, data.Length);

                        // 清理缓冲区
                        this.data.RemoveRange (0, len + this.PacketHeaderLength);

                        return true;
                    }
                }

                return false;
            }
            #endregion
        }
        #endregion
    }
}
