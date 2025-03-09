namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a String in NikoSharp environment.
/// </summary>
public class NikosStr : SType
{
    private readonly string _value;
    public NikosStr(string value = null) => _value = value;
    public override STypes Type => STypes.Str;
    public override object Value => _value;
    public NikosStr Add(NikosStr other) => new NikosStr(_value + other._value);
    public bool Contains(NikosStr other) => _value.Contains(other._value);


    [ExposedProperty("upper")]
    public NikosStr ToUpper() => new NikosStr(_value.ToUpper());
    [ExposedProperty("lower")]
    public NikosStr ToLower() => new NikosStr(_value.ToLower());
}
