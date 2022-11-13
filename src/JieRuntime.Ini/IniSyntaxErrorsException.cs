namespace JieRuntime.Ini
{
    /// <summary>
    /// 表示 Initialization 配置项在读取的过程冲发现语法错误
    /// </summary>
    public class IniSyntaxErrorsException : IniException
    {
        /// <summary>
        /// 初始化 <see cref="IniSyntaxErrorsException"/> 类的新实例
        /// </summary>
        /// <param name="lineNumber">指定错误行号</param>
        public IniSyntaxErrorsException (long lineNumber)
            : base ($"语法错误: 在第{lineNumber}行, 找不到与该行相应的匹配规则.")
        { }
    }
}
