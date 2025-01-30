using System.Collections.Generic;
namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a List in NPT environment.
/// </summary>
public class NptList : SType
{
    private readonly List<SType> _value;
    public NptList(List<SType> value) => _value = value;
    public override STypes Type => STypes.List;
    public override object Value => _value;
    
    public (Diagnostics, SType) GetAt(int index)
    {
        if (index < 0 || index >= _value.Count)
            return (Diagnostics.OutOfRangeException, null);
        return (Diagnostics.Success, _value[index]);
    }
    public void Add(SType item) => _value.Add(item);


    [ExposedProperty("count")]
    public NptInt Count() => new NptInt(_value.Count);
}