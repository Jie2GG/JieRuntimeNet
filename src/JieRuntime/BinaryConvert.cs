using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace JieRuntime
{
    /// <summary>
    /// 提供基本数据类型和二进制流的转换服务
    /// </summary>
    public static class BinaryConvert
    {
        #region --字段--
        private static readonly Regex hexStrRegex = new (@"^([A-Fa-f\d]{2}\s{0,})+$");
        #endregion

        #region --公开方法--
        /// <summary>
        /// 返回由字节数组中指定位置的两个字节转换来的 Unicode 字符
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由两个字节构成的 Unicode 字符</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static char ToChar (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (ushort));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToChar (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的两个字节转换来的 16 位有符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由两个字节构成的 16 位有符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static short ToInt16 (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (short));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToInt16 (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的两个字节转换来的 16 位无符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由两个字节构成的 16 位无符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static ushort ToUInt16 (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (ushort));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToUInt16 (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的 32 位有符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 32 位有符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static int ToInt32 (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (int));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToInt32 (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的 32 位无符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 32 位无符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static uint ToUInt32 (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (uint));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToUInt32 (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的八个字节转换来的 64 位有符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 64 位有符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static long ToInt64 (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (long));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToInt64 (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的八个字节转换来的 64 位无符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 64 位无符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static ulong ToUInt64 (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (ulong));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToUInt64 (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的单精度浮点数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的单精度浮点数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static float ToSingle (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (float));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToSingle (bytes, startIndex);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的八个字节转换来的双精度浮点数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="startIndex">从指定位置开始读取</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的双精度浮点数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static double ToDouble (byte[] bytes, int startIndex, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            bytes = ConvertFormat (bytes, sizeof (double));
            ConvertReverse (bytes, reverse);
            return BitConverter.ToDouble (bytes, startIndex);
        }

        /// <summary>
        /// 将指定字节数组中的所有字节解码为一个字符串
        /// </summary>
        /// <param name="bytes">包含要解码的字节序列的字节数组</param>
        /// <returns>包含指定字节序列解码结果的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static string ToString (byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToString (bytes, Encoding.UTF8);
        }

        /// <summary>
        /// 将指定字节数组中的所有字节解码为一个字符串
        /// </summary>
        /// <param name="bytes">包含要解码的字节序列的字节数组</param>
        /// <param name="encoding">提供对字节序列的编码</param>
        /// <returns>包含指定字节序列解码结果的字符串</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static string ToString (byte[] bytes, Encoding encoding)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            encoding ??= Encoding.Default;
            return encoding.GetString (bytes);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的两个字节转换来的 Unicode 字符
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由两个字节构成的 Unicode 字符</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static char ToChar (byte[] bytes, bool reverse = false)
        {
            return ToChar (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中开始位置的两个字节转换来的 16 位有符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由两个字节构成的 16 位有符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static short ToInt16 (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToInt16 (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中开始位置的两个字节转换来的 16 位无符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由两个字节构成的 16 位无符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static ushort ToUInt16 (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToUInt16 (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中开始位置的四个字节转换来的 32 位有符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 32 位有符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static int ToInt32 (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToInt32 (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的 32 位无符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 32 位无符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static uint ToUInt32 (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToUInt32 (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中开始位置的八个字节转换来的 64 位有符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 64 位有符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static long ToInt64 (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToInt64 (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中开始位置的八个字节转换来的 64 位无符号整数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的 64 位无符号整数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static ulong ToUInt64 (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToUInt64 (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中开始位置的四个字节转换来的单精度浮点数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的单精度浮点数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static float ToSingle (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToSingle (bytes, 0, reverse);
        }

        /// <summary>
        /// 返回由字节数组中开始位置的八个字节转换来的双精度浮点数
        /// </summary>
        /// <param name="bytes">指定数据存在的字节数组</param>
        /// <param name="reverse">是否反转数组进行数据读取</param>
        /// <returns>由四个字节构成的双精度浮点数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> 是 <see langword="null"/></exception>
        public static double ToDouble (byte[] bytes, bool reverse = false)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            return ToDouble (bytes, 0, reverse);
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 8 位无符号整数值
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <returns>长度为 1 的字节数组</returns>
        public static byte[] GetBytes (byte value)
        {
            return new byte[1] { value };
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 Unicode 字符
        /// </summary>
        /// <param name="value">要转换的 Unicode 字符</param>
        /// <param name="reverse">是否反序转换</param>
        /// <returns>长度为 2 的字节数组</returns>
        public static byte[] GetBytes (char value, bool reverse = false)
        {
            byte[] result = BitConverter.GetBytes (value);
            ConvertReverse (result, reverse);
            return result;
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 16 位有符号整数值
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="reverse">是否反序转换</param>
        /// <returns>长度为 2 的字节数组</returns>
        public static byte[] GetBytes (short value, bool reverse = false)
        {
            byte[] result = BitConverter.GetBytes (value);
            ConvertReverse (result, reverse);
            return result;
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 16 位无符号整数值
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="reverse">是否反序转换</param>
        /// <returns>长度为 2 的字节数组</returns>
        public static byte[] GetBytes (ushort value, bool reverse = false)
        {
            byte[] result = BitConverter.GetBytes (value);
            ConvertReverse (result, reverse);
            return result;
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 32 位有符号整数值
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="reverse">是否反序转换</param>
        /// <returns>长度为 4 的字节数组</returns>
        public static byte[] GetBytes (int value, bool reverse = false)
        {
            byte[] result = BitConverter.GetBytes (value);
            ConvertReverse (result, reverse);
            return result;
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 32 位无符号整数值
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="reverse">是否反序转换</param>
        /// <returns>长度为 4 的字节数组</returns>
        public static byte[] GetBytes (uint value, bool reverse = false)
        {
            byte[] result = BitConverter.GetBytes (value);
            ConvertReverse (result, reverse);
            return result;
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 64 位有符号整数值
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="reverse">是否反序转换</param>
        /// <returns>长度为 8 的字节数组</returns>
        public static byte[] GetBytes (long value, bool reverse = false)
        {
            byte[] result = BitConverter.GetBytes (value);
            ConvertReverse (result, reverse);
            return result;
        }

        /// <summary>
        /// 以字节数组的形式返回指定的 64 位无符号整数值
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="reverse">是否反序转换</param>
        /// <returns>长度为 8 的字节数组</returns>
        public static byte[] GetBytes (ulong value, bool reverse = false)
        {
            byte[] result = BitConverter.GetBytes (value);
            ConvertReverse (result, reverse);
            return result;
        }

        /// <summary>
        /// 将指定字符串中的所有字符编码为一个字节序列
        /// </summary>
        /// <param name="value">包含要编码的字符的字符串</param>
        /// <returns>一个字节数组，包含对指定的字符集进行编码的结果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> 是 <see langword="null"/></exception>
        public static byte[] GetBytes (string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException (nameof (value));
            }

            if ("".CompareTo (value) == 0)
            {
                return Array.Empty<byte> ();
            }

            return GetBytes (value, Encoding.UTF8);
        }

        /// <summary>
        /// 将指定字符串中的所有字符编码为一个字节序列
        /// </summary>
        /// <param name="value">包含要编码的字符的字符串</param>
        /// <param name="encoding">提供对字节序列的编码</param>
        /// <returns>一个字节数组，包含对指定的字符集进行编码的结果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> 是 <see langword="null"/></exception>
        public static byte[] GetBytes (string value, Encoding encoding)
        {
            if (value is null)
            {
                throw new ArgumentNullException (nameof (value));
            }

            if ("".CompareTo (value) == 0)
            {
                return Array.Empty<byte> ();
            }

            encoding ??= Encoding.Default;
            return encoding.GetBytes (value);
        }

        /// <summary>
        /// 将指定字节数组的每个元素的数值转换为它的等效十六进制字符串表示形式
        /// </summary>
        /// <param name="bytes">包含要编码为字符串的字节数组</param>
        /// <returns>由以连字符分隔的十六进制对构成的字符串，其中每一对表示当前封包中对应的元素；例如“7F 2C 4A</returns>
        public static string ToHexString (byte[] bytes)
        {
            if (bytes is null)
            {
                return string.Empty;
            }

            return BitConverter.ToString (bytes).Replace ("-", " ");
        }

        /// <summary>
        /// 将指定字符串中的所有十六进制值转换为它的等效字节数组表示形式
        /// </summary>
        /// <param name="hex">包含要转换为字节数组的十六进制字符串</param>
        /// <returns>一个字节数组，包含对指定的十六进制字符串转换的结果</returns>
        /// <exception cref="ArgumentException"><paramref name="hex"/> 不能为 <see langword="null"/> 或空白</exception>
        /// <exception cref="FormatException"><paramref name="hex"/> 不是十六进制字符串</exception>
        public static byte[] GetHexBytes (string hex)
        {
            if (string.IsNullOrWhiteSpace (hex))
            {
                throw new ArgumentException ($"“{nameof (hex)}”不能为 null 或空白。", nameof (hex));
            }

            hex = hex.Trim ();
            if (!hexStrRegex.IsMatch (hex))
            {
                throw new FormatException ($"指定转换的字符串不是十六进制字符串");
            }

            string[] hexStr = hex.Split (' ');
            return Array.ConvertAll (hexStr, s => Convert.ToByte (s, 16));
        }
        #endregion

        #region --私有方法--
        // 转换反转
        private static void ConvertReverse (byte[] bytes, bool reverse)
        {
            if (reverse)
            {
                Array.Reverse (bytes);
            }
        }
        // 转换格式化
        private static byte[] ConvertFormat (byte[] bytes, int len)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException (nameof (bytes));
            }

            if (len < 0)
            {
                len = 0;
            }

            if (bytes.Length < len)
            {
                byte[] temp = new byte[len];
                Array.Copy (bytes, 0, temp, len - bytes.Length, bytes.Length);
                return temp;
            }

            return bytes;
        }
        #endregion
    }
}
