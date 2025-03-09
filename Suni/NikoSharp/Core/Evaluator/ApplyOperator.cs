using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core.Evaluator;
partial class NikoSharpEvaluator
{
    private static (Diagnostics result, string resultMessage) ApplyOperator(Stack<SType> stackValues, string Operator)
    {
        if (stackValues.Count < 2) return (Diagnostics.MissingOperands, "Unexpected end of expression.");
        var b = stackValues.Pop();
        var a = stackValues.Pop();

        if (a is NikosIdentifier) return (Diagnostics.BadToken, $"At [{a.Value} {Operator} {b.Value}]: Syntax Error: left operand can't be an 'STypes.Indentifier'");
        if (b is NikosIdentifier identifier)
        {
            if (Operator == "::") //access property of
            {
                stackValues.Push(AccessProperty(a, identifier.ToString()));
                return (Diagnostics.Success, null);
            }
            else
                return (Diagnostics.SyntaxException, $"At [{a.Value} {Operator} {b.Value}]");
        }
            
        switch (Operator){

            //boolean operators
            case "&&":
            case "||":
                if (a is NikosBool boolA && b is NikosBool boolB)
                    stackValues.Push(boolA.CompareTo(boolB, Operator == "&&"));
                else
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: Expected 'STypes.Bool', got 'STypes.{a.Type}'");
                                                                    break;
            case "==":
                stackValues.Push(new NikosBool(a.Value.Equals(b.Value)));
                                                                    break;
            case "~=":
                stackValues.Push(new NikosBool(!a.Equals(b)));        break;
            case ">":
            case "<":
            case ">=":
            case "<=":
                if (a.Value is IComparable comparableA && b.Value is IComparable comparableB && a.Value.GetType() == b.Value.GetType()){
                    int comparison = comparableA.CompareTo(comparableB);
                    stackValues.Push(new NikosBool(Operator switch
                    {
                        ">" => comparison > 0,
                        "<" => comparison < 0,
                        ">=" => comparison >= 0,
                        "<=" => comparison <= 0,
                        _ => false
                    }));
                }
                else
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: 'STypes.{a.Type}' can't be compared with 'STypes.{b.Type}'");
                break;
            case "?": //contains operator
                stackValues.Push(new NikosBool(a.ToString().Contains(b.ToString())));
                break;

            //other objects operators
            case "#": //random operator
                stackValues.Push(_randomGenerator.Next(0, 2) == 0 ? a : b);
                break;

            case ",": //group creator/adder
                if (a is NikosGroup groupA){
                    if (b is NikosGroup groupB){
                        groupA.AddRange(groupB);
                        stackValues.Push(groupA);
                    }
                    else{
                        groupA.Add(b);
                        stackValues.Push(groupA);
                    }
                }
                else{
                    var newGroup = new NikosGroup([a, b]);
                    stackValues.Push(newGroup);
                    //return (Diagnostics.SyntaxException, $"At [{a.Value} {Operator} {b.Value}]: with ',' as Operator, left operand need to be 'STypes.Group' to add the left operand!");
                }
                break;

            //arithmetic operators
            case "+":
                if (a is NikosStr strAddA && b is NikosStr strAddB)
                    stackValues.Push(strAddA.Add(strAddB));
                else if (a is NikosInt intAddA && b is NikosInt intAddB)
                    stackValues.Push(intAddA.Add(intAddB));
                else if (a is NikosFloat floatAddA && b is NikosFloat floatAddB)
                    stackValues.Push(floatAddA.Add(floatAddB));
                else
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: 'STypes.{a.Type}' cannot be added with 'STypes.{b.Type}'");
                break;
            case "-":
            case "*":
            case "/":
                return ApplyArithmeticOperator(stackValues, a, b, Operator);

            default:
                return (Diagnostics.InvalidOperator, $"At [{a.Value} {Operator} {b.Value}]: '{Operator}' isin't a valid Operator in NikoSharp.");
        }
        return (Diagnostics.Success, null);
    }
}