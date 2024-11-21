using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Sun.PrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("exp")]
        public async Task PREFIXCommandCalc(CommandContext ctx,
        [Option("Expression","Ex: 2x=12-0")] string expression)
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
}