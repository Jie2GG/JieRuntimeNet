using System;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 提供远程调用服务服务端的基础类, 该类是抽象的
    /// </summary>
    public abstract class RpcServiceServerBase
    {
        #region --属性--
        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前服务端是否正在运行
        /// </summary>
        public abstract bool IsRunning { get; }
        #endregion

        #region --事件--
        /// <summary>
        /// 表示服务端启动的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceEventArgs> Started;

        /// <summary>
        /// 表示服务端停止的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceEventArgs> Stopped;

        /// <summary>
        /// 表示服务端异常的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceExceptionEventArgs> Exception;

        /// <summary>
        /// 表示有客户端连接到服务端的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceClientInfoEventArgs> ClientConnected;

        /// <summary>
        /// 表示客户端断开连接服务端的事件
        /// </summary>
        public abstract event EventHandler<RpcServiceClientInfoEventArgs> ClientDisconnected;
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
        /// 服务端启动
        /// </summary>
        /// <param name="e">包含服务端启动的事件参数</param>
        protected virtual void OnStarted (RpcServiceEventArgs e)
        { }

        /// <summary>
        /// 服务端停止
        /// </summary>
        /// <param name="e">包含服务端停止的事件参数</param>
        protected virtual void OnStopped (RpcServiceEventArgs e)
        { }

        /// <summary>
        /// 服务端异常
        /// </summary>
        /// <param name="e">包含服务端异常信息的事件数据</param>
        protected virtual void OnException (RpcServiceExceptionEventArgs e)
        { }

        /// <summary>
        /// 有客户端连接到服务端
        /// </summary>
        /// <param name="e">包含客户端的事件参数</param>
        protected virtual void OnClientConnected (RpcServiceClientInfoEventArgs e)
        { }

        /// <summary>
        /// 客户端断开连接服务端
        /// </summary>
        /// <param name="e">包含客户端的事件参数</param>
        protected virtual void OnClientDisconnected (RpcServiceClientInfoEventArgs e)
        { }
        #endregion
    }
}
