using System.Text.Json;

namespace Frontend;

public static class DebugSessionLogger
{
    private const string SessionId = "d36f80";
    private const string LogFileName = "debug-d36f80.log";

    public static void Log(string location, string message, object? data, string hypothesisId, string runId)
    {
        try
        {
            var payload = new
            {
                sessionId = SessionId,
                runId,
                hypothesisId,
                location,
                message,
                data,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var root = FindRepoRoot(AppContext.BaseDirectory) ?? Directory.GetCurrentDirectory();
            var path = Path.Combine(root, LogFileName);
            File.AppendAllText(path, JsonSerializer.Serialize(payload) + Environment.NewLine);
        }
        catch
        {
            // Never let debug logging break runtime.
        }
    }

    private static string? FindRepoRoot(string startDir)
    {
        try
        {
            var dir = new DirectoryInfo(startDir);
            while (dir is not null)
            {
                if (dir.EnumerateFileSystemInfos(".git").Any())
                    return dir.FullName;
                if (dir.EnumerateFiles("*.slnx").Any())
                    return dir.FullName;
                dir = dir.Parent;
            }
        }
        catch
        {
            // ignore
        }
        return null;
    }
}

