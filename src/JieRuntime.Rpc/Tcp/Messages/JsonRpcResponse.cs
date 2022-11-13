using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示 Json 远程调用响应的类
    /// </summary>
    internal class JsonRpcResponse
    {
        /// <summary>
        /// 获取或设置 JsonRPC 的版本号
        /// </summary>
        [JsonPropertyName ("jsonrpc")]
        public string JsonRpc { get; set; } = "2.1";

        /// <summary>
        /// 获取或设置远程调用服务响应的执行返回值
        /// </summary>
        [JsonPropertyName ("result")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Result { get; set; }

        /// <summary>
        /// 获取或设置远程调用服务响应的回传参数
        /// </summary>
        [JsonPropertyName ("params")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonRpcParameter[] Parameters { get; set; }

        /// <summary>
        /// 获取或设置远程调用服务的错误信息
        /// </summary>
        [JsonPropertyName ("error")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonRpcError Error { get; set; }

        public void SetError (JsonRpcError error)
        {
            this.Result = null;
            this.Parameters = null;
            this.Error = error;
        }
    }
}
