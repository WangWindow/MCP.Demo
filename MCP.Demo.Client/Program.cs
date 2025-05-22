using ModelContextProtocol.Client;

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "MCP",
    Command = "dotnet",
    Arguments = ["run", "--project", "../MCP.Demo.Server/MCP.Demo.Server.csproj"],
});

// 创建 MCP 客户端
var client = await McpClientFactory.CreateAsync(clientTransport);

// 列出所有可用工具
Console.WriteLine("可用工具列表:");
foreach (var tool in await client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}

// 调用 echo 工具
var result = await client.CallToolAsync(
    "Echo",
    new Dictionary<string, object?>() { ["message"] = "Hello MCP!" },
    cancellationToken: CancellationToken.None);

// 输出 echo 工具返回的文本内容
Console.WriteLine(result.Content.First(c => c.Type == "text").Text);
