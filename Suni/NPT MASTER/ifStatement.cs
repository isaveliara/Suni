using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sun.NPT.ScriptInterpreter
{
    class NptStatements
    {
        public static (Diagnostics, bool) EvaluateIFExpression(string expression)
        {
            if (!ValidateExpression(expression))
                return (Diagnostics.MalformedIFExpression, false);

            Stack<object> stackValues = new Stack<object>();
            Stack<string> stackOperators = new Stack<string>();

            string[] tokens = Tokenize(expression);

            foreach (string token in tokens)
            {
                if (int.TryParse(token, out int number))
                    stackValues.Push(number);
                else if (bool.TryParse(token, out bool boolean))
                    stackValues.Push(boolean);
                else if (token.StartsWith("\"") && token.EndsWith("\""))
                    stackValues.Push(token.Trim('"'));
                else if (token == "[")
                    stackOperators.Push(token);
                else if (token == "]")
                {
                    while (stackOperators.Count > 0 && stackOperators.Peek() != "[")
                    {
                        var error = ApplyOperator(stackValues, stackOperators.Pop());
                        if (error != Diagnostics.Success)
                            return (error, false);
                    }
                    stackOperators.Pop(); //remove the [
                }
                else if (IsIFOperator(token))
                {
                    while (stackOperators.Count > 0 && Precedence(stackOperators.Peek()) >= Precedence(token))
                    {
                        var error = ApplyOperator(stackValues, stackOperators.Pop());
                        if (error != Diagnostics.Success)
                            return (error, false);
                    }
                    stackOperators.Push(token);
                }
            }

            while (stackOperators.Count > 0)
            {
                var error = ApplyOperator(stackValues, stackOperators.Pop());
                if (error != Diagnostics.Success)
                    return (error, false);
            }

            return (Diagnostics.Success, Convert.ToBoolean(stackValues.Pop()));
        }

        private static Diagnostics ApplyOperator(Stack<object> stackValues, string Operator)
        {
            if (stackValues.Count == 0)
                return Diagnostics.MissingOperandsForIFOperator;

            if (Operator == "!")
            {
                bool valor = Convert.ToBoolean(stackValues.Pop());
                stackValues.Push(!valor);
            }
            else
            {
                if (stackValues.Count < 2)
                    return Diagnostics.IncompleteBinaryIFOperation;

                object b = stackValues.Pop();
                object a = stackValues.Pop();

                switch (Operator)
                {
                    case "&&":
                        stackValues.Push(Convert.ToBoolean(a) && Convert.ToBoolean(b));
                        break;
                    case "||":
                        stackValues.Push(Convert.ToBoolean(a) || Convert.ToBoolean(b));
                        break;
                    case "==":
                        stackValues.Push(a.Equals(b));
                        break;
                    case "!=":
                        stackValues.Push(!a.Equals(b));
                        break;
                    case ">":
                        stackValues.Push(Convert.ToInt32(a) > Convert.ToInt32(b));
                        break;
                    case "<":
                        stackValues.Push(Convert.ToInt32(a) < Convert.ToInt32(b));
                        break;
                    case ">=":
                        stackValues.Push(Convert.ToInt32(a) >= Convert.ToInt32(b));
                        break;
                    case "<=":
                        stackValues.Push(Convert.ToInt32(a) <= Convert.ToInt32(b));
                        break;
                    case "+":
                        stackValues.Push(Convert.ToInt32(a) + Convert.ToInt32(b));
                        break;
                    case "-":
                        stackValues.Push(Convert.ToInt32(a) - Convert.ToInt32(b));
                        break;
                    case "*":
                        stackValues.Push(Convert.ToInt32(a) * Convert.ToInt32(b));
                        break;
                    case "/":
                        if (Convert.ToInt32(b) == 0)
                            return Diagnostics.DivisionByZeroException;
                        stackValues.Push(Convert.ToInt32(a) / Convert.ToInt32(b));
                        break;
                }
            }

            return Diagnostics.Success;
        }

        private static string[] Tokenize(string expression)
        {
            string pattern = @"(\[|\]|\|\||&&|==|!=|>=|<=|>|<|!|\+|\-|\*|\/|\s+)";
            return Regex.Split(expression, pattern)
                        .Where(token => !string.IsNullOrWhiteSpace(token))
                        .ToArray();
        }

        private static int Precedence(string Operator)
        {
            return Operator switch
            {
                "[" => 4,
                "!" => 3,
                "*" or "/" => 2,
                "+" or "-" => 1,
                "&&" => 0,
                "||" => -1,
                _ => -2
            };
        }

        private static bool IsIFOperator(string token)
        {
            return new HashSet<string> { "&&", "||", "!", "==", "!=", ">", "<", ">=", "<=", "+", "-", "*", "/" }
                .Contains(token);
        }

        private static bool ValidateExpression(string expression)
        {
            int balancingBrackets = 0;

            foreach (char c in expression)
            {
                if (c == '[') balancingBrackets++;
                if (c == ']') balancingBrackets--;

                if (balancingBrackets < 0)
                    return false;
            }

            return balancingBrackets == 0;
        }
    }
}
