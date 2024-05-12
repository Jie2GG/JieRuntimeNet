using System;

using JieRuntime.IO;

namespace JieRuntime.Rpc.Extensions
{
    /// <summary>
    /// 提供一组 <see cref="BufferReader"/> 方法的扩展
    /// </summary>
    static class BufferReaderExtension
    {
        /// <summary>
        /// 从流中指定位置开始, 读取短令牌数据
        /// </summary>
        /// <param name="buffer">要读取的流</param>
        /// <returns>一个字节数组, 包含读取的段令牌数据</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> 不能为 <see langword="null"/></exception>
        public static byte[] ReadShortToken (this BufferReader buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException (nameof (buffer));
            }

            ushort len = buffer.ReadUInt16 ();
            return buffer.ReadBytes (len);
        }
    }
}
