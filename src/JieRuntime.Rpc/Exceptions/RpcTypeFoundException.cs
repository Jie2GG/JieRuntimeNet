namespace JieRuntime.Rpc.Exceptions
{
    /// <summary>
    /// 表示远程调用服务类型存在的异常的类
    /// </summary>
    public class RpcTypeFoundException : RpcException
    {
        #region --属性--
        /// <summary>
        /// 获取存在的类型的名称
        /// </summary>
        public string TypeName { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化一个新的 <see cref="RpcTypeFoundException"/> 实例
        /// </summary>
        /// <param name="typeName">存在的类型的名称</param>
        public RpcTypeFoundException (string typeName)
        {
            this.TypeName = typeName;
        }
        #endregion
    }
}
