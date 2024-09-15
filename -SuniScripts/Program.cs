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

//using Microsoft.Extensions.Logging;

namespace SunBot
{
    internal class DotenvItems
    {
        internal string sunitoken;

        public DotenvItems()
        {
            //load
            Env.Load();
            //var
            sunitoken = Environment.GetEnvironmentVariable("SUNITOKEN");
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
                Token = new DotenvItems().sunitoken,
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
            
            var SlashCommandsConfig = Client.UseSlashCommands();

            //////MISCELLANEOUS commands
            Commands.RegisterCommands<SunPrefixCommands.Miscellaneous>(); //prefix
            //SlashCommandsConfig.RegisterCommands<SunSlashCommands.Miscellaneous>(); //slash (deleted for while)
            SlashCommandsConfig.RegisterCommands<SunContextCommands.Miscellaneous>(); //menu context

            //////IMAGECOMMANDS commands
            Commands.RegisterCommands<SunPrefixCommands.ImageCommands>(); //prefix
            SlashCommandsConfig.RegisterCommands<SunCommands.Slash.ImageCommands>(); //slash
            
            //errored slash/prefix errored (debugging for canary!)
            Commands.CommandErrored += ErroredFunctions.CommandsErrored_Handler;
            SlashCommandsConfig.SlashCommandErrored += ErroredSlashFunctions.SlashCommandsErrored_Handler;
            SlashCommandsConfig.ContextMenuErrored += ErroredSlashFunctions.MenuContextCommandsErrored_Handler;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}

