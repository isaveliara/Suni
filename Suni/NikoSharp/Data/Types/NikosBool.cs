namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Boolean in NikoSharp environment.
/// </summary>
public class NikosBool : SType
{
    private readonly bool? _value;
    public NikosBool(bool? value = null) => _value = value;
    public override STypes Type => STypes.Bool;
    public override object Value => _value;
    public override string ToString() => _value.HasValue ? (_value.Value ? "true" : "false") : "nil";

    public NikosBool CompareTo(NikosBool other, bool And)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NikosBool(null);
        return new NikosBool(And ? (_value.Value && other._value.Value) : (_value.Value || other._value.Value));
    }
}
