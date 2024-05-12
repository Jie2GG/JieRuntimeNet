using Microsoft.Extensions.ObjectPool;

namespace JieRuntime.Rpc.Tcp
{
    class TcpWaitObjectFactory : IPooledObjectPolicy<TcpWait>
    {
        public TcpWait Create ()
        {
            return new TcpWait ();
        }

        public bool Return (TcpWait obj)
        {
            if (obj is not null)
            {
                // 释放阻塞线程
                obj.WaitHandler.Set ();
                obj.IsResponse = false;
                obj.Resutl = null;
                return true;
            }
            return false;
        }
    }
}
