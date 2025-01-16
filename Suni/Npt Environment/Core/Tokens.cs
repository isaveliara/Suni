using System.Text.RegularExpressions;

namespace Sun.NPT.ScriptInterpreter;
partial class NptStatements
{
    internal static int Precedence(string Operator)
        {
            return Operator switch
            {
                "[" => 4,
                "!" => 3,
                "*" or "/" => 2,
                "+" or "-" or "-&" or "+&" => 1,
                "&&" => 0,
                "||" => -1,
                _ => -2
            };
        }
}

partial class NptStatements
{
    internal static string[] Tokenize(string expression)
    {
        string pattern = @"(s\'[^']*\'|c\'[^\']\'|\[|\]|\|\||&&|==|~=|>=|<=|>|<|!|\+\&|-\&|\+|\-|\*|\/|\s+)";
        return Regex.Split(expression, pattern)
                    .Where(token => !string.IsNullOrWhiteSpace(token))
                    .ToArray();
    }
}
