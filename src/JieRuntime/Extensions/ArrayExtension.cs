using System;
using System.Text;

using JieRuntime.Utils;

namespace JieRuntime.Extensions
{
    /// <summary>
    /// 提供一组 <see cref="Array"/> 类的扩展方法
    /// </summary>
    public static class ArrayExtension
    {
        #region --公开方法--
        /// <summary>
		/// 将第二个数组的头部连接到第一个数组的尾部
		/// </summary>
		/// <typeparam name="T">arr1 和 arr2 的元素类型</typeparam>
		/// <param name="arr1">要连接的第一个数组</param>
		/// <param name="arr2">要连接到第一个数组的数组</param>
		/// <returns>一个数组, 包含两个输入数组的连接元素。</returns>
        public static T[] Concat<T> (this T[] arr1, T[] arr2)
        {
            return ArrayUtils.Concat (arr1, arr2);
        }

        /// <summary>
		/// 将输入的数组按顺序依次拼接到第一个数组的尾部
		/// </summary>
		/// <typeparam name="T">arrs 的元素类型</typeparam>
        /// <param name="source">要连接的第一个数组</param>
		/// <param name="arrs">要连接的所有数组</param>
		/// <returns>一个数组, 包含多个输入数组的连接元素。</returns>
        public static T[] Concat<T> (this T[] source, params T[][] arrs)
        {
            return source.Concat (ArrayUtils.Concat (arrs));
        }

        /// <summary>
		/// 从数组的开始返回指定数量的连续元素
		/// </summary>
		/// <typeparam name="T">source 的元素类型</typeparam>
		/// <param name="source">返回元素的序列</param>
		/// <param name="count">要返回的元素数量</param>
		/// <returns>一个数组, 包含从输入数组开始的指定数量的元素。</returns>
        public static T[] Left<T> (this T[] source, long count)
        {
            return ArrayUtils.Left (source, count);
        }

        /// <summary>
		/// 跳过数组中指定数量的元素，然后返回剩余元素
		/// </summary>
		/// <typeparam name="T">source 中的元素的类型</typeparam>
		/// <param name="source">返回元素的序列</param>
		/// <param name="count">返回剩余元素前要跳过的元素数量</param>
		/// <returns>一个数组, 其中包含输入数组中的指定数量后出现的元素</returns>
        public static T[] Skip<T> (this T[] source, long count)
        {
            return ArrayUtils.Skip (source, count);
        }

        /// <summary>
		/// 从数组的结尾返回指定数量的连续元素
		/// </summary>
		/// <typeparam name="T">source 的元素类型</typeparam>
		/// <param name="source">返回元素的序列</param>
		/// <param name="count">要返回的元素数量</param>
		/// <returns>一个数组, 包含从输入数组结尾的指定数量的元素。</returns>
        public static T[] Right<T> (this T[] source, long count)
        {
            return ArrayUtils.Right (source, count);
        }

        /// <summary>
		/// 通过使用两个元素类型的默认相等比较器来确定两个数组是否相等。
		/// </summary>
		/// <typeparam name="T">输入数组的元素类型</typeparam>
		/// <param name="source">一个 <see cref="Array"/> 与 dest 进行比较。</param>
		/// <param name="dest">一个 <see cref="Array"/> 与第一个数组比较。</param>
		/// <returns>如果两个源数组的长度相等，并且根据它们类型的默认相等比较器，它们对应的元素相等，则为 <see langword="true"/>; 否则为 <see langword="false"/></returns>
        public static bool Equals<T> (this T[] source, T[] dest)
        {
            return ArrayUtils.Equals (source, dest);
        }

        /// <summary>
		/// 在派生类中重写时，执行相同类型的两个对象的比较，并返回一个值，指示一个对象是小于、等于还是大于另一个对象。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型</typeparam>
		/// <param name="source">第一个要比较的对象</param>
		/// <param name="dest">第二个要比较的对象</param>
		/// <returns>符号表示x和y的相对值的有符号整数</returns>
        public static int Compare<T> (this T[] source, T[] dest)
        {
            return ArrayUtils.Compare (source, dest);
        }

        /// <summary>
		/// 使用特定的值初始化值类型 <see cref="Array"/> 的每一个元素
		/// </summary>
		/// <typeparam name="T">数组的元素类型</typeparam>
		/// <param name="source">一个 <see cref="Array"/> 需要初始化</param>
		/// <param name="value">指定初始化数组的值</param>
        public static void Initialize<T> (this T[] source, T value)
        {
            ArrayUtils.Initialize (source, value);
        }

        /// <summary>
        /// 将数组中的元素依次转换为字符串
        /// </summary>
        /// <typeparam name="T">source 中的元素的类型</typeparam>
        /// <param name="source">转换为字符串的元素数组</param>
        /// <returns>一个字符串, 包含输入数组中所有元素的字符串形式</returns>
        public static string ToString<T> (this T[] source)
        {
            return ArrayUtils.ToString (source);
        }

        /// <summary>
        /// 将指定字节数组中的所有字节解码为一个字符串
        /// </summary>
        /// <param name="source">包含要解码的字节序列的字节数组</param>
        /// <returns>包含指定字节序列解码结果的字符串</returns>
        public static string GetString (this byte[] source)
        {
            return BinaryConvert.ToString (source);
        }

        /// <summary>
        /// 将指定字节数组中的所有字节解码为一个字符串
        /// </summary>
        /// <param name="source">包含要解码的字节序列的字节数组</param>
        /// <param name="encoding">提供对字节序列的编码</param>
        /// <returns>包含指定字节序列解码结果的字符串</returns>
        public static string GetString (this byte[] source, Encoding encoding)
        {
            return BinaryConvert.ToString (source, encoding);
        }

        /// <summary>
        /// 随机返回数组中指定索引处的元素
        /// </summary>
        /// <typeparam name="T">指定序列的类型</typeparam>
        /// <param name="source">进行随机的目标序列</param>
        /// <returns>随机到的元素</returns>
        public static T RandomAt<T> (this T[] source)
        {
            return ArrayUtils.RandomAt (source);
        }
        #endregion
    }
}
