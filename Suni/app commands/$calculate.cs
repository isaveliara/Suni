using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

using DSharpPlus.SlashCommands;

namespace Sun.SlashCommands
{
    public partial class Miscellaneous : ApplicationCommandModule
    {
        [SlashCommand("calculate","Calcule uma expressão")]
        public async Task SLASHCommandDice(InteractionContext ctx,
        [Option("Expression","Ex: 2x=12-0")] string expression)
        {
            var (image, result) = await Functions.Functions.calculateExpression(expression);

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{expression}")
                .WithDescription($"{result}");
            
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AddFile("result.png", image));
        }
    }
}