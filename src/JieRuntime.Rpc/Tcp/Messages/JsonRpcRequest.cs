using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示 Json 远程调用请求的类
    /// </summary>
    internal class JsonRpcRequest
    {
        /// <summary>
        /// 获取或设置 JsonRPC 的版本号
        /// </summary>
        [JsonPropertyName ("jsonrpc")]
        public string JsonRpc { get; set; } = "2.1";

        /// <summary>
        /// 获取或设置远程调用服务请求执行的类型
        /// </summary>
        [JsonPropertyName ("type")]
        public string Type { get; set; }

        /// <summary>
        /// 获取或设置远程调用服务请求执行的方法
        /// </summary>
        [JsonPropertyName ("method")]
        public string Method { get; set; }

        /// <summary>
        /// 获取或设置远程调用请求所需的参数
        /// </summary>
        [JsonPropertyName ("params")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonRpcParameter[] Parameters { get; set; }
    }
}
