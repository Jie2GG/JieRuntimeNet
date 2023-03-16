<p align="center">
 <img width="100px" src="https://avatars.githubusercontent.com/u/39112172?v=4" align="center" alt="JieRuntimeNet" />
 <h2 align="center">JieRuntimeNet</h2>
</p>
  <p align="center">
    <a href="#demo">查看 Demo</a>
    ·
    <a href="https://github.com/Jie2GG/JieRuntimeNet/issues">报告 Bug</a>
    ·
    <a href="https://github.com/Jie2GG/JieRuntimeNet/issues">请求增加功能</a>
  </p>
</p>
<p align="center">喜欢这个项目？给我点个Star吧！

# 特性
  
- [Hook](#Hook)
- [IniConfiguration](#Ini)
- [Io](#Io)
- [Net](#Net)
- [Rpc](#Rpc)
- [Windows](#Windows)


# Hook


# IniConfiguration
  
### IniConfiguration 自述
  IniConfiguration 是基于 C# 开发, 针对于 Windows 平台下 Ini 配置文件的一款工具类, 
	该工具能快速的将 Ini 配置文件的 "节点", "键", "值", "注释" 分开, 在轻松实现对
	Ini 配置文件的增删改查的同时, 可直接移植到其它平台使用.
  
### IniConfiguration 示例 
  
> 1. 创建一个新的 Ini 配置项
```C#
  IniConfiguration ini = new IniConfiguration (文件路径);
```
> 2. 读取文件.
```C#
  ini.load()
```
> 3. 对文件的节点,value进行读取,读取value会自动判定变量的类型,根据对应的类型来读取value
```C#
  string str= ini.Configuration["节点1"][key];(value => string)
  int a = ini.Configuration["节点1"][key];(value => int)
```  
> 3. 也可以直接对文件的节点下的数据进行全部读取.
```C#
  IniSection Section= ini.Configuration["节点1"];
```  
> 4.  修改 Ini 配置文件
```C#
  ini.Configuration["节点1"][key] = str;
  ini.Configuration["节点1"][key1] = int;
  ini.Configuration["节点1"][key2] = fload;
  ini.save()
```  
  
  
# Io


# Net


# Rpc


# Windows
