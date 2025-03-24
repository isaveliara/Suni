using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private async Task<Diagnostics> ParseForStatementAsync()
    {
        ConsumeToken("for");
        ConsumeToken("each");

        string listExpr = ParseEncapsulation('(', ')');
        var listResult = NikoSharpEvaluator.EvaluateExpression(listExpr, _context);
        if (listResult.diagnostic != Diagnostics.Success)
            throw new ParseException(listResult.diagnostic, listResult.diagnosticMessage);

        if (!(listResult.resultValue is NikosList nikosList))
            throw new ParseException(Diagnostics.TypeMismatchException, "for loop expects a List type.");

        ConsumeToken("out");
        string iteratorName = ConsumeToken();
        ConsumeToken("do");

        List<string> blockTokens = CaptureBlockTokens();
        int iterationCount = 0;

        foreach (SType element in (List<SType>)nikosList.Value)
        {
            _context.BlockStack.Peek().LocalVariables[iteratorName] = element;
            _context.Debugs.Add($"Executando iteração do for: {iteratorName} = {element.ToNikosStr().Value}");
            var result = await ExecuteBlockAsync_Internal(blockTokens);
            if (result != Diagnostics.Success)
                return result;
            
            iterationCount++;
            if (iterationCount > NikoSharpConfigs.Configurations.LanguageSettings.MaxIterations)
            {
                _context.Outputs.Add($"Warn: Number of iterations in 'while' limited to {NikoSharpConfigs.Configurations.LanguageSettings.MaxIterations}. Exiting the Loop.");
                break;
            }
        }
        return Diagnostics.Success;
    }
}