﻿using DSharpPlus;
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
        internal readonly string SuniToken;
        public readonly string BaseUrlApi;
        internal readonly object BaseUrl;

        public DotenvItems()
        {
            //load
            Env.Load();
            //var
            SuniToken = Environment.GetEnvironmentVariable("SUNITOKEN");
            BaseUrlApi = Environment.GetEnvironmentVariable("BASEURLAPI");
            BaseUrl = Environment.GetEnvironmentVariable("BASEURL");
        }
    }

    public sealed class Sun
    {
        public static DiscordClient SuniClient { get; private set; }
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

            //////MISCELLANEOUS commands
            Commands.RegisterCommands<SunPrefixCommands.Miscellaneous>(); //prefix
            SlashCommandsConfig.RegisterCommands<SunSlashCommands.Miscellaneous>();
            SlashCommandsConfig.RegisterCommands<SunContextCommands.Miscellaneous>(); //menu context

            //////IMAGECOMMANDS commands
            Commands.RegisterCommands<SunPrefixCommands.ImageCommands>(); //prefix
            SlashCommandsConfig.RegisterCommands<SunSlashCommands.ImageCommands>(); //slash

            //////Minigame commands
            Commands.RegisterCommands<SunPrefixCommands.GameCommands>(); //prefix
            
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
            var db = new SunFunctions.DB.Methods();
            db.Setup();

            //connect
            await SuniClient.ConnectAsync();
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

