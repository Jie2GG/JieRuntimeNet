using JieRuntime.Ipc;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 表示包含远程调用服务端客户端信息事件数据的类
    /// </summary>
    public class RpcServiceClientInfoEventArgs : RpcServiceEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取当前事件的客户端
        /// </summary>
        public RpcServiceClientBase Client { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcServiceClientInfoEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="client">套接字客户端</param>
        public RpcServiceClientInfoEventArgs (RpcServiceClientBase client)
        {
            this.Client = client;
        }
        #endregion
    }
}
