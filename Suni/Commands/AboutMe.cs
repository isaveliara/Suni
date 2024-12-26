namespace Sun.Commands;

public class AboutMe
{
    [Command("sun")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task About(CommandContext ctx)
    {
        await ctx.RespondAsync($"Você pode ver meus detalhes em meu [website]({new Sun.Bot.DotenvItems().BaseUrl})!");
    }

    [Command("nerd")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task Nerd(CommandContext ctx)
    {
        var stat = Sun.Functions.Functions.GetSuniStatistics();
        await ctx.RespondAsync(stat);
    }
}