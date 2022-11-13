using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示 Json 远程调用请求的类
    /// </summary>
    internal class JsonRpcParameter
    {
        /// <summary>
        /// 获取或设置参数名称
        /// </summary>
        [JsonPropertyName ("name")]
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置参数值
        /// </summary>
        [JsonPropertyName ("value")]
        public object Value { get; set; }
    }
}
