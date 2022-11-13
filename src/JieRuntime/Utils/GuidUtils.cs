using System;

namespace JieRuntime.Utils
{
    /// <summary>
    /// 提供一组快速处理 GUID 的方法
    /// </summary>
    public static class GuidUtils
    {
        /// <summary>
        /// 创建一个新的 GUID 并转换为等效的字符串形式
        /// </summary>
        /// <returns>一个表示 GUID 的字符串</returns>
        public static string NewGuidString ()
        {
            return Guid.NewGuid ().ToString ("D");
        }

        /// <summary>
        /// 创建一个新的 GUID 并转换为 64 位有符号整数
        /// </summary>
        /// <returns>一个 64 位的有符号整数, 来源于新 GUID 的一部分</returns>
        public static long NewGuidInt64 ()
        {
            return BinaryConvert.ToInt64 (Guid.NewGuid ().ToByteArray ());
        }
    }
}
