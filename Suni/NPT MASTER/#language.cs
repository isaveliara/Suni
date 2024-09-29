using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace ScriptInterpreter
{
    //class for parser and execution
    public partial class ScriptParser
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
                
                //KEY WORDS DETECTION
                if (trimmedLine.StartsWith('@'))
                {
                    var keyWordName = trimmedLine.Substring(1);
                    switch (keyWordName.Replace(' ', '\0'))
                    {
                        //toggle _canExecute
                        case "disableexecuting":
                            _canExecute = false;
                            continue;
                        case "enableexecuting":
                            _canExecute = true;
                            continue;

                        case "kit":
                            if (!_canExecute) continue;
                            return (_debugs, _outputs, Diagnostics.EarlyTermination); //add info for why/where kited
                        case "raiseException":
                            if (!_canExecute) continue;
                            return (_debugs, _outputs, Diagnostics.RaisedException); //add info for why/where kited
                        
                        case "if": case "else": return (_debugs, _outputs, Diagnostics.UnfinishedFeatureException);

                        default: //no one
                            return (_debugs, _outputs, Diagnostics.InvalidKeywordDetectedException);
                    }
                }
                //test if _canExecute line for continue without executing
                if (!_canExecute) continue;

                //if its not any key-word, execute as object
                var executedLineResult = await ExecuteLineAsync(trimmedLine, ctx); //responsable for executing the objects in :: format
                
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