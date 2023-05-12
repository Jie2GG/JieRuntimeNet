<p align="center">
 <img width="100px" src="https://avatars.githubusercontent.com/u/39112172?v=4" align="center" alt="JieRuntimeNet.IO" />
 <h2 align="center">JieRuntimeNet.IO</h2>
</p>
  <p align="center">
    <p align="center">
    <a href="https://www.nuget.org/packages/JieRuntime.IO">
      <img alt="Nuget" src="https://img.shields.io/nuget/v/JieRuntime.IO?label=Nuget">
    </a>
    <a href="../LICENSE">
      <img alt="GitHub" src="https://img.shields.io/github/license/Jie2GG/JieRuntimeNet">
    </a>
    <br/>
    <a href="https://github.com/Jie2GG/JieRuntimeNet/issues">报告 Bug</a>
    ·
    <a href="https://github.com/Jie2GG/JieRuntimeNet/issues">请求增加功能</a>
  </p>
</p>
<p align="center">喜欢这个项目？给我点个Star吧！

# 关于项目

JieRuntimeNet.IO 库是一个用于快速操作字节流的库. 它提供了一组功能强大的 API, 可以快速读取、写入、转换字节流数据, 以及进行各种常见的二进制数据操作. 可以方便地对二进制数据进行读写操作, 如读取固定长度的字节、读取整型、浮点型等数据类型等.

# 快速开始

本运行库基于 .net standard 2.0 构建, 可以在支持 .net standard 2.0 依赖的任意项目上引用本运行库. 你可以在 Nuget 上搜索 JieRuntime.IO 来安装本运行库.
  
# Demo 

> 1. 读取字节流
```csharp
// ...假设 data 是从外部获取的字节流
byte[] data = new byte[1000];
// BufferReader 可以从现有的 Stream 或 byte[] 进行拆包读取
using BufferReader reader = new (data);

// 1. 读取基本数据类型 (true 则以大端序读取, false 则以小端序读取)
byte v1 = reader.ReadByte (true);
char v2 = reader.ReadChar (false);
bool v3 = reader.ReadBoolean ();
short v4 = reader.ReadInt16 ();     // 如果不传, 默认: true
ushort v5 = reader.ReadUInt16 ();
int v6 = reader.ReadInt32 ();
uint v7 = reader.ReadUInt32 ();
long v8 = reader.ReadInt64 ();
ulong v9 = reader.ReadUInt64 ();
float v10 = reader.ReadSingle ();
double v11 = reader.ReadDouble ();

// 2. 读取多字节
byte[] buf = reader.ReadAll ();         // 读取剩余所有的字节
char[] chars = reader.ReadChars (10);   // 读取10个字符
byte[] buf2 = reader.ReadBytes (10);    // 读取10个字节

// 3. 增加、回退流操作
reader.PutData (RandomUtils.RandomBytes (10));  // 向数据流尾部增加要读取的数据
reader.Rollback (100);                          // 将游标向前移动100个字节, 已读取的数据还可以重新读取
```

> 2. 写入字节流
```csharp
// BufferWriter 可以往现有的流中写入数据, 如果不传入则默认创建一个内存流
using BufferWriter writer = new ();

// 1. 写入基本数据类型 (true 则以大端序写入, false 则以小端序写入)
writer.Write ((byte) 0x01);
writer.Write (true);
writer.Write ('c', true);
writer.Write ((short) 10, false);
writer.Write ((ushort)11);  // 如果不传, 默认: true
writer.Write (12);
writer.Write ((uint)13);
writer.Write ((long)14);
writer.Write ((ulong)15);
writer.Write ((float)16.1);
writer.Write (16.2);

// 2. 写入多字节
writer.Write (RandomUtils.RandomBytes (10));    // 写入一组字节
writer.Write ("JieRuntime".ToCharArray ());     // 写入一组字符

// 3. 获取已写入数据的字节流, 获取后不会清空流
writer.GetBytes ();

// 4. 清空写入的数据
writer.Clear ();
```