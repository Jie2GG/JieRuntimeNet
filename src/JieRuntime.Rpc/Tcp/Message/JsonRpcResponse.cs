using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    class JsonRpcResponse
    {
        [JsonPropertyName ("jsonrpc")]
        public string JsonRpc { get; set; } = "3.0-jie";

        [JsonPropertyName ("result")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Result { get; set; }

        [JsonPropertyName ("params")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonRpcParameter[] Parameters { get; set; }

        [JsonPropertyName ("error")]
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonRpcError Error { get; set; }

        public static JsonRpcResponse CreateFromError (JsonRpcError error)
        {
            return new JsonRpcResponse ()
            {
                Result = null,
                Parameters = null,
                Error = error
            };
        }
    }
}
