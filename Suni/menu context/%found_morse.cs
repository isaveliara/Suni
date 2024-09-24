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
        
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "found Morse Code")]
        public async Task MENUCONTEXTFoundMorseText(ContextMenuContext ctx)
        {
            var e = new SunFunctions.Functions();
            var (translatedText, translations) = e.GetMorsePart(ctx.TargetMessage.Content);

            string translationsShow = "";
            int index = 0;
            foreach (string t in translations)
            {
                index++;
                translationsShow += $"\n-# {index} => **'{t}'**\n";
            }
            

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                        .AsEphemeral(true)
                        .WithContent(translationsShow)
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Yellow)
                            .WithTitle("Tradução")
                            .WithDescription(translatedText)));
        }
    }
}