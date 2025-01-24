using System.Collections.Generic;
using Suni.Suni.NptEnvironment.Data;

namespace Suni.Suni.NptEnvironment.Core;

partial class NptStatements
{
    private static object ConvertToken(string token)
    {
        if (int.TryParse(token, out int intValue))
            return intValue;

        if (double.TryParse(token, out double doubleValue))
            return doubleValue;

        if (bool.TryParse(token, out bool boolValue))
            return boolValue;

        if (token == "nil")
            return null;

        if (token.StartsWith("s'") && token.EndsWith("'"))
            return token.Substring(2, token.Length - 3);

        if (token.StartsWith("c'") && token.EndsWith("'"))
            return token[2];

        return token;
    }

    public static (Diagnostics, object) EvaluateExpression(string expression)
    {
        if (!ValidateExpression(expression))
            return (Diagnostics.MalformedExpression, null);

        var stackValues = new Stack<object>();
        var stackOperators = new Stack<string>();

        string[] tokens = Tokens.Tokenize(expression);

        foreach (string token in tokens){
            if (token == "[")
                stackOperators.Push(token);
            else if (token == "]"){
                while (stackOperators.Count > 0 && stackOperators.Peek() != "["){
                    var result = ApplyOperatorOrFunction(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (result, null);
                }
                stackOperators.Pop(); //removes "["
            }
            else if (token.Contains('#')){
                var result = ApplyFunctionWithFormat(stackValues, token);
                if (result != Diagnostics.Success)
                    return (result, null);
            }
            else if (IsOperator(token)){
                while (stackOperators.Count > 0 && Tokens.Precedence(stackOperators.Peek()) >= Tokens.Precedence(token)){
                    var result = ApplyOperator(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (result, null);
                }
                stackOperators.Push(token);
            }
            else{
                var converted = ConvertToken(token);
                if (converted is Diagnostics)
                    return ((Diagnostics)converted, null);
                stackValues.Push(converted);
            }
        }

        while (stackOperators.Count > 0){
            var result = ApplyOperator(stackValues, stackOperators.Pop());
            if (result != Diagnostics.Success)
                return (result, null);
        }

        return stackValues.Count == 1
            ? (Diagnostics.Success, stackValues.Pop())
            //: (Diagnostics.MalformedExpression, null); //here a error goes. i commented this and now works (other things bug btw i think)
            : (Diagnostics.Success, stackValues.Pop());
    }

    private static Diagnostics ApplyOperatorOrFunction(Stack<object> stackValues, string token){
        if (token.Contains('#'))
            return ApplyFunctionWithFormat(stackValues, token);

        return ApplyOperator(stackValues, token);
    }

    private static Diagnostics ApplyOperator(Stack<object> stackValues, string Operator)
    {
        if (stackValues.Count < 2)
            return Diagnostics.IncompleteBinaryIFOperation;
        var b = stackValues.Pop();
        var a = stackValues.Pop();
        try
        {
            switch (Operator){
                case "&&":
                case "||":
                    if (a is bool boolA && b is bool boolB)
                        stackValues.Push(Operator == "&&" ? boolA && boolB : boolA || boolB);
                    else
                        return Diagnostics.TypeMismatchException;
                    break;
                case "==":
                    stackValues.Push(Equals(a, b));    break;
                case "~=":
                    stackValues.Push(!Equals(a, b));   break;
                case ">":
                case "<":
                case ">=":
                case "<=":
                    if (a is IComparable comparableA && b is IComparable comparableB && a.GetType() == b.GetType()){
                        int comparison = comparableA.CompareTo(comparableB);
                        stackValues.Push(Operator switch
                        {
                            ">" => comparison > 0,
                            "<" => comparison < 0,
                            ">=" => comparison >= 0,
                            "<=" => comparison <= 0,
                            _ => throw new InvalidOperationException("Unsupported comparison operator.")
                        });
                    }
                    else
                        return Diagnostics.TypeMismatchException;
                    break;
                case "?":
                    stackValues.Push(b.ToString().Contains(a.ToString()));   break;
                
                case "+":
                    stackValues.Push(Add(a, b));        break;
                case "-":
                    stackValues.Push(Subtract(a, b));   break;
                case "*":
                    stackValues.Push(Multiply(a, b));   break;
                case "/":
                    if (b is int intB && intB == 0)
                        return Diagnostics.DivisionByZeroException;
                    stackValues.Push(Divide(a, b));
                    break;
                default:
                    return Diagnostics.InvalidOperator;
            }
        }
        catch{
            return Diagnostics.TypeMismatchException;
        }

        return Diagnostics.Success;
    }
    private static object Add(object a, object b){
        return a switch{
            int intA when b is int intB => intA + intB,
            double doubleA when b is double doubleB => doubleA + doubleB,
            string strA => strA + b?.ToString(),
            _ => throw new InvalidOperationException("Unsupported types for addition.")
        };
    }

    private static double Subtract(object a, object b){
        return a switch{
            int intA when b is int intB => intA - intB,
            double doubleA when b is double doubleB => doubleA - doubleB,
            _ => throw new InvalidOperationException("Unsupported types for subtraction.")
        };
    }

    private static double Multiply(object a, object b){
        return a switch{
            int intA when b is int intB => intA * intB,
            double doubleA when b is double doubleB => doubleA * doubleB,
            _ => throw new InvalidOperationException("Unsupported types for multiplication.")
        };
    }

    private static double Divide(object a, object b){
        return a switch{
            int intA when b is int intB && intB != 0 => intA / intB,
            double doubleA when b is double doubleB && doubleB != 0.0 => doubleA / doubleB,
            _ => throw new InvalidOperationException("Unsupported types or division by zero.")
        };
    }

    private static Diagnostics ApplyFunctionWithFormat(Stack<object> stackValues, string token)
    {
        var parts = token.Split('#'); //len#s'hi guys' | must return '7'
        if (parts.Length != 2)
            return Diagnostics.MalformedExpression;

        string function = parts[0];
        string valueToken = token.Substring(parts[0].Length+1); //ignores 'function#'

        var value = ConvertToken(valueToken);
        if (value is Diagnostics)
            return Diagnostics.BadToken;

        return ApplyFunction(stackValues, value, function);
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

            case "uper":
                if (value is string s){
                    stackValues.Push(s.ToUpper());
                    return Diagnostics.Success;
                }
                return Diagnostics.TypeMismatchException;

            case "lower":
                if (value is string sLower){
                    stackValues.Push(sLower.ToLower());
                    return Diagnostics.Success;
                }
                return Diagnostics.TypeMismatchException;

            default:
                return Diagnostics.UnlistedProperty;
        }
    }
    internal static bool IsOperator(string token)
        =>
            new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "+", "-", "*", "/", "?" }
                .Contains(token);
}

///examples of expressions:
    ///1 + 1                       | 2 (i think is wrong.)
    ///true && false               | False
    ///len#s'hi' == 2              | True
    ///a ? s'osvald'               | True
    ///a ? s'petter'               | False
    ///upper#s'Hello Worl'         | HELLO WORLD
    ///len#s'zzzzzzzzzz' >= 10     | True
    ///s'hel' + s'lo' == s'hello'  | True
    ///s'a' + s'b' ? s'oiba'       | False
    ///s'a' + s'b' ? s'oiab'       | True
