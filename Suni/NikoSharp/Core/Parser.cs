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
        Diagnostics result = Diagnostics.Success;
        while (parser.CurrentToken() != "EOF")
        {
            try
            {
                result = await parser.ParseStatementAsync();
                if (result != Diagnostics.Success)
                    return (ContextData.Debugs, ContextData.Outputs, result);
            }
            catch (ParseException ex)
            {
                ContextData.Outputs.Add($"{ex.Diagnostic}: {ex.Message}");
                return (ContextData.Debugs, ContextData.Outputs, ex.Diagnostic);
            }
        }
        return (ContextData.Debugs, ContextData.Outputs, Diagnostics.Success);
    }
}

public partial class NikoSharpParser
{
    private readonly string[] _tokens;
    private int _position;
    private readonly EnvironmentDataContext _context;

    public NikoSharpParser(string[] tokens, EnvironmentDataContext context)
    {
        _tokens = tokens;
        _context = context;
        _position = 0;

        _context.Debugs.Add($"all tokens: {string.Join(" % ", _tokens)}");//debugging///////////
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

        if (PeekToken() == "::")
        {
            var result = await ParseMethodCallAsync();
            return result.Diagnostic;
        }

        if (IsType(current))
            return await ParseVariableDeclarationAsync();
        
        if (IsClass(current))
            return await ClassEditing();
            //TypeClass burrice = new . burrice verificar_coisas ({}) = if (true) do . exit . end end   

        switch (current)
        {
            case "if": return await ParseIfStatementAsync();
            case "while": return await ParseWhileStatementAsync();
            case "for": return await ParseForStatementAsync();
            case "poeng": return await ParsePoengStatementAsync();
            case "exit": return ParseExitStatement();
        }

        throw new ParseException(Diagnostics.SyntaxException, $"UnexpectedToken: {current}");
    }

    private async Task<Diagnostics> ParseVariableDeclarationAsync()
    {
        var type = ConsumeToken();
        var identifier = ConsumeToken();
        ConsumeToken("=");

        if (!Enum.TryParse(type, out STypes wantedType))
            throw new ParseException(Diagnostics.InvalidTypeException, $"unknown exception");

        //if its STypes.TypeClass, create a new class.
        if (wantedType == STypes.TypeClass)
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
        var resultEvaluation = NikoSharpEvaluator.EvaluateExpression(expression);
        

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

    private async Task<Diagnostics> ClassEditing()
    {
        //syntax: classname methodname<args> = .....end
        //or: classname methodname<args> out <returnType> = .....end
        string className = ConsumeToken();
        
        if (!_context.BlockStack.Peek().LocalVariables.ContainsKey(className))
            throw new ParseException(Diagnostics.UnlistedVariable, $"Class '{className}' not found.");
        if (_context.BlockStack.Peek().LocalVariables[className] is not NikosTypeClass existingClass)
            throw new ParseException(Diagnostics.InvalidTypeException, $"'{className}' isnt a TypeClass.");

        string methodName = ConsumeToken();
        string args = ParseEncapsulation('<', '>');
        ConsumeToken("=");
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

    private Diagnostics ParseExitStatement()
    {
        ConsumeToken("exit");
        _context.Debugs.Add("exit command executed!");
        throw new ParseException(Diagnostics.EarlyTermination, "Exit requested.");
    }

    private async Task<(SType ResultVal, Diagnostics Diagnostic)> ParseMethodCallAsync()
    {
        string className = ConsumeToken();
        ConsumeToken("::");
        string methodName = ConsumeToken();
        var evaluatedArgs = NikoSharpEvaluator.EvaluateExpression(ParseEncapsulation('(', ')'));
        if (evaluatedArgs.diagnostic != Diagnostics.Success)
            throw new ParseException(evaluatedArgs.diagnostic, evaluatedArgs.diagnosticMessage);

        string key = $"{className}::{methodName}";
        
        if (NikoSharpConfigs.Configurations.RegisteredFunctions.TryGetValue(key, out Delegate externalFunction))
        {
            object result = ((Func<object[], object>)externalFunction)(new object[] { evaluatedArgs.resultValue });
            return (new NikosVoid(), Diagnostics.Success); //TODO
        }

        if (!_context.BlockStack.Peek().LocalVariables.TryGetValue(className, out var classInstance))
            throw new ParseException(Diagnostics.UnlistedVariable, $"Class '{className}' not found.");
        
        if (classInstance is not NikosTypeClass nikosClass)
            throw new ParseException(Diagnostics.InvalidTypeException, $"'{className}' isn't a TypeClass.");
        
        NikosClass targetClass = (NikosClass)nikosClass.Value;
        NikosMethod methodFound = targetClass.GetMethod(methodName) ?? throw new ParseException(Diagnostics.FunctionNotFound, $"'{methodName}' doesn't exist in '{className}' class.");
        var resultBlock =  await ExecuteBlockAsync_Internal(methodFound.Code);
        return (new NikosVoid(), resultBlock);
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
            throw new ParseException(Diagnostics.SyntaxException, "Unexpected End of FIle.");
        
        var currentToken = CurrentToken();
        if (expected != null && currentToken != expected)
            throw new ParseException(Diagnostics.SyntaxException, $"Expected: '{expected}', Got: '{currentToken}'");

        return _tokens[_position++];
    }

    public string CurrentToken()
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
    
    private bool IsClass(string token)
    {
        if (_context.BlockStack.Peek().LocalVariables.ContainsKey(token))
            if (_context.BlockStack.Peek().LocalVariables[token].Type == STypes.TypeClass)
                return true;
        return false;
    }

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
                        throw new ParseException(Diagnostics.UnlistedVariable, $"Variable '{variableName}' not found");
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
