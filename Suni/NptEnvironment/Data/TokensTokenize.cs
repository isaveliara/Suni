using System.Text.RegularExpressions;
namespace Suni.Suni.NptEnvironment.Data;

partial class Tokens
{
    public static string[] Tokenize(string expression)
    {
        string pattern = @"(s\'[^']*\'|c\'[^\']\'|\[|\]|\|\||&&|==|~=|>=|<=|>|<|!|\?|#|::|\+|\-|\*|\/|\s+)";
        return Regex.Split(expression, pattern)
                    .Where(token => !string.IsNullOrWhiteSpace(token))
                    .ToArray();
    }
}
