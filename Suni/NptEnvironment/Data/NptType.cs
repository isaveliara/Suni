/*using System.Collections.Generic;
namespace Suni.Suni.NptEnvironment.Data;

public partial class NptTypes
{
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
                Types.Fn => Value is NptFunction fn ? fn.ToString() : "<anonymous function>",
                Types.Tuple => $"{Value}",
                Types.List => $"{string.Join(", ", (Value as List<object>) ?? new List<object>())}",
                Types.Dict => $"{string.Join(", ", (Value as Dictionary<object, object>)?.Select(kv => $"{kv.Key}: {kv.Value}") ?? new string[0])}",
                _ => Value?.ToString() ?? "unknown"
            };
        }
    }
}
*/