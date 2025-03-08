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
                {
                    return (ContextData.Debugs, ContextData.Outputs, parseResult);
                }
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
            catch (ParseException ex)
            {
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
            {
                return await ParseVariableDeclarationAsync();
            }

            return current switch
            {
                "if"        => await ParseIfStatementAsync(),
                "while"     => await ParseWhileStatementAsync(),
                "poeng"     => await ParsePoengStatementAsync(),
                "exit"      => ParseExitStatement(),
                "std"       => await ParseMethodCallAsync(),
                _           => throw new ParseException(Diagnostics.SyntaxException, $"Token inesperado: {current}")
            };
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
            _context.Variables[0][identifier] = resultEvaluation.resultValue;
            _context.Debugs.Add($"Variável declarada: {type} {identifier} = {expression}");

            await Task.CompletedTask;
            return Diagnostics.Success;
        }

        private async Task<Diagnostics> ParseIfStatementAsync()
        {
            ConsumeToken("if");
            var condition = NptEvaluator.EvaluateExpression(ParseExpression());
            ConsumeToken("do");

            if (condition.resultValue is NptBool conditionResult)
            {
                _context.Debugs.Add($"Executando bloco if: {conditionResult.Value}");

                var result = await ExecuteBlockAsync();
                if (result != Diagnostics.Success) return result;

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
            
            ConsumeToken("do");

            if (countResult.resultValue is NptInt count)
            {
                for (int i = 0; i < (long)count.Value; i++)
                {
                    _context.Variables[0]["iterador"] = new NptInt(i + 1);
                    _context.Debugs.Add($"Executando iteração {i + 1} do poeng");
                    var result = await ExecuteBlockAsync();
                    if (result != Diagnostics.Success) return result;
                }
            }
            else
                throw new ParseException(Diagnostics.TypeMismatchException, "Contagem do poeng deve ser inteira");
            

            ConsumeToken("end");
            return Diagnostics.Success;
        }

        private Diagnostics ParseExitStatement()
        {
            ConsumeToken("exit");
            _context.Debugs.Add("Comando exit executado");
            throw new ParseException(Diagnostics.EarlyTermination, "Exit solicitado");
        }

        private async Task<Diagnostics> ParseMethodCallAsync()
        {
            await Task.CompletedTask;
            ConsumeToken("std");
            ConsumeToken("::");
            var method = ConsumeToken();
            var arg = ParseExpression();

            if (method == "out")
            {
                var result = NptEvaluator.EvaluateExpression(arg);
                if (result.diagnostic != Diagnostics.Success)
                    throw new ParseException(result.diagnostic, result.diagnosticMessage);
                _context.Outputs.Add(result.resultValue.ToString());
                return Diagnostics.Success;
            }

            throw new ParseException(Diagnostics.FunctionNotFound, $"Método não encontrado: std::{method}");
        }

        private string ParseExpression() => ConsumeToken().Trim('(', ')');

        private string ConsumeToken(string expected = null)
        {
            if (expected != null && CurrentToken() != expected)
                throw new ParseException(Diagnostics.SyntaxException, $"Esperado: {expected}");

            return _tokens[_position++];
        }

        private string CurrentToken() => _tokens[_position];
        private string PeekToken() => _position < _tokens.Length - 1 ? _tokens[_position + 1] : null;
        private bool IsType(string token) => Enum.TryParse<STypes>(token, out _);
    }

    public class ParseException : Exception
    {
        public Diagnostics Diagnostic { get; }
        public ParseException(Diagnostics diagnostic, string message) : base(message)
        {
            Diagnostic = diagnostic;
        }
    }

    public class CodeBlock
    {
        public int IndentLevel { get; set; }
        public bool CanExecute { get; set; }
    }
}