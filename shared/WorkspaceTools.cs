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
