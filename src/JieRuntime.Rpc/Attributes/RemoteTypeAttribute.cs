using System;

namespace JieRuntime.Rpc.Attributes
{
    /// <summary>
    /// 表示远程类型的特性
    /// </summary>
    [AttributeUsage (AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class RemoteTypeAttribute : Attribute
    {
        #region --属性--
        /// <summary>
        /// 获取或设置远程类型的名称
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化一个新的 <see cref="RemoteTypeAttribute"/> 实例
        /// </summary>
        /// <param name="name">远程类型的名称</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> 为 null</exception>
        public RemoteTypeAttribute (string name)
        {
            this.Name = name ?? throw new ArgumentNullException (nameof (name));
        }
        #endregion
    }
}
