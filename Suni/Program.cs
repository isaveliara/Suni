using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;

using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using DotNetEnv;
using HandlerFunctions;
using System;
using System.Threading.Tasks;

using DSharpPlus.SlashCommands;
using System.Threading;

//using Microsoft.Extensions.Logging;

namespace SunBot
{
    public class DotenvItems
    {
        internal string SuniToken;
        public string BaseUrlApi;

        public DotenvItems()
        {
            //load
            Env.Load();
            //var
            SuniToken = Environment.GetEnvironmentVariable("SUNITOKEN");
            BaseUrlApi = Environment.GetEnvironmentVariable("BASEURLAPI");
        }
    }

    public sealed class Sun
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static int Fun { get; private set; }

        static async Task Main()
        {
            
            //configure
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = new DotenvItems().SuniToken,
                ShardId = 0,
                ShardCount = 2,
                AutoReconnect = true,
                //MinimumLogLevel = LogLevel.Debug
            };
            //applying
            Client = new DiscordClient(discordConfig);
            //set default timeout for interactions
            Client.UseInteractivity(new InteractivityConfiguration(){Timeout=TimeSpan.FromSeconds(100)});
            
            //
            Client.Ready += Client_Ready;
            
            //other config
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = ["&"],
                EnableMentionPrefix = true,
                EnableDms = true,
                IgnoreExtraArguments = true,
                EnableDefaultHelp = true
            };
            //
            Commands = Client.UseCommandsNext(commandsConfig); //using configs

            //interaction
            Client.ComponentInteractionCreated += HandlerFunctions.Components.InteractionEventHandler;
            Client.ModalSubmitted += HandlerFunctions.Components.ModalsHandler;
            
            var SlashCommandsConfig = Client.UseSlashCommands();

            //////MISCELLANEOUS commands
            Commands.RegisterCommands<SunPrefixCommands.Miscellaneous>(); //prefix
            SlashCommandsConfig.RegisterCommands<SunSlashCommands.Miscellaneous>();
            SlashCommandsConfig.RegisterCommands<SunContextCommands.Miscellaneous>(); //menu context

            //////IMAGECOMMANDS commands
            Commands.RegisterCommands<SunPrefixCommands.ImageCommands>(); //prefix
            SlashCommandsConfig.RegisterCommands<SunSlashCommands.ImageCommands>(); //slash

            //////Minigame commands
            Commands.RegisterCommands<SunPrefixCommands.GameCommands>(); //prefix
            
            //errored slash/prefix errored (debugging for canary!)
            Commands.CommandErrored += ErroredFunctions.CommandsErrored_Handler;
            SlashCommandsConfig.SlashCommandErrored += ErroredSlashFunctions.SlashCommandsErrored_Handler;
            SlashCommandsConfig.ContextMenuErrored += ErroredSlashFunctions.MenuContextCommandsErrored_Handler;

            Timer task = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void ExecuteTask(object state)
        {
            Console.WriteLine("task!");
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}

