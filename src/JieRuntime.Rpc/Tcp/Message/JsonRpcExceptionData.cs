using System;

namespace JieRuntime.Rpc.Tcp.Messages
{
   class JsonRpcExceptionData
    {
        public string Source { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public JsonRpcExceptionData InnerException { get; set; }

        public static JsonRpcExceptionData Create (Exception exception)
        {
            if (exception is null)
            {
                return null;
            }

            return new JsonRpcExceptionData ()
            {
                Source = exception.Source,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException != null ? Create (exception.InnerException) : null
            };
        }
    }
}
