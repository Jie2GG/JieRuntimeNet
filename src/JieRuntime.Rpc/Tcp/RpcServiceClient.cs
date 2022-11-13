using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

using JieRuntime.Ipc;
using JieRuntime.Net.Sockets;
using JieRuntime.Rpc.Tcp.Messages;
using JieRuntime.Utils;

using Microsoft.Extensions.ObjectPool;

namespace JieRuntime.Rpc.Tcp
{
    /// <summary>
    /// 基于 TCP 协议提供远程调用服务客户端
    /// </summary>
    public class RpcServiceClient : RpcServiceClientBase, IRpcServiceProxy
    {
        #region --字段--
        private readonly IPEndPoint remoteEP;
        private readonly ObjectPool<TcpWait> waitPool;
        private readonly ConcurrentDictionary<long, TcpWait> waitReference;
        private readonly MessageFragmentManager fragmentManager;

        private static readonly JsonSerializerOptions jsonSerializeOptions = new ()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        #endregion

        #region --属性--
        /// <summary>
        /// 获取基础网络客户端 <see cref="TcpClient"/>
        /// </summary>
        public TcpClient Client { get; }

        /// <summary>
        /// 获取一个 <see cref="bool"/> 值, 指示当前客户端是否正在运行
        /// </summary>
        public override bool IsRunning => this.Client.IsRunning;
        #endregion

        #region --事件--
        /// <summary>
        /// 表示远程调用客户端成功连接到服务器的事件
        /// </summary>
        public override event EventHandler<RpcServiceEventArgs> Connected;

        /// <summary>
        /// 表示远程调用客户端断开与远程服务器连接的事件
        /// </summary>
        public override event EventHandler<RpcServiceEventArgs> Disconnected;

        /// <summary>
        /// 表示远程调用客户端出现异常的事件
        /// </summary>
        public override event EventHandler<RpcServiceExceptionEventArgs> Exception;
        #endregion

        #region --构造函数--
        /// <summary>
        /// 初始化 <see cref="RpcServiceClient"/> 类的新实例
        /// </summary>
        /// <param name="remoteEP">指定远程服务端的地址</param>
        /// <exception cref="ArgumentNullException"><paramref name="remoteEP"/> 是 <see langword="null"/></exception>
        public RpcServiceClient (IPEndPoint remoteEP)
            : this (new TcpClient ())
        {
            this.remoteEP = remoteEP ?? throw new ArgumentNullException (nameof (remoteEP));
        }

        internal RpcServiceClient (TcpClient client)
        {
            // 关联客户端事件
            this.Client = client ?? throw new ArgumentNullException (nameof (client));
            this.Client.PacketSize = ushort.MaxValue;
            this.Client.Connected += this.ClientConnectedEventhandler;
            this.Client.Disconnected += this.ClientDisconnectedEventHandler;
            this.Client.ReceiveData += this.ClientReceiveDataEventHandler;
            this.Client.Exception += this.ClientExceptionEventHandler;

            // 创建对象池
            this.waitPool = ObjectPool.Create (new TcpWaitPolicy ());
            this.waitReference = new ConcurrentDictionary<long, TcpWait> ();

            // 创建消息分片管理器
            this.fragmentManager = new MessageFragmentManager ();
        }
        #endregion

        #region --公开方法--
        /// <summary>
        /// 连接到远程调用服务端
        /// </summary>
        public override void Connect ()
        {
            this.Client?.Connect (this.remoteEP);
        }

        /// <summary>
        /// 断开与远程调用服务端的连接
        /// </summary>
        public override void Disconnect ()
        {
            this.Client?.Disconnect (true);
        }

        /// <summary>
        /// 向远程调用服务端发送数据, 以响应远程调用服务端的请求
        /// </summary>
        /// <param name="tag">指定数据的唯一标识</param>
        /// <param name="data">要发送的数据</param>
        protected override byte[] SendWaitResponse (long tag, byte[] data)
        {
            if (!this.waitReference.ContainsKey (tag))
            {
                // 获取 TCP 等待对象
                TcpWait wait = this.waitPool.Get ();

                // 等待对象和 tag 绑定
                if (this.waitReference.TryAdd (tag, wait))
                {
                    try
                    {
                        // 准备好消息等待
                        wait.WaitHandler.Reset ();

                        // 发送消息
                        this.Send (new Message (MessageType.Request, tag, data));

                        // 等待消息返回
                        if (this.WaitResponseTime == TimeSpan.Zero)
                        {
                            wait.WaitHandler.WaitOne ();
                        }
                        else
                        {
                            wait.WaitHandler.WaitOne (this.WaitResponseTime);
                        }

                        // 如果结果是 null 并且处于获取过响应的状态, 表示传输出现了问题
                        if (wait.Resutl == null && wait.IsGetResponse)
                        {
                            throw new JsonRpcNetworkException ();
                        }

                        // 获取响应数据
                        return wait.Resutl;
                    }
                    finally
                    {
                        this.waitReference.TryRemove (tag, out _);
                        this.waitPool.Return (wait);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 向远程调用服务端发送数据, 并等待服务端的回应
        /// </summary>
        /// <param name="tag">指定数据的唯一标识</param>
        /// <param name="data">要发送的数据</param>
        /// <returns>一个字节数组, 包含服务端回应的数据</returns>
        protected override void SendResponse (long tag, byte[] data)
        {
            // 发送消息
            this.Send (new Message (MessageType.Response, tag, data));
        }

        /// <summary>
        /// 获取远程调用服务代理对象
        /// </summary>
        /// <returns>一个 <see cref="IRpcServiceProxy"/>, 用于处理远程服务调用代理</returns>
        protected override IRpcServiceProxy GetProxyHandler ()
        {
            return this;
        }

        object IRpcServiceProxy.InvokeMethod (MethodInfo targetMethod, object[] args)
        {
            try
            {
                // 获取方法相关信息
                Type sourceType = targetMethod.DeclaringType;                       // 方法的来源类型
                ParameterInfo[] parameterInfos = targetMethod.GetParameters ();     // 方法的所有参数

                // 构建请求
                JsonRpcRequest request = new ()
                {
                    Type = sourceType.Name,
                    Method = targetMethod.Name,
                    Parameters = new JsonRpcParameter[args.Length],
                };

                // 填充参数
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    request.Parameters[i] = new JsonRpcParameter ()
                    {
                        Name = parameterInfos[i].Name,
                        Value = args[i]
                    };
                }

                // 序列化 Json
                byte[] requestBody = JsonSerializer.SerializeToUtf8Bytes (request, jsonSerializeOptions);

                // 发送请求
                long tag = GuidUtils.NewGuidInt64 ();
                byte[] responseBody = this.SendWaitResponse (tag, requestBody);

                // 有返回值的请求, 需要等待返回值
                if (responseBody != null)
                {
                    JsonRpcResponse response = JsonSerializer.Deserialize<JsonRpcResponse> (responseBody, jsonSerializeOptions);

                    // 如果有错误对象的存在, 则表示远程调用出现的异常
                    if (response.Error != null)
                    {
                        // 错误处理
                        throw new JsonRpcException (response.Error);
                    }
                    else
                    {
                        // 如果错误对象不存在, 则需要回填参数
                        for (int i = 0; i < parameterInfos.Length; i++)
                        {
                            Type parameterType = parameterInfos[i].ParameterType;

                            // 判断是否是按引用传递的类型. 例如 ref int...
                            if (parameterType.IsByRef)
                            {
                                parameterType = parameterType.GetElementType ();    // 获取此类类型的真实类型
                            }

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
                throw;  // 这个异常需要呈现给用户, 则直接抛出
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
        private void Send (Message message)
        {
            ReadOnlySpan<MessageFragment> fragments = MessageFragment.CreateFragments (message.MessageType, message.MessageTag, message.Data);
            foreach (MessageFragment fragment in fragments)
            {
                this.Client.Send (fragment.GetBytes ());
            }
        }

        private void InvokeConnectedEvent ()
        {
            RpcServiceEventArgs e = new ();
            this.OnConnected (e);
            this.Connected?.Invoke (this, e);
        }

        private void InvokeDisconnectedEvent ()
        {
            RpcServiceEventArgs e = new ();
            this.OnDisconnected (e);
            this.Disconnected?.Invoke (this, e);
        }

        private void InvokeExceptionEvent (Exception exception)
        {
            RpcServiceExceptionEventArgs e = new (exception);
            this.OnException (e);
            this.Exception?.Invoke (this, e);
        }

        private void ClientConnectedEventhandler (object sender, SocketEventArgs e)
        {
            this.InvokeConnectedEvent ();
        }

        private void ClientDisconnectedEventHandler (object sender, SocketEventArgs e)
        {
            this.InvokeDisconnectedEvent ();
        }

        private void ClientReceiveDataEventHandler (object sender, SocketDataEventArgs e)
        {
            // 接收消息分片
            if (MessageFragment.TryParse (e.Data, out MessageFragment? fragment))
            {
                // 分片重组
                this.fragmentManager.Push (fragment.Value);
                // 拉取消息
                while (this.fragmentManager.TryPull (out Message? result))
                {
                    switch (result.Value.MessageType)
                    {
                        case MessageType.Request:
                            {
                                JsonRpcRequest request = null;
                                JsonRpcResponse response = new ();

                                try
                                {
                                    request = JsonSerializer.Deserialize<JsonRpcRequest> (result.Value.Data, jsonSerializeOptions);

                                    // 根据类型获取指定的服务
                                    if (this.Dictionary.ContainsKey (request.Type))
                                    {
                                        // 获取服务类型对应的类型接口
                                        Type type = this[request.Type].InterfaceType;

                                        // 获取指定的成员
                                        MemberInfo memberInfo = type.GetMember (request.Method)
                                            .FirstOrDefault (p =>
                                            {
                                                // 如果成员是方法
                                                if (p is MethodInfo methodInfo)
                                                {
                                                    // 获取参数列表
                                                    ParameterInfo[] parameterInfos = methodInfo.GetParameters ();

                                                    // 参数个数判断
                                                    if (parameterInfos.Length != request.Parameters.Length)
                                                    {
                                                        return false;
                                                    }

                                                    // 参数名称判断
                                                    for (int i = 0; i < parameterInfos.Length; i++)
                                                    {
                                                        if (parameterInfos[i].Name != request.Parameters[i].Name)
                                                        {
                                                            return false;
                                                        }
                                                    }

                                                    return true;
                                                }

                                                return false;
                                            });

                                        // 将成员转换为方法
                                        if (memberInfo is MethodInfo methodInfo)
                                        {
                                            // 创建传参数组
                                            object[] args = new object[request.Parameters.Length];

                                            // 获取方法中的所有参数
                                            ParameterInfo[] parameterInfos = methodInfo.GetParameters ();

                                            // 将 Json 转换为对象
                                            for (int i = 0; i < args.Length; i++)
                                            {
                                                if (parameterInfos[i].IsOut)    // 处理 Out 参数
                                                {
                                                    args[i] = null;
                                                }
                                                else
                                                {
                                                    Type parameterType = parameterInfos[i].ParameterType;

                                                    // 如果类型是按引用传递 (ref) 的类型, 则获取其真实的类型
                                                    if (parameterType.IsByRef)
                                                    {
                                                        parameterType = parameterType.GetElementType ();
                                                    }

                                                    if (request.Parameters[i].Value is JsonElement element)
                                                    {
                                                        args[i] = element.Deserialize (parameterType, jsonSerializeOptions);
                                                    }
                                                }
                                            }

                                            // 调用方法

                                            object returnValue = methodInfo.Invoke (this[request.Type].Instance, args);

                                            // 赋值返回值
                                            response.Result = returnValue;
                                            response.Parameters = new JsonRpcParameter[args.Length];

                                            // 赋值改变的参数: Ref, Out
                                            for (int i = 0; i < args.Length; i++)
                                            {
                                                response.Parameters[i] = new JsonRpcParameter ()
                                                {
                                                    Name = parameterInfos[i].Name,
                                                    Value = args[i]
                                                };
                                            }
                                        }
                                        else
                                        {
                                            response.SetError (JsonRpcError.CreateFromMethodNotFoundError (request.Type, request.Method));
                                        }
                                    }
                                    else
                                    {
                                        response.SetError (JsonRpcError.CreateFromTypeNotFoundError (request.Type));
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    // 包装成Json解析错误
                                    response.SetError (JsonRpcError.CreateFromFormateError (ex));
                                }
                                catch (TargetInvocationException ex)
                                {
                                    // 包装成应用异常错误
                                    response.SetError (JsonRpcError.CreateFromApplicationError ($"在执行方法“{request!.Method}”时发生了异常", ex.InnerException));
                                }
                                catch (MethodAccessException ex)
                                {
                                    // 包装成应用异常错误
                                    response.SetError (JsonRpcError.CreateFromApplicationError ($"无法执行方法“{request!.Method}”, 指定的方法没有执行权限", ex.InnerException));
                                }
                                catch (Exception ex)
                                {
                                    // 包装成系统错误
                                    response.SetError (JsonRpcError.CreateFromSystemError ($"发生错误", ex));
                                }

                            SEND_JSON:
                                byte[] responseData;
                                try
                                {
                                    responseData = JsonSerializer.SerializeToUtf8Bytes (response, jsonSerializeOptions);
                                }
                                catch (Exception ex)
                                {
                                    // 发送一个错误防止对端卡住, 这个Json是一定可以被序列化的
                                    response.SetError (JsonRpcError.CreateFromSystemError ($"发生错误", ex));

                                    // 跳转到上层重新发送
                                    goto SEND_JSON;
                                }

                                try
                                {
                                    this.SendResponse (result.Value.MessageTag, responseData);
                                }
                                catch (Exception ex)
                                {
                                    this.InvokeExceptionEvent (ex);
                                }
                            }
                            break;
                        case MessageType.Response:
                            {
                                // 处理对端的 TCP 响应
                                if (this.waitReference.TryGetValue (result.Value.MessageTag, out TcpWait wait))
                                {
                                    wait.IsGetResponse = true;
                                    wait.Resutl = result.Value.Data;
                                    wait.WaitHandler.Set ();
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void ClientExceptionEventHandler (object sender, SocketExceptionEventArgs e)
        {
            this.InvokeExceptionEvent (e.Exception);
        }
        #endregion
    }
}
