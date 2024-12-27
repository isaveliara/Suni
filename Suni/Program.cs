using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;

using DSharpPlus.Interactivity.Extensions;

using DotNetEnv;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using DSharpPlus.Extensions;
using DSharpPlus.Net.Gateway;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;

//using Microsoft.Extensions.Logging;

namespace Sun.Bot
{
    public class DotenvItems
    {
        internal readonly string SuniToken;
        internal readonly string CanaryToken;
        public readonly string BaseUrlApi;
        internal readonly object BaseUrl;

        public DotenvItems()
        {
            //load
            Env.Load();
            //var
            SuniToken = Environment.GetEnvironmentVariable("SUNITOKEN");
            CanaryToken = Environment.GetEnvironmentVariable("CANARYTOKEN");
            BaseUrlApi = Environment.GetEnvironmentVariable("BASEURLAPI");
            BaseUrl = Environment.GetEnvironmentVariable("BASEURL");
        }
    }

    public sealed class SunClassBot
    {
        public static DiscordClient SuniClient;
        public static CommandsExtension Commands { get; private set; }
        public static int Fun { get; private set; }
        public const string SuniV = "suninstruction_1.2.0b";
        public static int TimerRepeats { get; private set; } = 0;

        static async Task Main()
        {
            var SuniBuilder = DiscordClientBuilder.CreateDefault(new DotenvItems().CanaryToken, DiscordIntents.All.RemoveIntent(DiscordIntents.GuildPresences));

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
                       .HandleMessageCreated(Sun.Events.MessageEvents.On_message)
            );

            //SuniBuilder.UseInteractivity(new InteractivityConfiguration{
            //    PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
            //    Timeout = TimeSpan.FromSeconds(30)
            //});
            
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
}
