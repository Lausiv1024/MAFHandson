using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

IChatClient chatClient = ProviderSetup.CreateChatClient();
string workspace = Path.GetFullPath("../../../../Workspace/SampleApp", AppContext.BaseDirectory);
var workspaceTools = new WorkspaceTools(workspace);
string prompt = ConsoleInput.ReadPrompt(args);

// Step 1では、ここにChatClientAgentを作成して実行するコードを追加します。
Console.WriteLine($"Prompt: {prompt}");
AIAgent agent = new ChatClientAgent(
    chatClient,
    instructions: """
        You are a coding agent.
        Help the user with programming tasks.
        """);

Console.WriteLine(await agent.RunAsync(prompt));