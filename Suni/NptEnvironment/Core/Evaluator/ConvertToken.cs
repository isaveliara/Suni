using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Core.Evaluator;
partial class NptEvaluator
{
    internal static SType ConvertToken(string token, EnvironmentDataContext context = null)
    {
        if (int.TryParse(token, out int intValue)) return new NptInt(intValue);
        if (double.TryParse(token, out double doubleValue)) return new NptFloat(doubleValue);
        if (bool.TryParse(token, out bool boolValue)) return new NptBool(boolValue);
        if (token == "nil") return new NptNil();
        if (token.StartsWith("s'") && token.EndsWith('\'') && token.Length >= 3) return new NptStr(token[2..^1]);
        if (token.StartsWith("c'") && token.EndsWith('\'') && token.Length == 4) return new NptChar(token[2]);
        
        var identifierVal = new NptIdentifier(token);
        return identifierVal.Value is not null
            ? identifierVal
            : new NptError(Diagnostics.BadToken, $"'{token}' is not a valid Token.");
    }
}