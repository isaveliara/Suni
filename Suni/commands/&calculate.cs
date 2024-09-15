using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SunPrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("calculate")]
        public async Task PREFIXCommandShip(CommandContext ctx,
        [Option("Expression","Ex: 2x=12-0")] string expression)
        {
            string result = SunFunctions.Functions.calculateExpression(expression);
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{expression}")
                .WithDescription($"{result}");
            
            await ctx.RespondAsync(new DiscordMessageBuilder()
                .AddEmbed(embed));
        }
    }
}