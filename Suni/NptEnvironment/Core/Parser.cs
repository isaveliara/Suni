using System.Text;
using Suni.Suni.NptEnvironment.Core.Evaluator;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;

namespace Suni.Suni.NptEnvironment.Core
{
    public partial class NptSystem
    {
        public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync()
        {
            blockStack.Push(new CodeBlock { IndentLevel = 0, CanExecute = true });

            for (int i = 0; i < ContextData.Lines.Count; i++)
            {
                ContextData.ActualLine = ContextData.Lines[i];
                if (string.IsNullOrWhiteSpace(ContextData.ActualLine)) continue;

                var parseResult = await ParseLineAsync(ContextData.ActualLine);
                if (parseResult != Diagnostics.Success)
                    return (ContextData.Debugs, ContextData.Outputs, parseResult);
            }

            return (ContextData.Debugs, ContextData.Outputs, Diagnostics.Success);
        }

        private async Task<Diagnostics> ParseLineAsync(string line)
        {
            try
            {
                var tokens = Tokens.Tokenize(line);
                //tokens.Append("EOL");
                var parser = new NptParser(tokens, ContextData);
                ContextData.Debugs.Add($"tokens da linha: {string.Join(" % ", tokens)}");//debugging///////
                return await parser.ParseStatementAsync();
            }
            catch (ParseException ex){
                ContextData.LogDiagnostic(ex.Diagnostic, ex.Message);
                return ex.Diagnostic;
            }
        }
    }

    public class NptParser
    {
        private readonly string[] _tokens;
        private int _position;
        private readonly EnvironmentDataContext _context;
        private const int MaxIterations = 10; //limit iterations to avoid infinite loops

        public NptParser(string[] tokens, EnvironmentDataContext context)
        {
            _tokens = tokens;
            _context = context;
            _position = 0;
        }

        public async Task<Diagnostics> ParseStatementAsync()
        {
            if (_position >= _tokens.Length) return Diagnostics.Success;

            var current = CurrentToken();
            if (IsType(current))
                return await ParseVariableDeclarationAsync();

            switch (current)
            {
                case "if": return await ParseIfStatementAsync();
                case "while": return await ParseWhileStatementAsync();
                case "poeng": return await ParsePoengStatementAsync();
                case "exit": return ParseExitStatement();
            }

            if (PeekToken() == "::")  
                return await ParseMethodCallAsync();

            throw new ParseException(Diagnostics.SyntaxException, $"Token inesperado: {current}");
        }

        private async Task<Diagnostics> ParseVariableDeclarationAsync()
        {
            var type = ConsumeToken();
            var identifier = ConsumeToken();
            ConsumeToken("=");
            var expression = ParseExpression();
            var resultEvaluation = NptEvaluator.EvaluateExpression(expression);
            if (resultEvaluation.diagnostic != Diagnostics.Success)
                throw new ParseException(resultEvaluation.diagnostic, resultEvaluation.diagnosticMessage);
            
            _context.BlockStack.Peek().LocalVariables[identifier] = resultEvaluation.resultValue; //register in the scope//
            _context.Debugs.Add($"Variável declarada: {type} {identifier} = {expression}");

            await Task.CompletedTask;
            return Diagnostics.Success;
        }

        private async Task<Diagnostics> ParseIfStatementAsync()
        {
            ConsumeToken("if");
            var condition = NptEvaluator.EvaluateExpression(ParseExpression());
            ConsumeToken("do");

            if (condition.resultValue.Value is bool conditionResult)
            {
                _context.Debugs.Add($"Bloco if: {conditionResult}");
                if (conditionResult)
                {
                    var result = await ExecuteBlockAsync();
                    if (result != Diagnostics.Success) return result;
                }

                if (PeekToken() == "else")
                {
                    ConsumeToken("else");
                    _context.Debugs.Add("Executando bloco else");
                    return await ExecuteBlockAsync();
                }
            }
            else
                throw new ParseException(Diagnostics.TypeMismatchException, "Condição do if deve ser booleana");

            return Diagnostics.Success;
        }

        private async Task<Diagnostics> ExecuteBlockAsync()
        {
            while (_position < _tokens.Length && CurrentToken() != "end")
            {
                var result = await ParseStatementAsync();
                if (result != Diagnostics.Success) return result;
            }
            ConsumeToken("end");
            return Diagnostics.Success;
        }

        private async Task<Diagnostics> ParseWhileStatementAsync()
        {
            ConsumeToken("while");
            var condition = ParseExpression();
            ConsumeToken("do");

            while (true)
            {
                var conditionResult = NptEvaluator.EvaluateExpression(condition);
                if (conditionResult.diagnostic != Diagnostics.Success)
                    throw new ParseException(conditionResult.diagnostic, conditionResult.diagnosticMessage);

                if (conditionResult.resultValue is NptBool conditionResultBool)
                {
                    if (!(bool)conditionResultBool.Value)
                        break;

                    _context.Debugs.Add("Executando bloco while");
                    var result = await ExecuteBlockAsync();
                    if (result != Diagnostics.Success) return result;
                }
                else
                    throw new ParseException(Diagnostics.CannotConvertType, "Esperava um STypes.Bool irmão");
                
            }

            ConsumeToken("end");
            return Diagnostics.Success;
        }

        private async Task<Diagnostics> ParsePoengStatementAsync()
        {
            ConsumeToken("poeng");

            var countExpr = ParseExpression();
            var countResult = NptEvaluator.EvaluateExpression(countExpr);
            if (countResult.diagnostic != Diagnostics.Success)
                throw new ParseException(countResult.diagnostic, countResult.diagnosticMessage);
            
            string usedOutVar = null;
            //you can set an local variable 
            if (PeekToken() == "out")
            {
                ConsumeToken("out");
                usedOutVar = ConsumeToken();
            }

            ConsumeToken("do");

            if (countResult.resultValue is NptInt count)
            {
                for (int i = 0; i < (long)count.Value; i++)
                {
                    if (usedOutVar is not null) _context.Variables[0][usedOutVar] = new NptInt(i + 1);
                    _context.Debugs.Add($"Executando iteração {i + 1} do poeng");
                    var result = await ExecuteBlockAsync();
                    if (result != Diagnostics.Success) return result;
                }
            }
            else
                throw new ParseException(Diagnostics.TypeMismatchException, "poeng requires an 'STypes.Int', less than 10.");
            

            ConsumeToken("end");
            return Diagnostics.Success;
        }

        private Diagnostics ParseExitStatement()
        {
            ConsumeToken("exit");
            _context.Debugs.Add("exit command executed!");
            throw new ParseException(Diagnostics.EarlyTermination, "Exit requested.");
        }

        private async Task<Diagnostics> ParseMethodCallAsync()
        {
            await Task.CompletedTask;
            string className = ConsumeToken("std"); //ill keep this in this commit cuz im lazy now
            ConsumeToken("::");
            string method = ConsumeToken();
            var args = ParseExpression();
            var evaluatedArgs = NptEvaluator.EvaluateExpression(args);
            if (evaluatedArgs.diagnostic != Diagnostics.Success)
                throw new ParseException(evaluatedArgs.diagnostic, evaluatedArgs.diagnosticMessage);

            //if (evaluatedArgs.resultValue is not NptGroup)
            ///    throw new ParseException(Diagnostics.CannotConvertType, "ajs ijajf jajfpaija sjasij please help mee");
            //NptGroup argsGroup = (NptGroup)evaluatedArgs.resultValue;
            
            _context.Outputs.Add(args.ToString());
            return Diagnostics.Success;

            throw new ParseException(Diagnostics.FunctionNotFound, $"Method not found: {className}::{method}");
        }

        private string ParseExpression()
        {
            return ResolveVariables(ConsumeToken().Trim('(', ')'));
        }

        private string ConsumeToken(string expected = null)
        {
            if (expected != null && CurrentToken() != expected)
                throw new ParseException(Diagnostics.SyntaxException, $"Esperado: {expected}");

            return _tokens[_position++];
        }

        private string CurrentToken() => _tokens[_position];
        private string PeekToken() => _position < _tokens.Length - 1 ? _tokens[_position + 1] : null;
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
                        i = end; // Avança até depois do '}'

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
}