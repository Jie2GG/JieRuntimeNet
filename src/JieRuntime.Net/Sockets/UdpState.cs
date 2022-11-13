using System.Net;

namespace JieRuntime.Net.Sockets
{
    /// <summary>
    /// 表示 UDP 状态的结构
    /// </summary>
    public struct UdpState
    {
        #region --属性--
        /// <summary>
        /// 获取或设置远程端点的信息
        /// </summary>
        public EndPoint RemoteEndPoint;

        /// <summary>
        /// 获取或设置数据
        /// </summary>
        public byte[] Data;
        #endregion

        #region --私有方法--
        /// <summary>
        /// 创建一个新的 <see cref="UdpState"/>
        /// </summary>
        /// <param name="bufSize">缓冲区大小</param>
        /// <returns>一个新的 <see cref="UdpState"/></returns>
        internal static UdpState Create (int bufSize)
        {
            return new UdpState ()
            {
                RemoteEndPoint = new IPEndPoint (IPAddress.Any, 0),
                Data = new byte[bufSize],
            };
        }

        /// <summary>
        /// 创建一个新的 <see cref="UdpState"/>
        /// </summary>
        /// <param name="remote">指定远程端点</param>
        /// <param name="data">指定状态中的数据</param>
        /// <returns>一个新的 <see cref="UdpState"/></returns>
        internal static UdpState Create (EndPoint remote, byte[] data)
        {
            return new UdpState ()
            {
                RemoteEndPoint = remote,
                Data = data
            };
        }
        #endregion
    }

}
