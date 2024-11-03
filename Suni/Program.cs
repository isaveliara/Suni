using DSharpPlus;
//using DSharpPlus.CommandsNext;
//using DSharpPlus.SlashCommands;
using DSharpPlus.Commands;
using DSharpPlus.EventArgs;

using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using DotNetEnv;
using HandlerFunctions;
using System;
using System.Threading.Tasks;
using System.Threading;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Entities;

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
        public static CommandsConfiguration Commands { get; private set; }
        public static int Fun { get; private set; }

        static async Task Main()
        {
            DiscordClientBuilder SuniBuilder = DiscordClientBuilder.CreateSharded(
                token:new DotenvItems().SuniToken,
                intents:DiscordIntents.All,
                shardCount:2
            );
            
            //commands
            SuniBuilder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands([typeof(SunPrefixCommands.Miscellaneous), typeof(SunPrefixCommands.ImageCommands)]);
                TextCommandProcessor textCommandProcessor = new(new(){
                    PrefixResolver = new DefaultPrefixResolver(true, "&").ResolvePrefixAsync
                });
            });

            //interactivity --idk
            /*
            SuniBuilder.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(30) //30s
            });
            */

            //events
            SuniBuilder.ConfigureEventHandlers
            (
                b => b.HandleGuildMemberUpdated(HandlerFunctions.Listeners.Handler.FatherMemberUpdated)
                      //.Handle(tananan)
            );

            //client
            SuniClient = SuniBuilder.Build();

            //status
            DiscordActivity status = new("new version", DiscordActivityType.Playing);
            await SuniClient.ConnectAsync(status, DiscordUserStatus.Online);

            //scripts
            Timer task = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            //infinitely
            await Task.Delay(-1);



            //trash
            /*
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
            
            var SlashCommandsConfig = Client.UseSlashCommands();

            //////MISCELLANEOUS commands
            Commands.RegisterCommands<SunPrefixCommands.Miscellaneous>(); //prefix--
            SlashCommandsConfig.RegisterCommands<SunSlashCommands.Miscellaneous>(); //slash
            SlashCommandsConfig.RegisterCommands<SunContextCommands.Miscellaneous>(); //menu context

            //////IMAGECOMMANDS commands
            Commands.RegisterCommands<SunPrefixCommands.ImageCommands>(); //prefix--
            SlashCommandsConfig.RegisterCommands<SunSlashCommands.ImageCommands>(); //slash

            //////Minigame commands
            Commands.RegisterCommands<SunPrefixCommands.GameCommands>(); //prefix--
            

            //<EVENTS>//
            //role events handler
            //Client.GuildRoleCreated += HandlerFunctions.Listeners.NEW.Role;
            Client.GuildMemberUpdated += HandlerFunctions.Listeners.Handler.FatherMemberUpdated;--
            //Client.GuildRoleUpdated += HandlerFunctions.Listeners.UPDATE.Role;--
            //Client.GuildRoleCreated += HandlerFunctions.Listeners.DEL.Role;
            
            //interaction handler
            Client.ComponentInteractionCreated += HandlerFunctions.Components.InteractionEventHandler;
            Client.ModalSubmitted += HandlerFunctions.Components.ModalsHandler;

            //errored slash/prefix handler (debugging for canary!)
            Commands.CommandErrored += ErroredFunctions.CommandsErrored_Handler;
            SlashCommandsConfig.SlashCommandErrored += ErroredSlashFunctions.SlashCommandsErrored_Handler;
            SlashCommandsConfig.ContextMenuErrored += ErroredSlashFunctions.MenuContextCommandsErrored_Handler;

            //<resume>//

            Timer task = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            await Client.ConnectAsync();
            await Task.Delay(-1);
            */
        }

        private static void ExecuteTask(object state)
        {
            Console.WriteLine("task!");
        }
    }
}

