using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;

using JieRuntime.Net.Sockets;
using JieRuntime.Net.Sockets.Tcp;

namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 提供基于 TCP 协议的远程调用服务端类
    /// </summary>
    public class RpcServer : RpcServerBase<RpcClient>
    {
        #region --字段--
        private readonly Collection<RpcClient> clients;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取基础网络客户端 <see cref="TcpServer"/>
        /// </summary>
        public TcpServer Server { get; }

        /// <summary>
        /// 获取当前服务端的客户端列表
        /// </summary>
        public override IReadOnlyCollection<RpcClient> Clients => this.clients;
        #endregion

        #region --事件--
        /// <summary>
        /// 表示服务端启动的事件
        /// </summary>
        public override event EventHandler<RpcEventArgs> Started;

        /// <summary>
        /// 表示服务端停止的事件
        /// </summary>
        public override event EventHandler<RpcEventArgs> Stopped;

        /// <summary>
        /// 表示服务端异常的事件
        /// </summary>
        public override event EventHandler<RpcExceptionEventArgs> Exception;

        /// <summary>
        /// 服务端和远程主机建立连接的事件
        /// </summary>
        public override event EventHandler<RpcClientEventArgs> ClientConnected;

        /// <summary>
        /// 服务端断开和远程主机连接的事件
        /// </summary>
        public override event EventHandler<RpcClientEventArgs> ClientDisconnected;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcServer"/> 类的新实例
        /// </summary>
        /// <param name="port"></param>
        public RpcServer (int port)
            : this (IPAddress.Any, port)
        { }

        /// <summary>
        /// 初始化 <see cref="RpcServer"/> 类的新实例
        /// </summary>
        /// <param name="localaddr">指定监听的地址</param>
        /// <param name="port">监听的端口号</param>
        public RpcServer (IPAddress localaddr, int port)
        {
            this.Server = new TcpServer (localaddr, port);
            this.Server.Started += this.ServerStartedEventHandler;
            this.Server.Stopped += this.ServerStoppedEventHandler;
            this.Server.Exception += this.ServerExceptionEventHandler;
            this.Server.ClientConnected += this.ServerClientConnectedEventHandler;

            this.clients = new Collection<RpcClient> ();
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
            this.Started?.Invoke (this, new RpcEventArgs ());
        }

        private void InvokeStoppedEvent ()
        {
            this.Stopped?.Invoke (this, new RpcEventArgs ());
        }

        private void InvokeExceptionEvent (Exception exception)
        {
            this.Exception?.Invoke (this, new RpcExceptionEventArgs (exception));
        }

        private void InvokeClientConnectedEvent (RpcClient client)
        {
            this.ClientConnected?.Invoke (this, new RpcClientEventArgs (client));
        }

        private void InvokeClientDisconnectedEvent (RpcClient client)
        {
            this.ClientDisconnected?.Invoke (this, new RpcClientEventArgs (client));
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

        private void ServerClientConnectedEventHandler (object sender, SocketClientEventArgs e)
        {
            // 创建远程调用客户端
            RpcClient client = new ((TcpClient)e.Client);
            client.Disconnected += this.ClientDisconnectedEventHandler;

            // 向客户端注册所有服务
            foreach (RpcInstance instance in this.Services.Values)
            {
                client.Register (instance.InstanceType, instance.Instance);
            }

            // 托管客户端
            this.clients.Add (client);
            this.InvokeClientConnectedEvent (client);
        }

        private void ClientDisconnectedEventHandler (object sender, RpcEventArgs e)
        {
            if (sender is RpcClient client)
            {
                client.Disconnected -= this.ClientDisconnectedEventHandler;

                // 移除托管客户端
                this.clients.Remove (client);
                this.InvokeClientDisconnectedEvent (client);
            }
        }
        #endregion
    }
}
