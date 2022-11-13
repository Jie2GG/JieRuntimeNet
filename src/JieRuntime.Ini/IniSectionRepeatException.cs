namespace JieRuntime.Ini
{
    /// <summary>
    /// 表示 Initialization 配置项 Section 重复的错误
    /// </summary>
    public class IniSectionRepeatException : IniException
    {
        /// <summary>
        /// 初始化 <see cref="IniSectionRepeatException"/> 类的新实例
        /// </summary>
        /// <param name="sectionName">指示错误的 section 名</param>
        public IniSectionRepeatException (string sectionName)
            : base ($"配置项中存在重复的节(Section): {sectionName}")
        { }
    }
}
