using DSharpPlus.Commands.ArgumentModifiers;

namespace Sun.Commands;

public class Calculate
{
    [Command("calc")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task CalculateCommand(CommandContext ctx,
        [RemainingText] string expression)
    {
        var (image, result) = await Sun.Functions.Functions.CalculateExpression(expression);

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"{expression}")
            .WithDescription($"{result}");
        
        await ctx.RespondAsync(new DiscordMessageBuilder()
            .AddEmbed(embed)
            .AddFile("result.png", image));
    }
}