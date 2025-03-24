using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private async Task<(SType ResultVal, Diagnostics Diagnostic)> ParseMethodCallAsync()
    {
        string className = ConsumeToken();
        ConsumeToken("::");
        string methodName = ConsumeToken();
        var evaluatedArgs = NikoSharpEvaluator.EvaluateExpression(ParseEncapsulation('(', ')'), _context);
        if (evaluatedArgs.diagnostic != Diagnostics.Success)
            throw new ParseException(evaluatedArgs.diagnostic, evaluatedArgs.diagnosticMessage);

        string key = $"{className}::{methodName}";
        
        if (NikoSharpConfigs.Configurations.RegisteredFunctions.TryGetValue(key, out Delegate externalFunction))
        {
            object result = ((Func<object[], object>)externalFunction)(new object[] { evaluatedArgs.resultValue });
            return (new NikosVoid(), Diagnostics.Success); //TODO
        }

        if (!_context.BlockStack.Peek().LocalVariables.TryGetValue(className, out var classInstance))
            throw new ParseException(Diagnostics.UnlistedVariable, $"Class '{className}' not found.");
        
        if (classInstance is not NikosTypeClass nikosClass)
            throw new ParseException(Diagnostics.InvalidTypeException, $"'{className}' isn't a TypeClass.");
        
        NikosClass targetClass = (NikosClass)nikosClass.Value;
        NikosMethod methodFound = targetClass.GetMethod(methodName) ?? throw new ParseException(Diagnostics.FunctionNotFound, $"'{methodName}' doesn't exist in '{className}' class.");
        var resultBlock =  await ExecuteBlockAsync_Internal(methodFound.Code);
        return (new NikosVoid(), resultBlock);
    }
}