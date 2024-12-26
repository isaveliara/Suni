using System.Linq;

namespace Sun.Commands;

public class Dice
{
    [Command("dice")]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM, DiscordInteractionContextType.PrivateChannel)]
    public static async Task DiceCommand(CommandContext ctx, 
        [Parameter("dices")] int number = 1,
        [Parameter("sides")] int sides = 6)
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