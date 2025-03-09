namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Dictionary in NPT environment.
/// </summary>
public class NptDict : SType
{
    private readonly Dictionary<NptStr, SType> _value;
    public NptDict(Dictionary<NptStr, SType> value) => _value = value;
    public override STypes Type => STypes.Dict;
    public override object Value => _value;
    public SType GetValue(NptStr key) => _value.ContainsKey(key)
        ? _value[key]
        : new NptNil();
    
    public override string ToString()
    {
        if (_value.Count == 0) return "Dict{}";
        var dictString = string.Join(", ", _value.Select(kvp => $"{kvp.Key.ToNptStr().Value}: {kvp.Value.ToNptStr().Value}"));

        return $"{{{dictString}}}";
    }

    [ExposedProperty("toStr")]
    public override NptStr ToNptStr()
    {
        if (_value.Count == 0) return new NptStr("{}");
        var dictString = string.Join(", ",_value.Select(kvp => $"{kvp.Key.ToNptStr().Value}: {kvp.Value.ToNptStr().Value}"));

        return new NptStr($"{{{dictString}}}");
    }

    [ExposedProperty("len")]
    public override NptInt Lenght() => new NptInt(_value.Count);
    [ExposedProperty("typeof")]
    public override NptStr TypeOf()
    {
        var valueTypes = _value.Values.Select(v => v.Type.ToString());
        var typesString = string.Join(", ", valueTypes);

        return new NptStr($"Dict<{typesString}>");
    }
}