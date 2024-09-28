using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ScriptInterpreter
{
    //enum for status
    public enum Diagnostics
    {
        Success,
        EarlyTermination,
        RaisedException,

        NotFoundObjectException,
        InvalidSyntaxException,
        InvalidArgsException,
        UnknowException,
        CannotSetConstantException,
        MissingONLYCASERequirement,
        UnrecognizedLineException,
        InvalidChannelNPTException,
        NotFoundClassException,
    }

    //class for parser and execution
    public class ScriptParser
    {
        private bool _canExecute = true;
        private Dictionary<string, string> _constants = new Dictionary<string, string>();
        private List<string> _debugs = new List<string>();
        private List<string> _outputs = new List<string>();

        public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync(string script, CommandContext ctx)
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
                    var parseDEFINITIONSResult = ParseDefinition(trimmedLine);

                    if (parseDEFINITIONSResult != Diagnostics.Success)
                        return (_debugs, _outputs, parseDEFINITIONSResult);
                    
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
                    return (_debugs, _outputs, Diagnostics.EarlyTermination); //add info for why/where kited
                
                else if (trimmedLine.StartsWith("@raiseExceptionEnds"))
                    return (_debugs, _outputs, Diagnostics.RaisedException); //add info for why/where kited
                
                //if its not any key-word, execute as object
                var executedLineResult = await ExecuteLineAsync(trimmedLine, ctx); //responsable for executing the objects in :: format
                
                //exceptions handler
                if (executedLineResult != Diagnostics.Success)
                {
                    return (_debugs, _outputs, executedLineResult);
                }
            }

            //end of parse
            return (_debugs, _outputs, Diagnostics.Success);
        }

        //Method responsable for parsing the --definitons-- --end--
        private Diagnostics ParseDefinition(string line)
        {
            var setKeywordMatch = Regex.Match(line, @"^@set<(\w+),\s*(.*)>$");
            var onlyCaseKeywordMatch = Regex.Match(line, @"^@onlycase<(.+)>$");

            if (setKeywordMatch.Success)
            {
                string variable = setKeywordMatch.Groups[1].Value;
                string value = setKeywordMatch.Groups[2].Value;
                _constants[variable] = value;
                return Diagnostics.Success;
            }
            else if (onlyCaseKeywordMatch.Success)
            {
                string expression = onlyCaseKeywordMatch.Groups[1].Value;
                if (!EvaluateExpression(expression))
                {
                    return Diagnostics.MissingONLYCASERequirement;
                }
                return Diagnostics.Success; 
            }
            return Diagnostics.UnrecognizedLineException;
        }

        //executing line
        private async Task<Diagnostics> ExecuteLineAsync(string line, CommandContext ctx)
        {
            //e.g: 'sys::Object("arg1", "arg2", 99) -> Pointer'
            var objMatch = Regex.Match(line, @"(\w+)::(\w+)\(([^)]*)\)\s*->\s*(\w+)");
            Diagnostics result = Diagnostics.Success;

            if (objMatch.Success)
            {
                string className = objMatch.Groups[1].Value;
                string methodName = objMatch.Groups[2].Value;
                string argumentsToSplit = objMatch.Groups[3].Value;
                string pointer = objMatch.Groups[4].Value;
                var args = argumentsToSplit.Split(',');

                _debugs.Add($"Executing {className}:: {methodName} with args: {argumentsToSplit}, pointer: {pointer}");
                try
                {
                    Console.WriteLine(methodName.ToLower());//debug
                    switch (className.ToLower())//////////////////////////////////////
                    {
                        case "npt":
                            result = await NptEntitie.Controler(methodName, args.ToList(), pointer, ctx);
                            break;
                        case "master":
                            result = MASTERControler(methodName, args.ToList(), pointer);
                            break;
                        default:
                            _debugs.Add($"Failed to execute '{className}': Unknow class");
                            _outputs.Add($"Failed to execute '{className}': Unknow class");
                            result = Diagnostics.NotFoundClassException;
                            break;
                    }
                }
                catch (Exception)
                {
                    _debugs.Add($"Failed to execute '{methodName}': invalid args");
                    _outputs.Add($"Failed to execute '{methodName}': invalid args");
                    result = Diagnostics.InvalidArgsException;
                }
            }
            else if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                _debugs.Add("Whitespace line.");
            }
            else
            {
                _debugs.Add($"Unrecognized line: {line}");
                _outputs.Add($"Unrecognized line: {line}");
                result = Diagnostics.UnrecognizedLineException;
            }
            return result;
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

        //objects that interact with the class itself
        public Diagnostics MASTERControler(string method, List<string> args, string pointer)
        {
            switch (method)
            {
                case "outputadd": //master::outputadd() -> hello world
                    _outputs.Add(pointer);
                    break;
                case "outputset": //master::outputset() -> hello world
                    _outputs = new List<string>{pointer};
                    break;
                case "outputclean"://master::outputadd() -> null
                    _outputs = new List<string>();
                    break;
                default:
                    return Diagnostics.NotFoundObjectException;
            }
            return Diagnostics.Success;
        }
    }

    //npt entities
    public static class NptEntitie
    {
        public static async Task<Diagnostics> Controler(string methodName, List<string> args, string pointer, CommandContext ctx)
        {
            Diagnostics result;
            switch (methodName)
            {
                case "log":
                    ulong argChannel = ulong.Parse(pointer);
                    string argMessage = args[0];
                    result = await NptEntitie.Log(ctx, argChannel, argMessage);

                    if (result != Diagnostics.Success) return result;
                    break;
                default:
                    return Diagnostics.NotFoundObjectException;
            }
            return Diagnostics.Success;
        }
        //log in channel (good for events-actions)
        public static async Task<Diagnostics> Log(CommandContext ctx, ulong channelId, string message){
            var channel = await ctx.Client.GetChannelAsync(channelId);
            if (channel.GuildId != ctx.Guild.Id)
                return Diagnostics.InvalidChannelNPTException;
            
            await ctx.Channel.SendMessageAsync(message); //sends
            return Diagnostics.Success;
        }

        //discord actions handler
        public static string Ban(string duration, string reason){
            return $"just pretend:: Ban applied for {duration} with reason: {reason}";
        }
        public static string Mute(string duration, string reason){
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