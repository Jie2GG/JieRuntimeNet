namespace JieRuntime.Ini
{
    /// <summary>
    /// 表示 Ini 值类型的枚举
    /// </summary>
    public enum IniValueType
    {
        /// <summary>
        /// 表示不可适用于 Ini 值类型
        /// </summary>
        NotApplicable,
        /// <summary>
        /// 表示一个空值, 通常是 <see langword="null"/>
        /// </summary>
        Empty,
        /// <summary>
        /// 表示布尔值的简单类型 <see langword="true"/> 或 <see langword="false"/>
        /// </summary>
        Boolean,
        /// <summary>
        /// 表示整数值的类型
        /// </summary>
        Integer,
        /// <summary>
        /// 表示浮点数值的类型
        /// </summary>
        Float,
        /// <summary>
        /// 表示日期和时间值的类型
        /// </summary>
        DateTime,
        /// <summary>
        /// 表示时间间的类型
        /// </summary>
        TimeSpan,
        /// <summary>
        /// 表示字节数组的类型
        /// </summary>
        Bytes,
        /// <summary>
        /// 表示 Unicode 字符字符串的密封的类类型
        /// </summary>
        String,
        /// <summary>
        /// 表示全局唯一标识(GUID)的类型
        /// </summary>
        Guid,
        /// <summary>
        /// 表示统一资源标识符 (URI) 的类型
        /// </summary>
        Uri,
        /// <summary>
        /// 表示IP地址 (IPv4 &amp; IPv6) 的类型
        /// </summary>
        IPAddress,
    }
}
