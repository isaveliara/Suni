using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SunFunctions;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ScriptInterpreter;

namespace SunPrefixCommands
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
            Match match = Regex.Match(ctx.Message.Content, @"```(.+?)```", RegexOptions.Singleline);
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
                string response = "```OUTPUT of SuniNPT code is here:";
                ScriptParser parser = new ScriptParser();
                var result = await parser.ParseScriptAsync(code, ctx);

                foreach (var debug in result.debugs)
                    response += $"\n    {debug}";
                response += "\n\nDEBUG:\n";

                foreach (var output in result.outputs)
                    response += $"\n    {output}";

                response += $"```\n\nResult Program: {result.result} [Finished]";
                await ctx.RespondAsync(response);
            }
        }
    }
}
namespace ScriptInterpreter
{
    //enum for status
    public enum ExecutionResult
    {
        Success,
        Error,
        EarlyTermination,
        RaisedException
    }

    //class for parser and execution
    public class ScriptParser
    {
        private bool _canExecute = true;
        private Dictionary<string, string> _constants = new Dictionary<string, string>();
        private List<string> _debugs = new List<string>();
        private List<string> _outputs = new List<string>();

        public async Task<(List<string> debugs, List<string> outputs, ExecutionResult result)> ParseScriptAsync(string script, CommandContext ctx)
        {
            var lines = script.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
            bool inDefinitionsBlock = false;

            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine == "--definitions--"){
                    inDefinitionsBlock = true;
                    continue;
                }else if (trimmedLine == "--ends--")
                {
                    inDefinitionsBlock = false;
                    continue;
                }
                //process --definitions-- block
                else if (inDefinitionsBlock){
                    ParseDefinition(trimmedLine);
                    continue;
                }   ///process script after --definitions--
                else if (trimmedLine.StartsWith("--"))
                    continue; //comentary

                //toggle _canExecute
                else if (trimmedLine.StartsWith("@disableexecuting"))
                {
                    _canExecute = false;
                }
                else if (trimmedLine.StartsWith("@enableexecuting"))
                {
                    _canExecute = true;
                }
                //test if _canExecute line for continue without executing
                if (!_canExecute)
                    continue;

                ///ACTIONS:

                //key-words
                if (trimmedLine.StartsWith("@kit"))
                    return (_debugs, _outputs, ExecutionResult.EarlyTermination); //add info for why/where kited
                
                else if (trimmedLine.StartsWith("@raiseExceptionEnds"))
                    return (_debugs, _outputs, ExecutionResult.RaisedException); //add info for why/where kited
                //else if (trimmedLine.StartsWith("@sun"))
                //    return (_debugs, ExecutionResult.SunTchola);//remove
                
                //if its not any key-word, execute as object
                await ExecuteLineAsync(trimmedLine, ctx); //responsable for executing the objects in :: format
            }

            //end of parse
            return (_debugs, _outputs, ExecutionResult.Success);
        }

        //Method responsable for parsing the --definitons-- --end--
        private void ParseDefinition(string line)
        {
            var setKeywordMatch = Regex.Match(line, @"^@set<(\w+),\s*(.*)>$");
            var onlyCaseKeywordMatch = Regex.Match(line, @"^@onlycase<(.+)>$");

            if (setKeywordMatch.Success)
            {
                string variable = setKeywordMatch.Groups[1].Value;
                string value = setKeywordMatch.Groups[2].Value;
                _constants[variable] = value;
            }
            else if (onlyCaseKeywordMatch.Success)
            {
                string expression = onlyCaseKeywordMatch.Groups[1].Value;
                if (!EvaluateExpression(expression))
                {
                    throw new Exception("Onlycase condition failed, script execution aborted."); //ajeita isso pelamor
                }
            }
        }

        //executing line
        private async Task ExecuteLineAsync(string line, CommandContext ctx)
        {
            //e.g: 'sys::Object("arg1", "arg2", 99) -> Pointer'
            var objMatch = Regex.Match(line, @"(\w+)::(\w+)\(([^)]*)\)\s*->\s*(\w+)");
            if (objMatch.Success)
            {
                string className = objMatch.Groups[1].Value;
                string methodName = objMatch.Groups[2].Value;
                string arguments = objMatch.Groups[3].Value;
                string pointer = objMatch.Groups[4].Value;

                _debugs.Add($"Executing {className}:: {methodName} with args: {arguments}, pointer: {pointer}");
                try
                {
                    Console.WriteLine(methodName.ToLower());
                    switch (methodName.ToLower())//////////////////////////////////////
                    {
                        case "log":
                            ulong argChannel = ulong.Parse(pointer);
                            string argMessage = arguments;
                            await NptEntitie.Log(ctx, argChannel, argMessage);
                            break;
                        default:
                            _debugs.Add($"FAILED TO EXECUTE {methodName}: UNKNOW OBJECT");
                            _outputs.Add($"FAILED TO EXECUTE {methodName}: UNKNOW OBJECT");
                            return;
                    }
                }
                catch (Exception)
                {
                    _debugs.Add($"FAILED TO EXECUTE {methodName}: invalid args");
                    _outputs.Add($"FAILED TO EXECUTE {methodName}: UNKNOW OBJECT");
                }
            }
            else if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                _debugs.Add("Whitespace line.");
            }
            else
            {
                _debugs.Add($"Unrecognized line: {line}");
            }
        }

        //evaluate expressions, like 1 == 1, in if statment
        private bool EvaluateExpression(string expression)
        {
            ///TODO: implement
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        //search for a @get<varname> value
        private string GetConstant(string varname)
        {
            return _constants.ContainsKey(varname) ? _constants[varname] : null;
        }
    }

    //sys:: objects
    public static class SysEntitie
    {
        public static bool Exp(string expression)
        {
            ///implement this
            return false;
        }
    }

    //npt entities
    public static class NptEntitie
    {
        public static async Task<string> Log(CommandContext ctx, ulong channelId, string message)
        {
            var channel = await ctx.Client.GetChannelAsync(channelId);
            if (channel.GuildId != ctx.Guild.Id)
            {
                return "FAILED: this channel is not from this server!";
            }
            
            await ctx.Channel.SendMessageAsync(message); //sends

            return $"Log in channel {ctx.Channel.Name} with message: {message}";
        }

        public static string Ban(string duration, string reason)
        {
            return $"just pretend:: Ban applied for {duration} with reason: {reason}";
        }

        public static string Mute(string duration, string reason)
        {
            return $"just pretend:: Mute applied for {duration} with reason: {reason}";
        }
    }

    //testing the program
    class Program2test
    {
        static async void Main2Test(string[] args)
        {
            string script = @"
                --definitions--
                @set<ban_duration, '27days'>
                --end--

                npt::BanAsync(@get<ban_duration>, 'Fez alguma coisa') -> 12345678910
                sys::Object('arg1', 'arg2', 99) -> Pointer
            ";

            ScriptParser parser = new ScriptParser();
            var result = await parser.ParseScriptAsync(script, null);

            Console.WriteLine("DEBUG:");
            foreach (var debug in result.debugs)
            {
                Console.WriteLine($"    {debug}");
            }
            Console.WriteLine("\n--OUTPUT--\n");
            foreach (var output in result.outputs)
            {
                Console.WriteLine($"    {output}");
            }

            Console.WriteLine();
            Console.WriteLine($"Result Program: {result.result}\n[Finished]");
        }
    }
}