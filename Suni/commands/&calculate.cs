using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SunPrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("exp")]
        public async Task PREFIXCommandDice(CommandContext ctx,
        [Option("Expression","Ex: 2x=12-0")] string expression)
        {
            var (image, result) = await SunFunctions.Functions.calculateExpression(expression);

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{expression}")
                .WithDescription($"{result}");
            
            await ctx.RespondAsync(new DiscordMessageBuilder()
                .AddEmbed(embed)
                .AddFile("result.png", image));
        }
    }
}