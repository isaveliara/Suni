using System.Reflection;
using System.Text.RegularExpressions;
using Suni.Suni.NptEnvironment.Core.Evaluator;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Core;

public partial class NptSystem
{
    public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync()
    {
        if (ContextData.ErrorMessages.Count > 0)
            return (ContextData.ErrorMessages, ContextData.ErrorMessages, Diagnostics.SyntaxException);
        
        blockStack.Push(new CodeBlock { IndentLevel = 0, CanExecute = true });

        for (int i = 0; i < ContextData.Lines.Count; i++)
        {
            ContextData.ActualLine = ReplaceVariables(ContextData.Lines[i]);
            if (string.IsNullOrWhiteSpace(ContextData.ActualLine)) continue;

            if (ContextData.ActualLine.StartsWith("&")){
                var keyWordName = Help.keywordLookahead(ContextData.ActualLine, 0);
                ContextData.Debugs.Add($"Identified keyword: >>{keyWordName.Chars}<<");

                if (keyWordName.Letters == "if"){
                    var ifMatch = Regex.Match(ContextData.ActualLine.Substring(2), @"\(([^)]+)\)\s*&do{", RegexOptions.None);
                    if (ifMatch.Success){
                        string condition = ifMatch.Groups[1].Value;
                        var (r, conditionResult) = NptSystem.IFStatement(condition);
                        if (r != Diagnostics.Success)
                            return (ContextData.Debugs, ContextData.Outputs, r);

                        ContextData.Debugs.Add($"A condição '{condition}' é {conditionResult}");
                        //creating a new block based on the "if" condition
                        blockStack.Push(new CodeBlock
                        {
                            IndentLevel = blockStack.Peek().IndentLevel + 1,
                            CanExecute = conditionResult //control whether the block can be executed based on the result of the condition
                        });

                        continue;
                    }
                    else
                        return (ContextData.Debugs, ContextData.Outputs, Diagnostics.SyntaxException);
                }

                else if (keyWordName.Letters == "goto"){
                    if (int.TryParse(keyWordName.Chars.Substring(4), out int targetLineIndex)){
                        if (targetLineIndex >= 1 && targetLineIndex - 1 < ContextData.Lines.Count){
                            ContextData.Lines[i] = "";
                            i = targetLineIndex - 2;
                            continue;
                        }
                        else
                            return (ContextData.Debugs, ContextData.Outputs, Diagnostics.OutOfRangeException);
                    }
                    else
                        return (ContextData.Debugs, ContextData.Outputs, Diagnostics.SyntaxException);
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
                        return (ContextData.Debugs, ContextData.Outputs, Diagnostics.OutOfRangeException);

                    var closingBlock = blockStack.Pop();
                    ContextData.Debugs.Add($"Fechando bloco no nível de indentação {closingBlock.IndentLevel}");
                    continue;
                }
                else if (keyWordName.Letters == "kit"){
                    ContextData.Debugs.Add("Script interrompido pela palavra-chave 'kit'.");
                    return (ContextData.Debugs, ContextData.Outputs, Diagnostics.EarlyTermination);
                }
                else if (keyWordName.Letters == "forget"){
                    return (null, null, Diagnostics.Forgotten);
                }

                else if (keyWordName.Letters == "raizeEx"){
                    ContextData.Debugs.Add("Exceção levantada pela palavra-chave 'raizeEx'.");
                    return (ContextData.Debugs, ContextData.Outputs, Diagnostics.RaisedException);
                }
                else
                    return (ContextData.Debugs, ContextData.Outputs, Diagnostics.InvalidKeywordException);
            }
            if (!blockStack.Peek().CanExecute) continue;

            //so, the only remaining action is to call a function:
            var objMatch = Regex.Match(ContextData.ActualLine, @"(?:(\w+)::)?(\w+)\(([^)]*)\)\s*->\s*(.+)");
            if (objMatch.Success)
            {
                var resultObj = await ExecuteObjectStatementAsync(objMatch, DiscordCtx);
                if (resultObj != Diagnostics.Success)
                {
                    return (ContextData.Debugs, ContextData.Outputs, resultObj);
                }
            }
            
            else{ 
                ContextData.Outputs.Add($"Error: unrecognized line: {ContextData.ActualLine}");
                return (ContextData.Debugs, ContextData.Outputs, Diagnostics.SyntaxException);
            }
        }

        return (ContextData.Debugs, ContextData.Outputs, Diagnostics.Success);
    }

    private async Task<Diagnostics> InvokeClassController(string className, string methodName, NptGroup args, CommandContext ctx)
    {
        try
        {
            string classNameProper = char.ToUpper(className[0]) + className.Substring(1).ToLower() + "Entitie";
            Type type = Type.GetType($"Suni.Suni.NptEnvironment.Data.Classes.{classNameProper}");

            if (type != null){
                var controlerMethod = type.GetMethod("Controler", BindingFlags.Static | BindingFlags.Public);
                if (controlerMethod != null){
                    var task = (Task<Diagnostics>)controlerMethod.Invoke(null, [methodName, args, ctx]);
                    return await task;
                }

                ContextData.Debugs.Add($"Internal Error: Method 'Controler' not found in '{classNameProper}'.");
                return Diagnostics.IncludeNotFoundException;
            }

            ContextData.Debugs.Add($"Error: '{classNameProper}' not found.");
            return Diagnostics.IncludeNotFoundException;
        }
        catch (Exception ex){
            ContextData.Outputs.Add($"Unknown Error while executing '{className}::{methodName}': {ex.Message}");
            return Diagnostics.UnknowException;
        }
    }
}
