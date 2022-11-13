using System;

using JieRuntime.IO;

namespace JieRuntime.Rpc.Extensions
{
    /// <summary>
    /// 提供一组 <see cref="BufferWriter"/> 方法的扩展
    /// </summary>
    internal static class BufferWriterExtension
    {
        /// <summary>
        /// 将数据以短令牌的形式写入流的指定位置
        /// </summary>
        /// <param name="buffer">要写入的流</param>
        /// <param name="data">令牌数据</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> 或 <paramref name="data"/> 为 <see langword="null"/></exception>
        public static void WriteShortToken (this BufferWriter buffer, byte[] data)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException (nameof (buffer));
            }

            if (data is null)
            {
                throw new ArgumentNullException (nameof (data));
            }

            buffer.Write ((short)data.Length);
            buffer.Write (data);
        }
    }
}
