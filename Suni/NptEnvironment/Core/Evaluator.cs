using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Core;

partial class NptSystem
{
    private static readonly Random _randomGenerator = new();

    private static SType ConvertToken(string token, EnvironmentDataContext context = null)
    {
        if (int.TryParse(token, out int intValue)) return new NptInt(intValue);
        if (double.TryParse(token, out double doubleValue)) return new NptFloat(doubleValue);
        if (bool.TryParse(token, out bool boolValue)) return new NptBool(boolValue);
        if (token == "nil") return new NptNil();
        if (token.StartsWith("s'") && token.EndsWith('\'') && token.Length >= 3) return new NptStr(token[2..^1]);
        if (token.StartsWith("c'") && token.EndsWith('\'') && token.Length == 4) return new NptChar(token[2]);

        if (token.Contains("::"))
        {
            var parts = token.Split("::");
            if (parts.Length != 2)
                return new NptError(Diagnostics.BadToken, $"Invalid property access: {token}");
            var variable = parts[0];
            var property = parts[1];
            var variableValue = ConvertToken(variable, context);
            if (variableValue is NptError){
                if (context == null)
                    return new NptError(Diagnostics.VariableNotFound, $"Variable '{variable}' not found and no context provided.");
                variableValue = GetVariableValue(variable, context);
                if (variableValue == null)
                    return new NptError(Diagnostics.VariableNotFound, $"Variable '{variable}' not found.");
            }
            return AccessProperty(variableValue, property);
        }
        if (context != null){
            var variableValue = GetVariableValue(token, context);
            if (variableValue != null)
                return variableValue;
        }
        return new NptError(Diagnostics.VariableNotFound, $"Token '{token}' is not a valid value or variable.");
    }

    public static (SType resultValue, Diagnostics diagnostic, string diagnosticMessage) EvaluateExpression(string expression, EnvironmentDataContext context = null)
    {
        if (!ValidateExpression(expression)) return (null, Diagnostics.MalformedExpression, $"[{expression}]");
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
                    var result = ApplyOperator(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (null, result, "invalid");
                }

                if (stackOperators.Count == 0 || stackOperators.Pop() != "[")
                    return (null, Diagnostics.MalformedExpression, "invalid expression.");
            }
            else if (IsOperator(token)){
                while (stackOperators.Count > 0 && Tokens.Precedence(stackOperators.Peek()) >= Tokens.Precedence(token)){
                    var result = ApplyOperator(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (null, result, "invalid expression");
                }
                stackOperators.Push(token);
            }
            else{
                var converted = ConvertToken(token, context);
                if (converted is NptError error)
                    return (null, error.Diagnostic, error.Message);
                stackValues.Push(converted);
            }
        }

        while (stackOperators.Count > 0){
            var result = ApplyOperator(stackValues, stackOperators.Pop());
            if (result != Diagnostics.Success)
                return (null, result, "invalid expression");
        }
        return stackValues.Count == 1
            ? (stackValues.Pop(), Diagnostics.Success, null)
            : (null, Diagnostics.MalformedExpression, "invalid expression");
    }

    private static Diagnostics ApplyOperator(Stack<SType> stackValues, string Operator)
    {
        if (stackValues.Count < 2) return Diagnostics.IncompleteBinaryIFOperation;
        var b = stackValues.Pop();
        var a = stackValues.Pop();
        switch (Operator){
            //boolean operators
            case "&&":
            case "||":
                if (a is NptBool boolA && b is NptBool boolB)
                    stackValues.Push(boolA.CompareTo(boolB, Operator == "&&"));
                else
                    return Diagnostics.TypeMismatchException;
                                                                    break;
            case "==":
                stackValues.Push(new NptBool(a.Equals(b)));
                                                                    break;
            case "~=":
                stackValues.Push(new NptBool(!a.Equals(b)));        break;
            case ">":
            case "<":
            case ">=":
            case "<=":
                if (a is IComparable comparableA && b is IComparable comparableB && a.GetType() == b.GetType()){
                    int comparison = comparableA.CompareTo(comparableB);
                    stackValues.Push(new NptBool(Operator switch
                    {
                        ">" => comparison > 0,
                        "<" => comparison < 0,
                        ">=" => comparison >= 0,
                        "<=" => comparison <= 0,
                        _ => false
                    }));
                }
                else
                    return Diagnostics.TypeMismatchException;       break;
            case "?":
                stackValues.Push(new NptBool(a.ToString().Contains(b.ToString())));
                                                                    break;

            //other objects operators
            case "#":
                stackValues.Push(_randomGenerator.Next(0, 2) == 0 ? a : b);
                                                                    break;
            case "+":
                if (a is NptStr strAddA && b is NptStr strAddB)
                    stackValues.Push(strAddA.Add(strAddB));
                else if (a is NptInt intAddA && b is NptInt intAddB)
                    stackValues.Push(intAddA.Add(intAddB));
                else if (a is NptFloat floatAddA && b is NptFloat floatAddB)
                    stackValues.Push(floatAddA.Add(floatAddB));
                else
                    return Diagnostics.TypeMismatchException;       break;
            case "-":
            case "*":
            case "/":
                return ApplyArithmeticOperator(stackValues, a, b, Operator);

            default:
                return Diagnostics.InvalidOperator;
        }
        return Diagnostics.Success;
    }

    private static Diagnostics ApplyArithmeticOperator(Stack<SType> stackValues, SType a, SType b, string op)
    {
        if (a is NptInt intA && b is NptInt intB){
            switch (op){
                case "+":
                    stackValues.Push(intA.Add(intB));               break;
                case "-":
                    stackValues.Push(intA.Subtract(intB));          break;
                case "*":
                    stackValues.Push(intA.Multiply(intB));          break;
                case "/":
                    var division = intA.Divide(intB);
                    if (division.diagnostic != Diagnostics.Success)
                                                                    return division.diagnostic;
                    
                    stackValues.Push(division.resultVal);           break;
            }
        }
        else if (a is NptFloat floatA && b is NptFloat floatB){
            switch (op){
                case "+":
                    stackValues.Push(floatA.Add(floatB));           break;
                case "-":
                    stackValues.Push(floatA.Subtract(floatB));      break;
                case "*":
                    stackValues.Push(floatA.Multiply(floatB));      break;
                case "/":
                    var division = floatA.Divide(floatB);
                    if (division.diagnostic != Diagnostics.Success) return division.diagnostic;
                    stackValues.Push(division.resultVal);           break;
            }
        }
        else
            return Diagnostics.TypeMismatchException;
        return Diagnostics.Success;
    }

    internal static bool IsOperator(string token) =>
        new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "#", "+", "-", "*", "/", "?", "::" }
            .Contains(token);
    
    private static SType AccessProperty(SType target, string property)
    {
        switch (target.Type){
            case STypes.Str:
                var strVal = (NptStr)target;
                return property switch{
                    "len" => new NptInt(strVal.ToString().Length),
                    "upper" => strVal.ToUpper(),
                    "lower" => strVal.ToLower(),
                    _ => new NptError(Diagnostics.UnlistedProperty, $"Property '{property}' not found for type 'Str'.")
                };
            case STypes.Int:
                var intVal = (NptInt)target;
                return property switch{
                    "len" => new NptInt(intVal.ToString().Length),
                    _ => new NptError(Diagnostics.UnlistedProperty, $"Property '{property}' not found for type 'Int'."),
                };
            
            default:
                return new NptError(Diagnostics.UnlistedProperty, $"Property access not supported for type '{target.Type}'.");
        }
    }
    private static SType GetVariableValue(string variableName, EnvironmentDataContext context){
        foreach (var scope in context.Variables)
            if (scope.ContainsKey(variableName))
                return scope[variableName];
        return null;
    }
}

///examples of expressions:
    ///1 + 1                       | 2 (i think is wrong.)
    ///true && false               | False
    ///len s'hi' == 2              | True
    ///a ? s'osvald'               | True
    ///a ? s'petter'               | False
    ///upper s'Hello Worl'         | HELLO WORLD
    ///len s'zzzzzzzzzz' >= 10     | True
    ///s'hel' + s'lo' == s'hello'  | True
    ///s'a' + s'b' ? s'oiba'       | False
    ///s'a' + s'b' ? s'oiab'       | True
