using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Core.Evaluator;
partial class NptEvaluator
{
    private static (Diagnostics result, string resultMessage) ApplyOperator(Stack<SType> stackValues, string Operator)
    {
        if (stackValues.Count < 2) return (Diagnostics.MissingOperands, "Unexpected end of expression.");
        var b = stackValues.Pop();
        var a = stackValues.Pop();

        if (a is NptIdentifier) return (Diagnostics.BadToken, $"At [{a.Value} {Operator} {b.Value}]: Syntax Error: left operand can't be an 'STypes.Indentifier'");
        if (b is NptIdentifier identifier)
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
                if (a is NptBool boolA && b is NptBool boolB)
                    stackValues.Push(boolA.CompareTo(boolB, Operator == "&&"));
                else
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: Expected 'STypes.Bool', got 'STypes.{a.Type}'");
                                                                    break;
            case "==":
                stackValues.Push(new NptBool(a.Value.Equals(b.Value)));
                                                                    break;
            case "~=":
                stackValues.Push(new NptBool(!a.Equals(b)));        break;
            case ">":
            case "<":
            case ">=":
            case "<=":
                if (a.Value is IComparable comparableA && b.Value is IComparable comparableB && a.Value.GetType() == b.Value.GetType()){
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
                    return (Diagnostics.TypeMismatchException, $"At [{a.Value} {Operator} {b.Value}]: 'STypes.{a.Type}' can't be compared with 'STypes.{b.Type}'");
                break;
            case "?": //contains operator
                stackValues.Push(new NptBool(a.ToString().Contains(b.ToString())));
                break;

            //other objects operators
            case "#": //random operator
                stackValues.Push(_randomGenerator.Next(0, 2) == 0 ? a : b);
                break;

            case ",": //group creator/adder
                if (a is NptGroup groupA){
                    if (b is NptGroup groupB){
                        groupA.AddRange(groupB);
                        stackValues.Push(groupA);
                    }
                    else{
                        groupA.Add(b);
                        stackValues.Push(groupA);
                    }
                }
                else{
                    var newGroup = new NptGroup([a, b]);
                    stackValues.Push(newGroup);
                    //return (Diagnostics.SyntaxException, $"At [{a.Value} {Operator} {b.Value}]: with ',' as Operator, left operand need to be 'STypes.Group' to add the left operand!");
                }
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
}