using System.Text.RegularExpressions;

namespace Sun.NPT.ScriptInterpreter
{
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
}