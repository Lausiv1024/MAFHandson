using Microsoft.Extensions.AI;
using OpenAI;

public static class ProviderSetup
{
    public static IChatClient CreateChatClient()
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
        string model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o";
        return new OpenAIClient(apiKey).GetChatClient(model).AsIChatClient();
    }
}
