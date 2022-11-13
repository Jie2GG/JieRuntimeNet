using System.Text;

using JieRuntime.Utils;

namespace JieRuntime.Extensions
{
    /// <summary>
    /// 提供一组 <see cref="string"/> 类的扩展方法
    /// </summary>
    public static class StringExtension
    {
        #region --公开方法--
        /// <summary>
        /// 取出字符串中指定特征中间的字符串数组
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="startStr">指定匹配的开始特征</param>
        /// <param name="endStr">指定匹配的结束特征</param>
        /// <param name="startIndex">指定开始查找的位置, 默认: 0</param>
        /// <returns>一个字符串数组, 包含从源字符串中匹配到的所有结果</returns>
        public static string[] GetMidString (this string sourceStr, string startStr, string endStr, int startIndex = 0)
        {
            return StringUtils.GetMidString (sourceStr, startStr, endStr, startIndex);
        }

        /// <summary>
        /// 取出目标字符串在源字符串中出现的次数
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="targetStr">要匹配的目标字符串</param>
        /// <returns>一个整数, 指示目标字符串的出现次数</returns>
        public static int StringCount (this string sourceStr, string targetStr)
        {
            return StringUtils.StringCount (sourceStr, targetStr);
        }

        /// <summary>
        /// 将指定字符串中的所有字符编码为一个字节序列
        /// </summary>
        /// <param name="sourceStr">包含要编码的字符的字符串</param>
        /// <returns>一个字节数组，包含对指定的字符集进行编码的结果</returns>
        public static byte[] GetBytes (this string sourceStr)
        {
            return BinaryConvert.GetBytes (sourceStr);
        }

        /// <summary>
        /// 将指定字符串中的所有字符编码为一个字节序列
        /// </summary>
        /// <param name="sourceStr">包含要编码的字符的字符串</param>
        /// <param name="encoding">提供对字节序列的编码</param>
        /// <returns>一个字节数组，包含对指定的字符集进行编码的结果</returns>
        public static byte[] GetBytes (this string sourceStr, Encoding encoding)
        {
            return BinaryConvert.GetBytes (sourceStr, encoding);
        }
        #endregion
    }
}
