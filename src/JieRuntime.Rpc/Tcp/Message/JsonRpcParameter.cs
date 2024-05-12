using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    class JsonRpcParameter
    {
        [JsonPropertyName ("name")]
        public string Name { get; set; }

        [JsonPropertyName ("value")]
        public object Value { get; set; }
    }
}
