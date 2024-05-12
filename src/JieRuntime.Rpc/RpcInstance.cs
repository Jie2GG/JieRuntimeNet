using System;
using System.Collections.Generic;
using System.Reflection;

using JieRuntime.Rpc.Attributes;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示远程调用服务实例的类
    /// </summary>
    public readonly struct RpcInstance
    {
        #region --属性--
        /// <summary>
        /// 获取实例的接口类型
        /// </summary>
        public Type InstanceType { get; }

        /// <summary>
        /// 获取接口实例对应的实例对象
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// 获取实例对象的所有方法
        /// </summary>
        public IReadOnlyDictionary<string, List<MethodInfo>> Methods { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcInstance"/> 的新实例
        /// </summary>
        /// <param name="instanceType">实例的类型</param>
        /// <param name="instance">实例对象</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public RpcInstance (Type instanceType, object instance)
        {
            this.InstanceType = instanceType ?? throw new ArgumentNullException (nameof (instanceType));
            this.Instance = instance ?? throw new ArgumentNullException (nameof (instance));

            if (!instanceType.IsInterface)
            {
                throw new ArgumentException ("实例类型必须是接口类型", nameof (instanceType));
            }

            // instance 必须继承自 instanceType
            if (!instanceType.IsAssignableFrom (instance.GetType ()))
            {
                throw new ArgumentException ($"实例类型必须继承自 {instanceType.FullName}", nameof (instanceType));
            }

            Dictionary<string, List<MethodInfo>> methods = new ();
            foreach (MethodInfo methodInfo in instanceType.GetMethods ())
            {
                RemoteMethodAttribute remoteMethod = methodInfo.GetCustomAttribute<RemoteMethodAttribute> ();
                if (remoteMethod is not null)
                {
                    if (!methods.ContainsKey (remoteMethod.Name))
                    {
                        methods.Add (remoteMethod.Name, new List<MethodInfo> ());
                    }

                    methods[remoteMethod.Name].Add (methodInfo);
                }
                else
                {
                    if (!methods.ContainsKey (methodInfo.Name))
                    {
                        methods.Add (methodInfo.Name, new List<MethodInfo> ());
                    }

                    methods[methodInfo.Name].Add (methodInfo);
                }
            }
            this.Methods = methods;
        }
        #endregion
    }
}
