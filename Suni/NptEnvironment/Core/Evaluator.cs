using System.Collections.Generic;
using Suni.Suni.NptEnvironment.Data;
namespace Suni.Suni.NptEnvironment.Core;

partial class NptStatements
{
    private static object ConvertToken(string token)
    {
        if (int.TryParse(token, out int intValue)) return intValue;
        if (double.TryParse(token, out double doubleValue)) return doubleValue;
        if (bool.TryParse(token, out bool boolValue)) return boolValue;
        if (token == "nil") return null;
        if (token.StartsWith("s'") && token.EndsWith('\'') && token.Length >= 3) return token[2..^1];
        if (token.StartsWith("c'") && token.EndsWith('\'') && token.Length == 4) return token[2];

        return Diagnostics.BadToken;
    }

    public static (Diagnostics, object) EvaluateExpression(string expression)
    {
        if (!ValidateExpression(expression)) return (Diagnostics.MalformedExpression, null);

        var stackValues = new Stack<object>();
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
                    var result = ApplyOperatorOrFunction(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (result, null);
                }

                if (stackOperators.Count == 0 || stackOperators.Pop() != "[")
                    return (Diagnostics.MalformedExpression, null);
            }
            else if (IsFunction(token))
            {
                if (i + 1 >= tokens.Length)
                    return (Diagnostics.MalformedExpression, null);

                string nextToken = tokens[++i];
                var value = ConvertToken(nextToken);
                if (value is Diagnostics diagnostic)
                    return (diagnostic, null);

                var result = ApplyFunction(stackValues, value, token);
                if (result != Diagnostics.Success)
                    return (result, null);
            }
            else if (IsOperator(token))
            {
                while (stackOperators.Count > 0 && Tokens.Precedence(stackOperators.Peek()) >= Tokens.Precedence(token))
                {
                    var result = ApplyOperator(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (result, null);
                }
                stackOperators.Push(token);
            }
            else
            {
                var converted = ConvertToken(token);
                if (converted is Diagnostics diagnostic)
                    return (diagnostic, null);
                stackValues.Push(converted);
            }
        }

        while (stackOperators.Count > 0)
        {
            var result = ApplyOperator(stackValues, stackOperators.Pop());
            if (result != Diagnostics.Success)
                return (result, null);
        }

        return stackValues.Count == 1
            ? (Diagnostics.Success, stackValues.Pop())
            : (Diagnostics.MalformedExpression, null);
    }

    private static Diagnostics ApplyOperatorOrFunction(Stack<object> stackValues, string token)
    {
        if (IsFunction(token))
            return Diagnostics.MalformedExpression;

        return ApplyOperator(stackValues, token);
    }

    private static Diagnostics ApplyOperator(Stack<object> stackValues, string Operator)
    {
        if (stackValues.Count < 2)
            return Diagnostics.IncompleteBinaryIFOperation;

        var b = stackValues.Pop();
        var a = stackValues.Pop();

        switch (Operator)
        {
            //bool operators
            case "&&":
            case "||":
                if (a is bool boolA && b is bool boolB)
                    stackValues.Push(Operator == "&&" ? boolA && boolB : boolA || boolB);
                else
                    return Diagnostics.TypeMismatchException;
                break;
            case "==":
                stackValues.Push(Equals(a, b));   break;
            case "~=":
                stackValues.Push(!Equals(a, b));  break;
            case ">":
            case "<":
            case ">=":
            case "<=":
                if (a is IComparable comparableA && b is IComparable && a.GetType() == b.GetType()){
                    int comparison = comparableA.CompareTo(b);
                    stackValues.Push(Operator switch{
                        ">" => comparison > 0,
                        "<" => comparison < 0,
                        ">=" => comparison >= 0,
                        "<=" => comparison <= 0,
                        _ => false
                    });
                }
                else
                    return Diagnostics.TypeMismatchException;
                break;

            case "?":
                stackValues.Push(b.ToString().Contains(a.ToString()));
                break;
            
            //other objects operators
            case "+":
                if (a is string || b is string)
                    stackValues.Push(a?.ToString() + b?.ToString());
                else if (a is int intA && b is int intB)
                    stackValues.Push(intA + intB);
                else if (a is double doubleA && b is double doubleB)
                    stackValues.Push(doubleA + doubleB);
                else
                    return Diagnostics.TypeMismatchException;
                break;

            case "-":
            case "*":
            case "/":
                return ApplyArithmeticOperator(stackValues, a, b, Operator);

            default:
                return Diagnostics.InvalidOperator;
        }
        return Diagnostics.Success;
    }

    private static Diagnostics ApplyArithmeticOperator(Stack<object> stackValues, object a, object b, string op)
    {
        if (a is int intA && b is int intB)
        {
            switch (op)
            {
                case "-": stackValues.Push(intA - intB); break;
                case "*": stackValues.Push(intA * intB); break;
                case "/":
                    if (intB == 0) return Diagnostics.DivisionByZeroException;
                    stackValues.Push(intA / intB);
                    break;
            }
        }
        else if (a is double doubleA && b is double doubleB)
        {
            switch (op)
            {
                case "-": stackValues.Push(doubleA - doubleB); break;
                case "*": stackValues.Push(doubleA * doubleB); break;
                case "/":
                    if (doubleB == 0.0) return Diagnostics.DivisionByZeroException;
                    stackValues.Push(doubleA / doubleB);
                    break;
            }
        }
        else
            return Diagnostics.TypeMismatchException;
        return Diagnostics.Success;
    }

    private static Diagnostics ApplyFunction(Stack<object> stackValues, object value, string function)
    {
        switch (function){
            case "len":
                if (value is string str){
                    stackValues.Push(str.Length);
                    return Diagnostics.Success;
                }
                return Diagnostics.TypeMismatchException;

            case "upper":
                if (value is string strU){
                    stackValues.Push(strU.ToUpper());
                    return Diagnostics.Success;
                }
                return Diagnostics.TypeMismatchException;

            case "lower":
                if (value is string strL){
                    stackValues.Push(strL.ToLower());
                    return Diagnostics.Success;
                }
                return Diagnostics.TypeMismatchException;

            default:
                return Diagnostics.UnlistedProperty;
        }
    }

    private static bool IsFunction(string token)
        => new HashSet<string> { "len", "upper", "lower" }.Contains(token);

    internal static bool IsOperator(string token) =>
        new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "+", "-", "*", "/", "?" }
            .Contains(token);
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
