using System;
using System.Collections.Generic;
using System.Text;

namespace JieRuntime.Utils
{
    /// <summary>
    /// 提供一组数组快速处理方法
    /// </summary>
    public static class ArrayUtils
    {
        #region --公开方法--
        /// <summary>
		/// 将输入的数组按顺序依次拼接到第一个数组的尾部
		/// </summary>
		/// <typeparam name="T">arrs 的元素类型</typeparam>
		/// <param name="arrs">要连接的所有数组</param>
		/// <returns>一个数组, 包含多个输入数组的连接元素。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="arrs"/> 是 <see langword="null"/></exception>
        public static T[] Concat<T> (params T[][] arrs)
        {
            if (arrs is null)
            {
                throw new ArgumentNullException (nameof (arrs));
            }

            long len = 0;
            for (long i = 0; i < arrs.LongLength; i++)
            {
                len += arrs[i].LongLength;
            }
            T[] newArr = new T[len];
            len = 0;
            for (long i = 0; i < arrs.LongLength; i++)
            {
                Array.Copy (arrs[i], 0L, newArr, len, arrs[i].LongLength);
                len += arrs[i].LongLength;
            }
            return newArr;
        }

        /// <summary>
		/// 从数组的开始返回指定数量的连续元素
		/// </summary>
		/// <typeparam name="T">source 的元素类型</typeparam>
		/// <param name="source">返回元素的序列</param>
		/// <param name="count">要返回的元素数量</param>
		/// <returns>一个数组, 包含从输入数组开始的指定数量的元素。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 是 <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="count"/> 不能小于0 并且不能大于 <paramref name="source"/> 的长度</exception>
        public static T[] Left<T> (T[] source, long count)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (count < 0)
            {
                throw new ArgumentException ("获取的数组元素的数量不能小于 0", nameof (count));
            }

            if (count > source.Length)
            {
                throw new ArgumentException ("获取的数组元素的数量不能大于源数组的长度", nameof (count));
            }

            if (count == 0)
            {
                return Array.Empty<T> ();
            }

            T[] newArr = new T[count];
            Array.Copy (source, newArr, newArr.Length);
            return newArr;
        }

        /// <summary>
		/// 跳过数组中指定数量的元素，然后返回剩余元素
		/// </summary>
		/// <typeparam name="T">source 中的元素的类型</typeparam>
		/// <param name="source">返回元素的序列</param>
		/// <param name="count">返回剩余元素前要跳过的元素数量</param>
		/// <returns>一个数组, 其中包含输入数组中的指定数量后出现的元素</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 是 <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="count"/> 不能小于0 并且不能大于 <paramref name="source"/> 的长度</exception>
        public static T[] Skip<T> (T[] source, long count)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            return Right (source, source.Length - count);
        }

        /// <summary>
		/// 从数组的结尾返回指定数量的连续元素
		/// </summary>
		/// <typeparam name="T">source 的元素类型</typeparam>
		/// <param name="source">返回元素的序列</param>
		/// <param name="count">要返回的元素数量</param>
		/// <returns>一个数组, 包含从输入数组结尾的指定数量的元素。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 是 <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="count"/> 不能小于0 并且不能大于 <paramref name="source"/> 的长度</exception>
        public static T[] Right<T> (T[] source, long count)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (count < 0)
            {
                throw new ArgumentException ("获取的数组元素的数量不能小于 0", nameof (count));
            }

            if (count > source.Length)
            {
                throw new ArgumentException ("获取的数组元素的数量不能大于源数组的长度", nameof (count));
            }

            if (count == 0)
            {
                return Array.Empty<T> ();
            }

            T[] newArr = new T[count];
            Array.Copy (source, source.LongLength - count, newArr, 0L, newArr.Length);
            return newArr;
        }

        /// <summary>
		/// 通过使用两个元素类型的默认相等比较器来确定两个数组是否相等。
		/// </summary>
		/// <typeparam name="T">输入数组的元素类型</typeparam>
		/// <param name="source">一个 <see cref="Array"/> 与 dest 进行比较。</param>
		/// <param name="dest">一个 <see cref="Array"/> 与第一个数组比较。</param>
		/// <returns>如果两个源数组的长度相等，并且根据它们类型的默认相等比较器，它们对应的元素相等，则为 <see langword="true"/>; 否则为 <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 和 <paramref name="dest"/> 不能为 <see langword="null"/></exception>
        public static bool Equals<T> (T[] source, T[] dest)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (dest is null)
            {
                throw new ArgumentNullException (nameof (dest));
            }

            // 比较引用
            if (source == dest)
            {
                return true;
            }

            // 比较长度
            if (source?.LongLength == dest?.LongLength)
            {
                // 元素比较
                for (long i = 0, j = 0; i < source!.LongLength && j < dest!.LongLength; i++, j++)
                {
                    if (source[i]?.Equals (dest[j]) != true)
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
		/// 执行相同类型的两个对象的比较，并返回一个值，指示一个对象是小于、等于还是大于另一个对象。
		/// </summary>
		/// <typeparam name="T">要比较的对象的类型</typeparam>
		/// <param name="source">第一个要比较的对象</param>
		/// <param name="dest">第二个要比较的对象</param>
		/// <returns>符号表示x和y的相对值的有符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 和 <paramref name="dest"/> 不能为 <see langword="null"/></exception>
        public static int Compare<T> (T[] source, T[] dest)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (dest is null)
            {
                throw new ArgumentNullException (nameof (dest));
            }

            int num = source.LongLength.CompareTo (dest.LongLength);
            if (num != 0)
            {
                return num;
            }
            for (long i = 0; i < source.LongLength; i++)
            {
                int temp = Comparer<T>.Default.Compare (source[i], dest[i]);
                if (temp != 0)
                {
                    return temp;
                }
            }
            return 0;
        }

        /// <summary>
		/// 使用特定的值初始化值类型 <see cref="Array"/> 的每一个元素
		/// </summary>
		/// <typeparam name="T">数组的元素类型</typeparam>
		/// <param name="source">一个 <see cref="Array"/> 需要初始化</param>
		/// <param name="value">指定初始化数组的值</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 不能为 <see langword="null"/></exception>
        public static void Initialize<T> (T[] source, T value)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            for (long i = 0; i < source.LongLength; i++)
            {
                source[i] = value;
            }
        }

        /// <summary>
        /// 将数组中的元素依次转换为字符串
        /// </summary>
        /// <typeparam name="T">source 中的元素的类型</typeparam>
        /// <param name="source">转换为字符串的元素数组</param>
        /// <returns>一个字符串, 包含输入数组中所有元素的字符串形式</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 不能为 <see langword="null"/></exception>
        public static string ToString<T> (T[] source)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            StringBuilder builder = new ();
            builder.Append ("{");
            foreach (T item in source)
            {
                if (item is null)
                {
                    builder.Append ("null");
                }
                else
                {
                    builder.Append (item.ToString ());
                }
                builder.Append (", ");
            }
            builder.Length -= 2;
            builder.Append ("}");
            return builder.ToString ();
        }

        /// <summary>
        /// 随机返回数组中指定索引处的元素
        /// </summary>
        /// <typeparam name="T">指定序列的类型</typeparam>
        /// <param name="source">进行随机的目标序列</param>
        /// <returns>随机到的元素</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 不能为 <see langword="null"/></exception>
        public static T RandomAt<T> (T[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (source.Length == 0)
            {
                return default;
            }

            return source[RandomUtils.RandomInt64 (0, source.LongLength)];
        }
        #endregion
    }
}
