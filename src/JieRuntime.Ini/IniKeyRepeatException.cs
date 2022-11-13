namespace JieRuntime.Ini
{
    /// <summary>
    /// 表示 Initialization 配置项 Key 重复的错误
    /// </summary>
    public class IniKeyRepeatException : IniException
    {
        /// <summary>
        /// 初始化 <see cref="IniKeyRepeatException"/> 类的新实例
        /// </summary>
        /// <param name="keyName">指示错误的 key 名</param>
        public IniKeyRepeatException (string keyName)
            : base ($"配置项中存在重复的键(Key): {keyName}")
        { }
    }
}
