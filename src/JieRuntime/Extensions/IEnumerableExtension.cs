using System.Collections.Generic;

using JieRuntime.Utils;

namespace JieRuntime.Extensions
{
    /// <summary>
    /// 提供一组 <see cref="IEnumerable{T}"/> 接口的扩展方法
    /// </summary>
    public static class IEnumerableExtension
    {
        #region --公开方法--
        /// <summary>
        /// 随机获取枚举器的一个元素
        /// </summary>
        /// <typeparam name="T">要枚举的对象的类型</typeparam>
        /// <param name="source">一个 <see cref="IEnumerable{T}"/>. 来随机获取元素</param>
        /// <returns>随机获取的一个元素. 如果 source 不包含任何元素则返回默认值</returns>
        public static T RandomElementAt<T> (this IEnumerable<T> source)
        {
            return IEnumerableUtils.RandomElementAt (source);
        }
        #endregion
    }
}
