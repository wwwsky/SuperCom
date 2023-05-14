# 快速入门

本文档使用 SuperCom 的基础教程

> 版本：SuperCom-3.6
>
> 时间：2023-05-10

# 环境配置

**运行环境**

.NET FrameWork 4.7 [点此下载](https://dotnet.microsoft.com/download/dotnet-framework)

**支持的系统**

本软件在 Win10 X64 上通过测试，在虚拟机的 Win7 X64 和 Win8 X64 上测试通过，不支持 WIN XP

**最低配置**

| 硬件配置 |         参数         |
| :------: | :------------------: |
|  处理器  | 1千兆赫 (GHz) 或更快 |
|   内存   |       2GB 以上       |
|   硬盘   |      16GB 以上       |
|   显卡   |   DirectX 9  以上    |

# 软件启动

解压 SuperCom-3.6.zip 后，双击 SuperCom.exe

> 管理员模式打开则支持虚拟串口的模拟

<img src="https://s1.ax1x.com/2023/05/10/p9reRET.png" alt="image-20230510074204611" style="zoom:80%;" />

打开后，**主界面**如下显示

<img src="https://s1.ax1x.com/2023/05/10/p9reWUU.png" alt="image-20230510074416835" style="zoom:80%;" />

任意打开一个串口进行连接

<img src="https://s1.ax1x.com/2023/05/10/p9re58J.png" alt="image-20230510074510655" style="zoom:80%;" />

关于**串口工作区**的功能及说明如下

<img src="https://s1.ax1x.com/2023/05/10/p9reI29.png" alt="image-20230510075328690" style="zoom: 80%;" />

| 功能         | 说明                                   |
| ------------ | -------------------------------------- |
| 断开/连接    | 断开或者连接串口，连接后自动保存日志   |
| 清屏         | 清除当前屏幕日志（本地硬盘的日志保存） |
| 固定/滚屏    | 可以固定日志，不滚屏（日志依然在收集） |
| 打开文件夹   | 打开日志所在文件目录                   |
| 打开日志     | 使用默认程序打开日志文件               |
| 加时间戳     | 可在每条日志前添加时间时间戳           |
| STR/HEX      | 切换字符串和 16 进制显示模式           |
| 日志立即分片 | 立即将日志保存到新的文件中             |
| RX/TX        | 接收/发送的字符数                      |

关于**串口配置区**的功能及说明如下

<img src="https://s1.ax1x.com/2023/05/10/p9reHDx.png" alt="image-20230510075343353" style="zoom:80%;" />

| 功能       | 说明                                                         |
| ---------- | ------------------------------------------------------------ |
| 波特率     | 设置串口收发的波特率，默认 9600，可下拉点击添加，或者在设置-串口设置中新增 |
| 数据位     | 数据位，默认为 8                                             |
| 编码       | 默认 UTF8                                                    |
| Parity     | 奇偶校验，默认 None                                          |
| StopBits   | 停止位，默认 One                                             |
| DTR        | 默认关                                                       |
| RTS        | 默认关                                                       |
| 读超时     | 默认 2000 ms                                                 |
| 写超时     | 默认 1000 ms                                                 |
| 握手       | 默认 None                                                    |
| 忽略空字符 | 默认关                                                       |
| 语法高亮   | 参考章节 <语法高亮>                                          |

关于**串口发送区**的功能及说明如下

<img src="https://s1.ax1x.com/2023/05/10/p9reLVK.png" alt="image-20230510075527916" style="zoom:80%;" />

| 功能    | 说明                         |
| ------- | ---------------------------- |
| 换行    | 发送时加回车换行（\r\n）     |
| STR/HEX | 切换字符串和 16 进制发送模式 |

# 命令编辑

SuperCom 支持保存自定义的命令，点击自动发送指令，循环发送指令

点击编辑命令按钮

<img src="https://s1.ax1x.com/2023/05/10/p9reX5D.png" alt="image-20230510075721558" style="zoom:80%;" />

显示窗口如下

<img src="https://s1.ax1x.com/2023/05/10/p9revPe.png" alt="image-20230510075856486" style="zoom:80%;" />

说明

<img src="https://s1.ax1x.com/2023/05/10/p9rex8H.png" alt="image-20230510075925207" style="zoom:80%;" />

- 添加命令后，勾选要发送的串口号，点击上述的运行三角形按钮，可以**循环**的向对应的串口发送命令

<img src="https://s1.ax1x.com/2023/05/10/p9rez2d.png" alt="image-20230510080028893" style="zoom:80%;" />

- 状态：就绪 (灰)、运行中 (黄)、完成 (绿)
- 顺序：决定运行的顺序，以及显示的顺序
- 名称：在主界面上显示的内容，不填则默认为 `发送命令`
- 延时：仅在循环发送中生效，发送**完成后**，暂停一段时间
- 命令：需要发送的串口命令，其发送字符串/16进制取决于**串口发送区**的发送模式

# 执行效果

我们打开串口，点击发送/接收，效果如下

<img src="https://s1.ax1x.com/2023/05/10/p9rmSxA.png" alt="image-20230510080625643" style="zoom:80%;" />






