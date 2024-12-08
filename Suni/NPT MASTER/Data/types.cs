using System.Collections.Generic;
using System.Globalization;
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
                    Types.Float => $"{Value:0.#}",
                    Types.Str => $"s'{Value}'",
                    Types.Char => $"c'{Value}'",
                    Types.Fn => "<function>",
                    Types.Tuple => $"{Value}",
                    Types.List => $"{string.Join(", ", (Value as List<object>) ?? new List<object>())}",
                    Types.Dict => $"{string.Join(", ", (Value as Dictionary<object, object>)?.Select(kv => $"{kv.Key}: {kv.Value}") ?? new string[0])}",
                    _ => Value?.ToString() ?? "unknown"
                };
            }

            public (Diagnostics, NptType) ConvertTo(Types targetType)
            {
                var strValue = Value as string;
                if (strValue == null)
                    return (Diagnostics.UnknowException, null);

                try{
                    return targetType switch
                    {
                        Types.Nil => strValue == "nil" ? (Diagnostics.Success, new NptType(Types.Nil, null)) : (Diagnostics.CannotConvertType, null),
                        Types.Bool => bool.TryParse(strValue, out var boolVal) ? (Diagnostics.Success, new NptType(Types.Bool, boolVal)) : (Diagnostics.CannotConvertType, null),
                        Types.Int => int.TryParse(strValue, out var intVal) ? (Diagnostics.Success, new NptType(Types.Int, intVal)) : (Diagnostics.CannotConvertType, null),
                        Types.Float => float.TryParse(strValue, out var floatVal) ? (Diagnostics.Success, new NptType(Types.Float, floatVal)) : (Diagnostics.CannotConvertType, null),
                        Types.Char => strValue.Length == 1 ? (Diagnostics.Success, new NptType(Types.Char, strValue[0])) : (Diagnostics.CannotConvertType, null),
                        Types.Str => (Diagnostics.Success, this),
                        _ => (Diagnostics.UnknowException, null)
                    };
                }
                catch{
                    return (Diagnostics.UnknowException, null);
                }
            }
        }
    }
}
