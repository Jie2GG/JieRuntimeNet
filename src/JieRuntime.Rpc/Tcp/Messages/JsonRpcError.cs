using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示 Json 远程调用错误的类
    /// </summary>
    internal class JsonRpcError
    {
        #region --属性--
        /// <summary>
        /// 获取或设置错误代码
        /// </summary>
        [JsonPropertyName ("code")]
        public int Code { get; set; }

        /// <summary>
        ///  获取或设置错误信息
        /// </summary>
        [JsonPropertyName ("message")]
        public string Message { get; set; }

        /// <summary>
        /// 获取或设置附加的错误信息
        /// </summary>
        [JsonPropertyName ("data")]
        public JsonRpcInnerError Data { get; set; }
        #endregion

        #region --私有方法--
        internal static JsonRpcError CreateFromApplicationError (string message, Exception exception)
        {
            return new JsonRpcError ()
            {
                Code = -32500,
                Message = message,
                Data = new JsonRpcInnerError (exception)
            };
        }

        internal static JsonRpcError CreateFromFormateError (JsonException ex)
        {
            return new JsonRpcError ()
            {
                Code = -32700,
                Message = ex.Message,
            };
        }

        internal static JsonRpcError CreateFromSystemError (string message, Exception ex)
        {
            return new JsonRpcError ()
            {
                Code = -32400,
                Message = message,
                Data = new JsonRpcInnerError (ex)
            };
        }

        internal static JsonRpcError CreateFromTypeNotFoundError (string type)
        {
            return new JsonRpcError ()
            {
                Code = -32601,
                Message = $"找不到与“{type}”匹配的服务, 可能该服务未在服务端中注册"
            };
        }

        internal static JsonRpcError CreateFromMethodNotFoundError (string type, string method)
        {
            return new JsonRpcError ()
            {
                Code = -32601,
                Message = $"无法在类型“{type}”中找到与“{method}”匹配的方法"
            };
        }
        #endregion
    }
}