namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示包含远程调用服务客户端事件数据的类
    /// </summary>
    public class RpcClientEventArgs : RpcEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取当前事件的客户端
        /// </summary>
        public RpcClientBase Client { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcClientEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="client">远程调用客户端</param>
        public RpcClientEventArgs (RpcClientBase client)
        {
            this.Client = client;
        }
        #endregion
    }
}
