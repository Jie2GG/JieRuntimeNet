using System;
using System.Collections.Generic;
using System.Reflection;

using JieRuntime.Rpc.Attributes;
using JieRuntime.Rpc.Exceptions;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 提供远程调用服务基本功能的基础类, 该类是抽象的
    /// </summary>
    public abstract class RpcBase : IProxyHandler
    {
        #region --属性--
        /// <summary>
        /// 获取远程调用服务实例的集合
        /// </summary>
        protected Dictionary<string, RpcInstance> Services { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcBase"/> 类的新实例
        /// </summary>
        public RpcBase ()
        {
            this.Services = new Dictionary<string, RpcInstance> ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 注册远程调用服务实例
        /// </summary>
        /// <typeparam name="T">远程调用服务实例接口的类</typeparam>
        /// <param name="obj">远程服务调用实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RpcTypeFoundException"></exception>
        public virtual void Register<T> (T obj)
            where T : class
        {
            this.Register (typeof (T), obj);
        }

        /// <summary>
        /// 注册远程调用服务实例
        /// </summary>
        /// <param name="type">远程调用服务实例接口的类型</param>
        /// <param name="obj">远程服务调用实例</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RpcTypeFoundException"></exception>
        public virtual void Register (Type type, object obj)
        {
            if (type is null)
            {
                throw new ArgumentNullException (nameof (type));
            }

            if (obj is null)
            {
                throw new ArgumentNullException (nameof (obj));
            }

            if (!type.IsInterface)
            {
                throw new ArgumentException ($"类型 {type.FullName} 不是接口类型", nameof (obj));
            }

            // 检查是否存在远程类型特性, 如果存在则注册为指定名称的服务
            RemoteTypeAttribute remoteType = type.GetCustomAttribute<RemoteTypeAttribute> ();
            if (remoteType is not null)
            {
                if (this.Services.ContainsKey (remoteType.Name))
                {
                    throw new RpcTypeFoundException (remoteType.Name);
                }

                this.Services.Add (remoteType.Name, new RpcInstance (type, obj));
            }
            else
            {
                if (this.Services.ContainsKey (type.Name))
                {
                    throw new RpcTypeFoundException (type.Name);
                }

                this.Services.Add (type.Name, new RpcInstance (type, obj));
            }
        }

        /// <summary>
        /// 取消注册远程调用服务实例
        /// </summary>
        /// <typeparam name="T">远程调用服务实例接口的类</typeparam>
        /// <exception cref="ArgumentException"></exception>
        public virtual void Unregister<T> ()
            where T : class
        {
            this.Unregister (typeof (T));
        }

        /// <summary>
        /// 取消注册远程调用服务实例
        /// </summary>
        /// <param name="type">远程调用服务实例接口的类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public virtual void Unregister (Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException (nameof (type));
            }

            if (!type.IsInterface)
            {
                throw new ArgumentException ($"类型 {type.FullName} 不是接口类型");
            }

            // 首先检查类型是否存在特性
            RemoteTypeAttribute remoteType = type.GetCustomAttribute<RemoteTypeAttribute> ();
            if (remoteType is not null)
            {
                if (this.Services.ContainsKey (remoteType.Name))
                {
                    this.Services.Remove (remoteType.Name);
                }
            }
            else
            {
                if (this.Services.ContainsKey (type.Name))
                {
                    this.Services.Remove (type.Name);
                }
            }
        }

        /// <summary>
        /// 解析远程服务指定接口的代理
        /// </summary>
        /// <typeparam name="T">远程服务调用的接口</typeparam>
        /// <returns>实现了远程调用服务接口的代理</returns>
        public virtual T Resolver<T> ()
            where T : class
        {
            T proxyObj = DispatchProxy.Create<T, RpcDispatchProxy> ();
            if (proxyObj is RpcDispatchProxy proxy)
            {
                proxy.ProxyHandler = this;
            }
            return proxyObj;
        }

        /// <summary>
        /// 每当调用代理类型上的任何方法时，都会调用此方法
        /// </summary>
        /// <param name="targetMethod">调用者调用的方法</param>
        /// <param name="args">调用者传递给方法的参数</param>
        /// <returns>返回给调用者的对象，void 方法将返回 <see langword="null"/></returns>
        public abstract object InvokeMethod (MethodInfo targetMethod, object[] args);
        #endregion
    }
}
