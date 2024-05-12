using System;

namespace JieRuntime.Rpc.Attributes
{
    /// <summary>
    /// 表示远程参数的特性
    /// </summary>
    [AttributeUsage (AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RemoteParameterAttribute : Attribute
    {
        #region --属性--
        /// <summary>
        /// 获取或设置远程参数的名称
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化一个新的 <see cref="RemoteParameterAttribute"/> 实例
        /// </summary>
        /// <param name="name">远程参数的名称</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> 为 null</exception>
        public RemoteParameterAttribute (string name)
        {
            this.Name = name ?? throw new ArgumentNullException (nameof (name));
        }
        #endregion
    }
}
