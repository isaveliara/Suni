using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.UserCommands;
using DSharpPlus.Commands.Trees.Metadata;

namespace Sun.Commands.ContextMenus;

public partial class Found_Commands
{
    [Command("translate To Andrês")]
    [AllowedProcessors(typeof(UserCommandProcessor))]
    [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel)]
    public static async Task ToAndres_Context(MessageCommandContext ctx, DiscordMessage targetMessage)
    {
        var (fullTranslation, _) = await new AndresTranslationService().GetLastWordSuggestions(targetMessage.Content, true);
        var msg = new DiscordInteractionResponseBuilder()
                    .AsEphemeral(true)
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.DarkButNotBlack)
                        .WithTitle("Mensagem")
                        .WithDescription(targetMessage.Content)
                        .AddField("Para Andrês", fullTranslation)
                    );
        await ctx.RespondAsync(msg);
    }
}