using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

using JieRuntime.Rpc;

namespace JieRuntime.Ipc
{
    /// <summary>
    /// 提供远程调用服务客户端的基础类, 该类是抽象的
    /// </summary>
    public abstract class RpcServiceClientBase : ReadOnlyDictionary<string, RpcServiceInstance>
    {
        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否正在运行
        /// </summary>
        public abstract bool IsRunning { get; }

        /// <summary>
        /// 获取或设置客户端的等待响应的时间. 默认: 10秒. 值为 <see cref="TimeSpan.Zero"/> 时将无限等待远程对象返回
        /// </summary>
        public TimeSpan WaitResponseTime { get; set; } = TimeSpan.FromSeconds (10D);

        /// <summary>
        /// 获取远程调用服务代理处理器
        /// </summary>
        protected IRpcServiceProxy ProxyHandler { get; }
        #endregion

        #region --事件--
        /// <summary>
        /// 表示远程调用客户端成功连接到服务器的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceEventArgs> Connected;

        /// <summary>
        /// 表示远程调用客户端断开与远程服务器连接的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceEventArgs> Disconnected;

        /// <summary>
        /// 表示远程调用客户端出现异常的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceExceptionEventArgs> Exception;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcServiceClientBase"/> 类的新实例
        /// </summary>
        public RpcServiceClientBase ()
            : base (new Dictionary<string, RpcServiceInstance> ())
        { }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 注册远程调用服务实例
        /// </summary>
        /// <typeparam name="T">指定远程调用服务实例的接口类型</typeparam>
        /// <param name="obj">远程调用服务实例</param>
        /// <exception cref="ArgumentException">T 不是接口</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> 是 <see langword="null"/></exception>
        public void Register<T> (T obj)
            where T : class
        {
            if (obj is null)
            {
                throw new ArgumentNullException (nameof (obj));
            }

            Type type = typeof (T);
            if (!type.IsInterface)
            {
                throw new ArgumentException ($"类型: {type.Name} 不是接口", nameof (T));
            }

            if (!this.Dictionary.ContainsKey (type.Name))
            {
                this.Dictionary.Add (type.Name, new RpcServiceInstance (type, obj));
            }
        }

        /// <summary>
        /// 取消注册远程调用服务实例
        /// </summary>
        /// <typeparam name="T">指定远程调用服务实例的接口类型</typeparam>
        /// <exception cref="ArgumentException">T 不是接口</exception>
        public void Unregister<T> ()
            where T : class
        {
            Type type = typeof (T);
            if (!type.IsInterface)
            {
                throw new ArgumentException ($"类型: {type.Name} 不是接口", nameof (T));
            }

            if (this.Dictionary.ContainsKey (type.Name))
            {
                this.Dictionary.Remove (type.Name);
            }
        }

        /// <summary>
        /// 解析指定远程服务接口的代理接口
        /// </summary>
        /// <typeparam name="T">远程服务调用的接口</typeparam>
        /// <returns>实现了远程调用服务接口的代理类</returns>
        public T Resolver<T> ()
            where T : class
        {
            // 创建动态代理
            T proxyObj = DispatchProxy.Create<T, RpcServiceProxy> ();

            // 初始化代理拦截器
            if (proxyObj is RpcServiceProxy rpcServiceProxy)
            {
                rpcServiceProxy.ProxyHandler = this.GetProxyHandler ();
            }

            return proxyObj;
        }

        /// <summary>
        /// 连接到远程调用服务端
        /// </summary>
        public abstract void Connect ();

        /// <summary>
        /// 断开与远程调用服务端的连接
        /// </summary>
        public abstract void Disconnect ();

        /// <summary>
        /// 向远程调用服务端发送数据, 以响应远程调用服务端的请求
        /// </summary>
        /// <param name="tag">指定数据的唯一标识</param>
        /// <param name="data">要发送的数据</param>
        protected abstract byte[] SendWaitResponse (long tag, byte[] data);

        /// <summary>
        /// 向远程调用服务端发送数据, 以响应远程调用服务端的请求
        /// </summary>
        /// <param name="tag">指定数据的唯一标识</param>
        /// <param name="data">要发送的数据</param>
        protected abstract void SendResponse (long tag, byte[] data);

        /// <summary>
        /// 获取远程调用服务代理对象
        /// </summary>
        /// <returns>一个 <see cref="IRpcServiceProxy"/>, 用于处理远程服务调用代理</returns>
        protected abstract IRpcServiceProxy GetProxyHandler ();

        /// <summary>
        /// 客户端连接到远程服务器
        /// </summary>
        /// <param name="e">包含客户端连接到远程服务器的事件参数</param>
        protected virtual void OnConnected (RpcServiceEventArgs e)
        { }

        /// <summary>
        /// 客户端断开与远程服务器的连接
        /// </summary>
        /// <param name="e">包含客户端断开连接的事件参数</param>
        protected virtual void OnDisconnected (RpcServiceEventArgs e)
        { }

        /// <summary>
        /// 客户端异常
        /// </summary>
        /// <param name="e">包含客户端异常信息的事件数据</param>
        protected virtual void OnException (RpcServiceExceptionEventArgs e)
        { }
        #endregion
    }
}
