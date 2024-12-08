using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sun.NPT.ScriptInterpreter
{
    partial class NptStatements
    {
        private static object ConvertToken(string token)
        {
            if (int.TryParse(token, out int intValue))
                return intValue;
            if (bool.TryParse(token, out bool boolValue))
                return boolValue;
            if (token.StartsWith("s'") && token.EndsWith("'"))
                return token.Substring(2, token.Length - 3);
            if (token.StartsWith("c'") && token.EndsWith("'"))
                return token[2];
            return token; //returns the original token for unknown types
        }
        public static (Diagnostics, bool) EvaluateExpression(string expression)
        {
            if (!ValidateExpression(expression))
                return (Diagnostics.MalformedIFExpression, false);

            Stack<object> stackValues = new Stack<object>();
            Stack<string> stackOperators = new Stack<string>();

            string[] tokens = Tokenize(expression);

            foreach (string token in tokens)
            {
                if (token == "[")
                    stackOperators.Push(token);
                else if (token == "]")
                {
                    while (stackOperators.Count > 0 && stackOperators.Peek() != "[")
                    {
                        var error = ApplyOperator2(stackValues, stackOperators.Pop());
                        if (error != Diagnostics.Success)
                            return (error, false);
                    }
                    stackOperators.Pop(); //remove [
                }
                else if (IsOperator(token))
                {
                    while (stackOperators.Count > 0 && Precedence(stackOperators.Peek()) >= Precedence(token))
                    {
                        var error = ApplyOperator2(stackValues, stackOperators.Pop());
                        if (error != Diagnostics.Success)
                            return (error, false);
                    }
                    stackOperators.Push(token);
                }
                else
                    stackValues.Push(ConvertToken(token));
            }

            while (stackOperators.Count > 0)
            {
                var error = ApplyOperator2(stackValues, stackOperators.Pop());
                if (error != Diagnostics.Success)
                    return (error, false);
            }

            return (Diagnostics.Success, Convert.ToBoolean(stackValues.Pop()));
        }

        private static Diagnostics ApplyOperator2(Stack<object> stackValues, string Operator)
        {
            if (stackValues.Count == 0)
                return Diagnostics.MissingOperandsForEvaluation;

            if (Operator == "!")
            {
                if (stackValues.Peek() is bool)
                {
                    bool value = (bool)stackValues.Pop();
                    stackValues.Push(!value);
                }
                else
                    return Diagnostics.TypeMismatchException;
            }
            else
            {
                if (stackValues.Count < 2)
                    return Diagnostics.IncompleteBinaryIFOperation;

                object b = stackValues.Pop();
                object a = stackValues.Pop();

                try
                {
                    switch (Operator)
                    {
                        case "&&":
                        case "||":
                            if (a is bool boolA && b is bool boolB)
                                stackValues.Push(Operator == "&&" ? boolA && boolB : boolA || boolB);
                            else
                                return Diagnostics.TypeMismatchException;
                            break;

                        case "==":
                            stackValues.Push(Equals(a, b));
                            break;

                        case "~=":
                            stackValues.Push(!Equals(a, b));
                            break;

                        case ">":
                        case "<":
                        case ">=":
                        case "<=":
                            if (a is IComparable comparableA && b is IComparable comparableB && a.GetType() == b.GetType())
                            {
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

                        case "+":
                        case "-":
                        case "*":
                        case "/":
                            if (a is int intA && b is int intB)
                            {
                                stackValues.Push(Operator switch
                                {
                                    "+" => intA + intB,
                                    "-" => intA - intB,
                                    "*" => intA * intB,
                                    "/" => intB != 0 ? intA / intB : throw new DivideByZeroException(),
                                    _ => throw new InvalidOperationException("Unsupported arithmetic operator.")
                                });
                            }
                            else
                                return Diagnostics.TypeMismatchException;
                            break;

                        case "+&":
                            stackValues.Push($"{a}{b}");
                            break;

                        case "-&":
                            if (a is string strA && b is int countB)
                                stackValues.Push(strA.Substring(0, Math.Max(0, strA.Length - countB)));
                            else
                                return Diagnostics.TypeMismatchException;
                            break;

                        default:
                            return Diagnostics.InvalidOperator;
                    }
                }
                catch (Exception)
                {
                    return Diagnostics.TypeMismatchException;
                }
            }

            return Diagnostics.Success;
        }

        private static bool IsOperator(string token)
        {
            return new HashSet<string> { "&&", "||", "!", "==", "~=", ">", "<", ">=", "<=", "+", "-", "*", "/", "-&", "+&" }
                .Contains(token);
        }
    }
}