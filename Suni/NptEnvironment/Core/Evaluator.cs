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
        
        var identifierVal = new NptIdentifier(token);
        return identifierVal.Value is not null
            ? identifierVal
            : new NptError(Diagnostics.BadToken, $"'{token}' is not a valid Token.");
    }

    public static (SType resultValue, Diagnostics diagnostic, string diagnosticMessage) EvaluateExpression(string expression, EnvironmentDataContext context = null)
    {
        if (!ValidateExpression(expression)) return (null, Diagnostics.MalformedExpression, $"At [{expression}]");
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
                    return (null, Diagnostics.MalformedExpression, $"At [{expression}]");
            }
            else if (IsOperator(token)){
                while (stackOperators.Count > 0 && Tokens.Precedence(stackOperators.Peek()) >= Tokens.Precedence(token)){
                    var (result, resultMessage) = ApplyOperator(stackValues, stackOperators.Pop());
                    if (result != Diagnostics.Success)
                        return (null, result, resultMessage);
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
            var (result, resultMessage) = ApplyOperator(stackValues, stackOperators.Pop());
            if (result != Diagnostics.Success)
                return (null, result, resultMessage);
        }
        return stackValues.Count == 1
            ? (stackValues.Pop(), Diagnostics.Success, null)
            : (null, Diagnostics.MalformedExpression, $"At [{expression}]");
    }

    private static (Diagnostics result, string resultMessage) ApplyOperator(Stack<SType> stackValues, string Operator)
    {
        if (stackValues.Count < 2) return (Diagnostics.MissingOperands, "Unexpected end of expression.");
        var b = stackValues.Pop();
        var a = stackValues.Pop();
        
        switch (Operator){

            //boolean operators
            case "&&":
            case "||":
                if (a is NptBool boolA && b is NptBool boolB)
                    stackValues.Push(boolA.CompareTo(boolB, Operator == "&&"));
                else
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: Expected 'STypes.Bool', got 'STypes.{a.Type}'");
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
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: 'STypes.{a.Type}' cannot be compared with 'STypes.{b.Type}'");
                break;
            case "?": //contains operator
                stackValues.Push(new NptBool(a.ToString().Contains(b.ToString())));
                break;

            //other objects operators
            case "#": //random operator
                stackValues.Push(_randomGenerator.Next(0, 2) == 0 ? a : b);
                break;
            case "::": //property access operator
                if (b is NptIdentifier property)
                    stackValues.Push(AccessProperty(a, property.ToString()));
                else
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: Expected <Identifier> Token, got '{b.Value}' value.");
                break;
            
            //arithmetic operators
            case "+":
                if (a is NptStr strAddA && b is NptStr strAddB)
                    stackValues.Push(strAddA.Add(strAddB));
                else if (a is NptInt intAddA && b is NptInt intAddB)
                    stackValues.Push(intAddA.Add(intAddB));
                else if (a is NptFloat floatAddA && b is NptFloat floatAddB)
                    stackValues.Push(floatAddA.Add(floatAddB));
                else
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: 'STypes.{a.Type}' cannot be added with 'STypes.{b.Type}'");
                break;
            case "-":
            case "*":
            case "/":
                return ApplyArithmeticOperator(stackValues, a, b, Operator);

            default:
                return (Diagnostics.InvalidOperator, $"At [{a.Value} {Operator} {b.Value}]: '{Operator}' isin't a valid Operator in SuniNpt.");
        }
        return (Diagnostics.Success, null);
    }

    private static (Diagnostics result, string resultMessage) ApplyArithmeticOperator(Stack<SType> stackValues, SType a, SType b, string op)
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
                        return (division.diagnostic, $"At [{a.Value} {op} {b.Value}]: division by zero.");
                    
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
                    if (division.diagnostic != Diagnostics.Success) return (division.diagnostic, $"At [{a.Value} {op} {b.Value}]: division by zero.");
                    stackValues.Push(division.resultVal);           break;
            }
        }
        else
            return (Diagnostics.TypeMismatchException, $"At [{a.Value} {op} {b.Value}]: 'STypes.{a.Type}' cannot be calculated with 'STypes.{b.Type}'");
        return (Diagnostics.Success, null);
    }

    internal static bool IsOperator(string token) =>
        new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "#", "+", "-", "*", "/", "?", "::" }
            .Contains(token);
    
    private static SType AccessProperty(SType target, string property)
    {
        var type = target.GetType();
        //searches for methods marked with the 'ExposedProperty' attribute.
        var methods = type.GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(ExposedPropertyAttribute), false)
                        .OfType<ExposedPropertyAttribute>()
                        .Any(attr => attr.Name == property))
            .ToList();

        if (methods.Count > 0){
            var method = methods[0];
            return (SType)method.Invoke(target, null);
        }

        //searches for properties marked with the 'ExposedProperty' attribute.
        var properties = type.GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(ExposedPropertyAttribute), false)
                        .OfType<ExposedPropertyAttribute>()
                        .Any(attr => attr.Name == property))
            .ToList();

        if (properties.Count > 0){
            var propertyInfo = properties[0];
            return (SType)propertyInfo.GetValue(target);
        }

        return new NptError(Diagnostics.UnlistedProperty, $"Property '{property}' not found for type '{target.Type}'");
    }
}

///examples of expressions:
    ///1 + 1                       | 2 (i think is wrong.)
    ///true && false               | False
    ///s'hi' :: len == 2           | True
    ///a ? s'osvald'               | True
    ///a ? s'petter'               | False
    ///s'Hello Worl':: len         | HELLO WORLD
    ///s'zzzzzzzzzz':: len >= 10   | True
    ///s'hel' + s'lo' == s'hello'  | True
    ///s'a' + s'b' ? s'oiba'       | False
    ///s'a' + s'b' ? s'oiab'       | True
    ///900 # s'hi'                 | Aleatory choices '900' or 'hi'