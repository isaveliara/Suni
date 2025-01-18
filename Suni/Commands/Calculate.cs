using DSharpPlus.Commands.ArgumentModifiers;
using Suni.Suni.Configuration.Interfaces;

namespace Suni.Suni.Commands;

public class Calculate(IAppConfig config)
{
    [Command("calc")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public async Task CalculateCommand(CommandContext ctx,
        [RemainingText] string expression)
    {
        var (image, result) = await Functions.Functions.CalculateExpression(expression, config);

        var embed = new DiscordEmbedBuilder()
            .WithTitle($"{expression}")
            .WithDescription($"{result}");

        await ctx.RespondAsync(new DiscordMessageBuilder()
            .AddEmbed(embed)
            .AddFile("result.png", image));
    }
}