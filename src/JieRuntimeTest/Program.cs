using System;
using System.Net;

using JieRuntime.Ini;
using JieRuntime.Rpc.Attributes;
using JieRuntime.Rpc.Tcp;

namespace JieRuntimeTest
{
    public static class Program
    {
        public static void Main ()
        {
            //IniConfiguration config = new (@"D:\Csharp\ChattyBot\ChattyBot.Client\bin\x86\Debug\net8.0-windows\data\10000\config.ini");
            //config.Load ();
            //bool? value = config.Configuration["AppStatus"]["com.chattybot.demo"];

            RpcServer server = new (8023);
            server.Start ();
            server.ClientConnected += (sender, e) =>
            {
                server.Register<IService> (new Service ());
            };

            RpcClient client = new (new IPEndPoint (IPAddress.Loopback, 8023));
            client.Connect ();

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
            throw new NotImplementedException ();
        }
    }
}