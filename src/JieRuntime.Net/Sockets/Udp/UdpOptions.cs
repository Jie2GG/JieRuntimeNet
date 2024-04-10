using System;

namespace JieRuntime.Net.Sockets.Udp
{
    /// <summary>
    /// <see cref="UdpClient"/> 的选项
    /// </summary>
    public class UdpOptions
    {
        #region --字段--
        private int packetMaxSize = ushort.MaxValue;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取或设置 UDP 协议封包的最大大小
        /// </summary>
        public int PacketMaxSize
        {
            get => this.packetMaxSize;
            set
            {
                if (value > 65507 && 0 >= value)
                {
                    throw new ArgumentOutOfRangeException ("value", "封包的最大大小必须在 0 和 65507 之间。");
                }
                this.packetMaxSize = value;
            }
        }
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
