using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieRuntime.Ini
{
    /// <summary>
    /// 描述配置项 (Ini) 节的类
    /// </summary>
    public class IniSection : IDictionary<string, IniValue>, IEquatable<IniSection>
    {
        #region --字段--
        private readonly Dictionary<string, IniValue> dict;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取或设置与指定的键关联的值.
        /// </summary>
        /// <param name="key">要获取或设置的值的键</param>
        /// <returns>与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 <see cref="KeyNotFoundException"/>，而 Set 操作创建一个带指定键的新元素</returns>
        /// <exception cref="ArgumentNullException">key 为 <see langword="null"/></exception>
        /// <exception cref="KeyNotFoundException">已检索该属性且集合中不存在 key</exception>
        public IniValue this[string key]
        {
            get => this.dict[key];
            set
            {
                if (!this.dict.ContainsKey (key))
                {
                    this.dict.Add (key, value);
                }
                else
                {
                    this.dict[key] = value;
                }
            }
        }

        /// <summary>
        /// 获取一个按排序顺序包含 <see cref="IniSection"/> 中的键的集合
        /// </summary>
        public ICollection<string> Keys => this.dict.Keys;

        /// <summary>
        /// 获得一个包含 <see cref="IniSection"/> 中的值的集合
        /// </summary>
        public ICollection<IniValue> Values => this.dict.Values;

        /// <summary>
        /// 获取包含在 <see cref="IniSection"/> 中的键/值对的数目
        /// </summary>
        public int Count => this.dict.Count;

        /// <summary>
        /// 获取一个值，该值指示当前 <see cref="IniSection"/> 是否为只读
        /// </summary>
        public bool IsReadOnly => ((ICollection<KeyValuePair<string, IniValue>>)this.dict).IsReadOnly;

        /// <summary>
        /// 获取当前实例的节名称
        /// </summary>
        public string Name { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="IniSection"/> 类的新实例，该示例为空且具有默认的初始容量，并使用默认的 <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="name"><see cref="IniSection"/> 使用的节名称</param>
        /// <exception cref="ArgumentException">name 是 null 或 name 是空字符串</exception>
        public IniSection (string name)
        {
            if (string.IsNullOrEmpty (name))
            {
                throw new ArgumentException ($"“{nameof (name)}”不能为 null 或空。", nameof (name));
            }

            this.Name = name;
            this.dict = new Dictionary<string, IniValue> ();
        }

        /// <summary>
        /// 初始化 <see cref="IniSection"/> 类的新实例，该示例为空且具有指定的初始容量，并使用默认的 <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="name"><see cref="IniSection"/> 使用的节名称</param>
        /// <param name="capacity"><see cref="IniSection"/> 可包含的初始元素数</param>
        /// <exception cref="ArgumentException">name 是 null 或 name 是空字符串</exception>
        public IniSection (string name, int capacity)
        {
            if (string.IsNullOrEmpty (name))
            {
                throw new ArgumentException ($"“{nameof (name)}”不能为 null 或空。", nameof (name));
            }

            this.Name = name;
            this.dict = new Dictionary<string, IniValue> (capacity);
        }

        /// <summary>
        /// 初始化 <see cref="IniSection"/> 类的新实例，该实例为空，具有默认的初始容量并使用指定的 <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="name"><see cref="IniSection"/> 使用的节名称</param>
        /// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 实现，或者为 null，以便为键类型使用默认的 <see cref="IEqualityComparer{T}"/></param>
        /// <exception cref="ArgumentException">name 是 null 或 name 是空字符串</exception>
        public IniSection (string name, IEqualityComparer<string> comparer)
        {
            if (string.IsNullOrEmpty (name))
            {
                throw new ArgumentException ($"“{nameof (name)}”不能为 null 或空。", nameof (name));
            }

            this.Name = name;
            this.dict = new Dictionary<string, IniValue> (comparer);
        }

        /// <summary>
        /// 初始化 <see cref="IniSection"/> 类的新实例，该实例包含从指定的 <see cref="IDictionary{TKey, TValue}"/> 中复制的元素，其容量足以容纳所复制的元素数并使用默认的 <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="name"><see cref="IniSection"/> 使用的节名称</param>
        /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/>，它的元素被复制到新 <see cref="IniSection"/></param>
        /// <exception cref="ArgumentException">name 是 null 或 name 是空字符串</exception>
        public IniSection (string name, IDictionary<string, IniValue> dictionary)
        {
            if (string.IsNullOrEmpty (name))
            {
                throw new ArgumentException ($"“{nameof (name)}”不能为 null 或空。", nameof (name));
            }

            this.Name = name;
            this.dict = new Dictionary<string, IniValue> (dictionary);
        }

        /// <summary>
        /// 初始化 <see cref="IniSection"/> 类的新实例，该实例为空，具有指定的初始容量并使用指定的 <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="name"><see cref="IniSection"/> 使用的节名称</param>
        /// <param name="capacity"><see cref="IniSection"/> 可包含的初始元素数</param>
        /// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 实现，或者为 null，以便为键类型使用默认的 <see cref="IEqualityComparer{T}"/></param>
        /// <exception cref="ArgumentException">name 是 null 或 name 是空字符串</exception>
        public IniSection (string name, int capacity, IEqualityComparer<string> comparer)
        {
            if (string.IsNullOrEmpty (name))
            {
                throw new ArgumentException ($"“{nameof (name)}”不能为 null 或空。", nameof (name));
            }

            this.Name = name;
            this.dict = new Dictionary<string, IniValue> (capacity, comparer);
        }

        /// <summary>
        /// 初始化 <see cref="IniSection"/> 类的新实例，该实例包含从指定的 <see cref="IDictionary{Tkey, TValue}"/> 中复制的元素，其容量足以容纳所复制的元素数并使用指定的 <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="name"><see cref="IniSection"/> 使用的节名称</param>
        /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/>，它的元素被复制到新 <see cref="IniSection"/></param>
        /// <param name="comparer">比较键时要使用的 <see cref="IEqualityComparer{T}"/> 实现，或者为 null，以便为键类型使用默认的 <see cref="IEqualityComparer{T}"/></param>
        /// <exception cref="ArgumentException">name 是 null 或 name 是空字符串</exception>
        public IniSection (string name, IDictionary<string, IniValue> dictionary, IEqualityComparer<string> comparer)
        {
            if (string.IsNullOrEmpty (name))
            {
                throw new ArgumentException ($"“{nameof (name)}”不能为 null 或空。", nameof (name));
            }

            this.Name = name;
            this.dict = new Dictionary<string, IniValue> (dictionary, comparer);
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 向 <see cref="IniSection"/> 添加一个带有所提供的键和值的元素
        /// </summary>
        /// <param name="key">用作要添加的元素的键的对象</param>
        /// <param name="value">用作要添加的元素的值的对象</param>
        /// <exception cref="ArgumentNullException">key 为 null</exception>
        /// <exception cref="ArgumentException"><see cref="IniSection"/> 中已存在具有相同键的元素</exception>
        /// <exception cref="NotSupportedException"><see cref="IniSection"/> 为只读</exception>
        public void Add (string key, IniValue value)
        {
            this.dict.Add (key, value);
        }

        /// <summary>
        /// 向 <see cref="IniSection"/> 添加一个带有所提供的键和值的元素
        /// </summary>
        /// <param name="item">用作要添加的元素的键值对的对象</param>
        /// <exception cref="ArgumentNullException">key 为 null</exception>
        /// <exception cref="ArgumentException"><see cref="IniSection"/> 中已存在具有相同键的元素</exception>
        /// <exception cref="NotSupportedException"><see cref="IniSection"/> 为只读</exception>
        public void Add (KeyValuePair<string, IniValue> item)
        {
            this.Add (item);
        }

        /// <summary>
        /// 从 <see cref="IniSection"/> 中移除带有指定键的元素
        /// </summary>
        /// <param name="key">要移除的元素的键</param>
        /// <exception cref="ArgumentNullException">key 为 null</exception>
        /// <exception cref="NotSupportedException"><see cref="IniSection"/> 为只读</exception>
        /// <returns>如果该元素已成功移除，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果在原始 <see langword="false"/> 中没有找到 key，则此方法也会返回 <see cref="IniSection"/></returns>
        public bool Remove (string key)
        {
            return this.dict.Remove (key);
        }

        /// <summary>
        /// 从 <see cref="IniSection"/> 中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">要从 <see cref="IniSection"/> 中删除的对象</param>
        /// <returns>如果从 <see cref="IniSection"/> 中成功移除 item，则为 <see langword="true"/>；否则为 <see langword="false"/>。<see cref="IniSection"/> 中没有找到 item，该方法也会返回 <see langword="false"/></returns>
        public bool Remove (KeyValuePair<string, IniValue> item)
        {
            return this.Remove (item.Key);
        }

        /// <summary>
        /// 从 <see cref="IniSection"/> 中移除所有项
        /// </summary>
        public void Clear ()
        {
            this.dict.Clear ();
        }

        /// <summary>
        /// 确定 <see cref="IniSection"/> 是否包含特定值
        /// </summary>
        /// <param name="item">要在 <see cref="IniSection"/> 中定位的对象</param>
        /// <returns>如果在 <see cref="IniSection"/> 中找到 item，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        public bool Contains (KeyValuePair<string, IniValue> item)
        {
            return this.dict.Contains (item);
        }

        /// <summary>
        /// 确定是否 <see cref="IniSection"/> 包含带有指定键的元素
        /// </summary>
        /// <param name="key">要在 <see cref="IniSection"/> 中定位的键</param>
        /// <exception cref="ArgumentNullException">key 为 null</exception>
        /// <returns>如果 <see cref="IniSection"/> 包含具有键的元素，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        public bool ContainsKey (string key)
        {
            return this.dict.ContainsKey (key);
        }

        /// <summary>
        /// 确定 <see cref="IniSection"/> 是否包含特定值
        /// </summary>
        /// <param name="value">要在 <see cref="IniSection"/> 中定位的值。</param>
        /// <returns>如果 <see cref="IniSection"/> 包含具有指定值的元素，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public bool ContainsValue (IniValue value)
        {
            return this.dict.ContainsValue (value);
        }

        /// <summary>
        /// 获取与指定键关联的值
        /// </summary>
        /// <param name="key">要获取其值的键</param>
        /// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。 此参数未经初始化即被传递</param>
        /// <exception cref="ArgumentNullException">key 为 null</exception>
        /// <returns><see langword="true"/> 如果该对象的实现 <see cref="IniSection"/> 包含具有指定的元素键; 否则为 <see langword="false"/>。</returns>
        public bool TryGetValue (string key, out IniValue value)
        {
            return this.dict.TryGetValue (key, out value);
        }

        /// <summary>
        /// 从特定的 <see cref="IniSection"/> 索引处开始，将 <see cref="Array"/> 的元素复制到一个 <see cref="Array"/> 中
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/>，它是从 <see cref="IniSection"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制</param>
        /// <exception cref="ArgumentNullException">array 为 null</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex 小于 0</exception>
        /// <exception cref="ArgumentException">源中的元素数目 <see cref="IniSection"/> 大于从的可用空间 arrayIndex 目标从头到尾 array</exception>
        public void CopyTo (KeyValuePair<string, IniValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException (nameof (array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException (nameof (arrayIndex), "arrayIndex 不能小于 0");
            }

            if (arrayIndex >= array.Length && arrayIndex != 0)
            {
                throw new ArgumentException ("arrayIndex 等于或大于数组的长度。", nameof (arrayIndex));
            }

            if (this.dict.Count > array.Length - arrayIndex)
            {
                throw new ArgumentException ("源 IContainer 中的元素数量大于从 arrayIndex 到目标数组末尾的可用空间");
            }

            int num = 0;
            foreach (KeyValuePair<string, IniValue> item in this.dict)
            {
                array[arrayIndex + num] = item;
                num++;
            }
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举数</returns>
        public IEnumerator<KeyValuePair<string, IniValue>> GetEnumerator ()
        {
            return this.dict.GetEnumerator ();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>一个可用于循环访问集合的 <see cref="IEnumerator"/> 对象</returns>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.dict.GetEnumerator ();
        }

        /// <summary>
        /// 确定此实例是否与另一个指定的 <see cref="IniSection"/> 对象具有相同的值
        /// </summary>
        /// <param name="other">要与实例进行比较的 <see cref="IniSection"/></param>
        /// <returns>如果 <see langword="true"/> 参数的值与此实例的值相同，则为 value；否则为 <see langword="false"/>。 如果 value 为 null，则此方法返回 <see langword="false"/></returns>
        public bool Equals (IniSection other)
        {
            if (this == other)
            {
                return true;
            }

            if (this.Count != other.Count || other is null || string.CompareOrdinal (this.Name, other.Name) != 0)
            {
                return false;
            }

            for (int i = 0; i < this.Count; i++)
            {
                string keyA = this.Keys.ElementAt (i);
                string KeyB = other.Keys.ElementAt (i);

                IniValue valueA = this.Values.ElementAt (i);
                IniValue valueB = other.Values.ElementAt (i);

                if (string.CompareOrdinal (keyA, KeyB) != 0 || !valueA.Equals (valueB))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 确定此实例是否与指定的对象（也必须是 <see cref="IniSection"/> 对象）具有相同的值
        /// </summary>
        /// <param name="obj">要与实例进行比较的 <see cref="IniSection"/></param>
        /// <returns>如果 obj 参数的值与此实例的值相同，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果 obj 为 <see langword="null"/>，则此方法返回 <see langword="false"/></returns>
        public override bool Equals (object obj)
        {
            return this.Equals (obj as IniSection);
        }

        /// <summary>
        /// 返回该实例的哈希代码
        /// </summary>
        /// <returns>32 位有符号整数哈希代码</returns>
        public override int GetHashCode ()
        {
            return this.Name.GetHashCode () & this.dict.GetHashCode ();
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串</returns>
        public override string ToString ()
        {
            StringBuilder builder = new ();

            // 添加节名称
            builder.AppendLine ($"[{this.Name}]");

            // 添加键值对
            foreach (KeyValuePair<string, IniValue> item in this)
            {
                builder.AppendLine ($"{item.Key}={item.Value}");
            }

            return builder.ToString ();
        }
        #endregion
    }
}
