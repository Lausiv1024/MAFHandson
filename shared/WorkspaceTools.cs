using System.ComponentModel;
using System.Diagnostics;
using System.Text;

public sealed class WorkspaceTools
{
    private readonly string workspaceRoot;

    public WorkspaceTools(string workspaceRoot)
    {
        this.workspaceRoot = Path.GetFullPath(workspaceRoot);

        if (!Directory.Exists(this.workspaceRoot))
        {
            throw new DirectoryNotFoundException($"Workspace not found: {this.workspaceRoot}");
        }
    }

    [Description("Lists files and directories directly inside a workspace directory. The path is relative to the workspace.")]
    public string ListFiles([Description("A directory path relative to the workspace, such as . or src")] string path)
    {
        Console.WriteLine($"[Tool] ListFiles(\"{path}\")");
        string directory = ResolvePath(path);

        if (!Directory.Exists(directory))
        {
            return $"Directory not found: {path}";
        }

        return string.Join(Environment.NewLine,
            Directory.EnumerateFileSystemEntries(directory)
                .Select(entry => Path.GetFileName(entry) + (Directory.Exists(entry) ? "/" : ""))
                .Order(StringComparer.OrdinalIgnoreCase));
    }

    [Description("Reads an entire text file from the workspace.")]
    public string ReadFile([Description("A file path relative to the workspace")] string path)
    {
        Console.WriteLine($"[Tool] ReadFile(\"{path}\")");
        return File.ReadAllText(ResolvePath(path));
    }

    [Description("Replaces an entire text file in the workspace with the supplied content.")]
    public string WriteFile(
        [Description("A file path relative to the workspace")] string path,
        [Description("The complete new contents of the file")] string content)
    {
        Console.WriteLine($"[Tool] WriteFile(\"{path}\", ...)");
        string file = ResolvePath(path);
        string? directory = Path.GetDirectoryName(file);

        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(file, content);
        return $"Wrote {path}";
    }

    [Description("Runs a shell command with the workspace as its working directory and returns its exit code, stdout, and stderr.")]
    public async Task<string> RunCommand([Description("The command to run, such as dotnet build")] string command)
    {
        Console.WriteLine($"[Tool] RunCommand(\"{command}\")");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = OperatingSystem.IsWindows() ? "cmd.exe" : "/bin/sh",
                Arguments = OperatingSystem.IsWindows() ? $"/c {command}" : $"-c \"{command.Replace("\"", "\\\"")}\"",
                WorkingDirectory = workspaceRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        Task<string> stdout = process.StandardOutput.ReadToEndAsync();
        Task<string> stderr = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return $"ExitCode: {process.ExitCode}{Environment.NewLine}" +
               $"STDOUT:{Environment.NewLine}{await stdout}{Environment.NewLine}" +
               $"STDERR:{Environment.NewLine}{await stderr}";
    }

    private string ResolvePath(string path)
    {
        string fullPath = Path.GetFullPath(path, workspaceRoot);
        string relativePath = Path.GetRelativePath(workspaceRoot, fullPath);

        if (relativePath == ".." || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Access outside the workspace is not allowed.");
        }

        return fullPath;
    }
}
