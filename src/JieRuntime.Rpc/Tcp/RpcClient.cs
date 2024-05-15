using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

using JieRuntime.Net.Sockets;
using JieRuntime.Net.Sockets.Tcp;
using JieRuntime.Rpc.Attributes;
using JieRuntime.Rpc.Exceptions;
using JieRuntime.Rpc.Tcp.Exceptions;
using JieRuntime.Rpc.Tcp.Messages;
using JieRuntime.Rpc.Tcp.Packets;
using JieRuntime.Utils;

using Microsoft.Extensions.ObjectPool;


namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 提供基于 TCP 协议的远程调用客户端类
    /// </summary>
    public class RpcClient : RpcClientBase
    {
        #region --常量--
        private static readonly JsonSerializerOptions DefualtSerializerOptions = new ()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        #endregion

        #region --字段--
        private readonly IPEndPoint remoteEndPoint;
        private readonly ObjectPool<TcpWait> tcpWaitPool;
        private readonly ConcurrentDictionary<long, TcpWait> tcpWaitCaches;
        private readonly MessageCache messageCache;
        #endregion

        #region --属性--
        /// <summary>
        /// 获取基础网络客户端 <see cref="TcpClient"/>
        /// </summary>
        public TcpClient Client { get; }
        #endregion

        #region --事件--
        /// <summary>
        /// 客户端连接远程主机事件
        /// </summary>
        public override event EventHandler<RpcEventArgs> Connected;

        /// <summary>
        /// 客户端断开远程主机连接事件
        /// </summary>
        public override event EventHandler<RpcEventArgs> Disconnected;

        /// <summary>
        /// 客户端出现异常的事件
        /// </summary>
        public override event EventHandler<RpcExceptionEventArgs> Exception;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcClient"/> 类的新实例
        /// </summary>
        /// <param name="remoteEP">表示远程设备的 <see cref="IPEndPoint"/></param>
        public RpcClient (IPEndPoint remoteEP)
            : this (new TcpClient ())
        {
            this.remoteEndPoint = remoteEP ?? throw new ArgumentNullException (nameof (remoteEP));
        }

        /// <summary>
        /// 初始化 <see cref="RpcClient"/> 类的新实例
        /// </summary>
        /// <param name="client">基础网络客户端 <see cref="TcpClient"/></param>
        internal RpcClient (TcpClient client)
        {
            // 初始化网络套接字客户端
            this.Client = client ?? throw new ArgumentNullException (nameof (client));
            this.Client.Connected += this.ClientConnctedEventhandler;
            this.Client.Disconnected += this.ClientDisconnectedEventHandler;
            this.Client.Received += this.ClientReceivedEventHandler;
            this.Client.Exception += this.ClientExceptionEventHandler;

            // 创建对象池
            this.tcpWaitPool = ObjectPool.Create (new TcpWaitObjectFactory ());
            this.tcpWaitCaches = new ConcurrentDictionary<long, TcpWait> ();

            // 创建消息缓存
            this.messageCache = new MessageCache ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 开始连接远程主机
        /// </summary>
        public override void Connect ()
        {
            this.Client.Connect (this.remoteEndPoint);
        }

        /// <summary>
        /// 断开与远程主机的连接
        /// </summary>
        public override void Disconnect ()
        {
            this.Client.Disconnect (true);
        }

        /// <summary>
        /// 释放当前实例所占用的资源
        /// </summary>
        public override void Dispose ()
        {
            this.Client.Dispose ();
        }

        /// <summary>
        /// 每当调用代理类型上的任何方法时，都会调用此方法
        /// </summary>
        /// <param name="targetMethod">调用者调用的方法</param>
        /// <param name="args">调用者传递给方法的参数</param>
        /// <returns>返回给调用者的对象，void 方法将返回 <see langword="null"/></returns>
        public override object InvokeMethod (MethodInfo targetMethod, object[] args)
        {
            if (!this.Client.IsRunning || !this.Client.IsConnected)
            {
                // 抛出网络未连接异常
                throw new InvalidOperationException ("远程调用服务失败! 因为未连接到远程主机");
            }

            try
            {
                // 获取方法相关信息
                Type type = targetMethod.DeclaringType;
                ParameterInfo[] parameters = targetMethod.GetParameters ();

                // 构建 JsonRPC 请求体
                JsonRpcRequest request = new ();

                // 请求的类型优先使用 RemoteTypeAttribute 标记的名称, 否则使用类型名
                RemoteTypeAttribute remoteType = type.GetCustomAttribute<RemoteTypeAttribute> ();
                request.Type = remoteType is not null ? remoteType.Name : type.Name;

                // 请求的方法名优先使用 RemoteMethodAttribute 标记的名称，否则使用方法名
                RemoteMethodAttribute remoteMethod = targetMethod.GetCustomAttribute<RemoteMethodAttribute> ();
                request.Method = remoteMethod is not null ? remoteMethod.Name : targetMethod.Name;

                // 请求的参数优先使用 RemoteParameterAttribute 标记的名称，否则使用参数名
                request.Parameters = new JsonRpcParameter[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    RemoteParameterAttribute remoteParameter = parameters[i].GetCustomAttribute<RemoteParameterAttribute> ();
                    request.Parameters[i] = new JsonRpcParameter ()
                    {
                        Name = remoteParameter is not null ? remoteParameter.Name : parameters[i].Name,
                        Value = args[i]
                    };
                }

                // 序列化 Json
                byte[] requestBody = JsonSerializer.SerializeToUtf8Bytes (request, DefualtSerializerOptions);

                // 发送请求并等待响应
                long tag = GuidUtils.NewGuidInt64 ();
                byte[] responseBody = this.SendWaitResponse (tag, requestBody);

                // 解析返回值
                if (responseBody is not null)
                {
                    JsonRpcResponse response = JsonSerializer.Deserialize<JsonRpcResponse> (responseBody, DefualtSerializerOptions);

                    // 检查返回值是否为异常信息，如果是则抛出异常
                    if (response.Error is not null)
                    {
                        if (response.Error.Data is JsonElement element)
                        {
                            response.Error.Data = element.Deserialize<JsonRpcExceptionData> (DefualtSerializerOptions);
                            throw new JsonRpcException (response.Error);
                        }
                    }
                    else
                    {
                        // 比较请求时的参数信息和返回的是否一致
                        if (response.Parameters.Length != request.Parameters.Length)
                        {
                            throw new JsonRpcException (JsonRpcError.CreateFromResponseParameterCountError (request.Parameters.Length, response.Parameters.Length));
                        }

                        // 回填参数
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            // 判断参数名称是否一致，不一致则抛出异常
                            if (request.Parameters[i].Name != response.Parameters[i].Name)
                            {
                                throw new JsonRpcException (JsonRpcError.CreateFromResponseInvalidParameter (request.Method, i));
                            }

                            // 获取参数类型, 如果有 ref 则获取真实类型
                            Type parameterType = parameters[i].ParameterType;
                            parameterType = parameterType.IsByRef ? parameterType.GetElementType () : parameterType;

                            // 最后根据响应结果将值放入 args 完成按引用传递
                            if (response.Parameters[i].Value is JsonElement element)
                            {
                                args[i] = element.Deserialize (parameterType);
                            }
                        }

                        // 参数回填完毕, 最后处理返回值
                        if (targetMethod.ReturnType != typeof (void))
                        {
                            if (response.Result is JsonElement element)
                            {
                                return element.Deserialize (targetMethod.ReturnType);
                            }
                        }
                    }
                }
            }
            catch (JsonRpcException)
            {
                throw;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception e)
            {
                this.InvokeExceptionEvent (e);
            }

            // 如果返回值是值类型, 则需要创建一个默认值返回, 否则将会报空指针
            if (targetMethod.ReturnType != typeof (void) && targetMethod.ReturnType.IsValueType)
            {
                return Activator.CreateInstance (targetMethod.ReturnType);
            }

            return null;
        }
        #endregion

        #region --私有方法--
        private byte[] SendWaitResponse (long tag, byte[] data)
        {
            if (!this.tcpWaitCaches.ContainsKey (tag))
            {
                TcpWait tcpWait = this.tcpWaitPool.Get ();
                if (this.tcpWaitCaches.TryAdd (tag, tcpWait))
                {
                    try
                    {
                        tcpWait.WaitHandler.Reset ();
                        this.Send (MessageType.Request, tag, data);

                        // 等待消息返回
                        if (this.ResponseTimeout == TimeSpan.Zero)
                        {
                            tcpWait.WaitHandler.WaitOne ();
                        }
                        else
                        {
                            tcpWait.WaitHandler.WaitOne (this.ResponseTimeout);
                        }

                        if (!tcpWait.IsResponse)
                        {
                            throw new RpcTimeoutException ($"远程客户端 {this.Client.RemoteEndPoint} 响应超时");
                        }

                        return tcpWait.Resutl;
                    }
                    finally
                    {
                        this.tcpWaitCaches.TryRemove (tag, out tcpWait);
                        this.tcpWaitPool.Return (tcpWait);
                    }
                }
            }

            return null;
        }

        private void SendResponse (long tag, byte[] data)
        {
            this.Send (MessageType.Response, tag, data);
        }

        private void Send (MessageType type, long tag, byte[] data)
        {
            ReadOnlySpan<Message> messages = Message.Create (type, tag, data);
            foreach (Message message in messages)
            {
                this.Client.Send (message.GetBytes ());
            }
        }

        private void InvokeConnectedEvent ()
        {
            this.Connected?.Invoke (this, new RpcEventArgs ());
        }

        private void InvokeDisconnectedEvent ()
        {
            this.Disconnected?.Invoke (this, new RpcEventArgs ());
        }

        private void InvokeExceptionEvent (Exception exception)
        {
            this.Exception?.Invoke (this, new RpcExceptionEventArgs (exception));
        }

        private void ClientConnctedEventhandler (object sender, SocketEventArgs e)
        {
            this.InvokeConnectedEvent ();
        }

        private void ClientDisconnectedEventHandler (object sender, SocketEventArgs e)
        {
            this.InvokeDisconnectedEvent ();
        }

        private void ClientExceptionEventHandler (object sender, SocketExceptionEventArgs e)
        {
            this.InvokeExceptionEvent (e.Exception);
        }

        private void ClientReceivedEventHandler (object sender, SocketDataEventArgs e)
        {
            if (Message.TryParse (e.Data, out Message message))
            {
                this.messageCache.Push (message);
                while (this.messageCache.TryPull (out message))
                {
                    switch (message.Type)
                    {
                        case MessageType.Request:
                            {
                                JsonRpcRequest request = null;
                                JsonRpcResponse response = null;

                                try
                                {
                                    // 解析请求参数
                                    request = JsonSerializer.Deserialize<JsonRpcRequest> (message.Data);

                                    // 查找是否有对应的服务, 如果没有则返回错误信息
                                    if (!this.Services.ContainsKey (request.Type))
                                    {
                                        response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromInvalidType (request.Type));
                                        break;
                                    }


                                    // 查找服务中是否有对应的方法组, 如果没有则返回错误信息
                                    RpcInstance requestService = this.Services[request.Type];
                                    if (!requestService.Methods.ContainsKey (request.Method))
                                    {
                                        response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromInvalidMethod (request.Type, request.Method));
                                        break;
                                    }

                                    // 查找方法组中是否包含和请求一致的方法, 如果没有则返回错误信息
                                    MethodInfo methodInfo = requestService.Methods[request.Method]
                                            .FirstOrDefault (method =>
                                            {
                                                // 获取参数列表进行判断
                                                ParameterInfo[] parameters = method.GetParameters ();

                                                // 先判断个数是否一致, 不一致直接返回了
                                                if (parameters.Length != request.Parameters.Length)
                                                {
                                                    response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromInvalidParameter (request.Type, request.Method));
                                                    return false;
                                                }

                                                // 判断参数的名称是否一致, 优先获取 Attribute 中定义的名称
                                                for (int i = 0; i < parameters.Length; i++)
                                                {
                                                    RemoteParameterAttribute remoteParameter = parameters[i].GetCustomAttribute<RemoteParameterAttribute> ();
                                                    if (remoteParameter is not null && remoteParameter.Name != request.Parameters[i].Name && parameters[i].Name != request.Parameters[i].Name)
                                                    {
                                                        response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromInvalidParameter (request.Type, request.Method, i));
                                                        return false;
                                                    }
                                                }

                                                // 执行到这里表示匹配到了指定的方法, 把表示错误的响应销毁
                                                response = null;
                                                return true;
                                            });

                                    // 如果查找到的对应的方法, 则进行调用
                                    if (methodInfo is not null)
                                    {
                                        // 获取方法的参数列表
                                        ParameterInfo[] parameters = methodInfo.GetParameters ();

                                        // 创建传递的参数数组
                                        object[] args = new object[parameters.Length];

                                        // 将 Json 转换为对象
                                        for (int i = 0; i < args.Length; i++)
                                        {
                                            if (!parameters[i].IsOut)
                                            {
                                                // 获取参数的实际类型 (包括 ref 的实际类型)
                                                Type parameterType = parameters[i].ParameterType;
                                                parameterType = parameterType.IsByRef ? parameterType.GetElementType () : parameterType;

                                                // 反序列化参数值, 并转换为实际的类型 (包括 ref 的实际类型)
                                                if (request.Parameters[i].Value is JsonElement element)
                                                {
                                                    args[i] = element.Deserialize (parameterType, DefualtSerializerOptions);
                                                }
                                            }
                                        }

                                        // 调用方法, 并获取返回值 (如果有的话)
                                        object returnValue = methodInfo.Invoke (requestService.Instance, args);

                                        // 创建返回值
                                        response = new JsonRpcResponse ()
                                        {
                                            Result = returnValue,
                                            Parameters = new JsonRpcParameter[args.Length]
                                        };

                                        // 赋值改变的参数: ref, out
                                        for (int i = 0; i < args.Length; i++)
                                        {
                                            response.Parameters[i] = new JsonRpcParameter ()
                                            {
                                                Name = request.Parameters[i].Name,
                                                Value = args[i]
                                            };
                                        }
                                    }

                                }
                                catch (JsonException)
                                {
                                    response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromParseError ());
                                }
                                catch (TargetInvocationException ex)
                                {
                                    response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromInvokeError (request.Type, request.Method, ex));
                                }
                                catch (MethodAccessException ex)
                                {
                                    response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromInvokeError (request.Type, request.Method, ex));
                                }
                                catch (Exception ex)
                                {
                                    response = JsonRpcResponse.CreateFromError (JsonRpcError.CreateFromInternalError (ex));
                                }

                                // 发送报文到对端
                                try
                                {
                                    byte[] responseData = JsonSerializer.SerializeToUtf8Bytes (response, DefualtSerializerOptions);
                                    this.SendResponse (message.Id, responseData);
                                }
                                catch (Exception ex)
                                {
                                    this.InvokeExceptionEvent (ex);
                                }
                            }
                            break;
                        case MessageType.Response:
                            {
                                // 收到响应, 取等待缓存池获取等待对象, 并设置结果值, 并唤醒等待线程.
                                if (this.tcpWaitCaches.TryGetValue (message.Id, out TcpWait tcpWait))
                                {
                                    tcpWait.IsResponse = true;
                                    tcpWait.Resutl = message.Data;
                                    tcpWait.WaitHandler.Set ();
                                }
                            }
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
