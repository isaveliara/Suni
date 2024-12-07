using System.Collections.Generic;
using System.Linq;

namespace Sun.NPT.ScriptInterpreter
{
    public partial class NptSystem
    {
        public enum Types
        {
            Nil, Bool,
            Int, Float, Tuple,
            Str, Fn, Char,
            List, Dict,
        }

        public class NptType
        {
            public Types Type { get; }
            public object Value { get; }

            public NptType(Types type, object value)
            {
                Type = type;
                Value = value;
            }

            public override string ToString()
            {
                return Type switch
                {
                    Types.Nil => "nil",
                    Types.Bool => $"{Value}",
                    Types.Int => Value.ToString(),
                    Types.Float => Value.ToString(),
                    Types.Str => $"{Value}",
                    Types.Char => $"{Value}",
                    Types.Fn => "<function>", //xd, ignore (TODO)
                    Types.Tuple => $"{Value}",
                    Types.List => $"{string.Join(", ", (Value as List<object>) ?? new List<object>())}",
                    Types.Dict => $"{string.Join(", ", (Value as Dictionary<object, object>)?.Select(kv => $"{kv.Key}: {kv.Value}") ?? new string[0])}",
                    _ => Value?.ToString() ?? "unknown"
                };
            }
        }
    }
}
