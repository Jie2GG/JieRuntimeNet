using System.Collections.Generic;
using System.Threading;

namespace JieRuntime.Net.Sockets.Tcp
{
    /// <summary>
    /// 提供给 <see cref="TcpClient"/> 使用的缓冲区
    /// </summary>
    class TcpCache
    {
        #region --字段--
        private readonly ReaderWriterLockSlim rwlock;
        private readonly List<byte> data;
        private readonly int packetHeaderBytesSize;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="TcpCache"/> 类的新实例
        /// </summary>
        /// <param name="packetHeaderBytesSize">指定封包包头长度</param>
        public TcpCache (int packetHeaderBytesSize)
        {
            this.rwlock = new ReaderWriterLockSlim ();
            this.data = new List<byte> ();
            this.packetHeaderBytesSize = packetHeaderBytesSize;
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 将数据推入缓冲区
        /// </summary>
        /// <param name="data">要推入的数据</param>
        public void Push (byte[] data)
        {
            try
            {
                this.rwlock.EnterWriteLock ();
                this.data.AddRange (data);
            }
            finally
            {
                this.rwlock.ExitWriteLock ();
            }
        }

        /// <summary>
        /// 拉取缓冲区中的完整数据包, 如果缓冲区中的数据可以形成完整数据包, 则返回完整的数据包
        /// </summary>
        /// <returns>如果缓冲区的数据可以形成完整的数据包, 则返回数据包的字节数组, 否则返回 <see langword="null"/></returns>
        public byte[] Pull ()
        {
            try
            {
                this.rwlock.EnterReadLock ();

                // 获取包头长度
                if (this.data.Count >= this.packetHeaderBytesSize)
                {
                    byte[] temp = new byte[this.packetHeaderBytesSize];
                    this.data.CopyTo (0, temp, 0, temp.Length);
                    int len = BinaryConvert.ToInt32 (temp, true);

                    // 读取数据
                    if (this.data.Count >= len)
                    {
                        // 长度减去包头字节数
                        len -= this.packetHeaderBytesSize;

                        // 复制数据包
                        byte[] data = new byte[len];
                        this.data.CopyTo (this.packetHeaderBytesSize, data, 0, data.Length);

                        // 清理缓冲区
                        this.data.RemoveRange (0, data.Length + this.packetHeaderBytesSize);
                        return data;
                    }
                }

                return null;
            }
            finally
            {
                this.rwlock.ExitReadLock ();
            }
        }
        #endregion
    }
}
