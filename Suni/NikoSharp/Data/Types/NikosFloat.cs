namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Float in NikoSharp environment.
/// </summary>
public class NikosFloat : SType
{
    private readonly double? _value;
    public NikosFloat(double? value = null) => _value = value;
    public override STypes Type => STypes.Float;
    public override object Value => _value;
    public override string ToString() => _value?.ToString() ?? "nil";
    public NikosFloat Add(NikosFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NikosFloat(null);
        return new NikosFloat(_value.Value + other._value.Value);
    }
    public NikosFloat Subtract(NikosFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NikosFloat(null);
        return new NikosFloat(_value.Value - other._value.Value);
    }
    public NikosFloat Multiply(NikosFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return new NikosFloat(null);
        return new NikosFloat(_value.Value * other._value.Value);
    }

    public (Diagnostics diagnostic, NikosFloat resultVal) Divide(NikosFloat other)
    {
        if (!_value.HasValue || !other._value.HasValue)
            return (Diagnostics.CannotConvertType, new NikosFloat(null));
        if (Math.Abs(other._value.Value) < double.Epsilon)
            return (Diagnostics.DivisionByZeroException, null);
        return (Diagnostics.Success, new NikosFloat(_value / other._value));
    }
}
