namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Intiger in NikoSharp environment.
/// </summary>
public class NikosInt : SType
{
    private readonly long? _value;
    public NikosInt(long? value = null) => _value = value;
    public override STypes Type => STypes.Int;
    public override object Value => _value;
    public override string ToString() => _value.ToString();
    public NikosInt Add(NikosInt other) => new NikosInt(_value + other._value);
    public NikosInt Subtract(NikosInt other) => new NikosInt(_value - other._value);
    public NikosInt Multiply(NikosInt other) => new NikosInt(_value * other._value);
    public (Diagnostics diagnostic, NikosInt resultVal) Divide(NikosInt other)
    {
        if (other._value == 0)
            return (Diagnostics.DivisionByZeroException, null);
        return (Diagnostics.Success, new NikosInt(_value / other._value));
    }
}
