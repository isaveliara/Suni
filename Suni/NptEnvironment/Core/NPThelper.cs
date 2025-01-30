using System.Text.RegularExpressions;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;

namespace Suni.Suni.NptEnvironment.Core;

public partial class Help
{
    //helper method for keywords lookahead
    public static (string Letters, string Chars) keywordLookahead(string code, int startIndex)
    {
        int currentIndex = startIndex + 1;
        int firstNonLetterIndex = currentIndex;

        while (currentIndex < code.Length && code[currentIndex] != ' ' && code[currentIndex] != '\n')
        {
            if (char.IsLetter(code[currentIndex]) && firstNonLetterIndex == currentIndex)
                firstNonLetterIndex++;
            currentIndex++;
        }

        string letters = code.Substring(startIndex + 1, firstNonLetterIndex - startIndex - 1);
        string chars = code.Substring(startIndex + 1, currentIndex - startIndex - 1);

        return (letters, chars);
    }

    /// <summary>
    /// helper method for get the type of something
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static (Diagnostics, SType) GetType(string value)
    {
        //value = value.Trim();

        //null (nil)
        if (value == "nil")
            return (Diagnostics.Success, new NptNil());

        //bool
        if (value == "true" || value == "false")
            return bool.TryParse(value, out bool boolVal)
            ? (Diagnostics.Success, new NptBool(boolVal))
            : (Diagnostics.CannotConvertType, null);

        //int
        if (int.TryParse(value, out int intVal))
            return (Diagnostics.Success, new NptInt(intVal));

        //double/float
        if (float.TryParse(value, out float floatVal))
            return (Diagnostics.Success, new NptFloat(floatVal));

        //char
        if (value.StartsWith("c'") && value.EndsWith("'"))
        {
            if (value.Length != 4)
                return (Diagnostics.OutOfRangeException, null);
            return (Diagnostics.Success, new NptChar(value[2]));
        }

        //string
        if (value.StartsWith("s'") && value.EndsWith("'"))
            return (Diagnostics.Success, new NptStr(value[2..^1]));

        /*//list (simple)
        if (value.StartsWith('{') && value.EndsWith('}'))
        {
            var items = value[1..^1]
                            .Split(',')
                            .Select(item => GetType(item.Trim(' ', '\'')).Item2.Value)
                            .ToList();
            return (Diagnostics.Success, new NptList(items));
        }

        //dict (simple)
        if (value.StartsWith("{") && value.EndsWith("}"))
        {
            var pairs = value.Substring(1, value.Length - 2)
                            .Split(',')
                            .Select(pair =>
                            {
                                var parts = pair.Split(':');
                                return new KeyValuePair<object, object>(
                                    GetType(parts[0].Trim(' ', '\'')).Item2.Value,
                                    GetType(parts[1].Trim(' ', '\'')).Item2.Value
                                );
                            })
                            .ToDictionary(kv => kv.Key, kv => kv.Value);

            return (Diagnostics.Success, new NptTypes.NptType(NptTypes.Types.Dict, pairs));
        }*/

        //function (e.g., [name<params>] "code")
        var fnMatch = Regex.Match(value, @"^\[(\w+)<([^>]*)>\]\s*""(.*)""$");
        if (fnMatch.Success)
        {
            string fnName = fnMatch.Groups[1].Value;
            var parameters = fnMatch.Groups[2].Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(param => param.Trim())
                .ToList();
            string fnBody = fnMatch.Groups[3].Value;
            return (Diagnostics.Success, new NptFunction(fnName, parameters, fnBody));
        }

        //convert a possible evaluated literal to a string (idk if it should be treated differently as this)
        return (Diagnostics.Anomaly, new NptStr(value));
    }
}
