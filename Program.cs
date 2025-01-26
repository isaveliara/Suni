using Suni.Suni.Configuration;
using Suni.Suni.Configuration.Interfaces;
namespace Suni;

public sealed class SunClassBot
{
    public static DiscordClient SuniClient;
    public const string SuniV = "build_3.3";
    private static readonly IAppConfig Config = AppConfig.NewAppConfig();
    public static int TimerRepeats { get; private set; } = 0;

    static async Task Main(string[] args)
    {
        if (args.Length > 0 && args[0].ToLower().Contains("test")) //runs the npt tester
        {
            await Tests.RunNptTester();
        }

        SuniClient = SuniBuilder.Configure(Config);
        DatabaseConfiguration.Configure();
        await SuniClient.ConnectAsync();

        var commands = await SuniClient.GetGlobalApplicationCommandsAsync();
        foreach (var command in commands)
            Console.WriteLine($"Command: {command.Name} | Type: {command.Type} | ID: {command.Id}");

        await Task.Delay(-1);
    }
    private static void ExecuteTask(object state)
    {
        Console.WriteLine($"task! ({TimerRepeats++} minutes running) - Executed at " + DateTime.Now);
    }
}

