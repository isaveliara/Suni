using System.Text.RegularExpressions;
using Sun.NptEnvironment.Data;
namespace Sun.NptEnvironment.Data;

public partial class NptData
{
    public static string[] Tokenize(string expression)
    {
        string pattern = @"(s\'[^']*\'|c\'[^\']\'|\[|\]|\|\||&&|==|~=|>=|<=|>|<|!|\+\&|-\&|\+|\-|\*|\/|\s+)";
        return Regex.Split(expression, pattern)
                    .Where(token => !string.IsNullOrWhiteSpace(token))
                    .ToArray();
    }
}
