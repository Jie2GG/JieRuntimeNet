namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示消息类型的枚举
    /// </summary>
    internal enum MessageType : byte
    {
        /// <summary>
        /// 请求
        /// </summary>
        Request = 0x10,
        /// <summary>
        /// 响应
        /// </summary>
        Response = 0x20,
    }
}
