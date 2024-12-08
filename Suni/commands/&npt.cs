using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Sun.Functions;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using Sun.NPT.ScriptInterpreter;

namespace Sun.PrefixCommands
{
    public partial class Miscellaneous
    {
        public enum NptActions
        {
            RunAct,
            TestAct,
            InfoAct,
            ParseAct
        }

        [Command("npt")]
        public async Task PREFIXCommandNpt(CommandContext ctx, [Option("act","npt action")] string act)
        {
            if (ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.Administrator) == false)
                return;
            
            string c = ctx.Message.Content;

            string code = "";
            Match match = Regex.Match(c, @"```(.+?)```", RegexOptions.Singleline);
            if (match.Success)
            {
                code = match.Groups[1].Value;
                Console.WriteLine(code);
            }
            
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
            if (action == NptActions.RunAct)
            {
                /*//formalize
                var (parsedcode, resultf) = new ScriptFormalizer.JoinScript().JoinHere(code, ctx);
                if (resultf != Diagnostics.Success)
                {
                    await ctx.RespondAsync($"Cannot formalize the provided script! :x:\nDetalhes: {resultf}");
                    return;
                }*/

                //building response
                string response = $"```OUTPUT of SuniNPT code `{Bot.SunClassBot.SuniV}` is here:";
                NptSystem parser = new NptSystem();
                var result = await parser.ParseScriptAsync(code, ctx);
                //first output
                foreach (var output in result.outputs)
                    response += $"\n    {output}";
                response += "\n\nDEBUG:\n";

                //debug
                foreach (var debug in result.debugs)
                    response += $"\n    {debug}";
                
                if (result.result == Diagnostics.Success)
                    response += $"```\n\nResult Program: **{result.result}**\n[Finished] :white_check_mark:";
                else
                    response += $"```\n\nOcorreu um erro ao executar o código:\n**{result.result}**\n[Finished] :x:";

                await ctx.RespondAsync(response);
            }
        }
    }
}