using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Sun.Functions;
using System;

namespace Sun.PrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("dice")]
        public async Task PREFIXCommandDice(CommandContext ctx,
        [Option("sides","sides")] byte sides = 6,
        [Option("number","number for roll")] int number = 1)
        {
            if (number > 14){
                await ctx.RespondAsync(new DiscordMessageBuilder()
                        .WithContent("Não podes lançar um número de dados maior que 14! :x:"));  return;
            }
            if (sides == 1){
                await ctx.RespondAsync(new DiscordMessageBuilder()
                        .WithContent($":game_die: | 1"));  return;
            }
            if (sides < 1 || number < 1){
                await ctx.RespondAsync(new DiscordMessageBuilder()
                        .WithContent($"erro ao calcular! :x:"));  return;
            }

            var dice = Sun.Functions.Functions.Dice((int)sides, number).ToList();
            var stringdice = string.Join(" , ", dice);
            int result = dice.Sum();

            await ctx.RespondAsync(new DiscordMessageBuilder()
                    .WithContent($":game_die: | result: {stringdice} (total {result})"));
        }
    }
}