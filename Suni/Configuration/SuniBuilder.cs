using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.EventArgs;
using DSharpPlus.Extensions;
using DSharpPlus.Net.Gateway;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Suni.Suni.Configuration.Interfaces;
using Suni.Suni.Controllers;
using Suni.Suni.Functions;
using Suni.Suni.Events;
using Suni.Suni;
namespace Suni.Suni.Configuration;

public class SuniBuilder
{
    public static DiscordClient Configure(IAppConfig config)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IAppConfig>(config);

        var SuniBuilder = DiscordClientBuilder.CreateDefault(
            config.CanaryToken,
            DiscordIntents.All.RemoveIntent(DiscordIntents.GuildPresences),
            serviceCollection
        );

        SuniBuilder.SetLogLevel(LogLevel.Information);

        SuniBuilder.ConfigureServices(services =>
        {
            services.Replace<IGatewayController, GatewayController>();
        });

        SuniBuilder.ConfigureExtraFeatures(config =>
        {
            config.LogUnknownAuditlogs = false;
            config.LogUnknownEvents = false;
        });
        SuniBuilder.ConfigureEventHandlers(builder =>
            builder.HandleSessionCreated(Client_Ready)
                    .HandleMessageReactionAdded(ReactionEvents.OnAddedReaction)
                    .HandleMessageCreated(MessageEvents.OnMessage)
        );
        var prefixes = new string[] { "&" };
        SuniBuilder.UseCommands((_, extension) =>
        {
            TextCommandProcessor textCommandProcessor = new(new()
            {
                PrefixResolver = new DefaultPrefixResolver(true, prefixes).ResolvePrefixAsync,
                EnableCommandNotFoundException = false
            });
            extension.AddProcessor(textCommandProcessor);

            extension.CommandErrored += ErrorEvents.CommandErrored;

            CommandsConfigs.RegisterCommands(extension);
        }, new CommandsConfiguration
        {
            UseDefaultCommandErrorHandler = false
        });

        return SuniBuilder.Build();
    }
    private static async Task Client_Ready(DiscordClient client, SessionCreatedEventArgs args)
        =>
            await Task.CompletedTask;
}