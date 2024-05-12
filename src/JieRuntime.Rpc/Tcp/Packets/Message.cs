using System;

using JieRuntime.Extensions;
using JieRuntime.IO;
using JieRuntime.Rpc.Extensions;

namespace JieRuntime.Rpc.Tcp.Packets
{
    class Message
    {
        public long Id { get; set; }

        public ushort Index { get; set; }

        public ushort Count { get; set; }

        public MessageType Type { get; set; }

        public byte[] Data { get; set; }

        public byte[] GetBytes ()
        {
            using BufferWriter buffer = new ();
            buffer.Write (this.Id);
            buffer.Write (this.Index);
            buffer.Write (this.Count);
            buffer.Write ((byte)this.Type);
            buffer.WriteShortToken (this.Data);
            return buffer.GetBytes ();
        }

        public static unsafe ReadOnlySpan<Message> Create (MessageType type, long tag, byte[] data)
        {
            long count = data.LongLength / short.MaxValue;
            if (count * short.MaxValue < data.LongLength)
            {
                count += 1;
            }

            Message[] fragments = new Message[count];

            for (int i = 0; i < fragments.Length; i++)
            {
                long length = short.MaxValue;
                long offset = i * length;
                if (data.LongLength - offset < length)
                {
                    length = data.LongLength - offset;
                }

                // 分割数据
                byte[] fragment = data.Skip (offset).Left (length);
                fragments[i] = new Message ()
                {
                    Id = tag,
                    Index = (ushort)i,
                    Count = (ushort)count,
                    Type = type,
                    Data = fragment
                };
            }

            return fragments;
        }

        public static bool TryParse (byte[] data, out Message message)
        {
            if (data is null)
            {
                throw new ArgumentNullException (nameof (data));
            }

            try
            {
                Message msg = new ();
                using BufferReader buffer = new (data);
                msg.Id = buffer.ReadInt64 ();
                msg.Index = buffer.ReadUInt16 ();
                msg.Count = buffer.ReadUInt16 ();
                msg.Type = (MessageType)buffer.ReadByte ();
                msg.Data = buffer.ReadShortToken ();
                message = msg;
                return true;
            }
            catch
            {
                message = null;
            }

            return false;
        }
    }
}
