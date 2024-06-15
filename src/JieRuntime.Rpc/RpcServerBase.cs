using JieRuntime.Rpc.Tcp;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 提供远程调用服务端的基础类, 该类是抽象的
    /// </summary>
    public abstract class RpcServerBase<TClient> : RpcBase
        where TClient : RpcClientBase
    {
        #region --属性--
        /// <summary>
        /// 获取当前服务端的客户端列表
        /// </summary>
        public abstract IReadOnlyCollection<TClient> Clients { get; }
        #endregion

        #region --事件--
        /// <summary>
        /// 表示服务端启动的事件
        /// </summary>
        public abstract event EventHandler<RpcEventArgs> Started;

        /// <summary>
        /// 表示服务端停止的事件
        /// </summary>
        public abstract event EventHandler<RpcEventArgs> Stopped;

        /// <summary>
        /// 表示服务端异常的事件
        /// </summary>
        public abstract event EventHandler<RpcExceptionEventArgs> Exception;

        /// <summary>
        /// 服务端和远程主机建立连接的事件
        /// </summary>
        public abstract event EventHandler<RpcClientEventArgs> ClientConnected;

        /// <summary>
        /// 服务端断开和远程主机连接的事件
        /// </summary>
        public abstract event EventHandler<RpcClientEventArgs> ClientDisconnected;
        #endregion

        #region --公开方法--
        /// <summary>
        /// 启动远程调用服务端
        /// </summary>
        public abstract void Start ();

        /// <summary>
        /// 停止远程调用服务端
        /// </summary>
        public abstract void Stop ();

        /// <summary>
        /// 注册远程调用服务实例
        /// </summary>
        /// <param name="type">远程调用服务实例接口的类型</param>
        /// <param name="obj">远程服务调用实例</param>
        public override void Register (Type type, object obj)
        {
            // 先执行父方法的注册逻辑
            base.Register (type, obj);

            // 补充注册, 防止在客户端已连接后再注册导致客户端无法调用服务端的远程服务
            foreach (TClient client in this.Clients)
            {
                if (!client.IsRegister (type))
                {
                    client.Register (type, obj);
                }
            }
        }

        /// <summary>
        /// 取消注册远程调用服务实例
        /// </summary>
        /// <param name="type">远程调用服务实例接口的类型</param>
        public override void Unregister (Type type)
        {
            // 先执行父方法的取消注册逻辑
            base.Unregister (type);

            // 补充取消注册, 防止客户端还能继续调用已取消注册的远程服务
            foreach (TClient client in this.Clients)
            {
                if (client.IsRegister (type))
                {
                    client.Unregister (type);
                }
            }
        }

        /// <summary>
        /// 解析远程服务指定接口的代理
        /// </summary>
        /// <typeparam name="T">远程服务调用的接口</typeparam>
        /// <returns>实现了远程调用服务接口的代理</returns>
        public override T Resolver<T> ()
        {
            throw new InvalidOperationException ("尝试的操作无效, 代理实例无法选定客户端");
        }

        /// <summary>
        /// 每当调用代理类型上的任何方法时，都会调用此方法
        /// </summary>
        /// <param name="targetMethod">调用者调用的方法</param>
        /// <param name="args">调用者传递给方法的参数</param>
        /// <returns>返回给调用者的对象，void 方法将返回 <see langword="null"/></returns>
        public override object InvokeMethod (MethodInfo targetMethod, object[] args)
        {
            throw new InvalidOperationException ("尝试的操作无效, 代理实例无法选定客户端");
        }
        #endregion
    }
}
