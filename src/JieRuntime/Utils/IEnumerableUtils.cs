using System;
using System.Collections.Generic;
using System.Linq;

namespace JieRuntime.Utils
{
    /// <summary>
    /// 提供一组 <see cref="IEnumerable{T}"/> 接口的快速处理方法
    /// </summary>
    public static class IEnumerableUtils
    {
        #region --公开方法--
        /// <summary>
        /// 随机获取枚举器的一个元素
        /// </summary>
        /// <typeparam name="T">要枚举的对象的类型</typeparam>
        /// <param name="source">一个 <see cref="IEnumerable{T}"/>. 来随机获取元素</param>
        /// <returns>随机获取的一个元素. 如果 source 不包含任何元素则返回默认值</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 不能为 <see langword="null"/></exception>
        public static T RandomElementAt<T> (IEnumerable<T> source)
        {
            return source is null
                ? throw new ArgumentNullException (nameof (source))
                : !source.Any () ? default : source.ElementAtOrDefault (RandomUtils.RandomInt32 (0, source.Count ()));
        }
        #endregion
    }
}
