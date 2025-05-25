using ModelContextProtocol.Client;

// 自动检测 Server 项目路径
string serverProjectPath = "MCP.Demo.Server/MCP.Demo.Server.csproj";
if (!File.Exists(serverProjectPath))
{
    var altPath = Path.Combine("..", "MCP.Demo.Server", "MCP.Demo.Server.csproj");
    if (File.Exists(altPath))
        serverProjectPath = altPath;
}

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "MCP",
    Command = "dotnet",
    Arguments = ["run", "--project", serverProjectPath],
});

// 创建 MCP 客户端
var client = await McpClientFactory.CreateAsync(clientTransport);


// 交互式工具调用
while (true)
{
    var tools = (await client.ListToolsAsync()).ToList();
    Console.WriteLine("\n📋 可用工具列表:");
    for (int i = 0; i < tools.Count; i++)
    {
        Console.WriteLine($"[{i}] {tools[i].Name} - {tools[i].Description}");
    }
    Console.WriteLine("[Q] 退出");

    Console.Write("\n🔧 请选择要调用的工具编号: ");
    var input = Console.ReadLine();
    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
        break;

    if (!int.TryParse(input, out int idx) || idx < 0 || idx >= tools.Count)
    {
        Console.WriteLine("❌ 输入无效，请重试。");
        continue;
    }
    var tool = tools[idx];
    var argsDict = new Dictionary<string, object?>();

    // 根据工具名称手动处理参数输入，支持相对和绝对路径
    switch (tool.Name)
    {
        case "Echo":
            Console.Write("📝 请输入参数 message (要回显的消息): ");
            argsDict["message"] = Console.ReadLine();
            break;
        case "CreateFile":
            Console.Write("📝 请输入参数 filePath (支持相对或绝对路径): ");
            argsDict["filePath"] = Console.ReadLine();
            Console.Write("📝 请输入参数 content (文件内容): ");
            argsDict["content"] = Console.ReadLine();
            break;
        case "DeleteFile":
        case "FileExists":
        case "ReadFile":
            Console.Write("📝 请输入参数 filePath (支持相对或绝对路径): ");
            argsDict["filePath"] = Console.ReadLine();
            break;
        case "GetFileInfo":
            Console.Write("📝 请输入参数 path (支持相对或绝对路径): ");
            argsDict["path"] = Console.ReadLine();
            break;
        default:
            Console.WriteLine("⚠️ 未知工具，跳过参数输入");
            break;
    }

    try
    {
        var result = await client.CallToolAsync(
            tool.Name,
            argsDict,
            cancellationToken: CancellationToken.None);

        var textContent = result.Content.FirstOrDefault(c => c.Type == "text");
        if (textContent != null)
            Console.WriteLine($"\n📤 结果: {textContent.Text}");
        else
            Console.WriteLine("\n⚠️ 无文本结果。");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ 调用工具时出错: {ex.Message}");
    }
}

Console.WriteLine("👋 程序已退出。");
