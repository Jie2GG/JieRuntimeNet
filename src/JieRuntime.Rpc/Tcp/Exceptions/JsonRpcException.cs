using JieRuntime.Rpc.Exceptions;
using JieRuntime.Rpc.Tcp.Messages;

namespace JieRuntime.Rpc.Tcp.Exceptions
{
    class JsonRpcException : RpcException
    {
        /// <summary>
        /// 获取异常代码
        /// </summary>
        public int Code { get; }


        public JsonRpcException (JsonRpcError error)
            : base (error.Message, error.Data is null ? null : new JsonRpcRemoteException ((JsonRpcExceptionData)error.Data))
        {
            this.Code = error.Code;
        }
    }
}
