using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private async Task<Diagnostics> ParseClassEditing()
    {
        string className = ConsumeToken();
        
        if (!_context.BlockStack.Peek().LocalVariables.ContainsKey(className))
            throw new ParseException(Diagnostics.UnlistedVariable, $"Class '{className}' not found.");
        if (_context.BlockStack.Peek().LocalVariables[className] is not NikosTypeClass existingClass)
            throw new ParseException(Diagnostics.InvalidTypeException, $"'{className}' isnt a TypeClass.");

        string methodName = ConsumeToken();
        ConsumeToken("=");
        ConsumeToken("function");
        string args = ParseEncapsulation('(', ')');
        List<string> code = CaptureBlockTokens();
        NikosMethod method = new NikosMethod
        {
            NameMethod = methodName,
            Code = code,
            ArgsValues = new NikosDict(new Dictionary<NikosStr, SType>()),
            ReturnType = STypes.Void
        };

        NikosClass targetClass = (NikosClass)existingClass.Value;
        targetClass.RegisterMethod(method);
        
        await Task.CompletedTask;
        return Diagnostics.Success;
    }

}