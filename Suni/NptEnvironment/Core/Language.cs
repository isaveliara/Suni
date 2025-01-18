using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Formalizer;

namespace Suni.Suni.NptEnvironment.Core;

public partial class NptSystem
{
    private Stack<CodeBlock> blockStack = new();

    private readonly List<string> _debugs = [];
    private List<string> _outputs = [];

    public List<string> Lines { get; private set; }
    public string ActualLine { get; private set; }
    public Dictionary<string, List<string>> Includes { get; private set; }
    public List<Dictionary<string, NptTypes.NptType>> Variables { get; private set; }
    public Dictionary<string, List<string>> NFuncs { get; private set; } = [];

    public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync(
    string script, CommandContext ctx)
    {
        var (lines, includes, variables, resultFormalization) = new FormalizingScript().Formalize(script, ctx);
        if (resultFormalization != Diagnostics.Success)
            return (_debugs, _outputs, resultFormalization);

        Includes = includes;
        Variables = variables;
        Lines = lines;

        blockStack.Push(new CodeBlock { IndentLevel = 0, CanExecute = true });

        for (int i = 0; i < Lines.Count; i++)
        {
            ActualLine = ReplaceVariables(Lines[i]);
            if (string.IsNullOrWhiteSpace(ActualLine)) continue;

            if (ActualLine.StartsWith("&")){
                var keyWordName = Help.keywordLookahead(ActualLine, 0);
                _debugs.Add($"Identified keyword: >>{keyWordName.Chars}<<");

                if (keyWordName.Letters == "if"){
                    var ifMatch = Regex.Match(ActualLine.Substring(2), @"\(([^)]+)\)\s*&do{", RegexOptions.None);
                    if (ifMatch.Success){
                        string condition = ifMatch.Groups[1].Value;
                        var (r, conditionResult) = NptStatements.IFStatement(condition);
                        if (r != Diagnostics.Success)
                            return (_debugs, _outputs, r);

                        _debugs.Add($"A condição '{condition}' é {conditionResult}");
                        //creating a new block based on the "if" condition
                        blockStack.Push(new CodeBlock
                        {
                            IndentLevel = blockStack.Peek().IndentLevel + 1,
                            CanExecute = conditionResult //control whether the block can be executed based on the result of the condition
                        });

                        continue;
                    }
                    else
                        return (_debugs, _outputs, Diagnostics.SyntaxException);
                }
                
                else if (keyWordName.Letters == "call"){
                    var funcMatch = Regex.Match(ActualLine, @"call\s+(\w+)\(([^)]*)\)");
                    if (funcMatch.Success){
                        string functionName = funcMatch.Groups[1].Value;
                        var args = funcMatch.Groups[2].Value.Split(',').Select(arg => arg.Trim()).ToList();

                        //localize the function
                        var functionVar = Variables.FirstOrDefault(dict => dict.ContainsKey(functionName));
                        if (functionVar != null && functionVar[functionName].Type == NptTypes.Types.Fn){
                            var fn = (NptFunction)functionVar[functionName].Value;
                            _debugs.Add($"Chamando função '{fn.Name}' com argumentos: {string.Join(", ", args)}");

                            //validates the number of arguments
                            if (args.Count != fn.Parameters.Count)
                                return (_debugs, _outputs, Diagnostics.ArgumentMismatch);

                            string functionCode = fn.Code;
                            for (int j = 0; j < args.Count; j++){
                                string placeholder = $"${{{fn.Parameters[j]}}}";
                                functionCode = functionCode.Replace(placeholder, args[j]);
                            }

                            var (debugs, outputs, result) = await ParseScriptAsync(functionCode, ctx);
                            _debugs.AddRange(debugs);
                            _outputs.AddRange(outputs);

                            if (result != Diagnostics.Success)
                                return (_debugs, _outputs, result);

                            continue;
                        }
                        else{
                            _outputs.Add($"Função '{functionName}' não encontrada ou não é do tipo Fn.");
                            return (_debugs, _outputs, Diagnostics.FunctionNotFound);
                        }
                    }
                    else
                        return (_debugs, _outputs, Diagnostics.SyntaxException);
                }

                else if (keyWordName.Letters == "goto"){
                    if (int.TryParse(keyWordName.Chars.Substring(4), out int targetLineIndex)){
                        if (targetLineIndex >= 1 && targetLineIndex - 1 < Lines.Count){
                            Lines[i] = "";
                            i = targetLineIndex - 2;
                            continue;
                        }
                        else
                            return (_debugs, _outputs, Diagnostics.OutOfRangeException);
                    }
                    else
                        return (_debugs, _outputs, Diagnostics.SyntaxException);
                }

                if (keyWordName.Chars == "do{"){
                    blockStack.Push(new CodeBlock{
                        IndentLevel = blockStack.Peek().IndentLevel + 1,
                        CanExecute = blockStack.Peek().CanExecute
                    });
                    continue;
                }

                else if (keyWordName.Chars == "}"){
                    if (blockStack.Count == 1)
                        return (_debugs, _outputs, Diagnostics.OutOfRangeException);

                    var closingBlock = blockStack.Pop();
                    _debugs.Add($"Fechando bloco do tipo '{closingBlock.Type}' no nível de indentação {closingBlock.IndentLevel}");
                    continue;
                }
                else if (keyWordName.Letters == "kit"){
                    _debugs.Add("Script interrompido pela palavra-chave 'kit'.");
                    return (_debugs, _outputs, Diagnostics.EarlyTermination);
                }
                else if (keyWordName.Letters == "forget"){
                    return (null, null, Diagnostics.Forgotten);
                }

                else if (keyWordName.Letters == "raizeEx"){
                    _debugs.Add("Exceção levantada pela palavra-chave 'raizeEx'.");
                    return (_debugs, _outputs, Diagnostics.RaisedException);
                }
                else
        return (_debugs, _outputs, Diagnostics.InvalidKeywordException);
            }

            if (!blockStack.Peek().CanExecute)
                continue;

            var executedLineResult = await ExecuteLineAsync(ctx);
            if (executedLineResult != Diagnostics.Success)
                return (_debugs, _outputs, executedLineResult);
        }

        return (_debugs, _outputs, Diagnostics.Success);
    }

    private string ReplaceVariables(string line)
    {
        return Regex.Replace(line, @"\${(\w+)}", match => {
            string varName = match.Groups[1].Value;
            if (Variables.Any(v => v.ContainsKey(varName))){
                NptTypes.NptType value = Variables.First(v => v.ContainsKey(varName))[varName];
                if (value.Type == NptTypes.Types.Fn)
                    return value.ToString();
                return value?.ToString() ?? "nil";
            }
            else{
                _debugs.Add($"Warning: Variable '{varName}' not found. Returning nil.");
                return "nil";
            }
        });
    }

    private async Task<Diagnostics> ExecuteLineAsync(CommandContext ctx)
    {
        var objMatch = Regex.Match(ActualLine, @"(?:(\w+)::)?(\w+)\(([^)]*)\)\s*->\s*(.+)");
        if (objMatch.Success)
            return await ExecuteObjectStatementAsync(objMatch, ctx);

        //unrecognized line
        _outputs.Add($"Error: unrecognized line: {ActualLine}");
        return Diagnostics.SyntaxException;
    }

    private async Task<Diagnostics> InvokeClassControler(string className, string methodName, List<string> args, string pointer, CommandContext ctx)
    {
        try{
            string classNameProper = char.ToUpper(className[0]) + className.Substring(1).ToLower() + "Entitie";
            Type type = Type.GetType($"Sun.NPT.ScriptInterpreter.{classNameProper}");

            if (type != null){
                var controlerMethod = type.GetMethod("Controler", BindingFlags.Static | BindingFlags.Public);
                if (controlerMethod != null){
                    var task = (Task<Diagnostics>)controlerMethod.Invoke(null, new object[] { methodName, args, pointer, ctx });
                    return await task;
                }

                _debugs.Add($"Internal Error: Method 'Controler' not found in '{classNameProper}'.");
                return Diagnostics.IncludeNotFoundException;
            }

            _debugs.Add($"Error: '{classNameProper}' not found.");
            return Diagnostics.IncludeNotFoundException;
        }
        catch (Exception ex){
            _outputs.Add($"Unknown Error while executing '{className}::{methodName}': {ex.Message}");
            return Diagnostics.UnknowException;
        }
    }

    private async Task<Diagnostics> InvokeFunction(string functionName, List<string> args, CommandContext ctx)
    {
        try{
            if (NFuncs.TryGetValue(functionName, out var functionLines)){
                _debugs.Add($"Starting execution of function '{functionName}'.");
                foreach (var line in functionLines){
                    ActualLine = ReplaceVariables(line);
                    var executedLineResult = await ExecuteLineAsync(ctx);
                    if (executedLineResult != Diagnostics.Success)
                        return executedLineResult;
                }

                _debugs.Add($"Execution of '{functionName}' finished.");
                return Diagnostics.Success;
            }

            _outputs.Add($"Error: Type Function '{functionName}' not defined.");
            return Diagnostics.FunctionNotFound;
        }
        catch (Exception ex){
            _outputs.Add($"Error while executing function '{functionName}': {ex.Message}");
            return Diagnostics.UnknowException;
        }
    }
}
public class CodeBlock{
    public int IndentLevel { get; set; }
    public List<string> Lines { get; } = [];
    public bool CanExecute { get; set; } = true;
    public string Condition { get; set; } = null;
    public BlockType Type { get; set; } = BlockType.Anonymous;
}