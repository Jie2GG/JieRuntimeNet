using Microsoft.Extensions.ObjectPool;

namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 提供 TCP 协议等待策略
    /// </summary>
    internal sealed class TcpWaitPolicy : IPooledObjectPolicy<TcpWait>
    {
        #region --公开方法--
        /// <summary>
        /// 创建 <see cref="TcpWait"/>
        /// </summary>
        /// <returns>新创建的 <see cref="TcpWait"/></returns>
        public TcpWait Create ()
        {
            return new TcpWait ();
        }

        /// <summary>
        /// 在将对象返回到池时运行一些处理, 可用于重置对象的状态并指示是否应将对象返回到池中
        /// </summary>
        /// <param name="obj">要返回到池的对象</param>
        /// <returns>如果应将对象返回到池, 则为 <see langword="true"/>; 如果不希望保留对象, 则为 <see langword="false"/></returns>
        public bool Return (TcpWait obj)
        {
            obj.WaitHandler.Set (); // 释放阻塞的线程
            obj.IsGetResponse = false;
            obj.Resutl = null;
            return true;
        }
        #endregion
    }
}
