namespace SharpDbg.Cli;

public static class Arguments
{
    public static (string? interpreter, int serverPort, string? logPath) Parse(string[] args)
    {
        string? interpreter = null;
        var serverPort = -1;
        string? logPath = null;

        // Parse command line arguments
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--interpreter="))
            {
                interpreter = args[i].Substring("--interpreter=".Length);
            }
            else if (args[i].StartsWith("--server="))
            {
                if (int.TryParse(args[i].Substring("--server=".Length), out var port))
                {
                    serverPort = port;
                }
            }
            else if (args[i].StartsWith("--engineLogging="))
            {
                logPath = args[i].Substring("--engineLogging=".Length);
            }
        }

        return (interpreter, serverPort, logPath);
    }
}
