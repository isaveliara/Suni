using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private async Task<Diagnostics> ParseWhileStatementAsync()
    {
        ConsumeToken("while");
        var condition = ParseEncapsulation('(', ')');
        ConsumeToken("do");

        List<string> blockTokens = CaptureBlockTokens();

        int iterationCount = 0;
        while (true)
        {
            var conditionResult = NikoSharpEvaluator.EvaluateExpression(condition, _context);
            if (conditionResult.diagnostic != Diagnostics.Success)
                throw new ParseException(conditionResult.diagnostic, conditionResult.diagnosticMessage);

            if (!(conditionResult.resultValue is NikosBool conditionResultBool))
                throw new ParseException(Diagnostics.CannotConvertType, "while expects STypes.Bool");

            if (!(bool)conditionResultBool.Value)
                break;

            iterationCount++;
            if (iterationCount > NikoSharpConfigs.Configurations.LanguageSettings.MaxIterations)
            {
                _context.Outputs.Add($"Warn: Number of iterations in 'while' limited to {NikoSharpConfigs.Configurations.LanguageSettings.MaxIterations}. Exiting the Loop");
                break;
            }

            _context.Debugs.Add("Executando bloco while");
            var result = await ExecuteBlockAsync_Internal(blockTokens);
            if (result != Diagnostics.Success)
                return result;
        }
        return Diagnostics.Success;
    }
}