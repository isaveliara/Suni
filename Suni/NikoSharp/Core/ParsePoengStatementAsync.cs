using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private async Task<Diagnostics> ParsePoengStatementAsync()
    {
        ConsumeToken("poeng");

        var countExpr = ParseEncapsulation('(', ')');
        var countResult = NikoSharpEvaluator.EvaluateExpression(countExpr, _context);
        if (countResult.diagnostic != Diagnostics.Success)
            throw new ParseException(countResult.diagnostic, countResult.diagnosticMessage);

        string usedOutVar = null;
        if (PeekToken(0) == "out")
        {
            ConsumeToken("out");
            usedOutVar = ConsumeToken();
        }
        ConsumeToken("do");

        List<string> blockTokens = CaptureBlockTokens();

        if (!(countResult.resultValue is NikosInt count))
            throw new ParseException(Diagnostics.TypeMismatchException, "poeng requires an 'STypes.Int'");

        for (int i = 0; i < (long)count.Value && i < NikoSharpConfigs.Configurations.LanguageSettings.MaxIterations; i++)
        {
            if (usedOutVar is not null)
            {
                _context.BlockStack.Peek().LocalVariables[usedOutVar] = new NikosInt(i + 1);
                _context.Debugs.Add($"Variable Added: Int {usedOutVar} = {i + 1}");
            }
            _context.Debugs.Add($"Executing iteration {i + 1} of poeng");
            var result = await ExecuteBlockAsync_Internal(blockTokens);
            if (result != Diagnostics.Success)
                return result;
        }
        return Diagnostics.Success;
    }
}