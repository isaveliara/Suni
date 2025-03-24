using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core.Evaluator;
partial class NikoSharpEvaluator
{
    internal static SType ConvertToken(string token, EnvironmentDataContext context = null)
    {
        if (long.TryParse(token, out long intValue)) return new NikosInt(intValue);
        if (double.TryParse(token, out double doubleValue)) return new NikosFloat(doubleValue);
        if (bool.TryParse(token, out bool boolValue)) return new NikosBool(boolValue);
        if (token == "nil") return new NikosNil();
        if (token == "void") return new NikosVoid();
        if (token.StartsWith('\'') && token.EndsWith('\'') && token.Length >= 2) return new NikosStr(token[ 1..^1]);
        Console.WriteLine(token);
        if (token.StartsWith('{') && token.EndsWith('}'))
        {
            string content = token[1..^1].Trim();

            if (string.IsNullOrWhiteSpace(content))
                return new NikosList([]);

            var elements = content.Split(',')
                    .Select(t => t.Trim())
                    .ToList();

            if (elements.All(e => e.Contains(':'))){
                var dict = new Dictionary<NikosStr, SType>();
                foreach (var pair in elements){
                    var parts = pair.Split(':', 2).Select(p => p.Trim()).ToArray();
                    if (parts.Length != 2) return new NikosError(Diagnostics.BadToken, $"Invalid dictionary entry '{pair}'.");

                    var key = ConvertToken(parts[0], context);
                    if (key is NikosStr keyStrVal)
                    {
                        var value = ConvertToken(parts[1], context);

                        if (value is NikosError)
                            return new NikosError(Diagnostics.BadToken, $"Invalid value in dictionary entry '{pair}'.");

                        dict[keyStrVal] = value;
                    }
                    else
                        return new NikosError(Diagnostics.CannotConvertType, $"in dictionaries, the Key can only be 'STypes.Str', so not s'STypes.{key.Type}'");
                }
                return new NikosDict(dict);
            }
            else
                return new NikosList([.. elements.Select(e => ConvertToken(e, context))]);
        }
        
        //var identifierVal = new NikosIdentifier(token);
        //return identifierVal.Value is not null
        //    ? identifierVal
        //    : new NikosError(Diagnostics.BadToken, $"'{token}' is not a valid Token.");

        //get var value
        if (context is null)
            return new NikosError(Diagnostics.BadToken, $"'{token}' is not a valid Token or for some reason variables cannot be captured now.");
        
        if (context.TryGetVariableValue(token, out SType variableVal))
            return variableVal;
        return new NikosError(Diagnostics.UnlistedVariable, $"'{token}' is an unknown variable.");
    }
}