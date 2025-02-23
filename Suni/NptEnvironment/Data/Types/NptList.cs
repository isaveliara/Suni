namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a List in NPT environment.
/// </summary>
public class NptList : SType
{
    private readonly List<SType> _value;
    public NptList(List<SType> value = null) => _value = value;
    public override STypes Type => STypes.List;
    public override object Value => _value;
    public override string ToString()
    {
        if (_value.Count == 0) return "List{}";
        var listString = string.Join(", ",_value.Select(v => $"{v.ToNptStr().Value}"));

        return $"{{{listString}}}";
    }
    public override bool Contains(string value) => _value.Any(x => x.ToString() == value);
    public int Count() => _value.Count;

    public (Diagnostics, SType) GetAt(int index)
    {
        if (index < 0 || index >= _value.Count)
            return (Diagnostics.OutOfRangeException, null);
        return (Diagnostics.Success, _value[index]);
    }
    public void Add(SType item) => _value.Add(item);


    [ExposedProperty("toStr")]
    public override NptStr ToNptStr()
    {
        if (_value.Count == 0) return new("{}");
        var listString = string.Join(", ",_value.Select(v => $"{v.ToNptStr().Value}"));

        return new($"{{{listString}}}");
    }

    [ExposedProperty("len")]
    public override NptInt Lenght() => new NptInt(_value.Count);

    [ExposedProperty("first")]
    public SType FirstValue() => _value.First();
    
    [ExposedProperty("last")]
    public SType LastValue() => _value.Last();

    [ExposedProperty("randomOf")]
    public SType RandomValueOf() => _value[Random.Shared.Next(_value.Count)];
    [ExposedProperty("typeof")]
    public override NptStr TypeOf()
    {
        var elementTypes = _value.Select(v => v.Type.ToString());
        var typesString = string.Join(", ", elementTypes);

        return new NptStr($"List<{typesString}>");
    }
}