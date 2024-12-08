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
    public partial class NptSystem
    {
        bool _canExecute = true;
        public byte _indentLevel { get; private set; }
        private List<string> _debugs = new List<string>();
        private List<string> _outputs = new List<string>();
        public Dictionary<string, List<string>> _includes { get; private set; }
        public List<Dictionary<string, NptType>> _variables { get; private set; }

        public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync(string script, CommandContext ctx)
        {
            //formalize
            var (lines, includes, variables, resultFormalization) = new ScriptFormalizer.JoinScript().JoinHere(script, ctx);
            _includes = includes; //set includes
            _variables = variables; //set variables

            if (resultFormalization != Diagnostics.Success)
                    return (_debugs, _outputs, resultFormalization);

            //byte indentLevel = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                //KEY WORDS DETECTION
                if (line.StartsWith('@'))
                {
                    var keyWordName = Help.keywordLookahead(line, 0);

                    //toggle _canExecute
                    switch (keyWordName)
                    {
                        case "disableExe":
                            _canExecute = false;
                            continue;
                        case "enableExe":
                            _canExecute = true;
                            continue;
                    }
                    if (!_canExecute) continue;

                    //instructions keywords
                    if (keyWordName.StartsWith("goto")){
                        if (int.TryParse(keyWordName.Substring(4), out int targetLineIndex))
                        {
                            if (targetLineIndex >= 1 && targetLineIndex-1 < lines.Count)
                            {
                                lines[i] = "";//RemoveAt(i); //remove the goto line to avoid infinite loop
                                i = targetLineIndex-2;
                                continue;
                            }
                            else
                                return(_debugs, _outputs, Diagnostics.OutOfRangeException); //line index out of bounds
                        }
                        else
                            return(_debugs, _outputs, Diagnostics.OutOfRangeException); //invalid line index
                    }

                    //other keywords, that can be influenced by _canExecute
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

                //if its not any key-word, execute as object or something else
                //so replace the variables with their values before executing the line
                line = ReplaceVariables(line);

                var executedLineResult = await ExecuteLineAsync(line, ctx); //responsable for executing the objects in :: format
                //exception handler of object execution result
                if (executedLineResult != Diagnostics.Success)
                    return (_debugs, _outputs, executedLineResult); //implement inf.
            }

            //natural end of parse
            return (_debugs, _outputs, Diagnostics.Success);
        }

        private string ReplaceVariables(string line)
        {
            return Regex.Replace(line, @"\${(\w+)}", match =>
            {
                var varName = match.Groups[1].Value;
                
                if (_variables.Any(v => v.ContainsKey(varName))){
                    var value = _variables.First(v => v.ContainsKey(varName))[varName];
                    return value?.ToString() ?? "nil";
                }
                else{
                    _debugs.Add($"Warning: Variable '{varName}' not found, returning nil.");
                    return "nil";
                }
            });
        }

        //executing line
        private async Task<Diagnostics> ExecuteLineAsync(string line, CommandContext ctx)
        {
            Diagnostics result = Diagnostics.Success;

            //objects statement
            var objMatch = Regex.Match(line, @"(?:(\w+)::)?(\w+)\(([^)]*)\)\s*->\s*(.+)");

            var ifMatch = Regex.Match(line, @"if\s*\(([^)]+)\)\s*", RegexOptions.IgnoreCase);
            if (objMatch.Success)
                result = await ExecuteObjectStatementAsync(objMatch, ctx);

            //if statement
            else if (ifMatch.Success)
            {
                string condition = ifMatch.Groups[1].Value;
                bool conditionResult = NptStatements.EvaluateIFExpression(condition).Item2;
                _debugs.Add($"the condition '{condition}' is {conditionResult}");
            }

            else if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                _debugs.Add("Whitespace line.");
            
            else{
                _outputs.Add($"Unrecognized line: {line}");
                result = Diagnostics.UnrecognizedLineException;
            }
            return result;
        }

        private async Task<Diagnostics> InvokeClassControler(string className, string methodName, List<string> args, string pointer, CommandContext ctx)
        {
            try{
                //formalize the class name to the real class name
                string classNameProper = char.ToUpper(className[0]) + className.Substring(1).ToLower() + "Entitie";

                //dynamically resolve class in namespace
                Type type = Type.GetType($"Sun.NPT.ScriptInterpreter.{classNameProper}");

                if (type != null){
                    //find the static method `Controler` in
                    var controlerMethod = type.GetMethod("Controler", BindingFlags.Static | BindingFlags.Public);

                    if (controlerMethod != null){
                        //invoke the `Controler` method dynamically
                        var task = (Task<Diagnostics>)controlerMethod.Invoke(null, [methodName, args, pointer, ctx]);
                        return await task;
                    }
                    else
                    {
                        _debugs.Add($"Controler method not found in '{classNameProper}'.");
                        return Diagnostics.NotFoundClassException;
                    }
                }
                else{
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
    }
}
