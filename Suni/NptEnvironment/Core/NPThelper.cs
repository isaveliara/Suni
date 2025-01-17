using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Sun.NPT.ScriptInterpreter.NptSystem;

namespace Sun.NPT.ScriptInterpreter
{
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

        //helper method for get the type of something
        public static (Diagnostics, NptType) GetType(string value)
        {
            value = value.Trim();

            //null (nil)
            if (value == "nil")
                return (Diagnostics.Success, new NptType(Types.Nil, null));

            //bool
            if (value == "true" || value == "false")
                return (Diagnostics.Success, new NptType(Types.Bool, value));

            //int
            if (int.TryParse(value, out int intValue))
                return (Diagnostics.Success, new NptType(Types.Int, intValue));

            //double/float
            if (float.TryParse(value, out float floatValue))
                return (Diagnostics.Success, new NptType(Types.Float, floatValue));

            //char
            if (value.StartsWith("c'") && value.EndsWith("'"))
            {
                if (value.Length != 4)
                    return (Diagnostics.OutOfRangeException, new NptType(Types.Nil, null));
                return (Diagnostics.Success, new NptType(Types.Char, value[2]));
            }

            //string
            if (value.StartsWith("s'") && value.EndsWith("'"))
                return (Diagnostics.Success, new NptType(Types.Str, value.Substring(2, value.Length - 3)));

            //tuple (simple)
            if (value.StartsWith("(") && value.EndsWith(")"))
            {
                var items = value.Substring(1, value.Length - 2)
                                .Split(',')
                                .Select(item => GetType(item.Trim(' ', '\'')).Item2.Value)
                                .ToList();
                return (Diagnostics.Success, new NptType(Types.Tuple, items));
            }

            //list (simple)
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                var items = value.Substring(1, value.Length - 2)
                                .Split(',')
                                .Select(item => GetType(item.Trim(' ', '\'')).Item2.Value)
                                .ToList();
                return (Diagnostics.Success, new NptType(Types.List, items));
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

                return (Diagnostics.Success, new NptType(Types.Dict, pairs));
            }

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

                var function = new NptFunction(fnName, parameters, fnBody);
                return (Diagnostics.Success, new NptType(Types.Fn, function));
            }

            //unknown
            return (Diagnostics.UnknowTypeException, null);
        }
    }
}