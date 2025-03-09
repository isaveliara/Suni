using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core.Evaluator;
partial class NikoSharpEvaluator
{
    private static (Diagnostics result, string resultMessage) ApplyArithmeticOperator(Stack<SType> stackValues, SType a, SType b, string op)
    {
        if (a is NikosInt intA && b is NikosInt intB){
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
        else if (a is NikosFloat floatA && b is NikosFloat floatB){
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