using System.Reflection;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示远程调用服务的代理类
    /// </summary>
    public class RpcDispatchProxy : DispatchProxy
    {
        #region --属性--
        /// <summary>
        /// 获取或设置代理处理程序
        /// </summary>
        public IProxyHandler ProxyHandler { get; set; }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 每当调用生成的代理类型上的任何方法时，都会调用此方法
        /// </summary>
        /// <param name="targetMethod">调用者调用的方法</param>
        /// <param name="args">调用者传递给方法的参数</param>
        /// <returns>返回给调用者的对象，void 方法将返回 <see langword="null"/></returns>
        protected override object Invoke (MethodInfo targetMethod, object[] args)
        {
            return this.ProxyHandler?.InvokeMethod (targetMethod, args);
        }
        #endregion
    }
}
