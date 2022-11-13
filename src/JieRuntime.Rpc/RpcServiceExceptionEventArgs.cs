using System;

namespace JieRuntime.Rpc
{
    /// <summary>
    /// 提供远程调用异常事件数据的类
    /// </summary>
    public class RpcServiceExceptionEventArgs : RpcServiceEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取当前事件的异常
        /// </summary>
        public Exception Exception { get; set; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcServiceExceptionEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="e">异常信息</param>
        public RpcServiceExceptionEventArgs (Exception e)
        {
            this.Exception = e;
        }
        #endregion
    }
}
