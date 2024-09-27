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
        
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "found Binary Code")]
        public async Task MENUCONTEXTFoundBinaryText(ContextMenuContext ctx)
        {
            var (translatedText, translations) = new SunFunctions.Functions().Get8bitPart(ctx.TargetMessage.Content);

            string translationsShow = "";
            int index = 0;
            foreach (string t in translations){
                index++;
                translationsShow += $"\n-# {index} => **'{t}'**";
            }
            var button = new DiscordButtonComponent(ButtonStyle.Primary, "send_this","Enviar aqui!");

            var msg = new DiscordInteractionResponseBuilder()
                        .AsEphemeral(true)
                        .WithContent(translationsShow)
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Yellow)
                            .WithTitle("Tradução")
                            .WithDescription(translatedText)
                            .WithFooter($"Mensagem enviada por {ctx.TargetMessage.Author.Username} e traduzida por {ctx.User.Username}")
                        );
            
            if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageChannels))
                msg = msg.AddComponents(button);
            else
                System.Console.WriteLine("No");

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, msg);
        }
        
    }
}