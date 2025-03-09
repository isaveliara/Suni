using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NptSystem
{
    private async Task<Diagnostics> WorkspaceFunctionsController(string functionName, NptGroup evaluatedArgs)
    {
        //localize the function
        var functionVar = ContextData.Variables.FirstOrDefault(dict => dict.ContainsKey(functionName));
        if (functionVar == null || functionVar[functionName].Type != STypes.Function){
            ContextData.Outputs.Add($"Função '{functionName}' não encontrada ou não é do tipo Fn.");
            return Diagnostics.FunctionNotFound;
        }
        
        var fnVal = (NptFunction)functionVar[functionName];
        var fn = (Function)fnVal.Value;

        ContextData.Debugs.Add($"Chamando função '{fn.Name}' com argumentos: {fn.Parameters.ToString}");

        //validates the number/type of arguments
        if (fn.Parameters.Count() != 0 && !evaluatedArgs.ValidateTypes(fn.ParametersTypes))
        {
            //DEBUG
            Console.WriteLine("largura "+(int)fn.Parameters.Count());
            Console.WriteLine("comaracao "+evaluatedArgs.ValidateTypes(fn.ParametersTypes));
            Console.WriteLine("tipos "+fn.Parameters);
            
            return Diagnostics.ArgumentMismatch;
        }

        string functionCode = fn.Code;
        for (int j = 0; j < evaluatedArgs.Count(); j++){
            string placeholder = $"${{{fn.Parameters.Value.ToString()[j]}}}";
            functionCode = functionCode.Replace(placeholder, evaluatedArgs.GetAt(j).ToString());
        }
        //this is a bad idea.
        List<string> originalLines = ContextData.Lines;
        var (debugs, outputs, resultNewSystem) = await new NptSystem(functionCode, DiscordCtx, contextData: ContextData).ParseScriptAsync();
        ContextData.Lines = originalLines;
        ContextData.Debugs.AddRange(debugs);
        ContextData.Outputs.AddRange(outputs);
        
        if (resultNewSystem != Diagnostics.Success)
            return resultNewSystem;

        return Diagnostics.Success;
    }
}