using System.Text.RegularExpressions;

namespace Sun.Commands;

public class AboutMe
{
    [Command("sun")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task About(CommandContext ctx)
        =>
            await ctx.RespondAsync($"Você pode ver meus detalhes em meu [website]({SunClassBot.BaseUrl})!");

    [Command("nerd")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task Nerd(CommandContext ctx)
        =>
            await ctx.RespondAsync(Functions.Functions.GetSuniStatistics());
    
    [Command("ping")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task Ping(CommandContext ctx)
    {
        await ctx.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Ping!"));
        ulong s = new DotenvItems().SupportServerId;

        var websocketPing = ctx.Client.GetConnectionLatency(
            ctx.Channel.IsPrivate? ctx.Guild?.Id ?? s
                                 : s
        ).TotalMilliseconds;

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent(
                $"Pong! \nWebsocket Ping: '{websocketPing}ms'"));
    }
}