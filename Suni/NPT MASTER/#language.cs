using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Sun.NPT.ScriptInterpreter
{
    //class for parser and execution
    public partial class ScriptParser
    {
        bool _canExecute = true;
        private Dictionary<string, string> _constants = new Dictionary<string, string>();
        private List<string> _debugs = new List<string>();
        private List<string> _outputs = new List<string>();

        public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync(string script, CommandContext ctx)
        {
            //formalize
            var (lines, re) = new ScriptFormalizer.JoinScript().JoinHere(script, ctx);
            if (re != Diagnostics.Success)
                    return (_debugs, _outputs, re);

            //byte indentLevel = 0;
            bool inDefinitionsBlock = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line == "#definitions"){
                    inDefinitionsBlock = true;
                    continue;
                }else if (line == "#ends")
                {
                    inDefinitionsBlock = false;
                    continue;
                }
                //process --definitions-- block
                else if (inDefinitionsBlock){
                    var parseDEFINITIONSResult = ParseDefinition(line);

                    if (parseDEFINITIONSResult != Diagnostics.Success)
                        return (_debugs, _outputs, parseDEFINITIONSResult);
                    
                    continue;
                }   ///process script after #definitions #ends
                
                //KEY WORDS DETECTION
                if (line.StartsWith('@'))
                {
                    var keyWordName = line.Substring(1);

                    //toggle _canExecute
                    if (keyWordName=="disableExe"){
                        _canExecute = false;
                        continue;
                    }
                    else if (keyWordName=="enableExe"){
                        _canExecute = true;
                        continue;
                    }
                    if (!_canExecute) continue;

                    //now, we skip if _canExecute is false.

                    //instructions keywords (>>)
                    if (keyWordName.StartsWith("goto>>")){
                        if (int.TryParse(keyWordName.Substring(6), out int targetLineIndex))
                        {
                            if (targetLineIndex >= 0 && targetLineIndex < lines.Count)
                            {
                                lines.RemoveAt(i); //remove the goto line to avoid infinite loop
                                i = targetLineIndex - 1;
                                continue;
                            }
                            else
                                return(_debugs, _outputs, Diagnostics.OutOfRangeException); //line index out of bounds
                        }
                        else
                            return(_debugs, _outputs, Diagnostics.OutOfRangeException); //invalid line index
                    }

                    //other
                    switch (keyWordName)
                    {
                        case "kit":
                            return (_debugs, _outputs, Diagnostics.EarlyTermination); //add info for why/where kited
                        case "raizeEx":
                            return (_debugs, _outputs, Diagnostics.RaisedException); //add info for why/where kited

                        default: //no one
                            return (_debugs, _outputs, Diagnostics.InvalidKeywordException);
                    }
                }
                
                //test if _canExecute line for continue without executing
                if (!_canExecute) continue;

                //if its not any key-word, execute as object
                var executedLineResult = await ExecuteLineAsync(line, ctx); //responsable for executing the objects in :: format
                
                //exception handler of object execution result
                if (executedLineResult != Diagnostics.Success)
                    return (_debugs, _outputs, executedLineResult); //implement inf.
            }

            //natural end of parse
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
                    switch (className.ToLower().Replace(" ", ""))//////////////////////////////////////
                    {
                        case "npt": //async.
                            result = await NptEntitie.Controler(methodName, args.ToList(), pointer, ctx);
                            break;
                        case "std": //n. async.
                            result = STDControler(methodName, args.ToList(), pointer);
                            break;
                        case "suni": //async.
                            result = await SuniEntitie.Controler(methodName, args.ToList(), pointer, ctx);
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
                    _outputs.Add($"Failed to execute '{methodName}': invalid args");
                    result = Diagnostics.InvalidArgsException;
                }
            }
            else if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                _debugs.Add("Whitespace line.");
            }
            else //if its none, UnrecognizedLineException
            {
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
    }
}

//test

//string script = @"
//    --definitions--
//    @set<ban_duration, '27days'>
//    --end--
//
//    npt::BanAsync(@get<ban_duration>, 'Fez alguma coisa') -> 12345678910
//    sys::Object('arg1', 'arg2', 99) -> Pointer
//    ";