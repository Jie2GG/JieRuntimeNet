using System;
using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    class JsonRpcRequest
    {
        [JsonPropertyName ("jsonrpc")]
        public string JsonRpc { get; set; } = "3.0-jie";

        [JsonPropertyName ("type")]
        public string Type { get; set; }

        [JsonPropertyName ("method")]
        public string Method { get; set; }

        [JsonPropertyName ("params")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonRpcParameter[] Parameters { get; set; }
    }
}
