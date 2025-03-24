using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private async Task<Diagnostics> ParseIfStatementAsync()
    {
        ConsumeToken("if");
        var condition = NikoSharpEvaluator.EvaluateExpression(ParseEncapsulation('(', ')'), _context);
        if (condition.diagnostic != Diagnostics.Success)
            throw new ParseException(condition.diagnostic, condition.diagnosticMessage);

        ConsumeToken("do");

        List<string> ifBlockTokens = CaptureBlockTokens();

        List<string> elseBlockTokens = null;
        if (PeekToken(0) == "else")
        {
            ConsumeToken("else");
            ConsumeToken("do");
            elseBlockTokens = CaptureBlockTokens();
        }

        if (condition.resultValue.Value is bool conditionResult)
        {
            _context.Debugs.Add($"If block: {conditionResult}");
            if (conditionResult)
                return await ExecuteBlockAsync_Internal(ifBlockTokens);
            else if (elseBlockTokens != null)
            {
                _context.Debugs.Add("Executando bloco else");
                return await ExecuteBlockAsync_Internal(elseBlockTokens);
            }
        }
        else
            throw new ParseException(Diagnostics.TypeMismatchException, "Condição do if deve ser booleana");
        return Diagnostics.Success;
    }
}