namespace JieRuntime.Rpc.Exceptions
{
    /// <summary>
    /// 表示远程调用超时的异常
    /// </summary>
    public class RpcTimeoutException : RpcException
    {
        /// <summary>
        /// 初始化 <see cref="RpcTimeoutException"/> 类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        public RpcTimeoutException (string message)
            : base (message)
        { }
    }
}
