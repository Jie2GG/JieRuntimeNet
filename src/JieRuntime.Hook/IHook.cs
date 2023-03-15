using System;

namespace JieRuntime.Hook
{
    /// <summary>
    /// 提供挂钩标准的接口
    /// </summary>
    public interface IHook
    {
        /// <summary>
        /// 安装挂钩到指定名称的函数
        /// </summary>
        /// <param name="functionName">被挂钩的函数名</param>
        /// <param name="callback">挂钩回调. 当被挂钩的函数被调用时将触发此回调</param>
        void Install (string moduleName, string functionName, Delegate callback);

        /// <summary>
        /// 从指定名称的函数处卸载挂钩
        /// </summary>
        /// <param name="functionname">被挂钩的函数名</param>
        void Uninstall (string moduleName, string functionname);
    }
}
