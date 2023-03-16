using System;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace JieRuntime.Ini
{
    /// <summary>
    /// 提供 Initialization 配置文件的读写相关操作类
    /// </summary>
    public class IniConfiguration
    {
        #region --字段--
        private static readonly Regex[] iniRule = new Regex[]
        {
            //匹配 Section
            new Regex(@"^\[(.+)\]", RegexOptions.Compiled),					
            //匹配 键值对
			new Regex(@"^([^\r\n=]+)=((?:[^\r\n]+)?)",RegexOptions.Compiled),
            //匹配 注释
            new Regex(@"^;(?:[\s\S]*)", RegexOptions.Compiled)
        };
        #endregion

        #region --属性--
        /// <summary>
        /// 获取当前配置文件的文件信息
        /// </summary>
        public FileInfo FileInfo { get; }

        /// <summary>
        /// 获取当前配置文件的文件编码, 默认: <see cref="Encoding.UTF8"/>
        /// </summary>
        public Encoding FileEncoding { get; }

        /// <summary>
        /// 获取当前配置文件的配置信息
        /// </summary>
        public IniObject Configuration { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="IniConfiguration"/> 类的新实例
        /// </summary>
        /// <param name="fileName">指定配置文件的路径</param>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> 不能为 <see langword="null"/> 或空字符串</exception>
        /// <exception cref="SecurityException">调用者没有所需的权限</exception>
        /// <exception cref="UnauthorizedAccessException">拒绝访问 <paramref name="fileName"/></exception>
        /// <exception cref="PathTooLongException">指定的路径、文件名或两者均超过系统定义的最大长度</exception>
        /// <exception cref="NotSupportedException"><paramref name="fileName"/> 在字符串中间包含冒号(:)</exception>
        public IniConfiguration (string fileName)
            : this (fileName, Encoding.UTF8)
        { }

        /// <summary>
        /// 初始化 <see cref="IniConfiguration"/> 类的新实例, 并指定其文件编码
        /// </summary>
        /// <param name="fileName">指定配置文件的路径</param>
        /// <param name="fileEncoding">指定配置文件的文件编码</param>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> 不能为 <see langword="null"/> 或空字符串</exception>
        /// <exception cref="SecurityException">调用者没有所需的权限</exception>
        /// <exception cref="UnauthorizedAccessException">拒绝访问 <paramref name="fileName"/></exception>
        /// <exception cref="PathTooLongException">指定的路径、文件名或两者均超过系统定义的最大长度</exception>
        /// <exception cref="NotSupportedException"><paramref name="fileName"/> 在字符串中间包含冒号(:)</exception>
        public IniConfiguration (string fileName, Encoding fileEncoding)
        {
            if (string.IsNullOrEmpty (fileName))
            {
                throw new ArgumentException ($"“{nameof (fileName)}”不能为 null 或空。", nameof (fileName));
            }

            this.Configuration = new IniObject ();
            this.FileEncoding = fileEncoding;
            this.FileInfo = new FileInfo (fileName);
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 将加载 Initialization 配置文件
        /// </summary>
        public void Load ()
        {
            if (this.FileInfo.Exists)
            {
                this.Configuration.Clear ();    // 清空内容

                long lineNumber = -1;
                int rule = 0;

                IniSection tempSection = null;
                using TextReader text = new StreamReader (this.FileInfo.Open (FileMode.OpenOrCreate, FileAccess.ReadWrite), this.FileEncoding);
                while (text.Peek () != -1)
                {
                    // 计算行数
                    lineNumber += 1;

                    // 处理行
                    string line = text.ReadLine ();
                    if (string.IsNullOrWhiteSpace (line))
                    {
                        continue;   // 跳过空行
                    }

                    // 注释匹配
                    if (iniRule[2].IsMatch (line))
                    {
                        continue;
                    }

                MATCH_SECTION:
                    // 匹配 Section
                    Match match = iniRule[rule].Match (line);
                    if (rule == 0)
                    {
                        if (match.Success)
                        {
                            rule = 1;   // 切换到内容读取

                            string sectionName = match.Groups[1].Value;

                            // 如果节点存在表示文件出现异常
                            if (this.Configuration.ContainsKey (sectionName))
                            {
                                // 发现重复的 Section
                                this.Configuration.Clear ();
                                throw new IniSectionRepeatException (sectionName);
                            }

                            // 将节添加到 Config 中
                            tempSection = new IniSection (sectionName);
                            this.Configuration.Add (tempSection);

                            continue;
                        }
                        else
                        {
                            // 匹配到这里就说明出现了语法错误
                            this.Configuration.Clear ();
                            throw new IniSyntaxErrorsException (lineNumber);

                        }
                    }
                    else if (rule == 1)
                    {
                        //匹配 Value
                        if (match.Success)
                        {
                            string key = match.Groups[1].Value.Trim ();
                            string value = match.Groups[2].Value;
                            if (tempSection.ContainsKey (key))
                            {
                                // 找到重复的键
                                this.Configuration.Clear ();
                                throw new IniKeyRepeatException (key);
                            }

                            tempSection.Add (key, value);

                            continue;
                        }
                        else
                        {
                            // 匹配失败, 切换到 Section 匹配
                            rule = 0;

                            // 跳转到上面继续解析
                            goto MATCH_SECTION;
                        }
                    }

                }
            }

        }

        /// <summary>
        /// 将保存 Initialization 配置文件
        /// </summary>
        public void Save ()
        {
            // 处理文件夹
            if (!this.FileInfo.Directory.Exists)
            {
                this.FileInfo.Directory.Create ();
            }

            FileStream fileStream = this.FileInfo.Open (FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            fileStream.SetLength (0);   // 清空文件

            using TextWriter text = new StreamWriter (fileStream, this.FileEncoding);
            text.Write (this.Configuration.ToString ());
        }
        #endregion
    }
}
