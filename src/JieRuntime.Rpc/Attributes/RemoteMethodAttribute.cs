﻿using System;

namespace JieRuntime.Rpc.Attributes
{
    /// <summary>
    /// 表示远程方法的特性
    /// </summary>
    [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RemoteMethodAttribute : Attribute
    {
        #region --属性--
        /// <summary>
        /// 获取或设置远程方法的名称
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化一个新的 <see cref="RemoteMethodAttribute"/> 实例
        /// </summary>
        /// <param name="name">远程方法的名称</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> 为 null</exception>
        public RemoteMethodAttribute (string name)
        {
            this.Name = name ?? throw new ArgumentNullException (nameof (name));
        }
        #endregion
    }
}
