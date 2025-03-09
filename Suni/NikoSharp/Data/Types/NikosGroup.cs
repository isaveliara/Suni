
namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Group of Values in NikoSharp environment.
/// </summary>
public class NikosGroup : SType
{
    private readonly List<SType> _value;
    public NikosGroup(List<SType> value = null) => _value = value;
    public override STypes Type => STypes.Group;
    public override object Value => _value;
    public override string ToString() => string.Join(", ", _value);
    public override bool Contains(string value) => _value.Any(x => x.ToString() == value);
    public int Count() => _value.Count;
    public void Add(SType item) => _value.Add(item);
    public (Diagnostics, SType) GetAt(int index)
    {
        if (index < 1 || index >= _value.Count)
            return (Diagnostics.OutOfRangeException, null);
        return (Diagnostics.Success, _value[index]);
    }

    internal void AddRange(NikosGroup item) => _value.AddRange(item);

    /// <summary>
    /// Compares whether an NikosGroup has the same types as the current.
    /// </summary>
    /// <returns></returns>
    public bool ValidateTypes(NikosGroup item)
    {
        if (_value.Count > item._value.Count) //allows extra args. Maybe remove this
            return false;

        for (int i = 0; i < _value.Count; i++)
            if (_value[i].TypeOf() != item._value[i].TypeOf())
                return false;
        
        return true;
    }


    [ExposedProperty("toStr")]
    public override NikosStr ToNikosStr() => new(string.Join(", ", _value));
    [ExposedProperty("len")]
    public override NikosInt Lenght() => new NikosInt(_value.Count);

    [ExposedProperty("first")]
    public SType FirstValue() => _value.First();
    [ExposedProperty("last")]
    public SType LastValue() => _value.Last();
}