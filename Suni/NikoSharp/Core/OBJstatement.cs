using System.Text.RegularExpressions;
using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NptSystem
{
    private async Task<Diagnostics> ExecuteObjectStatementAsync(Match objMatch, CommandContext ctx)
    {
        string className = objMatch.Groups[1].Success ? objMatch.Groups[1].Value : null;
        string methodName = objMatch.Groups[2].Value;
        string args = objMatch.Groups[3].Value;
        if (string.IsNullOrWhiteSpace(args)) args = "void";
        string pointer = objMatch.Groups[4].Value;

        var (evaluatedArgs, result, resultM) = NptEvaluator.EvaluateExpression($"[{args}], {pointer}", ContextData); //evals the expression
        
        NptGroup evaluatedArgsGroup = null;
        if (evaluatedArgs is not NptGroup)
            return Diagnostics.CannotConvertType;
        
        evaluatedArgsGroup = (NptGroup)evaluatedArgs;
        
        //check
        if (result != Diagnostics.Success){
            ContextData.LogDiagnostic(result, resultM);
            return result;
        }

        //if class is not specified, look in _includes
        if (string.IsNullOrEmpty(className))
        {
            className = ContextData.Includes.FirstOrDefault(kv => kv.Value.Contains(methodName)).Key;

            if (className == null){
                ContextData.Outputs.Add($"Method '{methodName}' not associated with any class in includes.");
                return Diagnostics.IncludeNotFoundException;
            }
        }

        ContextData.Debugs.Add($"Executing {className}::{methodName} with args: {args}, pointer: {pointer}");

        //STD is a SPECIAL case
        if (className == "std")
            return STDController(methodName, evaluatedArgsGroup);
        if (className == "wp")
            return await WorkspaceFunctionsController(methodName, evaluatedArgsGroup);
        
        //normal cases:
        try{
            //invoke the Controller of the appropriate class
            return Diagnostics.UnknowException;//////////////////////////////await InvokeClassController(className, methodName, evaluatedArgsGroup, ctx);
        }
        catch (Exception ex)
        {
            ContextData.Outputs.Add($"CRIT: NPT Internal Script Error while executing '{methodName}': {ex.Message}");
            return Diagnostics.UnknowException;
        }
    }
}
