using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sun.NPT.ScriptInterpreter
{
    //class for parser and execution
    public partial class ScriptParser
    {
        bool _canExecute = true;
        private Dictionary<string, string> _constants = new Dictionary<string, string>();
        private List<string> _debugs = new List<string>();
        private List<string> _outputs = new List<string>();
        private Dictionary<string, List<string>> _includes = new Dictionary<string, List<string>> {
            { "std", new List<string>{"outputadd", "outputset", "outputclean"} },
            { "npt", new List<string>{"log", "react", "ban", "unban"} },
        };

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
            var objMatch = Regex.Match(line, @"(?:(\w+)::)?(\w+)\(([^)]*)\)\s*->\s*(\w+)");
            Diagnostics result = Diagnostics.Success;

            if (objMatch.Success)
            {
                string className = objMatch.Groups[1].Success ? objMatch.Groups[1].Value : null;
                string methodName = objMatch.Groups[2].Value;
                string argumentsToSplit = objMatch.Groups[3].Value;
                string pointer = objMatch.Groups[4].Value;
                var args = argumentsToSplit.Split(',');

                //if class is not specified, look in _includes
                if (string.IsNullOrEmpty(className))
                {
                    className = _includes.FirstOrDefault(kv => kv.Value.Contains(methodName)).Key;

                    if (className == null)
                    {
                        _outputs.Add($"Method '{methodName}' not associated with any class in includes.");
                        return Diagnostics.NotFoundClassException;
                    }
                }

                _debugs.Add($"Executing {className}::{methodName} with args: {argumentsToSplit}, pointer: {pointer}");

                //STD is a SPECIAL case
                if (className == "std")
                    return STDControler(methodName, args.ToList(), pointer);
                
                //normal cases:
                try
                {
                    //invoke the Controller of the appropriate class
                    result = await InvokeClassControler(className, methodName, args.ToList(), pointer, ctx);
                }
                catch (Exception ex)
                {
                    _outputs.Add($"CRIT: NPT Internal Script Error while executing '{methodName}': {ex.Message}");
                    result = Diagnostics.UnknowException;
                }
            }
            else if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                _debugs.Add("Whitespace line.");
            }
            else
            {
                _outputs.Add($"Unrecognized line: {line}");
                result = Diagnostics.UnrecognizedLineException;
            }
            return result;
        }

        private async Task<Diagnostics> InvokeClassControler(string className, string methodName, List<string> args, string pointer, CommandContext ctx)
        {
            try
            {
                //formalize the class name to the real class name
                string classNameProper = char.ToUpper(className[0]) + className.Substring(1).ToLower() + "Entitie";

                //dynamically resolve class in namespace
                Type type = Type.GetType($"Sun.NPT.ScriptInterpreter.{classNameProper}");

                if (type != null)
                {
                    //Find the static method `Controler` in
                    var controlerMethod = type.GetMethod("Controler", BindingFlags.Static | BindingFlags.Public);

                    if (controlerMethod != null)
                    {
                        //Invoke the `Controler` method dynamically
                        var task = (Task<Diagnostics>)controlerMethod.Invoke(null, new object[] { methodName, args, pointer, ctx });
                        return await task;
                    }
                    else
                    {
                        _debugs.Add($"Controler method not found in '{classNameProper}'.");
                        return Diagnostics.NotFoundClassException;
                    }
                }
                else
                {
                    _debugs.Add($"Class '{classNameProper}' not found.");
                    return Diagnostics.NotFoundClassException;
                }
            }
            catch (Exception ex)
            {
                _outputs.Add($"Error executing '{className}::{methodName}': {ex.Message}");
                return Diagnostics.UnknowException;
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