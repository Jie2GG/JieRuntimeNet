using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using JieRuntime.Ipc;
using JieRuntime.Net.Sockets;

namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 基于 TCP 协议提供远程调用服务服务端
    /// </summary>
    public class RpcServiceServer : RpcServiceServerBase
    {
        #region --字段--
        private readonly Collection<RpcServiceClient> clients;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取基础网络服务端 <see cref="TcpServer"/>
        /// </summary>
        public TcpServer Server { get; }

        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前服务端是否正在运行
        /// </summary>
        public override bool IsRunning => this.Server.IsRunning;

        /// <summary>
        /// 获取已连接到服务端的客户端列表
        /// </summary>
        public IReadOnlyCollection<RpcServiceClient> Clients => this.clients;
        #endregion

        #region --事件--
        /// <summary>
        /// 表示服务端启动的事件
        /// </summary>
        public override event EventHandler<RpcServiceEventArgs> Started;

        /// <summary>
        /// 表示服务端停止的事件
        /// </summary>
        public override event EventHandler<RpcServiceEventArgs> Stopped;

        /// <summary>
        /// 表示服务端异常的事件
        /// </summary>
        public override event EventHandler<RpcServiceExceptionEventArgs> Exception;

        /// <summary>
        /// 表示有客户端连接到服务端的事件
        /// </summary>
        public override event EventHandler<RpcServiceClientInfoEventArgs> ClientConnected;

        /// <summary>
        /// 表示客户端断开连接服务端的事件
        /// </summary>
        public override event EventHandler<RpcServiceClientInfoEventArgs> ClientDisconnected;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcServiceServer"/> 类的新实例, 网络服务端将监听回环地址
        /// </summary>
        /// <param name="port">服务端使用的端口</param>
        public RpcServiceServer (int port)
            : this (IPAddress.Any, port)
        { }

        /// <summary>
        /// 使用指定的 IP 地址和端口初始化 <see cref="RpcServiceServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">本地 IP 地址</param>
        /// <param name="port">服务端使用的端口号</param>
        /// <exception cref="ArgumentNullException">localaddr 是 <see langword="null"/></exception>
        public RpcServiceServer (IPAddress localaddr, int port)
        {
            this.Server = new TcpServer (localaddr, port);
            this.Server.Started += this.ServerStartedEventHandler;
            this.Server.Stopped += this.ServerStoppedEventHandler;
            this.Server.Exception += this.ServerExceptionEventHandler;
            this.Server.ClientConnected += this.ClientConnectedEventHandler;

            this.clients = new Collection<RpcServiceClient> ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 启动远程调用服务端
        /// </summary>
        public override void Start ()
        {
            this.Server.Start ();
        }

        /// <summary>
        /// 停止远程调用服务端
        /// </summary>
        public override void Stop ()
        {
            this.Server.Stop ();
        }
        #endregion

        #region --私有方法--
        private void InvokeStartedEvent ()
        {
            RpcServiceEventArgs e = new ();
            this.OnStarted (e);
            Started?.Invoke (this, e);
        }

        private void InvokeStoppedEvent ()
        {
            RpcServiceEventArgs e = new ();
            this.OnStopped (e);
            Stopped?.Invoke (this, e);
        }

        private void InvokeExceptionEvent (Exception exception)
        {
            RpcServiceExceptionEventArgs e = new (exception);
            this.OnException (e);
            Exception?.Invoke (this, e);
        }

        private void InvokeClientConnectedEvent (RpcServiceClientBase client)
        {
            RpcServiceClientInfoEventArgs e = new (client);
            this.OnClientConnected (e);
            this.ClientConnected?.Invoke (this, e);
        }

        private void InvokeClientDisconnectedEvent (RpcServiceClientBase client)
        {
            RpcServiceClientInfoEventArgs e = new (client);
            this.OnClientDisconnected (e);
            this.ClientDisconnected?.Invoke (this, e);
        }

        private void ServerStartedEventHandler (object sender, SocketEventArgs e)
        {
            this.InvokeStartedEvent ();
        }

        private void ServerStoppedEventHandler (object sender, SocketEventArgs e)
        {
            this.InvokeStoppedEvent ();
        }

        private void ServerExceptionEventHandler (object sender, SocketExceptionEventArgs e)
        {
            this.InvokeExceptionEvent (e.Exception);
        }

        private void ClientConnectedEventHandler (object sender, SocketClientInfoEventArgs e)
        {
            if (e.Client is TcpClient client)
            {
                // 创建远程调用客户端
                RpcServiceClient rpcClient = new (client);

                // 挂接事件
                rpcClient.Disconnected += this.RpcServiceClientDisconnectedEventHandler;

                // 添加到托管列表
                this.clients.Add (rpcClient);

                // 触发事件
                this.InvokeClientConnectedEvent (rpcClient);
            }
        }

        private void RpcServiceClientDisconnectedEventHandler (object sender, RpcServiceEventArgs e)
        {
            if (sender is RpcServiceClient rpcClient)
            {
                rpcClient.Disconnected -= this.RpcServiceClientDisconnectedEventHandler;

                // 移出托管列表
                this.clients.Remove (rpcClient);

                // 触发事件
                this.InvokeClientDisconnectedEvent (rpcClient);
            }
        }


        #endregion
    }
}
