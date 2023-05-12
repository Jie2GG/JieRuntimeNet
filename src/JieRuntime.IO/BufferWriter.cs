using System;
using System.IO;

namespace JieRuntime.IO
{
    /// <summary>
    /// 提供缓冲区写入服务的类
    /// </summary>
    public class BufferWriter : IDisposable
    {
        #region --字段--
        private readonly BinaryWriter binaryWriter;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取或设置当前流中的位置
        /// </summary>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        /// <exception cref="IOException">I/O错误</exception>
        public long Position
        {
            get => this.binaryWriter.BaseStream.Position;
            set => this.binaryWriter.BaseStream.Position = value;
        }

        /// <summary>
        /// 获取当前流的长度
        /// </summary>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public long Length => this.binaryWriter.BaseStream.Length;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="BufferWriter"/> 类的新实例
        /// </summary>
        public BufferWriter ()
            : this (new MemoryStream ())
        { }

        /// <summary>
        /// 初始化 <see cref="BufferWriter"/> 类的新实例
        /// </summary>
        /// <param name="stream">输出流</param>
        public BufferWriter (Stream stream)
        {
            this.binaryWriter = new BinaryWriter (stream);
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 将 <see cref="byte"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="byte"/> 值</param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (byte value)
        {
            this.binaryWriter.Write (value);
        }

        /// <summary>
        /// 将字节数组写入流的指定位置
        /// </summary>
        /// <param name="buffer">要写入的字节数组</param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (byte[] buffer)
        {
            this.binaryWriter.Write (buffer);
        }

        /// <summary>
        /// 将 <see cref="bool"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="bool"/> 值</param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (bool value)
        {
            this.binaryWriter.Write (value);
        }

        /// <summary>
        /// 将 <see cref="char"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="char"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (char value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将字符数组写入流的指定位置
        /// </summary>
        /// <param name="chars">要写入的字符数组</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (char[] chars, bool isBigEndian = true)
        {
            if (chars is null)
            {
                throw new ArgumentNullException (nameof (chars));
            }

            foreach (char c in chars)
            {
                this.Write (c, isBigEndian);
            }
        }

        /// <summary>
        /// 将 <see cref="short"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="short"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (short value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将 <see cref="ushort"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="ushort"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (ushort value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将 <see cref="int"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="int"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (int value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将 <see cref="uint"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="uint"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (uint value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将 <see cref="long"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="long"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (long value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将 <see cref="ulong"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="ulong"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (ulong value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将 <see cref="float"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="float"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (float value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 将 <see cref="double"/> 写入流的指定位置
        /// </summary>
        /// <param name="value">要写入的 <see cref="double"/> 值</param>
        /// <param name="isBigEndian">是否以大端序模式写入, 默认: <see langword="true"/></param>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public void Write (double value, bool isBigEndian = true)
        {
            this.Write (BinaryConvert.GetBytes (value, isBigEndian));
        }

        /// <summary>
        /// 获取写入到流中的所有数据
        /// </summary>
        /// <returns>一个字节数组, 包含所有写入到流中的数据</returns>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">方法在流关闭后被调用</exception>
        public byte[] GetBytes ()
        {
            if (this.Length == 0)
            {
                return Array.Empty<byte> ();
            }

            byte[] buf = new byte[this.Length];

            // 读取所有数据
            long position = this.Position;
            this.Position = 0;
            int readLength = this.binaryWriter.BaseStream.Read (buf, 0, buf.Length);
            if (readLength != this.Length)
            {
                throw new InvalidDataException ($"基础流的数据无效. 当前流: {this.Length}, 基础流: {readLength}");
            }

            this.Position = position;

            return buf;
        }

        /// <summary>
        /// 清空当前流中的所有数据
        /// </summary>
        public void Clear ()
        {
            this.binaryWriter.Seek (0, SeekOrigin.Begin);
            this.binaryWriter.BaseStream.Seek (0, SeekOrigin.Begin);
            this.binaryWriter.BaseStream.SetLength (0);
        }

        /// <summary>
        /// 释放当前实例所使用的所有资源
        /// </summary>
        public void Dispose ()
        {
            ((IDisposable)this.binaryWriter).Dispose ();
        }
        #endregion
    }
}