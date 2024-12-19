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
        //execution flow
        bool _canExecute = true;
        public byte IndentLevel { get; private set; }
        Stack<bool> executionStack = new();

        //front-end infs.
        private List<string> _debugs = [];
        private List<string> _outputs = [];

        //values
        public List<string> Lines { get; private set; }
        public string ActualLine { get; private set; }
        public Dictionary<string, List<string>> Includes { get; private set; }
        public List<Dictionary<string, NptType>> Variables { get; private set; }

        public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync(string script, CommandContext ctx)
        {
            //formalize
            var (lines, includes, variables, resultFormalization) = new ScriptFormalizer.JoinScript().JoinHere(script, ctx);

            Includes = includes; //set includes
            Variables = variables; //set variables
            Lines = lines; //set lines

            if (resultFormalization != Diagnostics.Success)
                return (_debugs, _outputs, resultFormalization);

            IndentLevel = 0;
            executionStack.Push(_canExecute);

            for (int i = 0; i < Lines.Count; i++)
            {
                //replace the variables with their values before executing the line
                ActualLine = ReplaceVariables(Lines[i]);
                if (string.IsNullOrWhiteSpace(ActualLine)) continue;

                //KEY WORDS DETECTION
                if (ActualLine.StartsWith('&'))
                {
                    var keyWordName = Help.keywordLookahead(ActualLine, 0);
                    _debugs.Add($"identified keyword: >>{keyWordName.Chars}<<");
                    if (keyWordName.Chars == "do{")
                    {
                        IndentLevel++;
                        executionStack.Push(_canExecute);
                        continue;
                    }
                    else if (keyWordName.Chars == "}")
                    {
                        if (IndentLevel == 0)
                            return (_debugs, _outputs, Diagnostics.OutOfRangeException); //unmatched &}
                        
                        IndentLevel--;
                        executionStack.Pop();
                        _canExecute = executionStack.Peek();
                        continue;
                    }
                    //update execution ctx.
                    _canExecute = executionStack.Peek();

                    //instructions keywords
                    if (keyWordName.Letters == "if")
                    {
                        var ifMatch = Regex.Match(ActualLine, @"if\s*\(([^)]+)\)\s*", RegexOptions.None);
                        //if statement
                        if (ifMatch.Success)
                        {
                            string condition = ifMatch.Groups[1].Value;
                            var (r, conditionResult) = NptStatements.IFStatement(condition);
                            if (r != Diagnostics.Success)
                                return (_debugs, _outputs, r);
                            
                            _debugs.Add($"the condition '{condition}' is {conditionResult}");
                            IndentLevel++;

                            //set executionStack to false if could no longer execute lines (_canExecute)
                            if (_canExecute) executionStack.Push(conditionResult);
                            else{
                                executionStack.Push(false);
                                continue;
                            }

                            _canExecute = conditionResult;
                            continue;
                        }
                        else
                            return (_debugs, _outputs, Diagnostics.SyntaxException); //invalid if statement
                    }

                    if (!_canExecute) continue; //check

                    else if (keyWordName.Letters == "goto"){
                        if (int.TryParse(keyWordName.Chars.Substring(4), out int targetLineIndex)) //this is wrong
                        {
                            if (targetLineIndex >= 1 && targetLineIndex-1 < Lines.Count)
                            {
                                Lines[i] = "";//RemoveAt(i); //remove the goto line to avoid infinite loop
                                i = targetLineIndex-2;
                                continue;
                            }
                            else
                                return(_debugs, _outputs, Diagnostics.OutOfRangeException); //line index out of bounds
                        }
                        else
                            return(_debugs, _outputs, Diagnostics.OutOfRangeException); //invalid line index
                    }

                    //normal keywords
                    switch (keyWordName.Letters)
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

                var executedLineResult = await ExecuteLineAsync(ctx); //responsable for executing the objects in :: format
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
                
                if (Variables.Any(v => v.ContainsKey(varName))){
                    var value = Variables.First(v => v.ContainsKey(varName))[varName];
                    return value?.ToString() ?? "nil";
                }
                else{
                    _debugs.Add($"Warning: Variable '{varName}' not found, returning nil.");
                    return "nil";
                }
            });
        }

        //executing line
        private async Task<Diagnostics> ExecuteLineAsync(CommandContext ctx)
        {
            Diagnostics result = Diagnostics.Success;

            //objects statement
            var objMatch = Regex.Match(ActualLine, @"(?:(\w+)::)?(\w+)\(([^)]*)\)\s*->\s*(.+)");
            if (objMatch.Success)
                result = await ExecuteObjectStatementAsync(objMatch, ctx);
            
            else{
                _outputs.Add($"Unrecognized line: {ActualLine}");
                result = Diagnostics.SyntaxException;
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
