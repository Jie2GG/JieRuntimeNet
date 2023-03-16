using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static System.Collections.Specialized.BitVector32;

namespace JieRuntime.Ini
{
    /// <summary>
    /// 描述配置项 (Ini) 的类
    /// </summary>
    public class IniObject : ICollection<IniSection>, IEquatable<IniObject>
    {
        #region --字段--
        //private readonly SortedList<string, IniSection> list;
        private readonly Dictionary<string, int> keyDict;
        private readonly List<IniSection> list;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取或设置与指定的键关联的配置项节
        /// </summary>
        /// <param name="key">要获取或设置其值的键</param>
        /// <returns>与指定的键相关联的值。 如果找不到指定的键，则 get 操作会引发一个 <see cref="KeyNotFoundException"/>，而 set 操作会创建一个使用指定键的新元素。</returns>
        /// <exception cref="ArgumentNullException">key 为 <see langword="null"/></exception>
        /// <exception cref="KeyNotFoundException">已检索该属性且集合中不存在 key</exception>
        public IniSection this[string key]
        {
            get
            {
                if (!this.ContainsKey (key))
                {
                    this.Add (new IniSection (key));
                }
                return this.list[this.keyDict[key]];
            }
            set
            {
                if (!this.ContainsKey (key))
                {
                    this.Add (new IniSection (key));
                }
                else
                {
                    this.list[this.keyDict[key]] = value;
                }
            }
        }

        /// <summary>
        /// 获取 <see cref="IniObject"/> 中包含的元素数
        /// </summary>
        public int Count => this.list.Count;

        /// <summary>
        /// 获取一个值，该值指示 <see cref="IniObject"/> 是否为只读
        /// </summary>
        public bool IsReadOnly => ((ICollection<IniSection>)this.list).IsReadOnly;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="IniObject"/> 类的新实例，该实例为空并且具有默认初始容量
        /// </summary>
        public IniObject ()
            : this (0)
        { }

        /// <summary>
        /// 初始化 <see cref="IniObject"/> 类的新实例，该实例为空并且具有指定的初始容量
        /// </summary>
        /// <param name="capacity">新列表最初可以存储的元素数</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity 小于 0</exception>
        public IniObject (int capacity)
        {
            this.list = new List<IniSection> (capacity);
            this.keyDict = new Dictionary<string, int> (capacity);
        }

        /// <summary>
        /// 初始化 <see cref="IniObject"/> 类的新实例，该实例包含从指定集合复制的元素并且具有足够的容量来容纳所复制的元素
        /// </summary>
        /// <param name="collection">一个集合，其元素被复制到新列表中</param>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        public IniObject (IEnumerable<IniSection> collection)
        {
            this.list = new List<IniSection> (collection);
            // 将索引和键关联
            for (int i = 0; i < list.Count; i++)
            {
                this.keyDict.Add (list[i].Name, i);
            }
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 将某项添加到 <see cref="IniObject"/> 中
        /// </summary>
        /// <param name="item">要添加到 <see cref="IniObject"/> 的对象</param>
        public void Add (IniSection item)
        {
            if (this.ContainsKey (item.Name))
            {
                throw new ArgumentException ($"“{item.Name}”存在一个同名的实例。", nameof (item));
            }

            this.list.Add (item);
            this.keyDict.Add (item.Name, this.list.IndexOf (item));
        }

        /// <summary>
        /// 从 <see cref="IniObject"/> 中移除指定名称的元素
        /// </summary>
        /// <param name="key">要移除的元素的键</param>
        /// <exception cref="ArgumentNullException">key 为 null 或 key 是空字符串</exception>
        /// <returns>如果该元素已成功移除，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果在原始 <see langword="true"/> 中没有找到 key，则此方法也会返回 <see cref="IniObject"/>。</returns>
        public bool Remove (string key)
        {
            if (string.IsNullOrEmpty (key))
            {
                throw new ArgumentException ($"“{nameof (key)}”不能为 null 或空。", nameof (key));
            }

            if (this.keyDict.ContainsKey (key))
            {
                this.list.RemoveAt (this.keyDict[key]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 从 <see cref="IniObject"/> 中移除指定的元素
        /// </summary>
        /// <param name="item">要移除的元素</param>
        /// <exception cref="ArgumentNullException">item 为 null</exception>
        /// <returns>如果该元素已成功移除，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果在原始 <see cref="IniObject"/> 中没有找到 key，则此方法也会返回 <see langword="false"/>。</returns>
        public bool Remove (IniSection item)
        {
            if (item == null)
            {
                throw new ArgumentNullException (nameof (item));
            }

            if (this.keyDict.ContainsKey (item.Name))
            {
                this.keyDict.Remove (item.Name);
                return this.list.Remove (item);
            }

            return false;
        }

        /// <summary>
        /// 从 <see cref="IniObject"/> 中移除所有项
        /// </summary>
        public void Clear ()
        {
            this.list.Clear ();
            this.keyDict.Clear ();
        }

        /// <summary>
        /// 确定 <see cref="IniObject"/> 是否包含特定值
        /// </summary>
        /// <param name="item">要在 <see cref="IniObject"/> 中定位的对象</param>
        /// <returns>如果在 <see cref="IniObject"/> 中找到 item，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        public bool Contains (IniSection item)
        {
            return this.list.Contains (item);
        }

        /// <summary>
        /// 确定 <see cref="IniObject"/> 是否包含特定名称的 <see cref="IniSection"/>
        /// </summary>
        /// <param name="key">要在 <see cref="IniObject"/> 中定位的键</param>
        /// <exception cref="ArgumentNullException">key 为 null</exception>
        /// <returns>如果 <see cref="IniObject"/> 包含具有指定键的元素，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        public bool ContainsKey (string key)
        {
            return this.keyDict.ContainsKey (key);
        }

        /// <summary>
        /// 获取与指定键关联的值
        /// </summary>
        /// <param name="key">要获取其值的键</param>
        /// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。 此参数未经初始化即被传递</param>
        /// <exception cref="ArgumentNullException">key 为 null</exception>
        /// <returns>如果 <see cref="IniObject"/> 包含具有指定键的元素，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        public bool TryGetValue (string key, out IniSection value)
        {
            value = null;

            if (this.keyDict.TryGetValue (key, out int index))
            {
                value = this.list[index];
            }

            return false;
        }

        /// <summary>
        /// 从特定的 <see cref="IniObject"/> 索引处开始，将 <see cref="IniObject"/> 的元素复制到一个 <see cref="Array"/> 中
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/>，它是从 <see cref="IniObject"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制</param>
        /// <exception cref="ArgumentNullException">array 为 null</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex 小于 0</exception>
        /// <exception cref="ArgumentException">源中的元素数目 <see cref="IniObject"/> 大于从的可用空间 arrayIndex 目标从头到尾 array</exception>
        public void CopyTo (IniSection[] array, int arrayIndex)
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

            if (this.list.Count > array.Length - arrayIndex)
            {
                throw new ArgumentException ("源 IObject 中的元素数量大于从 arrayIndex 到目标数组末尾的可用空间");
            }

            int num = 0;
            foreach (IniSection item in this.list)
            {
                array[arrayIndex + num] = item;
                num++;
            }
        }

        /// <summary>
        /// 返回循环访问 <see cref="IniObject"/> 的枚举数
        /// </summary>
        /// <returns><see cref="IEnumerator"/> 的类型 <see cref="IEnumerator{IniSection}"/> 的 <see cref="IniObject"/></returns>
        public IEnumerator<IniSection> GetEnumerator ()
        {
            return this.list.GetEnumerator ();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>用于循环访问集合的枚举数</returns>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.list.GetEnumerator ();
        }

        /// <summary>
        /// 确定此实例是否与另一个指定的 <see cref="IniObject"/> 对象具有相同的值
        /// </summary>
        /// <param name="other">要与实例进行比较的 <see cref="IniObject"/></param>
        /// <returns>如果 other 参数的值与此实例的值相同，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果 value 为 null，则此方法返回 <see langword="false"/></returns>
        public bool Equals (IniObject other)
        {
            if (this == other)
            {
                return true;
            }

            if (this.Count != other.Count || other is null)
            {
                return false;
            }

            for (int i = 0; i < this.Count; i++)
            {
                IniSection valueA = this.list[i];
                IniSection valueB = other.list[i];

                if (!valueA.Equals (valueB) || !this.keyDict[valueA.Name].Equals (other.keyDict[valueB.Name]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 确定此实例是否与指定的对象（也必须是 <see cref="IniObject"/> 对象）具有相同的值
        /// </summary>
        /// <param name="obj">要与实例进行比较的 <see cref="IniObject"/></param>
        /// <returns>如果 obj 参数的值与此实例的值相同，则为 <see langword="true"/>；否则为 <see langword="false"/>。 如果 obj 为 null，则此方法返回 <see langword="false"/></returns>
        public override bool Equals (object obj)
        {
            return this.Equals (obj as IniObject);
        }

        /// <summary>
        /// 返回该实例的哈希代码
        /// </summary>
        /// <returns>32 位有符号整数哈希代码</returns>
        public override int GetHashCode ()
        {
            return this.list.GetHashCode () & this.keyDict.GetHashCode ();
        }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>表示当前对象的字符串</returns>
        public override string ToString ()
        {
            StringBuilder builder = new ();
            foreach (IniSection item in this.list)
            {
                builder.AppendLine (item.ToString ());
            }
            return builder.ToString ();
        }
        #endregion
    }
}
