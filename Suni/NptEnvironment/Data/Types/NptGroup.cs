
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
    public override string ToString() => string.Join(", ", Value);
    public override bool Contains(string value) => _value.Any(x => x.ToString() == value);
    public void Add(SType item) => _value.Add(item);

    internal void AddRange(NptGroup item) => _value.AddRange(item);

    
    [ExposedProperty("toStr")]
    public override NptStr ToNptStr() => new(string.Join(", ", _value));
}