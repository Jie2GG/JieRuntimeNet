using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using JieRuntime.Hook.Exceptions;
using JieRuntime.Utils;
using JieRuntime.Windows;
using JieRuntime.Windows.Enums;

namespace JieRuntime.Hook
{
    /// <summary>
    /// 提供 Windows 平台的进程挂钩
    /// </summary>
    public sealed class WindowsHook : IHook
    {
        #region --常量--
        private const int HEAD_CODE_X86_LENGTH = 5;
        private const int HEAD_CODE_X64_LENGTH = 12;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取当前远程挂钩劫持的目标进程
        /// </summary>
        public Process HookProcess { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="WindowsHook"/> 类的新实例, 并指定自身进程作为挂钩的目标进程
        /// </summary>
        public WindowsHook ()
            : this (Process.GetCurrentProcess ().Id)
        { }

        /// <summary>
        /// 初始化 <see cref="WindowsHook"/> 类的新实例
        /// </summary>
        /// <param name="processId">指定要挂钩的进程 Id</param>
        public WindowsHook (int processId)
        {
            if (!RuntimeInformation.IsOSPlatform (OSPlatform.Windows))
            {
                throw new OSNotSupportedException ();
            }

            if (processId != -1)
            {
                this.HookProcess = Process.GetProcessById (processId);
            }
            else
            {
                this.HookProcess = Process.GetCurrentProcess ();
            }
        }
        #endregion

        #region --公开方法--
        public void Install (string moduleName, string functionName, Delegate callback)
        {
            if (string.IsNullOrEmpty (moduleName))
            {
                throw new ArgumentException ($"“{nameof (moduleName)}”不能为 null 或空。", nameof (moduleName));
            }

            if (string.IsNullOrEmpty (functionName))
            {
                throw new ArgumentException ($"“{nameof (functionName)}”不能为 null 或空。", nameof (functionName));
            }

            if (callback is null)
            {
                throw new ArgumentNullException (nameof (callback));
            }

            // 刷新进程信息
            this.HookProcess.Refresh ();

            if (this.HookProcess.HasExited)
            {
                throw new ProcessExitedException ();
            }

            // 打开进程
            ObjectSafeHandle processHandle = Kernel32.OpenProcess (ProcessAccessRights.All, true, this.HookProcess.Id);
            if (processHandle.IsInvalid)
            {
                throw new ProcessOpenFailException ();
            }

            // 遍历模块
            foreach (ProcessModule processModule in this.HookProcess.Modules)
            {
                if (string.Compare (processModule.ModuleName, moduleName, true) == 0)
                {
                    // 获取指定模块的函数指针
                    IntPtr remoteProcAddress = Kernel32.GetProcAddress (processModule.BaseAddress, functionName);
                    if (remoteProcAddress == IntPtr.Zero)
                    {
                        throw new EntryPointNotFoundException ($"未在模块 {processModule.ModuleName} 中找到指定的函数 {functionName}");
                    }

                    // 转换本机函数指针
                    IntPtr localProcAddress = Marshal.GetFunctionPointerForDelegate (callback);
                    if (localProcAddress == IntPtr.Zero)
                    {
                        throw new EntryPointNotFoundException ($"用于挂钩的委托 {callback.Method.Name} 是无效的");
                    }

                    WindowsHookAsm hookAsm = null;

                    // 解锁内存块
                    if (!UnlockMemory (processHandle.DangerousGetHandle (), remoteProcAddress, ref hookAsm))
                    {
                        throw new ProcessDenyAccessException ();
                    }

                    // 替换函数跳转地址
                    if (!ChangeFunctionPtr (processHandle.DangerousGetHandle (), remoteProcAddress, localProcAddress, ref hookAsm))
                    {
                        throw new HookException ();
                    }

                    // 锁定内存块
                    if (!LockMemory (processHandle.DangerousGetHandle (), remoteProcAddress, ref hookAsm))
                    {
                        throw new ProcessDenyAccessException ();
                    }

                    break;
                }
            }

            // 释放进程
            processHandle.DangerousRelease ();
        }

        public void Uninstall (string moduleName, string functionname)
        {

        }
        #endregion

        #region --私有方法--
        // 解锁内存
        private static bool UnlockMemory (IntPtr hModule, IntPtr lpAddress, ref WindowsHookAsm hookAsm)
        {
            hookAsm ??= new WindowsHookAsm ();

            // 32位 call 长度: 5
            // 64位 call 长度: 12
            ulong dwSize = (ulong)(Environment.Is64BitProcess == false ? HEAD_CODE_X86_LENGTH : HEAD_CODE_X64_LENGTH);

            // 修改内存保护标志
            bool result = Kernel32.VirtualProtectEx (hModule, lpAddress, dwSize, MemoryProtectionConstants.ExecuteReadWrite, out MemoryProtectionConstants oldMemoryProtectionConstants);

            // 保存旧的内存保护标志
            hookAsm.MemoryProtectionConstants = oldMemoryProtectionConstants;

            // 返回结果
            return result;
        }

        // 锁定内存
        private static bool LockMemory (IntPtr hModule, IntPtr lpAddress, ref WindowsHookAsm hookAsm)
        {
            if (hookAsm is null)
            {
                throw new ArgumentNullException (nameof (hookAsm));
            }

            // 32位 call 长度: 5
            // 64位 call 长度: 12
            ulong dwSize = (ulong)(Environment.Is64BitProcess == false ? HEAD_CODE_X86_LENGTH : HEAD_CODE_X64_LENGTH);

            // 修改内存保护标志
            return Kernel32.VirtualProtectEx (hModule, lpAddress, dwSize, hookAsm.MemoryProtectionConstants, out _);
        }

        // 替换函数指针
        private static bool ChangeFunctionPtr (IntPtr hModule, IntPtr oldFunctionPtr, IntPtr newFunctionPtr, ref WindowsHookAsm hookAsm)
        {
            // 原始函数的 HeadCode
            hookAsm.OriginalFunctionAsm = GetHeadCode (oldFunctionPtr);
            // 新函数的 HeadCode
            if (Environment.Is64BitProcess)
            {
                // 64位
                hookAsm.HookFunctionAsm = ArrayUtils.Concat (new byte[] { 0x48, 0xB8 }, BinaryConvert.GetBytes (newFunctionPtr.ToInt64 ()), new byte[] { 0xFF, 0xE0 });
            }
            else
            {
                // 32位
                hookAsm.HookFunctionAsm = ArrayUtils.Concat (new byte[] { 0xE9 }, BinaryConvert.GetBytes (newFunctionPtr.ToInt32 () - (oldFunctionPtr.ToInt32 () + 5)));
            }

            ulong nSize = (ulong)(Environment.Is64BitProcess == false ? HEAD_CODE_X86_LENGTH : HEAD_CODE_X64_LENGTH);

            // 修改函数跳转地址
            return Kernel32.WriteProcessMemory (hModule, oldFunctionPtr, hookAsm.HookFunctionAsm, nSize, out _);
        }

        // 获取头部代码
        private static byte[] GetHeadCode (IntPtr ptr)
        {
            byte[] buffer = new byte[(Environment.Is64BitProcess == false ? HEAD_CODE_X86_LENGTH : HEAD_CODE_X64_LENGTH)];
            Marshal.Copy (ptr, buffer, 0, buffer.Length);
            return buffer;
        }


        #endregion
    }
}
