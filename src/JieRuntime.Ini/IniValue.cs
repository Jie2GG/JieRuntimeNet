using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Numerics;

namespace JieRuntime.Ini
{
    /// <summary>
    /// 描述配置项 (Ini) 值的类
    /// </summary>
    public sealed class IniValue : IComparable, IComparable<IniValue>, ICloneable, IConvertible, IConvertExtension
    {
        #region --字段--
        private IniValueType valueType;
        private object value;
        private static readonly IniValueType[] ConvertBooleanTypes = new IniValueType[]
        {
            IniValueType.Integer,
            IniValueType.Float,
            IniValueType.String,
            IniValueType.Boolean
        };
        private static readonly IniValueType[] ConvertBytesTypes = new IniValueType[]
        {
            IniValueType.Bytes,
            IniValueType.String,
            IniValueType.Integer
        };
        private static readonly IniValueType[] ConvertCharTypes = new IniValueType[]
        {
            IniValueType.Integer,
            IniValueType.Float,
            IniValueType.String
        };
        private static readonly IniValueType[] ConvertDateTimeTypes = new IniValueType[]
        {
            IniValueType.DateTime,
            IniValueType.String,
        };
        private static readonly IniValueType[] ConvertGuidTypes = new IniValueType[]
        {
            IniValueType.String,
            IniValueType.Guid,
            IniValueType.Bytes
        };
        private static readonly IniValueType[] ConvertNumberTypes = new IniValueType[]
        {
            IniValueType.Integer,
            IniValueType.Float,
            IniValueType.String,
            IniValueType.Boolean
        };
        private static readonly IniValueType[] ConvertStringTypes = new IniValueType[]
        {
            IniValueType.Empty,
            IniValueType.DateTime,
            IniValueType.Integer,
            IniValueType.Float,
            IniValueType.String,
            IniValueType.Boolean,
            IniValueType.Bytes,
            IniValueType.Guid,
            IniValueType.TimeSpan,
            IniValueType.Uri
        };
        private static readonly IniValueType[] ConvertTimeSpanTypes = new IniValueType[]
        {
            IniValueType.String,
            IniValueType.TimeSpan
        };
        private static readonly IniValueType[] ConvertUirTypes = new IniValueType[]
        {
            IniValueType.String,
            IniValueType.Uri
        };
        private static readonly IniValueType[] ConvertIPAddressTypes = new IniValueType[]
        {
            IniValueType.String,
            IniValueType.Bytes,
            IniValueType.IPAddress
        };
        #endregion

        #region --属性--
        /// <summary>
        /// 获取当前实例描述值的类型
        /// </summary>
        public IniValueType ValueType => this.valueType;

        /// <summary>
        /// 获取当前实例描述的值
        /// </summary>
        public object Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.valueType = GetValueType (value);
            }
        }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        public IniValue ()
            : this (null, IniValueType.Empty)
        { }

        /// <summary>
        /// 使用 <see cref="bool"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="bool"/> 对象</param>
        public IniValue (bool value)
            : this (value, IniValueType.Boolean)
        { }

        /// <summary>
        /// 使用 <see cref="sbyte"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="sbyte"/> 对象</param>
        public IniValue (sbyte value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="byte"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="byte"/> 对象</param>
        public IniValue (byte value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="char"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="char"/> 对象</param>
        public IniValue (char value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="short"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="short"/> 对象</param>
        public IniValue (short value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="ushort"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="ushort"/> 值</param>
        public IniValue (ushort value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="int"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="int"/> 值</param>
        public IniValue (int value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="uint"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="uint"/> 值</param>
        public IniValue (uint value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="long"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="long"/> 值</param>
        public IniValue (long value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="ulong"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="ulong"/> 值</param>
        public IniValue (ulong value)
            : this (value, IniValueType.Integer)
        { }

        /// <summary>
        /// 使用 <see cref="float"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="float"/> 值</param>
        public IniValue (float value)
            : this (value, IniValueType.Float)
        { }

        /// <summary>
        /// 使用 <see cref="double"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="double"/> 值</param>
        public IniValue (double value)
            : this (value, IniValueType.Float)
        { }

        /// <summary>
        /// 使用 <see cref="decimal"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="decimal"/> 值</param>
        public IniValue (decimal value)
            : this (value, IniValueType.Float)
        { }

        /// <summary>
        /// 使用 <see cref="DateTime"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="DateTime"/> 值</param>
        public IniValue (DateTime value)
            : this (value, IniValueType.DateTime)
        { }

        /// <summary>
        /// 使用 <see cref="DateTimeOffset"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="DateTimeOffset"/> 值</param>
        public IniValue (DateTimeOffset value)
            : this (value, IniValueType.DateTime)
        { }

        /// <summary>
        /// 使用 <see cref="TimeSpan"/> 值来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="TimeSpan"/> 值</param>
        public IniValue (TimeSpan value)
            : this (value, IniValueType.TimeSpan)
        { }

        /// <summary>
        /// 使用 <see cref="byte"/> 数组来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="byte"/> 数组</param>
        public IniValue (byte[] value)
            : this (value, IniValueType.Bytes)
        { }

        /// <summary>
        /// 使用 <see cref="string"/> 对象来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="string"/> 对象</param>
        public IniValue (string value)
            : this (value, IniValueType.String)
        { }

        /// <summary>
        /// 使用 <see cref="Guid"/> 对象来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="Guid"/> 对象</param>
        public IniValue (Guid value)
            : this (value, IniValueType.Guid)
        { }

        /// <summary>
        /// 使用 <see cref="Uri"/> 对象来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="Uri"/> 对象</param>
        public IniValue (Uri value)
            : this (value, IniValueType.Uri)
        { }

        /// <summary>
        /// 使用 <see cref="IPAddress"/> 对象来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个 <see cref="IPAddress"/> 对象</param>
        public IniValue (IPAddress value)
            : this (value, IniValueType.IPAddress)
        { }

        /// <summary>
        /// 使用基础数据类型的可空对象来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">一个可空的基础数据类型对象</param>
        public IniValue (object value)
            : this (value, GetValueType (value))
        { }

        /// <summary>
        /// 使用指定的值和类型来初始化 <see cref="IniValue"/> 类的新实例
        /// </summary>
        /// <param name="value">初始化的值</param>
        /// <param name="valueType">值的Ini类型</param>
        private IniValue (object value, IniValueType valueType)
        {
            this.value = value;
            this.valueType = valueType;
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 将此实例的值转换为等效的布尔值
        /// </summary>
        /// <returns>一个与此实例的值等效的布尔值</returns>
        public bool ToBoolean ()
        {
            return this.ToBoolean (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效的布尔值使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>一个与此实例的值等效的布尔值</returns>
        public bool ToBoolean (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertBooleanTypes, false))
            {
                throw new ArgumentException ("无法转换为 Boolean 类型");
            }

            if (this.Value is BigInteger v1)
            {
                return Convert.ToBoolean (v1, provider);
            }

            return Convert.ToBoolean (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效的 Unicode 字符
        /// </summary>
        /// <returns>一个与此实例的值等效的 Unicode 字符</returns>
        public char ToChar ()
        {
            return this.ToChar (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效的 Unicode 字符使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"> </param>
        /// <returns>一个与此实例的值等效的 Unicode 字符</returns>
        public char ToChar (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertCharTypes, false))
            {
                throw new ArgumentException ("无法转换为 Char 类型");
            }

            if (this.Value is BigInteger v1)
            {
                return (char)(ushort)v1;
            }

            return Convert.ToChar (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 8 位有符号整数
        /// </summary>
        /// <returns>为此实例的值等效的 8 位有符号的整数</returns>
        public sbyte ToSByte ()
        {
            return this.ToSByte (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为使用指定的区域性特定格式设置信息的等效 8 位有符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>为此实例的值等效的 8 位有符号的整数</returns>
        public sbyte ToSByte (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 SByte 类型");
            }

            if (this.Value is BigInteger v2)
            {
                return (sbyte)v2;
            }

            return Convert.ToSByte (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 8 位无符号整数
        /// </summary>
        /// <returns>为此实例的值等效的 8 位无符号的整数</returns>
        public byte ToByte ()
        {
            return this.ToByte (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为使用指定的区域性特定格式设置信息的等效 8 位无符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>为此实例的值等效的 8 位无符号的整数</returns>
        public byte ToByte (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertCharTypes, false))
            {
                throw new ArgumentException ("无法转换为 Byte 类型");
            }

            if (this.Value is BigInteger v2)
            {
                return (byte)v2;
            }

            return Convert.ToByte (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 16 位有符号整数
        /// </summary>
        /// <returns>此实例的值等效的 16 位有符号的整数</returns>
        public short ToInt16 ()
        {
            return this.ToInt16 (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为使用指定的区域性特定格式设置信息的等效 16 位有符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 16 位有符号的整数</returns>
        public short ToInt16 (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 Int16 类型");
            }

            if (this.Value is BigInteger v2)
            {
                return (short)v2;
            }

            return Convert.ToInt16 (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 16 位无符号整数
        /// </summary>
        /// <returns>为此实例的值等效的 16 位无符号的整数</returns>
        public ushort ToUInt16 ()
        {
            return this.ToUInt16 (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为使用指定的区域性特定格式设置信息的等效 16 位无符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>为此实例的值等效的 16 位无符号的整数</returns>
        public ushort ToUInt16 (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 UInt16 类型");
            }

            if (this.Value is BigInteger v2)
            {
                return (ushort)v2;
            }

            return Convert.ToUInt16 (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 32 位有符号整数
        /// </summary>
        /// <returns>此实例的值等效的 32 位有符号的整数</returns>
        public int ToInt32 ()
        {
            return this.ToInt32 (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为使用指定的区域性特定格式设置信息的等效 32 位有符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 32 位有符号的整数</returns>
        public int ToInt32 (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 Int32 类型");
            }

            if (this.Value is BigInteger v2)
            {
                return (int)v2;
            }

            return Convert.ToInt32 (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 32 位无符号整数
        /// </summary>
        /// <returns>此实例的值等效的 32 位无符号的整数</returns>
        public uint ToUInt32 ()
        {
            return this.ToUInt32 (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效使用指定的区域性特定格式设置信息的 32 位无符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 32 位无符号的整数</returns>
        public uint ToUInt32 (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 UInt32 类型");
            }

            if (this.Value is BigInteger v2)
            {
                return (uint)v2;
            }

            return Convert.ToUInt32 (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 64 位有符号整数
        /// </summary>
        /// <returns>此实例的值等效的 64 位有符号的整数</returns>
        public long ToInt64 ()
        {
            return this.ToInt64 (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为使用指定的区域性特定格式设置信息的等效 64 位有符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 64 位有符号的整数</returns>
        public long ToInt64 (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 Int64 类型");
            }

            if (this.Value is BigInteger v1)
            {
                return (long)v1;
            }

            return Convert.ToInt64 (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 64 位无符号整数
        /// </summary>
        /// <returns>此实例的值等效的 64 位无符号的整数</returns>
        public ulong ToUInt64 ()
        {
            return this.ToUInt64 (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效使用指定的区域性特定格式设置信息的 64 位无符号整数
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 64 位无符号的整数</returns>
        public ulong ToUInt64 (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 UInt64 类型");
            }
            if (this.Value is BigInteger v2)
            {
                return (ulong)v2;
            }
            return Convert.ToUInt64 (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效单精度浮点数
        /// </summary>
        /// <returns>此实例的值等效的单精度浮点数</returns>
        public float ToSingle ()
        {
            return this.ToSingle (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效单精度浮点数使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的单精度浮点数</returns>
        public float ToSingle (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 Single 类型");
            }
            if (this.Value is BigInteger v2)
            {
                return (float)v2;
            }
            return Convert.ToSingle (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效双精度浮点数
        /// </summary>
        /// <returns>此实例的值等效的双精度浮点数</returns>
        public double ToDouble ()
        {
            return this.ToDouble (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效双精度浮点数使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的双精度浮点数</returns>
        public double ToDouble (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 Double 类型");
            }
            if (this.Value is BigInteger v2)
            {
                return (double)v2;
            }
            return Convert.ToDouble (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="decimal"/> 数字
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="decimal"/></returns>
        public decimal ToDecimal ()
        {
            return this.ToDecimal (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="decimal"/> 数字使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="decimal"/></returns>
        public decimal ToDecimal (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertNumberTypes, false))
            {
                throw new ArgumentException ("无法转换为 Decimal 类型");
            }
            if (this.Value is BigInteger v2)
            {
                return (decimal)v2;
            }
            return Convert.ToDecimal (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="DateTime"/>
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="DateTime"/></returns>
        public DateTime ToDateTime ()
        {
            return this.ToDateTime (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="DateTime"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="DateTime"/></returns>
        public DateTime ToDateTime (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertDateTimeTypes, false))
            {
                throw new ArgumentException ("无法转换为 DateTime 类型");
            }

            if (this.Value is DateTime dateTime)
            {
                return dateTime;
            }

            if (this.Value is DateTimeOffset offset)
            {
                return offset.DateTime;
            }

            return Convert.ToDateTime (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="string"/>
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="string"/></returns>
        public override string ToString ()
        {
            return this.ToString (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="string"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="string"/></returns>
        public string ToString (IFormatProvider provider)
        {
            if (this.Value is null)
            {
                return string.Empty;
            }

            if (!IsConvert (this.ValueType, ConvertStringTypes, true))
            {
                throw new ArgumentException ("无法转换为 String 类型");
            }

            if (this.Value is byte[] v2)
            {
                return Convert.ToBase64String (v2);
            }

            if (this.Value is BigInteger v3)
            {
                return v3.ToString (provider);
            }

            if (this.Value is Uri v4)
            {
                return v4.ToString ();
            }

            if (this.Value is IPAddress v5)
            {
                return v5.ToString ();
            }

            return Convert.ToString (this.Value, provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="DateTimeOffset"/>
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="DateTimeOffset"/></returns>
        public DateTimeOffset ToDateTimeOffset ()
        {
            return this.ToDateTimeOffset (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="DateTimeOffset"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="DateTimeOffset"/></returns>
        public DateTimeOffset ToDateTimeOffset (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertDateTimeTypes, false))
            {
                throw new ArgumentException ("无法转换为 DateTimeOffset 类型");
            }

            if (this.Value is DateTimeOffset v1)
            {
                return v1;
            }

            if (this.Value is string v2)
            {
                return DateTimeOffset.Parse (v2, provider);
            }

            return new DateTimeOffset (this.ToDateTime (provider));
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="TimeSpan"/>
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="TimeSpan"/></returns>
        public TimeSpan ToTimeSpan ()
        {
            return this.ToTimeSpan (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="TimeSpan"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="TimeSpan"/></returns>
        public TimeSpan ToTimeSpan (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertTimeSpanTypes, false))
            {
                throw new ArgumentException ("无法转换为 TimeSpan 类型");
            }

            if (this.Value is TimeSpan v1)
            {
                return v1;
            }

            return TimeSpan.Parse (this.ToString (provider), provider);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="byte"/> 数组
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="byte"/> 数组</returns>
        public byte[] ToBytes ()
        {
            return this.ToBytes (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="byte"/> 数组, 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="byte"/> 数组</returns>
        public byte[] ToBytes (IFormatProvider provider)
        {
            if (this.Value is null)
            {
                return Array.Empty<byte> ();
            }

            if (!IsConvert (this.ValueType, ConvertBytesTypes, true))
            {
                throw new ArgumentException ("无法转换为 Byte[] 类型");
            }

            if (this.Value is string s1)
            {
                return Convert.FromBase64String (s1);
            }

            if (this.Value is BigInteger v1)
            {
                return v1.ToByteArray ();
            }

            if (this.Value is byte[] v2)
            {
                return v2;
            }

            if (this.Value is IPAddress v3)
            {
                return v3.GetAddressBytes ();
            }

            throw new ArgumentException ("无法转换为 Byte[] 类型");
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="Guid"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="Guid"/></returns>
        public Guid ToGuid ()
        {
            return this.ToGuid (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="Guid"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="Guid"/></returns>
        public Guid ToGuid (IFormatProvider provider)
        {
            if (this.Value is null || !IsConvert (this.ValueType, ConvertGuidTypes, false))
            {
                throw new ArgumentException ("无法转换为 Guid 类型");
            }

            if (this.Value is Guid guid)
            {
                return guid;
            }

            if (this.Value is byte[] v1)
            {
                return new Guid (v1);
            }

            return new Guid (this.ToString (provider));
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="Uri"/>
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="Uri"/></returns>
        public Uri ToUri ()
        {
            return this.ToUri (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="Uri"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="Uri"/></returns>
        public Uri ToUri (IFormatProvider provider)
        {
            if (this.Value is null)
            {
                return null;
            }

            if (!IsConvert (this.ValueType, ConvertUirTypes, true))
            {
                throw new ArgumentException ("无法转换为 Uri 类型");
            }

            if (this.Value is Uri uri)
            {
                return uri;
            }

            return new Uri (this.ToString (provider), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="IPAddress"/>
        /// </summary>
        /// <returns>此实例的值等效的 <see cref="IPAddress"/></returns>
        public IPAddress ToIPAddress ()
        {
            return this.ToIPAddress (CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例的值转换为等效 <see cref="IPAddress"/> 使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns>此实例的值等效的 <see cref="IPAddress"/></returns>
        public IPAddress ToIPAddress (IFormatProvider provider)
        {
            if (this.Value is null)
            {
                return null;
            }

            if (!IsConvert (this.ValueType, ConvertIPAddressTypes, true))
            {
                throw new ArgumentException ("无法转换为 IPAddress 类型");
            }

            if (this.Value is IPAddress ipAddr)
            {
                return ipAddr;
            }

            if (this.Value is string strAddr)
            {
                return IPAddress.Parse (strAddr);
            }

            if (this.Value is byte[] v2)
            {
                return new IPAddress (v2);
            }

            throw new ArgumentException ("无法转换为 IPAddress 类型");
        }

        /// <summary>
        /// 将此实例与的值转换 <see cref="object"/> 指定 <see cref="Type"/>，具有等效值
        /// </summary>
        /// <param name="conversionType"><see cref="Type"/> 此实例的值转换为</param>
        /// <returns><see cref="object"/> 类型的实例 conversionType 其值等效于此实例的值</returns>
        public object ToType (Type conversionType)
        {
            return this.ToType (conversionType, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 将此实例与的值转换 <see cref="object"/> 指定 <see cref="Type"/>，具有等效值，使用指定的区域性特定格式设置信息
        /// </summary>
        /// <param name="conversionType"><see cref="Type"/> 此实例的值转换为</param>
        /// <param name="provider"><see cref="IFormatProvider"/> 接口实现，提供区域性特定格式设置信息</param>
        /// <returns><see cref="object"/> 类型的实例 conversionType 其值等效于此实例的值</returns>
        public object ToType (Type conversionType, IFormatProvider provider)
        {
            if (conversionType.Equals (typeof (byte[])))
            {
                return this.ToBytes (provider);
            }

            if (conversionType.Equals (typeof (DateTimeOffset)))
            {
                return this.ToDateTimeOffset (provider);
            }

            if (conversionType.Equals (typeof (Guid)))
            {
                return this.ToGuid (provider);
            }

            if (conversionType.Equals (typeof (Uri)))
            {
                return this.ToUri (provider);
            }

            if (conversionType.Equals (typeof (TimeSpan)))
            {
                return this.ToTimeSpan (provider);
            }

            return Convert.ChangeType (this.Value, conversionType, provider);
        }

        /// <summary>
        /// 返回此实例的 <see cref="TypeCode"/>
        /// </summary>
        /// <returns>枚举的常数，它是 <see cref="TypeCode"/> 实现此接口的类或值类型</returns>
        public TypeCode GetTypeCode ()
        {
            if (this.Value is null)
            {
                return TypeCode.Empty;
            }

            if (this.Value is IConvertible convertible)
            {
                return convertible.GetTypeCode ();
            }

            return TypeCode.Object;
        }

        /// <summary>
        /// 将当前实例与同一类型的另一个对象进行比较，并返回一个整数，该整数指示当前实例在排序顺序中的位置是位于另一个对象之前、之后还是与其位置相同。
        /// </summary>
        /// <param name="obj">与此实例进行比较的对象</param>
        /// <returns>一个值，指示要比较的对象的相对顺序。 返回值的含义如下： 值 含义 小于零 此实例在排序顺序中位于 obj 之前。 零 此实例在排序顺序中的同一位置中发生obj。大于零 此实例在排序顺序中位于 obj 之后。</returns>
        public int CompareTo (object obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is IniValue value)
            {
                return Compare (this, value);
            }

            return Comparer<object>.Default.Compare (this.Value, obj);
        }

        /// <summary>
        /// 将当前实例与同一类型的另一个对象进行比较，并返回一个整数，该整数指示当前实例在排序顺序中的位置是位于另一个对象之前、之后还是与其位置相同。
        /// </summary>
        /// <param name="other">与此实例进行比较的对象</param>
        /// <returns>一个值，指示要比较的对象的相对顺序。 返回值的含义如下： 值 含义 小于零 此实例在排序顺序中位于 other 之前。 零 此实例中出现的相同位置在排序顺序中是other。 大于零 此实例在排序顺序中位于 other 之后。</returns>
        public int CompareTo (IniValue other)
        {
            if (other == null)
            {
                return 1;
            }

            if (this.ValueType == IniValueType.String && this.ValueType != other.ValueType)
            {
                return Compare (other, this);
            }
            else
            {
                return Compare (this, other);
            }
        }

        /// <summary>
        /// 创建作为当前实例副本的新对象
        /// </summary>
        /// <returns>作为此实例副本的新对象</returns>
        public object Clone ()
        {
            return new IniValue (this.Value, this.ValueType);
        }

        /// <summary>
        /// 获取指定对象描述为 Ini
        /// </summary>
        /// <param name="value">获取类型的值</param>
        /// <returns><see cref="IniValueType"/> 枚举的值</returns>
        public static IniValueType GetValueType (object value)
        {
            if (value is null)
            {
                return IniValueType.Empty;
            }

            if (value is bool)
            {
                return IniValueType.Boolean;
            }

            if (value is sbyte || value is byte || value is char || value is short || value is ushort || value is int || value is uint || value is long || value is ulong)
            {
                return IniValueType.Integer;
            }

            if (value is float || value is double || value is decimal)
            {
                return IniValueType.Float;
            }

            if (value is DateTime || value is DateTimeOffset)
            {
                return IniValueType.DateTime;
            }

            if (value is TimeSpan)
            {
                return IniValueType.TimeSpan;
            }

            if (value is byte[])
            {
                return IniValueType.Bytes;
            }

            if (value is Guid)
            {
                return IniValueType.Guid;
            }

            if (value is Uri)
            {
                return IniValueType.Uri;
            }

            if (value is IPAddress)
            {
                return IniValueType.IPAddress;
            }

            if (value is string)
            {
                return IniValueType.String;
            }

            return IniValueType.NotApplicable;
        }

        /// <summary>
        /// 比较两个指定的 <see cref="IniValue"/> 对象，并返回一个指示二者在排序顺序中的相对位置的整数
        /// </summary>
        /// <param name="valueA">要比较的第一个对象</param>
        /// <param name="valueB">要比较的第二个对象</param>
        /// <returns>一个 32 位带符号整数，指示两个比较数之间的关系。</returns>
        public static int Compare (IniValue valueA, IniValue valueB)
        {
            #region 引用判断
            if (valueA == valueB)
            {
                return 0;
            }

            if (valueB is null)
            {
                return 1;
            }

            if (valueA is null)
            {
                return -1;
            }
            #endregion

            #region 数据比较
            switch (valueA.ValueType)
            {
                case IniValueType.Boolean:
                    bool b1 = Convert.ToBoolean (valueA.Value, CultureInfo.CurrentCulture);
                    bool b2 = Convert.ToBoolean (valueB.Value, CultureInfo.CurrentCulture);
                    return b1.CompareTo (b2);
                case IniValueType.Integer:
                    if (valueA.Value is BigInteger intA)
                    {
                        return CompareBigInteger (intA, valueB.value);
                    }
                    if (valueB.Value is BigInteger intB)
                    {
                        return -CompareBigInteger (intB, valueA.Value);
                    }
                    if (valueA.Value is ulong || valueA.Value is decimal || valueB.Value is ulong || valueB.Value is decimal)
                    {
                        decimal decA = Convert.ToDecimal (valueA.Value, CultureInfo.CurrentCulture);
                        decimal decB = Convert.ToDecimal (valueB.Value, CultureInfo.CurrentCulture);
                        return decA.CompareTo (decB);
                    }
                    if (valueA.Value is float || valueA.Value is double || valueB.Value is float || valueB.Value is double)
                    {
                        return CompareFloat (valueA.Value, valueB.Value);
                    }
                    long l1 = Convert.ToInt64 (valueA.Value, CultureInfo.CurrentCulture);
                    long l2 = Convert.ToInt64 (valueB.Value, CultureInfo.CurrentCulture);
                    return l1.CompareTo (l2);
                case IniValueType.Float:
                    if (valueA.Value is BigInteger fltA)
                    {
                        return CompareBigInteger (fltA, valueB.Value);
                    }
                    if (valueB.Value is BigInteger fltB)
                    {
                        return -CompareBigInteger (fltB, valueA.Value);
                    }
                    if (valueA.Value is ulong || valueA.Value is decimal || valueB.Value is ulong || valueB.Value is decimal)
                    {
                        decimal decA = Convert.ToDecimal (valueA.Value, CultureInfo.CurrentCulture);
                        decimal decB = Convert.ToDecimal (valueB.Value, CultureInfo.CurrentCulture);
                        return decA.CompareTo (decB);
                    }
                    return CompareFloat (valueA.Value, valueB.Value);
                case IniValueType.DateTime:
                    if (valueA.Value is DateTime dtA)
                    {
                        DateTime dtB;
                        if (valueB.Value is DateTimeOffset temp)
                        {
                            dtB = temp.DateTime;
                        }
                        else
                        {
                            dtB = Convert.ToDateTime (valueB.Value, CultureInfo.CurrentCulture);
                        }
                        return dtA.CompareTo (dtB);
                    }

                    DateTimeOffset offsetA = (DateTimeOffset)valueA.Value;
                    DateTimeOffset offsetB;
                    if (valueB.Value is DateTimeOffset temp2)
                    {
                        offsetB = temp2;
                    }
                    else
                    {
                        offsetB = new DateTimeOffset (Convert.ToDateTime (valueB.Value, CultureInfo.CurrentCulture));
                    }
                    return offsetA.CompareTo (offsetB);
                case IniValueType.TimeSpan:
                    if (valueB.Value is not TimeSpan)
                    {
                        throw new ArgumentException ("要比较的 IValue 对象的值必须是 TimeSpan 类型", nameof (valueB));
                    }
                    TimeSpan span1 = (TimeSpan)valueA.Value;
                    TimeSpan span2 = (TimeSpan)valueB.Value;
                    return span1.CompareTo (span2);
                case IniValueType.Bytes:
                    if (valueB.Value is not byte[])
                    {
                        throw new ArgumentException ("要比较的 IValue 对象的值必须是 Byte[] 类型", nameof (valueB));
                    }
                    byte[] bytesA = (byte[])valueA.Value;
                    byte[] bytesB = (byte[])valueB.Value;
                    return BytesCompare (bytesA, bytesB);
                case IniValueType.String:
                    string strA = valueA.ToString ();
                    string strB = valueB.ToString ();
                    return string.CompareOrdinal (strA, strB);
                case IniValueType.Guid:
                    if (valueB.Value is not Guid)
                    {
                        throw new ArgumentException ("要比较的 IValue 对象的值必须是 Guid 类型", nameof (valueB));
                    }
                    Guid guidA = (Guid)valueA.Value;
                    Guid guidB = (Guid)valueB.Value;
                    return guidA.CompareTo (guidB);
                case IniValueType.Uri:
                    if (valueB.Value is not Uri)
                    {
                        throw new ArgumentException ("要比较的 IValue 对象的值必须是 Uri 类型", nameof (valueB));
                    }
                    Uri uriA = (Uri)valueA.Value;
                    Uri uriB = (Uri)valueB.Value;
                    return Comparer<string>.Default.Compare (uriA.ToString (), uriB.ToString ());
                case IniValueType.NotApplicable:
                case IniValueType.Empty:
                default:
                    throw new ArgumentException ("无法比较 valueA 和 valueB, 因为遇到意外的值类型");
            }
            #endregion
        }
        #endregion

        #region --私有方法--
        private static bool IsConvert (IniValueType sourceType, IniValueType[] targetTypes, bool nullable)
        {
            return Array.IndexOf (targetTypes, sourceType) != -1 || nullable && sourceType == IniValueType.Empty;
        }

        private static int CompareBigInteger (BigInteger intA, object objB)
        {
            int num = intA.CompareTo (ConvertToBigInteger (objB));
            if (num != 0)
            {
                return num;
            }
            if (objB is decimal num2)
            {
                return 0m.CompareTo (Math.Abs (num2 - Math.Truncate (num2)));
            }
            if (objB is double || objB is float)
            {
                double num3 = Convert.ToDouble (objB, CultureInfo.CurrentCulture);
                return 0d.CompareTo (Math.Abs (num3 - Math.Truncate (num3)));
            }
            return num;
        }

        private static int CompareFloat (object v1, object v2)
        {
            double d = Convert.ToDouble (v1, CultureInfo.CurrentCulture);
            double num = Convert.ToDouble (v2, CultureInfo.CurrentCulture);
            if (ApproxEquals (d, num))
            {
                return 0;
            }
            return d.CompareTo (num);
        }

        private static BigInteger ConvertToBigInteger (object value)
        {
            if (value is BigInteger v1)
            {
                return v1;
            }

            if (value is string v2)
            {
                return BigInteger.Parse (v2, CultureInfo.CurrentCulture);
            }

            if (value is float v3)
            {
                return new BigInteger (v3);
            }

            if (value is double v4)
            {
                return new BigInteger (v4);
            }

            if (value is decimal v5)
            {
                return new BigInteger (v5);
            }

            if (value is int v6)
            {
                return new BigInteger (v6);
            }

            if (value is long v7)
            {
                return new BigInteger (v7);
            }

            if (value is uint v8)
            {
                return new BigInteger (v8);
            }

            if (value is ulong v9)
            {
                return new BigInteger (v9);
            }

            if (value is byte[] v10)
            {
                return new BigInteger (v10);
            }

            throw new FormatException ($"无法将目标值转换为 BigInteger");
        }

        private static bool ApproxEquals (double a, double b)
        {
            if (a == b)
            {
                return true;
            }
            double num = (Math.Abs (a) + Math.Abs (b) + 10.0) * 2.2204460492503131E-16;
            double num2 = a - b;
            return -num < num2 && num > num2;
        }

        private static int BytesCompare (byte[] b1, byte[] b2)
        {
            int num = b1.Length.CompareTo (b2.Length);
            if (num != 0)
            {
                return num;
            }
            for (int i = 0; i < b1.Length; i++)
            {
                int temp = b1[i].CompareTo (b2[i]);
                if (temp != 0)
                {
                    return temp;
                }
            }
            return 0;
        }
        #endregion

        #region --运算符--
        /// <summary>
        /// 定义将 <see cref="bool"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="bool"/> 对象</param>
        public static implicit operator IniValue (bool value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="sbyte"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="sbyte"/> 对象</param>
        public static implicit operator IniValue (sbyte value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="byte"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="byte"/> 对象</param>
        public static implicit operator IniValue (byte value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="char"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="char"/> 对象</param>
        public static implicit operator IniValue (char value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="short"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="short"/> 对象</param>
        public static implicit operator IniValue (short value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="ushort"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="ushort"/> 对象</param>
        public static implicit operator IniValue (ushort value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="int"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="int"/> 对象</param>
        public static implicit operator IniValue (int value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="uint"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="uint"/> 对象</param>
        public static implicit operator IniValue (uint value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="long"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="long"/> 对象</param>
        public static implicit operator IniValue (long value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="ulong"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="ulong"/> 对象</param>
        public static implicit operator IniValue (ulong value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="float"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="float"/> 对象</param>
        public static implicit operator IniValue (float value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="double"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="double"/> 对象</param>
        public static implicit operator IniValue (double value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="decimal"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="decimal"/> 对象</param>
        public static implicit operator IniValue (decimal value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="DateTime"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="DateTime"/> 对象</param>
        public static implicit operator IniValue (DateTime value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="DateTimeOffset"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="DateTimeOffset"/> 对象</param>
        public static implicit operator IniValue (DateTimeOffset value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="TimeSpan"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="TimeSpan"/> 对象</param>
        public static implicit operator IniValue (TimeSpan value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="byte"/> 数组转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="byte"/> 数组</param>
        public static implicit operator IniValue (byte[] value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="string"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="string"/> 对象</param>
        public static implicit operator IniValue (string value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="Guid"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="Guid"/> 对象</param>
        public static implicit operator IniValue (Guid value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="Uri"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="Uri"/> 对象</param>
        public static implicit operator IniValue (Uri value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="IPAddress"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="IPAddress"/> 对象</param>
        public static implicit operator IniValue (IPAddress value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="bool"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="bool"/> 对象</param>
        public static implicit operator IniValue (bool? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="sbyte"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="sbyte"/> 对象</param>
        public static implicit operator IniValue (sbyte? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="byte"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="byte"/> 对象</param>
        public static implicit operator IniValue (byte? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="char"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="char"/> 对象</param>
        public static implicit operator IniValue (char? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="short"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="short"/> 对象</param>
        public static implicit operator IniValue (short? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="ushort"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="ushort"/> 对象</param>
        public static implicit operator IniValue (ushort? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="int"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="int"/> 对象</param>
        public static implicit operator IniValue (int? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="uint"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="uint"/> 对象</param>
        public static implicit operator IniValue (uint? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="long"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="long"/> 对象</param>
        public static implicit operator IniValue (long? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="ulong"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="ulong"/> 对象</param>
        public static implicit operator IniValue (ulong? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="float"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="float"/> 对象</param>
        public static implicit operator IniValue (float? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="double"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="double"/> 对象</param>
        public static implicit operator IniValue (double? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="decimal"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="decimal"/> 对象</param>
        public static implicit operator IniValue (decimal? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="DateTime"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="DateTime"/> 对象</param>
        public static implicit operator IniValue (DateTime? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="DateTimeOffset"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="DateTimeOffset"/> 对象</param>
        public static implicit operator IniValue (DateTimeOffset? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="TimeSpan"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="TimeSpan"/> 对象</param>
        public static implicit operator IniValue (TimeSpan? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="Guid"/> 转换为 <see cref="IniValue"/> 对象
        /// </summary>
        /// <param name="value">一个 <see cref="Guid"/> 对象</param>
        public static implicit operator IniValue (Guid? value)
        {
            return new IniValue (value);
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="bool"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator bool (IniValue value)
        {
            if (value is null)
            {
                return false;
            }
            return value.ToBoolean ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="sbyte"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator sbyte (IniValue value)
        {
            if (value is null)
            {
                return 0;
            }
            return value.ToSByte ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="byte"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator byte (IniValue value)
        {
            if (value is null)
            {
                return 0;
            }
            return value.ToByte ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="char"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator char (IniValue value)
        {
            if (value is null)
            {
                return char.MinValue;
            }
            return value.ToChar ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="short"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator short (IniValue value)
        {
            if (value is null)
            {
                return 0;
            }
            return value.ToInt16 ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="ushort"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator ushort (IniValue value)
        {
            if (value is null)
            {
                return 0;
            }
            return value.ToUInt16 ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="int"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator int (IniValue value)
        {
            if (value is null)
            {
                return 0;
            }
            return value.ToInt32 ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="uint"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator uint (IniValue value)
        {
            if (value is null)
            {
                return 0U;
            }
            return value.ToUInt32 ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="long"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator long (IniValue value)
        {
            if (value is null)
            {
                return 0L;
            }
            return value.ToInt64 ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="ulong"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator ulong (IniValue value)
        {
            if (value is null)
            {
                return 0UL;
            }
            return value.ToUInt64 ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="float"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator float (IniValue value)
        {
            if (value is null)
            {
                return 0F;
            }
            return value.ToSingle ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="double"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator double (IniValue value)
        {
            if (value is null)
            {
                return 0D;
            }
            return value.ToDouble ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="decimal"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator decimal (IniValue value)
        {
            if (value is null)
            {
                return decimal.Zero;
            }
            return value.ToDecimal ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="DateTime"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator DateTime (IniValue value)
        {
            if (value is null)
            {
                return DateTime.MinValue;
            }
            return value.ToDateTime ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="DateTimeOffset"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator DateTimeOffset (IniValue value)
        {
            if (value is null)
            {
                return DateTimeOffset.MinValue;
            }
            return value.ToDateTimeOffset ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator TimeSpan (IniValue value)
        {
            if (value is null)
            {
                return TimeSpan.Zero;
            }
            return value.ToTimeSpan ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="byte"/> 数组
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator byte[] (IniValue value)
        {
            if (value is null)
            {
                return Array.Empty<byte> ();
            }
            return value.ToBytes ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="string"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator string (IniValue value)
        {
            if (value is null)
            {
                return string.Empty;
            }
            return value.ToString ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="Guid"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator Guid (IniValue value)
        {
            if (value is null)
            {
                return Guid.Empty;
            }
            return value.ToGuid ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="Uri"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator Uri (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value.ToUri ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="IPAddress"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator IPAddress (IniValue value)
        {
            if (value is null)
            {
                return IPAddress.None;
            }
            return value.ToIPAddress ();
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="bool"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator bool? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }    
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="sbyte"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator sbyte? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="byte"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator byte? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="char"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator char? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="short"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator short? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="ushort"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator ushort? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="int"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator int? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="uint"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator uint? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="long"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator long? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="ulong"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator ulong? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="float"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator float? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="double"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator double? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="decimal"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator decimal? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="DateTime"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator DateTime? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="DateTimeOffset"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator DateTimeOffset? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator TimeSpan? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 定义将 <see cref="IniValue"/> 对象转换为 <see cref="Guid"/>
        /// </summary>
        /// <param name="value">转换的 <see cref="IniValue"/> 对象</param>
        public static implicit operator Guid? (IniValue value)
        {
            if (value is null)
            {
                return null;
            }
            return value; ;
        }
        #endregion
    }
}
