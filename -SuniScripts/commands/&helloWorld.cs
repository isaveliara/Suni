using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using SunFunctions;

namespace SunPrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("test")]
        public async Task PREFIXCommandTest(CommandContext ctx)
        {
            await ctx.RespondAsync(Functions.HelloWorld());
        }
    }
}