using System;

namespace JieRuntime.Net.Sockets.Tcp
{
    /// <summary>
    /// <see cref="TcpClient"/> 的选项
    /// </summary>
    public class TcpOptions
    {
        #region --字段--
        private int packetHeaderBytesSize = 2;
        private int packetMaxSize = ushort.MaxValue;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取或设置 TCP 协议封包包头的字节大小
        /// </summary>
        public int PacketHeaderBytesSize
        {
            get => this.packetHeaderBytesSize;
            set
            {
                if (value != 0 && GetByteSize (this.packetMaxSize) > value)
                {
                    throw new ArgumentOutOfRangeException (nameof (value), $"报文头字节数不足以表达 {this.packetMaxSize} 长度的数据");
                }
                this.packetHeaderBytesSize = value;
            }
        }
        
        /// <summary>
        /// 获取或设置 TCP 协议封包的最大大小
        /// </summary>
        public int PacketMaxSize
        {
            get => this.packetMaxSize;
            set
            {
                if (this.packetHeaderBytesSize != 0 && GetByteSize (value) > this.packetHeaderBytesSize)
                {
                    throw new ArgumentOutOfRangeException (nameof (value), $"报文头字节数无法表达 {value} 长度的数据");
                }
                this.packetMaxSize = value;
            }
        }
        
        /// <summary>
        /// 获取或设置挂起连接队列的最大长度
        /// </summary>
        public int ListenSize { get; set; } = int.MaxValue;
        #endregion

        #region --私有方法--
        private static int GetByteSize (int len)
        {
            int size = 0;
            do
            {
                len >>= 8;
                size++;
            } while (len > 0);
            return size;
        }
        #endregion
    }
}
