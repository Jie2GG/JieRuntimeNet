using System;

using JieRuntime.Extensions;
using JieRuntime.IO;
using JieRuntime.Rpc.Extensions;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示消息分片的结构
    /// </summary>
    internal readonly struct MessageFragment
    {
        #region --常量--
        private const int PACKET_SIZE = 60000;
        #endregion

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
        /// 获取当前分片的索引
        /// </summary>
        public ushort FragmentIndex { get; }

        /// <summary>
        /// 获取当前分片的数量
        /// </summary>
        public ushort FragmentCount { get; }

        /// <summary>
        /// 获取当前分片的分片数据
        /// </summary>
        public byte[] FragmentData { get; }
        #endregion

        #region --构造函数--
        private MessageFragment (MessageType type, long tag, ushort index, ushort count, byte[] data)
        {
            this.MessageType = type;
            this.MessageTag = tag;
            this.FragmentIndex = index;
            this.FragmentCount = count;
            this.FragmentData = data;
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 获取消息分片的所有数据
        /// </summary>
        /// <returns>一个字节数组, 包含消息分片的所有数据</returns>
        public byte[] GetBytes ()
        {
            using BufferWriter buffer = new ();
            buffer.Write ((byte)this.MessageType);
            buffer.Write (this.MessageTag);
            buffer.Write (this.FragmentIndex);
            buffer.Write (this.FragmentCount);
            buffer.WriteShortToken (this.FragmentData);
            return buffer.GetBytes ();
        }

        /// <summary>
        /// 依据指定的封包大小和消息标识创建消息分片
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="tag">消息标识</param>
        /// <param name="data">消息原始数据</param>
        /// <returns>一个只读数组, 包含切分好的消息分片</returns>
        public static unsafe ReadOnlySpan<MessageFragment> CreateFragments (MessageType type, long tag, byte[] data)
        {
            long count = data.LongLength / PACKET_SIZE;
            if ((count * PACKET_SIZE) < data.LongLength)
            {
                count += 1;
            }

            MessageFragment[] fragments = new MessageFragment[count];
            fixed (byte* pData = data)
            {
                for (int i = 0; i < fragments.Length; i++)
                {
                    long length = PACKET_SIZE;
                    long offset = i * length;
                    if (data.LongLength - offset < length)
                    {
                        length = data.Length - offset;
                    }

                    byte[] temp = data.Skip (offset)
                        .Left (length);
                    fragments[i] = new MessageFragment (type, tag, (ushort)i, (ushort)count, temp);
                }
            }

            return fragments;
        }

        /// <summary>
        /// 尝试将一个字节数组解析成消息分片
        /// </summary>
        /// <param name="data">要尝试解析的数据</param>
        /// <param name="result">如果解析成功, 将结果存放在此参数内</param>
        /// <returns>如果解析成功, 则为 <see langword="true"/>; 否则为 <see langword="false"/></returns>
        public static bool TryParse (byte[] data, out MessageFragment? result)
        {
            result = null;
            /*
             * Type:  1
             * Tag:   8
             * Index: 2
             * Count: 2
             * Data:  2 + ...
             */
            if (data != null && data.Length >= 0x0F)
            {
                try
                {
                    using BufferReader buffer = new (data);
                    MessageType type = (MessageType)buffer.ReadByte ();
                    long tag = buffer.ReadInt64 ();
                    ushort index = buffer.ReadUInt16 ();
                    ushort count = buffer.ReadUInt16 ();
                    byte[] d = buffer.ReadShortToken ();
                    result = new MessageFragment (type, tag, index, count, d);
                    return true;
                }
                catch
                { }
            }

            return false;
        }
        #endregion
    }
}
