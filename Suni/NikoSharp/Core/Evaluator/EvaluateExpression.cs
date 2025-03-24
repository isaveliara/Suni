using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core.Evaluator;
partial class NikoSharpEvaluator
{
    private static readonly Random _randomGenerator = new();

    public static (SType resultValue, Diagnostics diagnostic, string diagnosticMessage) EvaluateExpression(string expression, EnvironmentDataContext context = null)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return (new NikosVoid(), Diagnostics.Success, null);
        
        if (!ValidateExpression(expression)) return (null, Diagnostics.MalformedExpression, $"At [{expression}]: Brackets are not balanced; Did you forget to open/close them correctly?");
        
        var stackValues = new Stack<SType>();
        var stackOperators = new Stack<string>();
        string[] tokens = Tokens.Tokenize(expression);

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            if (token == "[")
                stackOperators.Push(token);
            else if (token == "]")
            {
                while (stackOperators.Count > 0 && stackOperators.Peek() != "[")
                {
                    var (result, resultMessage) = ApplyOperator(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (null, result, resultMessage);
                }

                if (stackOperators.Count == 0 || stackOperators.Pop() != "[")
                    return (null, Diagnostics.MalformedExpression, $"At [{expression}]: Mismatched brackets.");
            }
            else if (IsOperator(token)){
                while (stackOperators.Count > 0 && stackOperators.Peek() != "[" && 
                    Tokens.Precedence(stackOperators.Peek()) >= Tokens.Precedence(token))
                {
                    var (result, resultMessage) = ApplyOperator(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (null, result, resultMessage);
                }
                stackOperators.Push(token);
            }
            else{
                var converted = ConvertToken(token, context);
                if (converted is NikosError error)
                    return (null, error.Diagnostic, error.Message);
                stackValues.Push(converted);
            }
        }

        while (stackOperators.Count > 0){
            var op = stackOperators.Pop();
            if (op == "[" || op == "]")
                return (null, Diagnostics.MalformedExpression, $"At [{expression}]: Unbalanced brackets.");

            var (result, resultMessage) = ApplyOperator(stackValues, op);
            if (result != Diagnostics.Success)
                return (null, result, resultMessage);
        }
        return stackValues.Count == 1
            ? (stackValues.Pop(), Diagnostics.Success, null)
            : (null, Diagnostics.MalformedExpression, $"At [{expression}]: Expression could not be fully evaluated.");
    }
}

///examples of expressions:
    ///1 + 1                       | 2 (i think is wrong.)
    ///true && false               | False
    ///'hi' :: len == 2           | True
    ///a ? 'osvald'               | True
    ///a ? 'petter'               | False
    ///'Hello Worl':: len         | HELLO WORLD
    ///'zzzzzzzzzz':: len >= 10   | True
    ///'hel' + s'lo' == 'hello'  | True
    ///'a' + 'b' ? 'oiba'       | False
    ///'a' + 'b' ? 'oiab'       | True
    ///900 # s'hi'                 | Aleatory choices '900' or 'hi'