# MotionEngine
MotionEngine是一个用于开发战斗服务器的框架。框架基于微软的.net core，所以服务器支持跨平台部署。

## 框架结构
框架分为多个模块，每个模块互相独立，开发者可以灵活选择需要的模块。

#### Engine.Common
通用模块

#### Engine.IO
IO模块：部分模块依赖于IO模块

#### Engine.Core
核心模块

#### Engine.AI
AI模块：有限状态机，神经网络，后面会加入A星寻路，导航网格寻路，行为树。

#### Engine.Math
数学库：支持SIMD的数学库，实现了Unity3D的Transform类和一个高效的Random类。

#### Engine.Net
网络库：异步IOCP SOCKET支持高并发，协议解析和生成过程无GC。

#### Engine.Res
资源模块：高效灵活的Config系统。

#### Engine.Utility
工具模块
