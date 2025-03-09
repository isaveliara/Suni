namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Boolean in NPT environment.
/// </summary>
public class NptBool : SType
{
    private readonly bool? _value;
    public NptBool(bool? value = null) => _value = value;
    public override STypes Type => STypes.Bool;
    public override object Value => _value;
    public override string ToString() => _value.HasValue ? (_value.Value ? "true" : "false") : "nil";

    public NptBool CompareTo(NptBool other, bool And)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NptBool(null);
        return new NptBool(And ? (_value.Value && other._value.Value) : (_value.Value || other._value.Value));
    }
}
