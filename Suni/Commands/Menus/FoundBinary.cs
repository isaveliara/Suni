using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.UserCommands;
using DSharpPlus.Commands.Trees.Metadata;

namespace Sun.Commands.ContextMenus;

public partial class Found_Commands
{
    [Command("found Binary Code")]
    [AllowedProcessors(typeof(UserCommandProcessor))]
    [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.PrivateChannel)]
    public static async Task FoundBinaryCode_Context(MessageCommandContext ctx, DiscordMessage targetMessage)
    {
        var (translatedText, translations) = new Sun.Functions.Functions().Get8bitPart(targetMessage.Content);
        string translationsShow = "";
        int index = 0;
        foreach (string t in translations){
            index++;
            translationsShow += $"\n-# {index} => **'{t}'**";
        }

        var msg = new DiscordInteractionResponseBuilder()
                    .AsEphemeral(true)
                    .WithContent(translationsShow)
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Yellow)
                        .WithTitle("Tradução")
                        .WithDescription(translatedText)
                        .WithFooter($"Mensagem enviada por {targetMessage.Author.Username} e traduzida por {ctx.User.Username}")
                    );
        await ctx.RespondAsync(msg);
    }
}