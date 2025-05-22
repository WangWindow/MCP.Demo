# 🚀 MCP.Demo 示例项目

本项目演示了如何使用 [Model Context Protocol (MCP)](https://github.com/modelcontextprotocol) C# SDK 实现一个简单的 MCP Server 和 MCP Client。

---

## 📦 项目结构

- `MCP.Demo.Server`：MCP 服务端，提供 Echo 工具。
- `MCP.Demo.Client`：MCP 客户端，支持交互式调用服务端工具。

---

## 🛠️ 功能简介

### MCP Server
- 基于 `ModelContextProtocol.Server` 实现。
- 通过标准输入输出（stdio）与客户端通信。
- 提供 `Echo` 工具：原样返回客户端输入的消息。

### MCP Client
- 自动发现服务端所有工具。
- 支持交互式选择工具、输入参数并调用。
- 实时显示服务端返回结果。

---

## ▶️ 启动方式

### 1️⃣ 还原依赖包

```shell
dotnet restore
```

### 2️⃣ 启动 MCP Client

```shell
cd MCP.Demo.Client
dotnet run
```

> ⚠️ 建议先启动 Server，再启动 Client。

---

## ✨ 交互演示

1. 启动后，Client 会自动列出所有可用工具：

```
可用工具列表:
[0] Echo (Echoes the message back to the client.)
```

2. 选择工具编号，输入参数：

```
请选择要调用的工具编号: 0
请输入参数 message (Echoes the message back to the client.): 你好，MCP！

结果: hello 你好，MCP！
```

---

## 📚 参考
- [Model Context Protocol 官方文档](https://github.com/modelcontextprotocol)
- [NuGet: ModelContextProtocol](https://www.nuget.org/packages/ModelContextProtocol)

---

> 🧑‍💻 欢迎体验和二次开发！如有问题请提交 Issue。
