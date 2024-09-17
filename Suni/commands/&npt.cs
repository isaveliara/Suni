using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SunFunctions;
using System;
using System.Text.RegularExpressions;

namespace SunPrefixCommands
{
    public partial class Miscellaneous : BaseCommandModule
    {
        [Command("npt")]
        public async Task PREFIXCommandNpt(CommandContext ctx, [Option("act","npt act")] string act)
        {
            string c = ctx.Message.Content;

            string code = "";
            Match match = Regex.Match(ctx.Message.Content, @"```(.+?)```");
            if (match.Success)
                code = match.Groups[1].Value;
            
            NptActions action;
            switch (act.ToLower())
            {
                case "run":
                    action = NptActions.RunAct;
                    break;
                case "test":
                    action = NptActions.TestAct;
                    break;
                case "info":
                    action = NptActions.InfoAct;
                    break;
                case "parse":
                    action = NptActions.ParseAct;
                    break;
                default:
                    await ctx.RespondAsync("Invalid action provided. Use 'run', 'test' or 'info'");
                    return;
            }
            
            var response = SunFunctions.Functions.NPTEXECUTE(action, ctx, code);
            await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithContent(response));
            
            Console.WriteLine(act);
        }
    }

    public enum NptActions
    {
        RunAct,
        TestAct,
        InfoAct,
        ParseAct
    }
}

namespace SunFunctions
{
    public partial class Functions
    {
        internal static string NPTEXECUTE(SunPrefixCommands.NptActions act, CommandContext ctx, string code)
        {
            return "[Finished]";
        }
    }
}