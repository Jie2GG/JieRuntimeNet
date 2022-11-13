namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 表示 Json 远程调用服务执行期间发生的网络错误
    /// </summary>
    public class JsonRpcNetworkException : RpcException
    {
        /// <summary>
        /// 初始化 <see cref="JsonRpcNetworkException"/> 类的新实例
        /// </summary>
        public JsonRpcNetworkException ()
            : base ("数据接收失败, 网络环境可能存在异常")
        {
            this.HResult = -32300;
        }
    }
}
