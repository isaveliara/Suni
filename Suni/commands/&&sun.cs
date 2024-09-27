using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SunPrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Group("sun")] [Aliases("suni")]
        public class SunPrefixCommandsGroup : BaseCommandModule
        {
            [Command("statistics")] [Aliases("nerd")]
            public async Task PREFIXCommandStatistcs(CommandContext ctx)
            {
                var stat = SunFunctions.Functions.GetSuniStatistics();
                await ctx.RespondAsync(stat);
            }

            [Command("inf")] [Aliases("info")]
            public async Task PREFIXCommandInfo(CommandContext ctx)
            {
                await ctx.RespondAsync($"Você pode ver meus detalhes em meu [website]({new SunBot.DotenvItems().BaseUrl})!");
            }
        }
    }
}