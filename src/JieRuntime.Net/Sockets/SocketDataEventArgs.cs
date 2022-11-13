using System;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 表示套接字数据事件数据的类
    /// </summary>
    public class SocketDataEventArgs : SocketEventArgs
    {
        #region --属性--
        /// <summary>
        /// 获取当前事件的数据
        /// </summary>
        public byte[] Data { get; }
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="SocketDataEventArgs"/> 类的新实例
        /// </summary>
        /// <param name="data">相关的数据</param>
        public SocketDataEventArgs (byte[] data)
        {
            this.Data = data;
        }
        #endregion
    }
}
