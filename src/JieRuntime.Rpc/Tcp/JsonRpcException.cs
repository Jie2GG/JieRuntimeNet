using System.Collections;

using JieRuntime.Rpc.Tcp.Messages;

namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 表示 Json 远程调用服务调用指定服务期间发生的错误
    /// </summary>
    internal class JsonRpcException : RpcException
    {
        #region --属性--
        public override string StackTrace { get; }

        public override IDictionary Data { get; }
        #endregion

        #region --构造函数--
        public JsonRpcException (JsonRpcError error)
            : base (error.Message, error.Data == null ? null : new JsonRpcException (error.Data))
        {
            this.HResult = error.Code;
        }

        public JsonRpcException (JsonRpcInnerError innerError)
            : base (innerError.Message, innerError.InnerError == null ? null : new JsonRpcException (innerError.InnerError))
        {
            this.StackTrace = innerError.StackTrace;
            this.Source = innerError.Source;
            this.HResult = innerError.HResult;
            this.Data = innerError.Data;
            this.HelpLink = innerError.HelpLink;
        }
        #endregion
    }
}
