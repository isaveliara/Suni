using System.Collections.Generic;
using System.Linq;
using static Sun.NPT.ScriptInterpreter.NptSystem;

namespace Sun.NPT.ScriptInterpreter
{
    public partial class Help
    {
        //helper method for keywords lookahead
        public static string keywordLookahead(string code, int startIndex)
        {
            int endIndex = startIndex + 1;
            while (endIndex < code.Length && char.IsLetter(code[endIndex]))
                endIndex++;
            return code.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        //helper method for get the type of something
        public static NptType GetType(string value)
        {
            //null (nil)
            if (value.Trim() == "nil")
                return new NptType(Types.Nil, null);

            //bool
            if (bool.TryParse(value, out bool boolValue))
                return new NptType(Types.Bi, boolValue);

            //int
            if (int.TryParse(value, out int intValue))
                return new NptType(Types.Int, intValue);

            //double/float
            if (float.TryParse(value, out float floatValue))
                return new NptType(Types.Flt, floatValue);

            //char
            if (value.Length == 3 && value.StartsWith("'") && value.EndsWith("'"))
                return new NptType(Types.Char, value[1]);

            //string
            if (value.StartsWith("\"") && value.EndsWith("\""))
                return new NptType(Types.Str, value.Substring(1, value.Length - 2));

            //tuple (simple)
            if (value.StartsWith("(") && value.EndsWith(")"))
            {
                var items = value.Substring(1, value.Length - 2)
                                .Split(',')
                                .Select(item => GetType(item.Trim()).Value)
                                .ToList();
                return new NptType(Types.Tuple, items);
            }

            //list (simple)
            if (value.StartsWith("[") && value.EndsWith("]"))
            {
                var items = value.Substring(1, value.Length - 2)
                                .Split(',')
                                .Select(item => GetType(item.Trim()).Value)
                                .ToList();
                return new NptType(Types.List, items);
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
                                         GetType(parts[0].Trim()).Value,
                                         GetType(parts[1].Trim()).Value
                                     );
                                 })
                                 .ToDictionary(kv => kv.Key, kv => kv.Value);

                return new NptType(Types.Dict, pairs);
            }

            //default: str
            return new NptType(Types.Str, value);
        }
    }
}