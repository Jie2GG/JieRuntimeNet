using System;
using System.IO;

namespace JieRuntime.IO
{
    /// <summary>
    /// 提供缓冲区读取服务的类
    /// </summary>
    public class BufferReader : IDisposable
    {
        #region --字段--
        private readonly BinaryReader binaryReader;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取或设置当前流中的位置
        /// </summary>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        /// <exception cref="IOException">I/O错误</exception>
        public long Position
        {
            get => this.binaryReader.BaseStream.Position;
            set => this.binaryReader.BaseStream.Position = value;
        }

        /// <summary>
        /// 获取当前流的长度
        /// </summary>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public long Length => this.binaryReader.BaseStream.Length;

        /// <summary>
        /// 获取当前缓冲区数据的剩余长度
        /// </summary>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public long Overlength => this.Length - this.Position;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="BufferReader"/> 类的新实例
        /// </summary>
        /// <param name="input">输入字节数组</param>
        public BufferReader (byte[] input)
            : this (new MemoryStream (input))
        { }

        /// <summary>
        /// 初始化 <see cref="BufferReader"/> 类的新实例
        /// </summary>
        /// <param name="input">输入流</param>
        /// <exception cref="ArgumentException">流不支持读取、为 <see langword="null"/> 或已关闭</exception>
        public BufferReader (Stream input)
        {
            this.binaryReader = new BinaryReader (input);
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 从流中指定位置开始, 读取流中的所有字节
        /// </summary>
        /// <returns>一个新的字节数组, 包含流中剩余数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public byte[] ReadAll ()
        {
            return this.ReadBytes (this.Overlength);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取1字节长度的数据
        /// </summary>
        /// <returns>一个 <see cref="byte"/> 值</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public byte ReadByte ()
        {
            return this.binaryReader.ReadByte ();
        }

        /// <summary>
        /// 从流中指定位置开始, 读取指定长度的数据
        /// </summary>
        /// <returns>一个字节数组, 包含已读取的指定长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public byte[] ReadBytes (long count)
        {
            byte[] bytes = new byte[count];
            if (this.binaryReader.Read (bytes, 0, bytes.Length) != bytes.Length)
            {
                throw new EndOfStreamException ("无法继续读取数据, 因为已经读取到流的末尾");
            }
            return bytes;
        }

        /// <summary>
        /// 从流中指定位置开始, 读取1字节长度的数据, 并转换为 <see cref="bool"/> 值
        /// </summary>
        /// <returns>一个 <see cref="bool"/> 值, 如果读取到的值不是0则为 <see langword="true"/>, 否则为 <see langword="false"/></returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public bool ReadBoolean ()
        {
            return BitConverter.ToBoolean (this.binaryReader.ReadBytes (sizeof (bool)), 0);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取2字节长度的数据, 并转换为 Unicode 字符
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 Unicode 字符, 等效于已读取的2个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public char ReadChar (bool isBigEndian = true)
        {
            return BinaryConvert.ToChar (this.binaryReader.ReadBytes (sizeof (char)), isBigEndian);
        }

        /// <summary>
        /// 从当前流中读取指定数量的字符，返回字符数组中的数据
        /// </summary>
        /// <param name="count">要读取的字符数</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 Unicode 字符数组, 包含已读取的所有 Unicode 字符</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public char[] ReadChars (int count, bool isBigEndian = true)
        {
            char[] chars = new char[count];
            for (int i = 0; i < count; i++)
            {
                chars[i] = this.ReadChar (isBigEndian);
            }

            return chars;
        }

        /// <summary>
        /// 从流中指定位置开始, 读取2字节长度的数据, 并转换为 <see cref="short"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="short"/> 值, 等效于已读取的2个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public short ReadInt16 (bool isBigEndian = true)
        {
            return BinaryConvert.ToInt16 (this.binaryReader.ReadBytes (sizeof (short)), isBigEndian);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取2字节长度的数据, 并转换为 <see cref="ushort"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="ushort"/> 值, 等效于已读取的2个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public ushort ReadUInt16 (bool isBigEndian = true)
        {
            return BinaryConvert.ToUInt16 (this.binaryReader.ReadBytes (sizeof (ushort)), isBigEndian);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取4字节长度的数据, 并转换为 <see cref="int"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="int"/> 值, 等效于已读取的4个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public int ReadInt32 (bool isBigEndian = true)
        {
            return BinaryConvert.ToInt32 (this.binaryReader.ReadBytes (sizeof (int)), isBigEndian);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取4字节长度的数据, 并转换为 <see cref="uint"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="uint"/> 值, 等效于已读取的4个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public uint ReadUInt32 (bool isBigEndian = true)
        {
            return BinaryConvert.ToUInt32 (this.binaryReader.ReadBytes (sizeof (uint)), isBigEndian);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取8字节长度的数据, 并转换为 <see cref="long"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="long"/> 值, 等效于已读取的8个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public long ReadInt64 (bool isBigEndian = true)
        {
            return BinaryConvert.ToInt64 (this.binaryReader.ReadBytes (sizeof (long)), isBigEndian);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取8字节长度的数据, 并转换为 <see cref="ulong"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="ulong"/> 值, 等效于已读取的8个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public ulong ReadUInt64 (bool isBigEndian = true)
        {
            return BinaryConvert.ToUInt64 (this.binaryReader.ReadBytes (sizeof (ulong)), isBigEndian);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取4字节长度的数据, 并转换为 <see cref="float"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="float"/> 值, 等效于已读取的4个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public float ReadSingle (bool isBigEndian = true)
        {
            return BinaryConvert.ToSingle (this.binaryReader.ReadBytes (sizeof (float)), isBigEndian);
        }

        /// <summary>
        /// 从流中指定位置开始, 读取8字节长度的数据, 并转换为 <see cref="double"/> 值
        /// </summary>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <returns>一个 <see cref="double"/> 值, 等效于已读取的8个字节长度的数据</returns>
        /// <exception cref="EndOfStreamException">在读取所有字节之前到达流的末尾</exception>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public double ReadDouble (bool isBigEndian = true)
        {
            return BinaryConvert.ToDouble (this.binaryReader.ReadBytes (sizeof (double)), isBigEndian);
        }

        /// <summary>
        /// 将一个字节数组数据并入流的末尾
        /// </summary>
        /// <param name="data">并入的字节数组</param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void PutData (byte[] data)
        {
            long position = this.Position;  // 记录当前记录
            this.Position = this.Length;    // 设置游标为到最后
            this.binaryReader.BaseStream.Write (data, 0, data.Length);
            this.Position = position;
        }

        /// <summary>
        /// 将当前位置向前移动指定的长度
        /// </summary>
        /// <param name="len">移动的长度</param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Rollback (long len)
        {
            if (len > this.Position)
            {
                throw new ArgumentOutOfRangeException (nameof (len), len, $"移动的长度超过了流的当前位置");
            }

            this.Position -= len;
        }

        /// <summary>
        /// 释放当前实例所占用的资源
        /// </summary>
        public void Dispose ()
        {
            ((IDisposable)this.binaryReader).Dispose ();
        }
        #endregion
    }
}
