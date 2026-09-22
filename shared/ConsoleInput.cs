public static class ConsoleInput
{
    public static string ReadPrompt(string[] args)
    {
        if (args.Length > 0)
        {
            return string.Join(' ', args);
        }

        Console.Write("User> ");
        return Console.ReadLine() ?? "";
    }
}
