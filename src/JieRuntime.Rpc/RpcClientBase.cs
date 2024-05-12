using System;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 提供远程调用客户端的基础类, 该类是抽象的
    /// </summary>
    public abstract class RpcClientBase : RpcBase, IDisposable
    {
        #region --属性--
        /// <summary>
        /// 响应超时, 默认10秒. 值为 <see cref="TimeSpan.Zero"/> 时将无限等待远程对象返回
        /// </summary>
        public TimeSpan ResponseTimeout { get; set; }
        #endregion

        #region --事件--
        /// <summary>
        /// 客户端连接远程主机事件
        /// </summary>
        public abstract event EventHandler<RpcEventArgs> Connected;

        /// <summary>
        /// 客户端断开远程主机连接事件
        /// </summary>
        public abstract event EventHandler<RpcEventArgs> Disconnected;

        /// <summary>
        /// 客户端出现异常的事件
        /// </summary>
        public abstract event EventHandler<RpcExceptionEventArgs> Exception;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcClientBase"/> 类的新实例
        /// </summary>
        protected RpcClientBase ()
        {
            this.ResponseTimeout = TimeSpan.FromSeconds (10);
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 开始连接远程主机
        /// </summary>
        public abstract void Connect ();

        /// <summary>
        /// 断开与远程主机的连接
        /// </summary>
        public abstract void Disconnect ();

        /// <summary>
        /// 释放当前实例所占用的资源
        /// </summary>
        public abstract void Dispose ();
        #endregion
    }
}
