namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Boolean in NPT environment.
/// </summary>
public class NptBool : SType
{
    private readonly bool _value;
    public NptBool(bool value) => _value = value;
    public override STypes Type => STypes.Bool;
    public override object Value => _value;
    public override string ToString() => _value ? "true" : "false";

    public NptBool CompareTo(NptBool other, bool And) => new NptBool(And ? (_value && other._value) : (_value || other._value));

}
