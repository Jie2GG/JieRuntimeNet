using System;
using System.Collections;
using System.Text.Json.Serialization;

namespace JieRuntime.Rpc.Tcp.Messages
{
    /// <summary>
    /// 表示 Json 远程调用内部错误的类
    /// </summary>
    internal class JsonRpcInnerError
    {
        #region --属性--
        /// <summary>
        /// 获取或设置调用堆栈上即时帧的字符串表示形式
        /// </summary>
        [JsonPropertyName ("stack_trace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// 获取或设置引起错误的应用程序或对象的名称
        /// </summary>
        [JsonPropertyName ("source")]
        public string Source { get; set; }

        /// <summary>
        /// 获取或设置描述当前错误的消息
        /// </summary>
        [JsonPropertyName ("message")]
        public string Message { get; set; }

        /// <summary>
        /// 获取导致当前错误的 <see cref="JsonRpcInnerError"/> 实例
        /// </summary>
        [JsonPropertyName ("inner_error")]
        public JsonRpcInnerError InnerError { get; set; }

        /// <summary>
        /// 获取或设置HRESULT, 这是分配给特定异常的编码数值
        /// </summary>
        [JsonPropertyName ("hresult")]
        public int HResult { get; set; }

        /// <summary>
        /// 获取或设置键/值对的集合, 这些键/值对提供有关异常的额外用户定义信息
        /// </summary>
        [JsonPropertyName ("data")]
        public IDictionary Data { get; set; }

        /// <summary>
        /// 获取或设置指向与此异常关联的帮助文件的链接
        /// </summary>
        [JsonPropertyName ("help_link")]
        public string HelpLink { get; set; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="JsonRpcInnerError"/> 类的新实例
        /// </summary>
        public JsonRpcInnerError ()
        { }

        /// <summary>
        /// 初始化 <see cref="JsonRpcInnerError"/> 类的新实例
        /// </summary>
        /// <param name="exception">用于描述错误的异常</param>
        public JsonRpcInnerError (Exception exception)
        {
            this.StackTrace = exception.StackTrace;
            this.Source = exception.Source;
            this.Message = exception.Message;
            this.InnerError = exception.InnerException != null ? new JsonRpcInnerError (exception.InnerException) : null;
            this.HResult = exception.HResult;
            this.Data = exception.Data;
            this.HelpLink = exception.HelpLink;
        }
        #endregion
    }
}
