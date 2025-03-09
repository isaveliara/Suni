namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a String in NPT environment.
/// </summary>
public class NptStr : SType
{
    private readonly string _value;
    public NptStr(string value = null) => _value = value;
    public override STypes Type => STypes.Str;
    public override object Value => _value;
    public NptStr Add(NptStr other) => new NptStr(_value + other._value);
    public bool Contains(NptStr other) => _value.Contains(other._value);


    [ExposedProperty("upper")]
    public NptStr ToUpper() => new NptStr(_value.ToUpper());
    [ExposedProperty("lower")]
    public NptStr ToLower() => new NptStr(_value.ToLower());
}
