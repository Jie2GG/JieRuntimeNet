using System.Threading;

namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 提供 TCP 等待服务的类
    /// </summary>
    class TcpWait
    {
        /// <summary>
        /// 获取或设置一个 <see cref="bool"/> 值, 指示是否获得了响应
        /// </summary>
        public bool IsResponse { get; set; } = false;

        /// <summary>
        /// 获取或设置 TCP 等待的结果
        /// </summary>
        public byte[] Resutl { get; set; }

        /// <summary>
        /// 获取用于等待 TCP 结果返回的等待处理者
        /// </summary>
        public AutoResetEvent WaitHandler { get; } = new AutoResetEvent (false);
    }
}