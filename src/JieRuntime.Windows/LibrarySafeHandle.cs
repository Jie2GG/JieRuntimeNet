using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JieRuntime.Windows
{
    /// <summary>
    /// 表示可以用 FreeLibrary 关闭的 Win32 句柄
    /// </summary>
    public class LibrarySafeHandle : SafeHandle
    {
        #region --字段--
        /// <summary>
        /// 表示 Win32 的无效句柄 <see cref="Kernel32.INVALID_HANDLE_VALUE"/>.
        /// </summary>
        public static readonly LibrarySafeHandle Invalid = new ();

        /// <summary>
        /// 表示 Win32 null 的句柄 <see cref="IntPtr.Zero"/>.
        /// </summary>
        public static readonly LibrarySafeHandle Null = new (IntPtr.Zero, false);
        #endregion

        #region --属性--
        /// <summary>
        /// 获取指示句柄值是否无效的值
        /// </summary>
        /// <returns>句柄无效则为 <see langword="true"/>; 否则为 <see langword="false"/></returns>
        public override bool IsInvalid => this.handle == Kernel32.INVALID_HANDLE_VALUE || this.handle == IntPtr.Zero;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="LibrarySafeHandle"/> 类的新实例
        /// </summary>
        /// <exception cref="TypeLoadException">派生类驻留在没有非托管代码访问权限的程序集中</exception>
        public LibrarySafeHandle ()
            : base (Kernel32.INVALID_HANDLE_VALUE, true)
        { }

        /// <summary>
        /// 初始化 <see cref="LibrarySafeHandle"/> 类的新实例
        /// </summary>
        /// <param name="preexistingHandle">表示要使用的已存在句柄的对象。</param>
        /// <param name="ownsHandle">当安全句柄被释放或结束时释放本机句柄则为 <see langword="true" />; 否则为 <see langword="false" /></param>
        /// <exception cref="TypeLoadException">派生类驻留在没有非托管代码访问权限的程序集中</exception>
        public LibrarySafeHandle (IntPtr preexistingHandle, bool ownsHandle = true)
            : base (Kernel32.INVALID_HANDLE_VALUE, ownsHandle)
        {
            this.SetHandle (preexistingHandle);
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 执行释放句柄所需的代码
        /// </summary>
        /// <returns>如果句柄成功释放, 则为 <see langword="true"/>; 否则, 在灾难性失败的情况下为 <see langword="false"/>. 在本例中, 它生成一个 releaseHandleFailed 托管调试助手</returns>
        protected override bool ReleaseHandle () => Kernel32.FreeLibrary (this.handle);
        #endregion
    }
}
