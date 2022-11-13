using System.Reflection;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示远程调用服务代理接口
    /// </summary>
    public interface IRpcServiceProxy
    {
        /// <summary>
        /// 每当调用代理类型上的任何方法时，都会调用此方法
        /// </summary>
        /// <param name="targetMethod">调用者调用的方法</param>
        /// <param name="args">调用者传递给方法的参数</param>
        /// <returns>返回给调用者的对象，void 方法将返回 <see langword="null"/></returns>
        object InvokeMethod (MethodInfo targetMethod, object[] args);
    }
}
