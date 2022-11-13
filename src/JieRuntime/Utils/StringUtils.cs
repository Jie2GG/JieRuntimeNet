using System;
using System.Collections.Generic;

namespace JieRuntime.Utils
{
    /// <summary>
    /// 提供一组字符串快速处理方法
    /// </summary>
    public static class StringUtils
    {
        #region --公开方法--
        /// <summary>
        /// 取出字符串中指定特征中间的字符串数组
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="startStr">指定匹配的开始特征</param>
        /// <param name="endStr">指定匹配的结束特征</param>
        /// <param name="startIndex">指定开始查找的位置, 默认: 0</param>
        /// <returns>一个字符串数组, 包含从源字符串中匹配到的所有结果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>、<paramref name="startStr"/> 和 <paramref name="endStr"/> 不能为 <see langword="null"/></exception>
        public static string[] GetMidString (string source, string startStr, string endStr, int startIndex = 0)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (startStr is null)
            {
                throw new ArgumentNullException (nameof (startStr));
            }

            if (endStr is null)
            {
                throw new ArgumentNullException (nameof (endStr));
            }

            if (source.Length > 0)
            {
                List<string> result = new ();

                unsafe
                {
                    int start = 0;          // 要截取的字符串开始位置
                    int length = 0;         // 要截取的字符串结束位置
                    bool searchStart = true;

                    fixed (char* pStr = source)     // 获取指针
                    {
                        for (int i = startIndex; i < source.Length; i++)
                        {
                            // 搜索到和 startStr 一致的字符串
                            if (searchStart && source[i] == startStr[0] && CompareString (pStr, i, startStr))
                            {
                                start = i + startStr.Length;
                                searchStart = false;
                            }
                            // 搜索到和 endStr 一致的字符串
                            else if (!searchStart && source[i] == endStr[0] && CompareString (pStr, i, endStr))
                            {
                                length = i - start;
                                searchStart = true;

                                result.Add (new string (pStr, start, length));
                            }
                        }
                    }
                }

                return result.ToArray ();
            }

            return new string[0];
        }

        /// <summary>
        /// 取出目标字符串在源字符串中出现的次数
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="targetStr">要匹配的目标字符串</param>
        /// <returns>一个整数, 指示目标字符串的出现次数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 和 <paramref name="targetStr"/> 不能为 <see langword="null"/></exception>
        public static int StringCount (string source, string targetStr)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (targetStr is null)
            {
                throw new ArgumentNullException (nameof (targetStr));
            }

            if (source.Length > 0)
            {
                unsafe
                {
                    int result = 0;

                    fixed (char* pStr = source)
                    {
                        for (int i = 0; i < source.Length; i++)
                        {
                            // 搜索到和 startStr 一致的字符串
                            if (source[i] == targetStr[0] && CompareString (pStr, i, targetStr))
                            {
                                result++;
                            }
                        }
                    }

                    return result;
                }
            }

            return 0;
        }
        #endregion

        #region --私有方法--
        // 快速比较字符串
        private static unsafe bool CompareString (char* pSrcStr, int startIndex, string targetStr)
        {
            for (int i = 0; i < targetStr.Length; i++)
            {
                if (pSrcStr[startIndex + i] != targetStr[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
