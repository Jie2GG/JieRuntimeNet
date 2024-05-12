using System;

using JieRuntime.Rpc.Attributes;
using JieRuntime.Rpc.Tcp;

namespace JieRuntimeTest
{
    public static class Program
    {
        public static void Main ()
        {
            RpcServer server = new (8000);
            server.Register<IService> (new Service ());
            server.ClientConnected += (sender, e) =>
            {
                Console.WriteLine ("Client connected");
            };
            server.Start ();

            Console.Read ();
        }

    }

    [RemoteType ("IService")]
    interface IService
    {
        [RemoteMethod ("HelloMethod")]
        void Hello ([RemoteParameter ("valueA")] int var0, [RemoteParameter ("valueB")] double var1, [RemoteParameter ("valueC")] string var2);
    }

    class Service : IService
    {
        public void Hello (int var0, double var1, string var2)
        {
            Console.WriteLine ("Hello, World!");
        }
    }
}