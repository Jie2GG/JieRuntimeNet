namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示消息的结构
    /// </summary>
    internal readonly struct Message
    {
        #region --属性--
        /// <summary>
        /// 获取消息类型
        /// </summary>
        public MessageType MessageType { get; }

        /// <summary>
        /// 获取消息标识
        /// </summary>
        public long MessageTag { get; }

        /// <summary>
        /// 获取消息数据
        /// </summary>
        public byte[] Data { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="Message"/> 类的新实例
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="tag">消息标识</param>
        /// <param name="data">消息数据</param>
        public Message (MessageType type, long tag, byte[] data)
        {
            this.MessageType = type;
            this.MessageTag = tag;
            this.Data = data;
        }
        #endregion
    }
}
