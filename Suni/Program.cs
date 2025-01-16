using DSharpPlus.EventArgs;
using DotNetEnv;
using System.Threading;
using Microsoft.Extensions.Logging;
using DSharpPlus.Extensions;
using DSharpPlus.Net.Gateway;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace Sun.Bot;

public class DotenvItems : IAppConfig
{
    public string SuniToken { get; private set; }
    public string CanaryToken { get; private set; }
    public string BaseUrlApi { get; private set; }
    public string BaseUrl { get; private set; }
    public ulong SupportServerId { get; private set; }

    public DotenvItems()
    {
        Env.Load();

        SuniToken = Environment.GetEnvironmentVariable("SUNITOKEN");
        CanaryToken = Environment.GetEnvironmentVariable("CANARYTOKEN");
        BaseUrlApi = Environment.GetEnvironmentVariable("BASEURLAPI");
        BaseUrl = Environment.GetEnvironmentVariable("BASEURL");
        SupportServerId = ulong.Parse(Environment.GetEnvironmentVariable("SUPPORTSERVERID"));
    }
}

public sealed class SunClassBot
{
    public static DiscordClient SuniClient;
    public const string SuniV = "build_3";
    public static string BaseUrl = new DotenvItems().BaseUrl;
    public static ulong SupportServerId = new DotenvItems().SupportServerId;
    public static int TimerRepeats { get; private set; } = 0;

    static async Task Main()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IAppConfig, DotenvItems>();

        var SuniBuilder = DiscordClientBuilder.CreateDefault(
            new DotenvItems().CanaryToken,
            DiscordIntents.All.RemoveIntent(DiscordIntents.GuildPresences),
            serviceCollection
        );

        //SuniBuilder.SetLogLevel(LogLevel.Debug);
        SuniBuilder.SetLogLevel(LogLevel.Information);

        SuniBuilder.ConfigureServices(services =>{
            services.Replace<IGatewayController, GatewayController>();
        });

        SuniBuilder.ConfigureExtraFeatures(config =>{
            config.LogUnknownAuditlogs = false;
            config.LogUnknownEvents = false;
        });
        SuniBuilder.ConfigureEventHandlers(builder =>
            builder.HandleSessionCreated(Client_Ready)
                    .HandleMessageReactionAdded(Sun.Events.ReactionEvents.On_addedReaction)
                    .HandleMessageCreated(Sun.Events.MessageEvents.On_message)
        );
        
        var prefixes = new string[] { "&" };
        SuniBuilder.UseCommands((_, extension) =>{
            TextCommandProcessor textCommandProcessor = new(new()
            {
                PrefixResolver = new DefaultPrefixResolver(true, prefixes).ResolvePrefixAsync,
                EnableCommandNotFoundException = false
            });
            extension.AddProcessor(textCommandProcessor);

            //Register error handling
            extension.CommandErrored += Sun.Events.ErrorEvents.CommandErrored;

            //Register logging
            //extension.CommandExecuted +=

            //Register Interaction Commands
            Sun.Functions.Helpers.RegisterAllCommands(extension);
        }, new CommandsConfiguration
        {
            UseDefaultCommandErrorHandler = false
        });

        //build the client
        SuniClient = SuniBuilder.Build();

        Timer task = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        var db = new Sun.Functions.DB.DBMethods();
        db.Setup();

        //connect
        await SuniClient.ConnectAsync();

        var commands = await SuniClient.GetGlobalApplicationCommandsAsync();
        foreach (var command in commands)
            Console.WriteLine($"Command: {command.Name} | Type: {command.Type} | ID: {command.Id}");

        await Task.Delay(-1);
    }

    private static async Task Client_Ready(DiscordClient client, SessionCreatedEventArgs args)
    {
        await Task.CompletedTask;
    }

    private static void ExecuteTask(object state)
    {
        Console.WriteLine($"task! ({TimerRepeats++} minutes running) - Executed at " + DateTime.Now);
    }
}

public class GatewayController : IGatewayController
{
#pragma warning disable CS1998
    public async Task HeartbeatedAsync(IGatewayClient client) { }
    public async Task ResumeAttemptedAsync(IGatewayClient _) { }
    public async Task ZombiedAsync(IGatewayClient _) { }
    public async Task ReconnectRequestedAsync(IGatewayClient _) { }
    public async Task ReconnectFailedAsync(IGatewayClient _) { }
    public async Task SessionInvalidatedAsync(IGatewayClient _) { }
    
}
#pragma warning restore CS1998 
