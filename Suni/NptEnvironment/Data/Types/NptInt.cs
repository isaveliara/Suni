namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Intiger in NPT environment.
/// </summary>
public class NptInt : SType
{
    private readonly int _value;
    public NptInt(int value) => _value = value;
    public override STypes Type => STypes.Int;
    public override object Value => _value;
    public override string ToString() => _value.ToString();
    public NptInt Add(NptInt other) => new NptInt(_value + other._value);
    public NptInt Subtract(NptInt other) => new NptInt(_value - other._value);
    public NptInt Multiply(NptInt other) => new NptInt(_value * other._value);
    public (Diagnostics, NptInt) Divide(NptInt other)
    {
        if (other._value == 0)
            return (Diagnostics.DivisionByZeroException, null);
        return (Diagnostics.Success, new NptInt(_value / other._value));
    }
}
