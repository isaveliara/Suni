namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Dictionary in NikoSharp environment.
/// </summary>
public class NikosDict : SType
{
    private readonly Dictionary<NikosStr, SType> _value;
    public NikosDict(Dictionary<NikosStr, SType> value) => _value = value;
    public override STypes Type => STypes.Dict;
    public override object Value => _value;
    public SType GetValue(NikosStr key) => _value.ContainsKey(key)
        ? _value[key]
        : new NikosNil();
    
    public override string ToString()
    {
        if (_value.Count == 0) return "Dict{}";
        var dictString = string.Join(", ", _value.Select(kvp => $"{kvp.Key.ToNikosStr().Value}: {kvp.Value.ToNikosStr().Value}"));

        return $"{{{dictString}}}";
    }

    [ExposedProperty("toStr")]
    public override NikosStr ToNikosStr()
    {
        if (_value.Count == 0) return new NikosStr("{}");
        var dictString = string.Join(", ",_value.Select(kvp => $"{kvp.Key.ToNikosStr().Value}: {kvp.Value.ToNikosStr().Value}"));

        return new NikosStr($"{{{dictString}}}");
    }

    [ExposedProperty("len")]
    public override NikosInt Lenght() => new NikosInt(_value.Count);
    [ExposedProperty("typeof")]
    public override NikosStr TypeOf()
    {
        var valueTypes = _value.Values.Select(v => v.Type.ToString());
        var typesString = string.Join(", ", valueTypes);

        return new NikosStr($"Dict<{typesString}>");
    }
}