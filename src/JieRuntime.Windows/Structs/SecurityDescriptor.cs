using System;

namespace JieRuntime.Windows.Structs
{
    /// <summary>
    /// SECURITY_DESCRIPTOR 结构包含与对象关联的安全信息. 应用程序使用此结构来设置和查询对象的安全状态
    /// </summary>
    public struct SecurityDescriptor
    {
#pragma warning disable CS1591 // 微软源中并未提供相关注释
        public byte Revision;
        public byte Sbz1;
        public ushort Control;
        public IntPtr Owner;
        public IntPtr Group;
        public IntPtr Sacl;
        public IntPtr Dacl;
#pragma warning restore CS1591 // 微软源中并未提供相关注释
    }
}
