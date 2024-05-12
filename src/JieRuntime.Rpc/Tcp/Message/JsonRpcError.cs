using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using JieRuntime.Rpc.Tcp.Exceptions;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示 Json 远程调用错误的类
    /// </summary>
    class JsonRpcError
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
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Data { get; set; }
        #endregion

        #region --私有方法--
        internal static JsonRpcError CreateFromParseError ()
        {
            return new JsonRpcError ()
            {
                Code = -32700,
                Message = $"接收到无效的 JSON, 请检查是否符合 JSON-RPC 协议"
            };
        }

        internal static JsonRpcError CreateFromInvalidRequest ()
        {
            return new JsonRpcError ()
            {
                Code = -32600,
                Message = $"无效的请求, 不是有效的 Request 对象"
            };
        }

        internal static JsonRpcError CreateFromInvalidMethod (string type, string method)
        {
            return new JsonRpcError ()
            {
                Code = -32601,
                Message = $"请求的 “{method}” 方法无效, 可能该方法未在服务 “{type}” 中注册"
            };
        }

        internal static JsonRpcError CreateFromInvalidParameter (string type, string method)
        {
            return new JsonRpcError ()
            {
                Code = -32602,
                Message = $"请求的参数无效, 请检查是否符合 “{type}.{method}” 的参数要求"
            };
        }

        internal static JsonRpcError CreateFromInvalidParameter (string type, string method, int paramIndex)
        {
            return new JsonRpcError ()
            {
                Code = -32602,
                Message = $"请求的参数无效, 请检查是否符合 “{type}.{method}” 的参数要求, 位置: {paramIndex}"
            };
        }

        internal static JsonRpcError CreateFromInternalError (Exception exception)
        {
            return new JsonRpcError ()
            {
                Code = -32603,
                Message = "内部错误",
                Data = JsonRpcExceptionData.Create (exception)
            };
        }

        internal static JsonRpcError CreateFromInvalidType (string type)
        {
            return new JsonRpcError ()
            {
                Code = -32604,
                Message = $"请求的类型 “{type}” 无效, 请检查类型是否在远程注册"
            };
        }

        internal static JsonRpcError CreateFromResponseParameterCountError (int want, int practical)
        {
            return new JsonRpcError ()
            {
                Code = -32000,
                Message = $"响应的参数个数错误, 预期: {want}, 实际: {practical}"
            };
        }

        internal static JsonRpcError CreateFromResponseInvalidParameter (string method, int paramIndex)
        {
            return new JsonRpcError ()
            {
                Code = -32001,
                Message = $"请求响应的参数无效, 请检查是否符合远程方法 “{method}” 的参数要求, 位置: {paramIndex}"
            };
        }

        internal static JsonRpcError CreateFromInvokeError (string type, string method, Exception exception)
        {
            return new JsonRpcError ()
            {
                Code = -32002,
                Message = $"调用“{type}.{method}” 时发生错误",
                Data = JsonRpcExceptionData.Create (exception)
        };
    }
    #endregion
}
}