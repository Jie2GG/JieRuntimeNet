using JieRuntime.Rpc.Exceptions;
using JieRuntime.Rpc.Tcp.Messages;

namespace JieRuntime.Rpc.Tcp.Exceptions
{
    class JsonRpcRemoteException : RpcException
    {
        public JsonRpcRemoteException (JsonRpcExceptionData data)
            : base (data.Message, data.InnerException is null ? null : new JsonRpcRemoteException (data.InnerException))
        {
            this.Source = data.Source;
        }
    }
}
