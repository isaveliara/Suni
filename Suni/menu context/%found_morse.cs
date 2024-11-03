using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.SlashCommands;

namespace SunContextCommands
{
    public partial class Miscellaneous : ApplicationCommandModule
    {
        [ContextMenu(DiscordApplicationCommandType.MessageContextMenu, "found Morse Code")]
        public async Task MENUCONTEXTFoundMorseText(ContextMenuContext ctx)
        {
            var (translatedText, translations) = new SunFunctions.Functions().GetMorsePart(ctx.TargetMessage.Content);

            string translationsShow = "";
            int index = 0;
            foreach (string t in translations){
                index++;
                translationsShow += $"\n-# {index} => **'{t}'**";
            }
            var button = new DiscordButtonComponent(DiscordButtonStyle.Primary, "send_this","Enviar aqui!");

            var msg = new DiscordInteractionResponseBuilder()
                        .AsEphemeral(true)
                        .WithContent(translationsShow)
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Yellow)
                            .WithTitle("Tradução")
                            .WithDescription(translatedText)
                            .WithFooter($"Mensagem enviada por {ctx.TargetMessage.Author.Username} e traduzida por {ctx.User.Username}")
                        );
            
            if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(DiscordPermissions.ManageChannels))
                msg = msg.AddComponents(button);
            

            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, msg);
        }
    }
}