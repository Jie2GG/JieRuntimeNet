using System;

namespace JieRuntime.Windows.Enums
{
    /// <summary>
    /// 格式化消息标志
    /// </summary>
    [Flags]
    public enum FormatMessageFlags
    {
        /// <summary>
        /// 该函数分配足够大的缓冲区来保存格式化的消息, 并将指针置于 lpBuffer 指定的地址上分配的缓冲区. lpBuffer 参数是指向 LPTSTR 的指针;必须将指针强制转换为 LPTSTR (， (LPTSTR)&lpBuffer 例如) 。 nSize 参数指定要为输出消息缓冲区分配的最小 TCHAR 数。 调用方应使用 LocalFree 函数在不再需要缓冲区时释放缓冲区
        /// </summary>
        AllocateBuffer = 0x00000100,
        /// <summary>
        /// Arguments 参数不是va_list结构, 而是指向表示参数的值数组的指针
        /// </summary>
        ArgumentArray = 0x00002000,
        /// <summary>
        /// lpSource 参数是一个模块句柄, 其中包含要搜索的消息表资源 (). 如果此 lpSource 句柄为 NULL, 则将搜索当前进程的应用程序映像文件. 此标志不能与 FORMAT_MESSAGE_FROM_STRING 一起使用
        /// </summary>
        FromHmodule = 0x00000800,
        /// <summary>
        /// lpSource 参数是指向包含消息定义的以 null 结尾的字符串的指针. 消息定义可能包含插入序列, 就像消息表资源中的消息文本一样。 此标志不能与 FORMAT_MESSAGE_FROM_HMODULE 或 FORMAT_MESSAGE_FROM_SYSTEM 一起使用
        /// </summary>
        FromString = 0x00000400,
        /// <summary>
        /// 该函数应搜索系统消息表资源, (请求的消息). 如果使用 FORMAT_MESSAGE_FROM_HMODULE 指定此标志, 则函数将在 lpSource 指定的模块中找到消息时搜索系统消息表。 此标志不能与 FORMAT_MESSAGE_FROM_STRING 一起使用
        /// </summary>
        FromSystem = 0x00001000,
        /// <summary>
        /// 消息定义 (如 %1) 中的插入序列将被忽略并传递到输出缓冲区不变. 此标志可用于获取消息以供以后的格式设置. 如果设置了此标志, 则忽略 Arguments 参数
        /// </summary>
        IgnoreInserts = 0x00000200
    }
}