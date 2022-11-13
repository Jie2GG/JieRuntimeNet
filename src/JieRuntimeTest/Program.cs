using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

using JieRuntime.Extensions;
using JieRuntime.Ini;
using JieRuntime.IO;
using JieRuntime.Ipc;
using JieRuntime.Net.Sockets;
using JieRuntime.Rpc.Tcp;
using JieRuntime.Rpc.Tcp.Messages;
using JieRuntime.Utils;

using TcpClient = JieRuntime.Net.Sockets.TcpClient;
using UdpClient = JieRuntime.Net.Sockets.UdpClient;

#pragma warning disable
namespace JieRuntimeTest
{
    public static class Program
    {
        public static void Main ()
        {
            IniConfiguration ini = new IniConfiguration (@"D:\1.ini");
            ini.Configuration.Add (new IniSection ("Test")
            {
                { "Key1", "Value1" },
                { "Key2", 10 }
            });
            ini.Save ();

            Console.ReadLine ();
        }
    }
}


#pragma warning restore