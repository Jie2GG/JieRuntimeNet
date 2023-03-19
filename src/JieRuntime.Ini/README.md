<p align="center">
 <img width="100px" src="https://avatars.githubusercontent.com/u/39112172?v=4" align="center" alt="JieRuntimeNet.Ini" />
 <h2 align="center">JieRuntimeNet.Ini</h2>
</p>
  <p align="center">
    <p align="center">
    <a href="https://www.nuget.org/packages/JieRuntime.Ini">
      <img alt="Nuget" src="https://img.shields.io/nuget/v/JieRuntime.Ini?label=Nuget">
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

JieRuntime.Ini 是基于 .NET 平台的一款跨平台 ini 配置项处理工具类, 该工具能快速实现 ini 配置文件的增删改查, 轻松分离 ini 配置文件中的 "节"、"键"、"值" 和 "注释", 大量使用了 .NET 平台的特性, 使其更易于使用.

# 快速开始

本运行库基于 .net standard 2.0 构建, 可以在支持 .net standard 2.0 依赖的任意项目上引用本运行库. 你可以在 Nuget 上搜索 JieRuntime.Ini 来安装本运行库.
  
# Demo 

> 1. 创建一个新的配置项
```C#
    // 指定文件路径, 默认使用 UTF8 编码
    IniConfiguration ini = new IniConfiguration (文件路径);
    // 指定文件路径, 指定文件的编码
    IniConfiguration ini = new IniConfiguration (文件路径, 文件编码);
```  

> 2. 从现有的配置项读取文件
```C#
    // 创建对象时指定了文件路径, 因此直接读取
    ini.Load ();
```

> 3. 增加配置
```C#
    // 增加 "节"
    ini.Configuration.Add (new IniSection ("Section1"));
    // 增加 "键值"
    ini.Configuration["Section1"].Add ("key1", value);

    // 如果 Section1 或 Key1 不存在会自动创建
    ini.Configuration["Section1"]["Key1"] = value;
```

> 4. 删除配置
```C#
    // 删除 "节"
    ini.Configuration.Remove ("Section1");

    // 删除 "键值"
    ini.Configuration["Section1"].Remove ("Key1");
```

> 5. 修改配置
```C#
    // 修改 "节"
    // 无论传入的 IniSection 对象的 Name 是何值, 都会被修改成 Section1
    ini.Configuration["Section1"] = new IniSection("xxxxx");

    // 修改 "键值"
    // value 可以是已知范围的任何类型的值, 将会自动转换为文本, 如果 Section1 或 Key1 不存在会自动创建
    ini.Configuration["Section1"]["Key1"] = value;
```

> 6. 查询配置
```C#
    // 查询 "节", 不存在会自动创建
    IniSection section = ini.Configuration["Section1"];

    // 查询 "键值", 不存在会返回默认值, 会依据返回值的类型自动转换
    var value = ini.Configuration["Section1"]["Key1"];
```

> 7. 将修改保存到文件
```C#
    // 创建对象时指定了文件路径, 因此直接写入
    ini.Save ();
```