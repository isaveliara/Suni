namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Float in NPT environment.
/// </summary>
public class NptFloat : SType
{
    private readonly double? _value;
    public NptFloat(double? value = null) => _value = value;
    public override STypes Type => STypes.Float;
    public override object Value => _value;
    public override string ToString() => _value?.ToString() ?? "nil";
    public NptFloat Add(NptFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NptFloat(null);
        return new NptFloat(_value.Value + other._value.Value);
    }
    public NptFloat Subtract(NptFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NptFloat(null);
        return new NptFloat(_value.Value - other._value.Value);
    }
    public NptFloat Multiply(NptFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NptFloat(null);
        return new NptFloat(_value.Value * other._value.Value);
    }

    public (Diagnostics diagnostic, NptFloat resultVal) Divide(NptFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return (Diagnostics.CannotConvertType, new NptFloat(null));
        if (Math.Abs(other._value.Value) < double.Epsilon)
            return (Diagnostics.DivisionByZeroException, null);
        return (Diagnostics.Success, new NptFloat(_value / other._value));
    }
}
