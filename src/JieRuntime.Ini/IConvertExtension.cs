using System;
using System.Net;

namespace JieRuntime.Ini
{
    /// <summary>
    /// 提供 <see cref="IConvertible"/> 的扩展接口
    /// </summary>
    public interface IConvertExtension
    {
        /// <summary>
        /// 将此实例的值转换为等效 <see cref="DateTimeOffset"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="DateTimeOffset"/></returns>
        DateTimeOffset ToDateTimeOffset (IFormatProvider provider);

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="TimeSpan"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="TimeSpan"/></returns>
        TimeSpan ToTimeSpan (IFormatProvider provider);

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="byte"/> 数组, 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="byte"/> 数组</returns>
        byte[] ToBytes (IFormatProvider provider);

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="Guid"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="Guid"/></returns>
        Guid ToGuid (IFormatProvider provider);

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="Uri"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="Uri"/></returns>
        Uri ToUri (IFormatProvider provider);

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="IPAddress"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="IPAddress"/></returns>
        IPAddress ToIPAddress (IFormatProvider provider);
    }
}
