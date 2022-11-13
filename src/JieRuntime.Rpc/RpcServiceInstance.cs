using System;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示远程调用服务实例的结构
    /// </summary>
    public readonly struct RpcServiceInstance
    {
        #region --属性--
        /// <summary>
        /// 获取远程调用服务实例的接口类型
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// 获取远程调用服务实例的具体执行实例
        /// </summary>
        public object Instance { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcServiceInstance"/> 的新结构
        /// </summary>
        /// <param name="interfaceType">指定接口类型</param>
        /// <param name="instance">指定接口类型对应的实例</param>
        /// <exception cref="ArgumentNullException"><paramref name="interfaceType"/> 或 <paramref name="instance"/> 参数是 <see langword="null"/></exception>
        internal RpcServiceInstance (Type interfaceType, object instance)
        {
            this.InterfaceType = interfaceType ?? throw new ArgumentNullException (nameof (interfaceType));
            this.Instance = instance ?? throw new ArgumentNullException (nameof (instance));
        }
        #endregion
    }
}
