using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private async Task<Diagnostics> ParseVariableDeclarationAsync()
    {
        var type = ConsumeToken();
        var identifier = ConsumeToken();
        ConsumeToken("=");

        if (!Enum.TryParse(type, out STypes wantedType))
            throw new ParseException(Diagnostics.InvalidTypeException, $"unknown exception");

        //if its STypes.TypeClass, create a new class.
        if (wantedType == STypes.Class)
        {
            ConsumeToken("new"); //just to keep the syntax consistent.
            NikosTypeClass typeClass = new NikosTypeClass(identifier);
            _context.Debugs.Add($"Creating a new TypeClass: {identifier}");
            if (_context.BlockStack.Peek().LocalVariables.ContainsKey(identifier))
                throw new ParseException(Diagnostics.InvalidTypeException, $"class '{identifier}' already exists. Use a diffrent name!");
            _context.BlockStack.Peek().LocalVariables[identifier] = typeClass;
            return Diagnostics.Success;
        }
        //else, evaluate and store as a commum variable.
        var expression = ParseEncapsulation('(', ')');
        var resultEvaluation = NikoSharpEvaluator.EvaluateExpression(expression, _context);
        

        //check diagnostic
        if (resultEvaluation.diagnostic != Diagnostics.Success)
            throw new ParseException(resultEvaluation.diagnostic, resultEvaluation.diagnosticMessage);
        //check typed result
        if (resultEvaluation.resultValue.Type != wantedType)
            throw new ParseException(Diagnostics.CannotConvertType, $"'{resultEvaluation.resultValue.Type}' cannot be explicitly converted to '{wantedType}'");
        
        _context.BlockStack.Peek().LocalVariables[identifier] = resultEvaluation.resultValue; //register in the scope//
        _context.Debugs.Add($"Variável declarada: {type} {identifier} = {expression}");

        await Task.CompletedTask;
        return Diagnostics.Success;
    }
}