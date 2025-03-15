using System.Text;
using Suni.Suni.NikoSharp.Core.Evaluator;
using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpSystem
{
    public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync()
    {
        var parser = new NikoSharpParser(ContextData.Tokens, ContextData);

        try{
            var parseResult = await parser.ParseStatementAsync();

            if (parseResult != Diagnostics.Success)
                return (ContextData.Debugs, ContextData.Outputs, parseResult);
        }
        catch (ParseException ex)
        {
            ContextData.LogDiagnostic(ex.Diagnostic, ex.Message);
            return (ContextData.Debugs, ContextData.Outputs, ex.Diagnostic);
        }
        
        return (ContextData.Debugs, ContextData.Outputs, Diagnostics.Success);
    }
}

public partial class NikoSharpParser
{
    private readonly string[] _tokens;
    private int _position;
    private readonly EnvironmentDataContext _context;
    private const int MaxIterations = 10; //limit iterations to avoid infinite loops

    public NikoSharpParser(string[] tokens, EnvironmentDataContext context)
    {
        _tokens = tokens;
        _context = context;
        _position = 0;

        _context.Debugs.Add($"todos os tokens: {string.Join(" % ", _tokens)}");//debugging///////////
    }

    public async Task<Diagnostics> ParseStatementAsync()
    {
        if (_position >= _tokens.Length) return Diagnostics.Success;

        var current = CurrentToken();

        if (current == "EOL")
        {
            _position++;
            return await ParseStatementAsync();
        }

        if (IsType(current))
            return await ParseVariableDeclarationAsync();

        switch (current)
        {
            case "if": return await ParseIfStatementAsync();
            case "while": return await ParseWhileStatementAsync();
            case "for": return await ParseForStatementAsync();
            case "poeng": return await ParsePoengStatementAsync();
            case "exit": return ParseExitStatement();
        }

        if (PeekToken() == "::")
        {
            var result = await ParseMethodCallAsync();
            
            return result.Diagnostic;
        }

        throw new ParseException(Diagnostics.SyntaxException, $"Token inesperado: {current}");
    }

    private async Task<Diagnostics> ParseVariableDeclarationAsync()
    {
        var type = ConsumeToken();
        if (!Enum.TryParse(type, out STypes wantedType))
            throw new ParseException(Diagnostics.InvalidTypeException, $"Type '{type}' is not a valid type.");
        
        var identifier = ConsumeToken();
        ConsumeToken("=");
        var expression = ParseEncapsulation('(', ')');
        var resultEvaluation = NikoSharpEvaluator.EvaluateExpression(expression);
        
        //checl diagnostic
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

    private async Task<Diagnostics> ParseIfStatementAsync()
    {
        ConsumeToken("if");
        var condition = NikoSharpEvaluator.EvaluateExpression(ParseEncapsulation('(', ')'));
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
            _context.Debugs.Add($"Bloco if: {conditionResult}");
            if (conditionResult)
            {
                return await ExecuteBlockAsync_Internal(ifBlockTokens);
            }
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

    private async Task<Diagnostics> ParseWhileStatementAsync()
    {
        ConsumeToken("while");
        var condition = ParseEncapsulation('(', ')');
        ConsumeToken("do");

        List<string> blockTokens = CaptureBlockTokens();

        int iterationCount = 0;
        while (true)
        {
            var conditionResult = NikoSharpEvaluator.EvaluateExpression(condition);
            if (conditionResult.diagnostic != Diagnostics.Success)
                throw new ParseException(conditionResult.diagnostic, conditionResult.diagnosticMessage);

            if (!(conditionResult.resultValue is NikosBool conditionResultBool))
                throw new ParseException(Diagnostics.CannotConvertType, "while expects STypes.Bool");

            if (!(bool)conditionResultBool.Value)
                break;

            iterationCount++;
            if (iterationCount > MaxIterations)
            {
                _context.LogDiagnostic(Diagnostics.Anomaly, $"Número de iterações no 'while' limitado a {MaxIterations}.");
                break;
            }

            _context.Debugs.Add("Executando bloco while");
            var result = await ExecuteBlockAsync_Internal(blockTokens);
            if (result != Diagnostics.Success)
                return result;
        }
        return Diagnostics.Success;
    }

    private async Task<Diagnostics> ParsePoengStatementAsync()
    {
        ConsumeToken("poeng");

        var countExpr = ParseEncapsulation('(', ')');
        var countResult = NikoSharpEvaluator.EvaluateExpression(countExpr);
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
            throw new ParseException(Diagnostics.TypeMismatchException, "poeng requires an 'STypes.Int', less than 10.");

        for (int i = 0; i < (long)count.Value && i < MaxIterations; i++)
        {
            if (usedOutVar is not null)
            {
                _context.BlockStack.Peek().LocalVariables[usedOutVar] = new NikosInt(i + 1);
                _context.Debugs.Add($"Variável declarada: Int {usedOutVar} = {i + 1}");
            }
            _context.Debugs.Add($"Executando iteração {i + 1} do poeng");
            var result = await ExecuteBlockAsync_Internal(blockTokens);
            if (result != Diagnostics.Success)
                return result;
        }
        return Diagnostics.Success;
    }

    private async Task<Diagnostics> ParseForStatementAsync()
    {
        ConsumeToken("for");
        ConsumeToken("each");

        string listExpr = ParseEncapsulation('(', ')');
        var listResult = NikoSharpEvaluator.EvaluateExpression(listExpr);
        if (listResult.diagnostic != Diagnostics.Success)
            throw new ParseException(listResult.diagnostic, listResult.diagnosticMessage);

        if (!(listResult.resultValue is NikosList nikosList))
            throw new ParseException(Diagnostics.TypeMismatchException, "for loop expects a List type.");

        ConsumeToken("out");
        string iteratorName = ConsumeToken();
        ConsumeToken("do");

        List<string> blockTokens = CaptureBlockTokens();

        foreach (SType element in (List<SType>)nikosList.Value)
        {
            _context.BlockStack.Peek().LocalVariables[iteratorName] = element;
            _context.Debugs.Add($"Executando iteração do for: {iteratorName} = {element.ToNikosStr().Value}");
            var result = await ExecuteBlockAsync_Internal(blockTokens);
            if (result != Diagnostics.Success)
                return result;
        }
        return Diagnostics.Success;
    }

    private Diagnostics ParseExitStatement()
    {
        ConsumeToken("exit");
        _context.Debugs.Add("exit command executed!");
        throw new ParseException(Diagnostics.EarlyTermination, "Exit requested.");
    }

    private async Task<(SType ResultVal, Diagnostics Diagnostic)> ParseMethodCallAsync()
    {
        await Task.CompletedTask;
        string className = ConsumeToken("std"); //ill keep this in this commit cuz im lazy now
        ConsumeToken("::");
        string method = ConsumeToken();
        var args = ParseEncapsulation('(', ')');
        var evaluatedArgs = NikoSharpEvaluator.EvaluateExpression(args);
        if (evaluatedArgs.diagnostic != Diagnostics.Success)
            throw new ParseException(evaluatedArgs.diagnostic, evaluatedArgs.diagnosticMessage);

        if (method == "out")
        {
            _context.Outputs.Add(evaluatedArgs.resultValue.ToString());
            return (new NikosVoid(), Diagnostics.Success);
        }

        throw new ParseException(Diagnostics.FunctionNotFound, $"Method not found: {className}::{method}");
    }

    private string ParseEncapsulation(char open, char close)
    {
        return ResolveVariables(ConsumeToken().Trim(open, close));
    }

    private string ConsumeToken(string expected = null)
    {
        while (_position < _tokens.Length && _tokens[_position] == "EOL")
            _position++;
        if (_position >= _tokens.Length || _tokens[_position] == "EOF")
            throw new ParseException(Diagnostics.SyntaxException, "Fim do arquivo inesperado.");

        if (expected != null && CurrentToken() != expected)
            throw new ParseException(Diagnostics.SyntaxException, $"Esperado: {expected}");

        return _tokens[_position++];
    }

    private string CurrentToken()
    {
        while (_position < _tokens.Length && _tokens[_position] == "EOL")
            _position++;

        return _position < _tokens.Length ? _tokens[_position] : "EOF";
    }

    private string PeekToken(int aditionalPos = 1)
    {
        int peekPos = _position + aditionalPos;
        while (peekPos < _tokens.Length && _tokens[peekPos] == "EOL")
            peekPos++;
        return peekPos < _tokens.Length ? _tokens[peekPos] : "EOF";
    }

    private bool IsType(string token) => Enum.TryParse<STypes>(token, out _);

    private string ResolveVariables(string expression)
    {
        StringBuilder result = new StringBuilder(expression.Length);
        int length = expression.Length;

        for (int i = 0; i < length; i++)
        {
            if (i < length - 2 && expression[i] == '$' && expression[i + 1] == '{')
            {
                int start = i + 2;
                int end = start;

                while (end < length && expression[end] != '}')
                    end++;

                if (end < length)
                {
                    string variableName = expression.Substring(start, end - start);
                    i = end;

                    if (_context.TryGetVariableValue(variableName, out var value))
                        result.Append(value);
                    else
                        throw new ParseException(Diagnostics.UnlistedVariable, $"Variável '{variableName}' não encontrada");
                }
                else
                    result.Append("${");
            }
            else
                result.Append(expression[i]);
        }

        return result.ToString();
    }
}

public class ParseException : Exception
{
    public Diagnostics Diagnostic { get; }
    public ParseException(Diagnostics diagnostic, string message) : base(message)
    {
        Diagnostic = diagnostic;
    }
}
