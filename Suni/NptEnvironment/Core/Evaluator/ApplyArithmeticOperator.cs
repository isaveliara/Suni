using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;
namespace Suni.Suni.NptEnvironment.Core.Evaluator;
partial class NptEvaluator
{
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
            return (Diagnostics.TypeMismatchException, $"At [{a.Value} {op} {b.Value}]: 'STypes.{a.Type}' can't be calculated with 'STypes.{b.Type}'");
        return (Diagnostics.Success, null);
    }
}