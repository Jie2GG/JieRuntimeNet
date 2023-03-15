using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace JieRuntime.Windows.Structs
{
    /// <summary>
    /// SECURITY_ATTRIBUTES 结构包含对象的安全描述符, 并指定通过指定此结构检索的句柄是否可继承. 有关详细信息, 请参阅使用<a href="https://learn.microsoft.com/zh-cn/previous-versions/windows/desktop/legacy/aa379560(v=vs.85)">SECURITY_ATTRIBUTES</a>
    /// </summary>
    public struct SecurityAttributes
    {
        #region --字段--
        /// <summary>
        /// 此结构的大小 (以字节为单位). 将此值设置为 SECURITY_ATTRIBUTES 结构的大小
        /// </summary>
        public int nLength;
        /// <summary>
        /// 指向控制对对象的访问的 SECURITY_DESCRIPTOR 结构的指针. 如果此成员的值为 NULL, 则会为对象分配与调用进程的访问令牌关联的默认安全描述符. 这与通过分配 NULL 自由访问控制列表 (DACL) 向每个人授予访问权限不同. 默认情况下, 进程的访问令牌中的默认 DACL 仅允许访问令牌表示的用户
        /// </summary>
        [MarshalAs (UnmanagedType.LPStruct)]
        public SecurityDescriptor lpSecurityDescriptor;
        /// <summary>
        /// 一个布尔值, 它指定在创建新进程时是否继承返回的句柄. 如果此成员为 <see langword="true"/>, 则新进程将继承句柄
        /// </summary>
        public int bInheritHandle;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取一个值, 该值指示在创建新进程时是否继承返回的句柄. 如果该成员为 <see langword="true"/>, 则新进程继承该句柄
        /// </summary>
        public bool InheritHandle => this.bInheritHandle != 0;
        #endregion

        #region --公开方法--
        /// <summary>
        /// 初始化一个新的 <see cref="SecurityAttributes"/> 结构.
        /// </summary>
        /// <returns>一个新的 <see cref="SecurityAttributes"/> 结构.</returns>
        public static unsafe SecurityAttributes Create ()
        {
            return new ()
            {
                nLength = sizeof (SecurityAttributes)
            };
        }
        #endregion
    }
}
