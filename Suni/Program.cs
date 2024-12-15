using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;

using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using DotNetEnv;
using Sun.HandlerFunctions;
using System;
using System.Threading.Tasks;
using Sun.PrefixCommands;
using DSharpPlus.SlashCommands;
using System.Threading;

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
        public static DiscordClient SuniClient { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static int Fun { get; private set; }
        public static string SuniV { get; } = "suninstruction_1.0.2c";
        public static int TimerRepeats { get; private set; } = 0;
        //public static int Err { get; private set; }
        //public static int ErrCommands { get; private set; }

        static async Task Main()
        {
            //configure
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = new DotenvItems().CanaryToken,
                ShardId = 0,
                ShardCount = 2,
                AutoReconnect = true,
                //MinimumLogLevel = LogLevel.Debug
            };
            //applying
            SuniClient = new DiscordClient(discordConfig);
            //set default timeout for interactions
            SuniClient.UseInteractivity(new InteractivityConfiguration(){Timeout=TimeSpan.FromSeconds(100)});
            
            SuniClient.Ready += Client_Ready;
            
            //other config
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = ["&"],
                EnableMentionPrefix = true,
                EnableDms = true,
                IgnoreExtraArguments = true,
                EnableDefaultHelp = true
            };
            Commands = SuniClient.UseCommandsNext(commandsConfig); //using configs
            var SlashCommandsConfig = SuniClient.UseSlashCommands();
            await SlashCommandsConfig.RefreshCommands();

            //////MISCELLANEOUS commands
            SlashCommandsConfig.RegisterCommands<Sun.Dimensions.Utilities.Sla>(); //slash
            Commands.RegisterCommands<Sun.PrefixCommands.Miscellaneous>(); //prefix
            SlashCommandsConfig.RegisterCommands<Sun.SlashCommands.Miscellaneous>();
            SlashCommandsConfig.RegisterCommands<Sun.ContextCommands.MiscellaneousC>(); //menu context

            //////IMAGECOMMANDS commands
            Commands.RegisterCommands<Sun.Dimensions.Romance.Pre>(); //prefix
            SlashCommandsConfig.RegisterCommands<Sun.Dimensions.Romance.Sla>(); //slash

            //////Minigame commands
            SlashCommandsConfig.RegisterCommands<Sun.Dimensions.Fun.FunSla>(); //slash
            Commands.RegisterCommands<Sun.Dimensions.Fun.FunPre>(); //prefix
            
            //<EVENTS>//
            //role events handler
            SuniClient.GuildMemberUpdated += HandlerFunctions.Listeners.Handler.FatherMemberUpdated;
            
            //interaction handler
            SuniClient.ComponentInteractionCreated += HandlerFunctions.Components.InteractionEventHandler;
            SuniClient.ModalSubmitted += HandlerFunctions.Components.ModalsHandler;

            //errored slash/prefix handler (debugging for canary!)
            Commands.CommandErrored += ErroredFunctions.CommandsErrored_Handler;
            SlashCommandsConfig.SlashCommandErrored += ErroredSlashFunctions.SlashCommandsErrored_Handler;
            SlashCommandsConfig.ContextMenuErrored += ErroredSlashFunctions.MenuContextCommandsErrored_Handler;
            //<resume>//

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

        private static void ExecuteTask(object state)
        {
            Console.WriteLine($"task! ({TimerRepeats++}) - Executed at " + DateTime.Now);
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}

