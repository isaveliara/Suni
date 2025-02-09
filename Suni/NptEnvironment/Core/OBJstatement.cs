using System.Text.RegularExpressions;
using Suni.Suni.NptEnvironment.Data;
namespace Suni.Suni.NptEnvironment.Core;

public partial class NptSystem
{
    private async Task<Diagnostics> ExecuteObjectStatementAsync(Match objMatch, CommandContext ctx)
    {
        string className = objMatch.Groups[1].Success ? objMatch.Groups[1].Value : null;
        string methodName = objMatch.Groups[2].Value;
        string argumentsToSplit = objMatch.Groups[3].Value;
        string pointer = objMatch.Groups[4].Value;
        var args = argumentsToSplit.Split(',');

        //if class is not specified, look in _includes
        if (string.IsNullOrEmpty(className))
        {
            className = ContextData.Includes.FirstOrDefault(kv => kv.Value.Contains(methodName)).Key;

            if (className == null){
                ContextData.Outputs.Add($"Method '{methodName}' not associated with any class in includes.");
                return Diagnostics.IncludeNotFoundException;
            }
        }

        ContextData.Debugs.Add($"Executing {className}::{methodName} with args: {argumentsToSplit}, pointer: {pointer}");

        //STD is a SPECIAL case
        if (className == "std")
            return STDControler(methodName, args.ToList(), pointer);
        
        //normal cases:
        try{
            //invoke the Controller of the appropriate class
            return await InvokeClassControler(className, methodName, args.ToList(), pointer, ctx);
        }
        catch (Exception ex)
        {
            ContextData.Outputs.Add($"CRIT: NPT Internal Script Error while executing '{methodName}': {ex.Message}");
            return Diagnostics.UnknowException;
        }
    }
}
