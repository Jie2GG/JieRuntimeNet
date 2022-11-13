using System;
using System.Reflection;

namespace JieRuntime.Utils
{
    /// <summary>
    /// 提供一组用于快速反射的方法
    /// </summary>
    public static class ReflectionUtils
    {
        #region --公开方法--
        /// <summary>
        /// 获取对象中指定字段的值, 其搜索的范围包含 <see cref="BindingFlags.Public"/>、<see cref="BindingFlags.NonPublic"/> 和 <see cref="BindingFlags.Instance"/>
        /// </summary>
        /// <param name="source">要获取其字段值的源对象</param>
        /// <param name="name">字段的名称</param>
        /// <returns>如果对象中包含指定的字段, 返回字段对应的值; 否则返回 <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 不能为 <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> 不能为 <see langword="null"/> 或空</exception>
        public static object GetFieldValue (object source, string name)
        {
            return GetFieldValue (source, name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// 获取对象中指定字段的值, 并指定对象成员类型的搜索方式
        /// </summary>
        /// <param name="source">要获取其字段值的源对象</param>
        /// <param name="name">字段的名称</param>
        /// <param name="flags">指定对象成员的检索方式</param>
        /// <returns>如果对象中包含指定的字段, 返回字段对应的值; 否则返回 <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 不能为 <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> 不能为 <see langword="null"/> 或空</exception>
        public static object GetFieldValue (object source, string name, BindingFlags flags)
        {
            if (source is null)
            {
                throw new ArgumentNullException (nameof (source));
            }

            if (string.IsNullOrWhiteSpace (name))
            {
                throw new ArgumentException ($"“{nameof (name)}”不能为 null 或空白。", nameof (name));
            }

            try
            {
                Type type = source.GetType ();
                FieldInfo fieldInfo = type?.GetField (name, flags);
                return fieldInfo?.GetValue (source);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}