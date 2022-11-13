using System;

namespace JieRuntime.Utils
{
    /// <summary>
    /// 提供一组随机数快速处理方法
    /// </summary>
    public static class RandomUtils
    {
        #region --属性--
        private static Random random = new ((int)DateTime.Now.Ticks);
        #endregion

        #region --公开方法--
        /// <summary>
        /// 设置随机数种子
        /// </summary>
        /// <param name="seed">指定随机数种子</param>
        public static void SetRandomSeed (int seed)
        {
            random = new Random (seed);
        }

        /// <summary>
        /// 返回一个非负随机 <see cref="ushort"/> 整数
        /// </summary>
        /// <returns>大于或等于 0 且小于 <see cref="ushort.MaxValue"/> 的 16 位有符号整数</returns>
        public static ushort RandomUInt16 ()
        {
            return RandomUInt16 (ushort.MinValue, ushort.MaxValue);
        }

        /// <summary>
        /// 返回在指定范围内的任意整数
        /// </summary>
        /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
        /// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。 <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
        /// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 的 16 位无符号整数，即：返回的值范围包括 <paramref name="minValue"/> 但不包括 <paramref name="maxValue"/>。 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 <paramref name="minValue"/>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> 大于 <paramref name="maxValue"/></exception>
        public static ushort RandomUInt16 (ushort minValue, ushort maxValue)
        {
            return (ushort)random.Next (minValue, maxValue);
        }

        /// <summary>
        /// 返回一个非负随机整数
        /// </summary>
        /// <returns>大于或等于 0 且小于 <see cref="int.MaxValue"/> 的 32 位有符号整数</returns>
        public static int RandomInt32 ()
        {
            return RandomInt32 (0, int.MaxValue);
        }

        /// <summary>
        /// 返回一个随机整数
        /// </summary>
        /// <returns>大于或等于 0 且小于 <see cref="uint.MaxValue"/> 的 32 位无符号整数</returns>
        public static uint RandomUInt32 ()
        {
            return BinaryConvert.ToUInt32 (BinaryConvert.GetBytes (RandomInt32 (int.MinValue, int.MaxValue)));
        }

        /// <summary>
        /// 返回在指定范围内的任意整数
        /// </summary>
        /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
        /// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。 <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
        /// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 的 32 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 但不包括 <paramref name="maxValue"/>。 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 <paramref name="minValue"/>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> 大于 <paramref name="maxValue"/></exception>
        public static int RandomInt32 (int minValue, int maxValue)
        {
            return random.Next (minValue, maxValue);
        }

        /// <summary>
        /// 返回一个非负随机整数
        /// </summary>
        /// <returns>大于或等于 0 且小于 <see cref="long.MaxValue"/> 的 64 位有符号整数</returns>
        public static long RandomInt64 ()
        {
            return RandomInt64 (0L, long.MaxValue);
        }

        /// <summary>
        /// 返回一个随机整数
        /// </summary>
        /// <returns>大于或等于 0 且小于 <see cref="ulong.MaxValue"/> 的 64 位无符号整数</returns>
        public static ulong RandomUInt64 ()
        {
            return BinaryConvert.ToUInt64 (BinaryConvert.GetBytes (RandomInt64 (long.MinValue, long.MaxValue)));
        }

        /// <summary>
        /// 返回在指定范围内的任意整数
        /// </summary>
        /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
        /// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。 <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
        /// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 的 64 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 但不包括 <paramref name="maxValue"/>。 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 <paramref name="minValue"/>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> 大于 <paramref name="maxValue"/></exception>
        public static long RandomInt64 (long minValue, long maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException ($"{nameof (minValue)} 不能大于 {nameof (maxValue)}");
            }

            double Key = random.NextDouble ();
            return minValue + (long)((maxValue - minValue) * Key);
        }

        /// <summary>
        /// 返回指定长度的随机字节数组
        /// </summary>
        /// <param name="len">要随机的长度</param>
        /// <returns>一个新的字节数组, 内容是随机值</returns>
        public static byte[] RandomBytes (long len)
        {
            if (len < 0)
            {
                throw new ArgumentOutOfRangeException (nameof (len), "随机数组的长度不能小于 0");
            }

            if (len == 0)
            {
                return Array.Empty<byte> ();
            }

            byte[] buf = new byte[len];
            random.NextBytes (buf);
            return buf;
        }
        #endregion
    }
}
