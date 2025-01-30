namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Float in NPT environment.
/// </summary>
public class NptFloat : SType
{
    private readonly double _value;
    public NptFloat(double value) => _value = value;
    public override STypes Type => STypes.Float;
    public override object Value => _value;
    public override string ToString() => _value.ToString();
    public NptFloat Add(NptFloat other) => new NptFloat(_value + other._value);
    public NptFloat Subtract(NptFloat other) => new NptFloat(_value - other._value);
    public NptFloat Multiply(NptFloat other) => new NptFloat(_value * other._value);
    public (Diagnostics, NptFloat) Divide(NptFloat other)
    {
        if (Math.Abs(other._value) < double.Epsilon)
            return (Diagnostics.DivisionByZeroException, null);
        return (Diagnostics.Success, new NptFloat(_value / other._value));
    }
}
