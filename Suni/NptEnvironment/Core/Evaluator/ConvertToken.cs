using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Core.Evaluator;
partial class NptEvaluator
{
    internal static SType ConvertToken(string token, EnvironmentDataContext context = null)
    {
        if (long.TryParse(token, out long intValue)) return new NptInt(intValue);
        if (double.TryParse(token, out double doubleValue)) return new NptFloat(doubleValue);
        if (bool.TryParse(token, out bool boolValue)) return new NptBool(boolValue);
        if (token == "nil") return new NptNil();
        if (token == "void") return new NptVoid();
        if (token.StartsWith("s'") && token.EndsWith('\'') && token.Length >= 3) return new NptStr(token[2..^1]);
        if (token.StartsWith("c'") && token.EndsWith('\'') && token.Length == 4) return new NptChar(token[2]);
        Console.WriteLine(token);
        if (token.StartsWith('{') && token.EndsWith('}'))
        {
            string content = token[1..^1].Trim();

            if (string.IsNullOrWhiteSpace(content))
                return new NptList([]);

            var elements = content.Split(',')
                    .Select(t => t.Trim())
                    .ToList();

            if (elements.All(e => e.Contains(':'))){
                var dict = new Dictionary<NptStr, SType>();
                foreach (var pair in elements){
                    var parts = pair.Split(':', 2).Select(p => p.Trim()).ToArray();
                    if (parts.Length != 2) return new NptError(Diagnostics.BadToken, $"Invalid dictionary entry '{pair}'.");

                    var key = ConvertToken(parts[0], context);
                    if (key is NptStr keyStrVal)
                    {
                        var value = ConvertToken(parts[1], context);

                        if (value is NptError)
                            return new NptError(Diagnostics.BadToken, $"Invalid value in dictionary entry '{pair}'.");

                        dict[keyStrVal] = value;
                    }
                    else
                        return new NptError(Diagnostics.CannotConvertType, $"in dictionaries, the Key can only be 'STypes.Str', so not s'STypes.{key.Type}'");
                }
                return new NptDict(dict);
            }
            else
                return new NptList([.. elements.Select(e => ConvertToken(e, context))]);
        }
        
        var identifierVal = new NptIdentifier(token);
        return identifierVal.Value is not null
            ? identifierVal
            : new NptError(Diagnostics.BadToken, $"'{token}' is not a valid Token.");
    }
}