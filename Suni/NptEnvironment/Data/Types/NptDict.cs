namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Dictionary in NPT environment.
/// </summary>
public class NptDict : SType
{
    private readonly Dictionary<SType, SType> _value;
    public NptDict(Dictionary<SType, SType> value) => _value = value;
    public override STypes Type => STypes.Dict;
    public override object Value => _value;
    public SType GetValue(SType key) => _value.ContainsKey(key)
        ? _value[key]
        : new NptNil();
    

    [ExposedProperty("toStr")]
    public override NptStr ToNptStr()
    {
        if (_value.Count == 0) return new NptStr("{}");
        
        var dictString = string.Join(", ", 
            _value.Select(kvp => $"{kvp.Key.ToNptStr().Value}: {kvp.Value.ToNptStr().Value}")
        );

        return new NptStr($"{{{dictString}}}");
    }

    [ExposedProperty("count")]
    public NptInt Count() => new NptInt(_value.Count);
}