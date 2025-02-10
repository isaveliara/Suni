
namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Group of Values in NPT environment.
/// </summary>
public class NptGroup : SType
{
    private readonly List<SType> _value;
    public NptGroup(List<SType> value) => _value = value;
    public override STypes Type => STypes.Group;
    public override object Value => _value;
    public override string ToString() => string.Join(", ", _value);
    public override bool Contains(string value) => _value.Any(x => x.ToString() == value);
    public void Add(SType item) => _value.Add(item);

    internal void AddRange(NptGroup item) => _value.AddRange(item);

    /// <summary>
    /// Compares whether an OptGroup has the same types as the current.
    /// </summary>
    /// <returns></returns>
    public bool ValidateTypes(NptGroup item)
    {
        if (_value.Count != item._value.Count)
            return false;

        for (int i = 0; i < _value.Count; i++)
            if (_value[i].TypeOf != item._value[i].TypeOf)
                return false;
        
        return true;
    }


    [ExposedProperty("toStr")]
    public override NptStr ToNptStr() => new(string.Join(", ", _value));
    [ExposedProperty("count")]
    public NptInt Count() => new NptInt(_value.Count);
}