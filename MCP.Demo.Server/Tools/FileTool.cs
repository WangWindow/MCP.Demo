using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCP.Demo.Server.Tools;

[McpServerToolType]
public static class FileTool
{
    /// <summary>
    /// 规范化文件路径，支持相对路径和绝对路径
    /// </summary>
    private static string NormalizePath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        // 如果是相对路径，转换为绝对路径
        if (!Path.IsPathRooted(filePath))
        {
            filePath = Path.Combine(Environment.CurrentDirectory, filePath);
        }

        // 规范化路径
        return Path.GetFullPath(filePath);
    }

    /// <summary>
    /// 安全检查文件路径，防止路径遍历攻击
    /// </summary>
    private static bool IsPathSafe(string filePath)
    {
        try
        {
            var normalizedPath = NormalizePath(filePath);
            var currentDir = Environment.CurrentDirectory;

            // 允许在当前目录及其子目录下操作，或者允许绝对路径
            return normalizedPath.StartsWith(currentDir, StringComparison.OrdinalIgnoreCase) ||
                   Path.IsPathRooted(filePath);
        }
        catch
        {
            return false;
        }
    }

    [McpServerTool, Description("创建一个新文件并写入指定内容，支持相对路径和绝对路径")]
    public static string CreateFile(
        [Description("要创建的文件路径（支持相对路径如 './test.txt' 或绝对路径如 'C:\\temp\\test.txt'）")] string filePath,
        [Description("要写入文件的内容")] string content = "")
    {
        try
        {
            if (!IsPathSafe(filePath))
            {
                return $"❌ 不安全的文件路径: {filePath}";
            }

            var normalizedPath = NormalizePath(filePath);

            // 确保目录存在
            var directory = Path.GetDirectoryName(normalizedPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Console.Error.WriteLine($"📁 创建目录: {directory}");
            }

            // 创建文件并写入内容
            File.WriteAllText(normalizedPath, content);

            var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, normalizedPath);
            return $"✅ 文件创建成功\n📍 绝对路径: {normalizedPath}\n📍 相对路径: {relativePath}\n📄 内容长度: {content.Length} 字符";
        }
        catch (UnauthorizedAccessException)
        {
            return $"❌ 访问被拒绝，请检查文件权限: {filePath}";
        }
        catch (DirectoryNotFoundException)
        {
            return $"❌ 目录不存在且无法创建: {filePath}";
        }
        catch (Exception ex)
        {
            return $"❌ 文件创建失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("删除指定的文件，支持相对路径和绝对路径")]
    public static string DeleteFile([Description("要删除的文件路径（支持相对路径如 './test.txt' 或绝对路径）")] string filePath)
    {
        try
        {
            if (!IsPathSafe(filePath))
            {
                return $"❌ 不安全的文件路径: {filePath}";
            }

            var normalizedPath = NormalizePath(filePath);

            if (!File.Exists(normalizedPath))
            {
                var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, normalizedPath);
                return $"⚠️ 文件不存在\n📍 绝对路径: {normalizedPath}\n📍 相对路径: {relativePath}";
            }

            File.Delete(normalizedPath);
            var relativePathResult = Path.GetRelativePath(Environment.CurrentDirectory, normalizedPath);
            return $"✅ 文件删除成功\n📍 绝对路径: {normalizedPath}\n📍 相对路径: {relativePathResult}";
        }
        catch (UnauthorizedAccessException)
        {
            return $"❌ 访问被拒绝，请检查文件权限: {filePath}";
        }
        catch (Exception ex)
        {
            return $"❌ 文件删除失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("检查文件是否存在，支持相对路径和绝对路径")]
    public static string FileExists([Description("要检查的文件路径（支持相对路径如 './test.txt' 或绝对路径）")] string filePath)
    {
        try
        {
            if (!IsPathSafe(filePath))
            {
                return $"❌ 不安全的文件路径: {filePath}";
            }

            var normalizedPath = NormalizePath(filePath);
            bool exists = File.Exists(normalizedPath);
            var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, normalizedPath);

            return exists
                ? $"✅ 文件存在\n📍 绝对路径: {normalizedPath}\n📍 相对路径: {relativePath}"
                : $"❌ 文件不存在\n📍 绝对路径: {normalizedPath}\n📍 相对路径: {relativePath}";
        }
        catch (Exception ex)
        {
            return $"❌ 路径检查失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("读取文件内容，支持相对路径和绝对路径")]
    public static string ReadFile([Description("要读取的文件路径（支持相对路径如 './test.txt' 或绝对路径）")] string filePath)
    {
        try
        {
            if (!IsPathSafe(filePath))
            {
                return $"❌ 不安全的文件路径: {filePath}";
            }

            var normalizedPath = NormalizePath(filePath);

            if (!File.Exists(normalizedPath))
            {
                var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, normalizedPath);
                return $"⚠️ 文件不存在\n📍 绝对路径: {normalizedPath}\n📍 相对路径: {relativePath}";
            }

            string content = File.ReadAllText(normalizedPath);
            var relativePathResult = Path.GetRelativePath(Environment.CurrentDirectory, normalizedPath);
            var fileInfo = new FileInfo(normalizedPath);

            return $"📄 文件读取成功\n" +
                   $"📍 绝对路径: {normalizedPath}\n" +
                   $"📍 相对路径: {relativePathResult}\n" +
                   $"📊 文件大小: {fileInfo.Length} 字节\n" +
                   $"🕒 最后修改: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n" +
                   $"📝 文件内容:\n{new string('-', 40)}\n{content}\n{new string('-', 40)}";
        }
        catch (UnauthorizedAccessException)
        {
            return $"❌ 访问被拒绝，请检查文件权限: {filePath}";
        }
        catch (Exception ex)
        {
            return $"❌ 文件读取失败: {ex.Message}";
        }
    }

    [McpServerTool, Description("获取文件或目录的详细信息")]
    public static string GetFileInfo([Description("要查看信息的文件或目录路径")] string path)
    {
        try
        {
            if (!IsPathSafe(path))
            {
                return $"❌ 不安全的路径: {path}";
            }

            var normalizedPath = NormalizePath(path);
            var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, normalizedPath);

            if (File.Exists(normalizedPath))
            {
                var fileInfo = new FileInfo(normalizedPath);
                return $"📄 文件信息\n" +
                       $"📍 绝对路径: {normalizedPath}\n" +
                       $"📍 相对路径: {relativePath}\n" +
                       $"📊 大小: {fileInfo.Length} 字节\n" +
                       $"🕒 创建时间: {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss}\n" +
                       $"🕒 最后修改: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n" +
                       $"🕒 最后访问: {fileInfo.LastAccessTime:yyyy-MM-dd HH:mm:ss}\n" +
                       $"🔒 只读: {(fileInfo.IsReadOnly ? "是" : "否")}\n" +
                       $"📁 扩展名: {fileInfo.Extension}";
            }
            else if (Directory.Exists(normalizedPath))
            {
                var dirInfo = new DirectoryInfo(normalizedPath);
                var files = dirInfo.GetFiles();
                var subdirs = dirInfo.GetDirectories();

                return $"📁 目录信息\n" +
                       $"📍 绝对路径: {normalizedPath}\n" +
                       $"📍 相对路径: {relativePath}\n" +
                       $"🕒 创建时间: {dirInfo.CreationTime:yyyy-MM-dd HH:mm:ss}\n" +
                       $"🕒 最后修改: {dirInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n" +
                       $"📄 文件数量: {files.Length}\n" +
                       $"📁 子目录数量: {subdirs.Length}\n" +
                       $"📊 总大小: {files.Sum(f => f.Length)} 字节";
            }
            else
            {
                return $"❌ 路径不存在\n📍 绝对路径: {normalizedPath}\n📍 相对路径: {relativePath}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ 获取信息失败: {ex.Message}";
        }
    }
}