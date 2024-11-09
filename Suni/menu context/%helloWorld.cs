using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.SlashCommands;

using Sun.Functions;

namespace Sun.ContextCommands
{
    public partial class Miscellaneous : ApplicationCommandModule
    {
        /*
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "testONc")]
        public async Task MENUCONTEXTTest2(ContextMenuContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder()
                        .AsEphemeral(true)
                        .WithContent("test"));
        }
        */
    }
}