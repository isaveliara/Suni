using System.Collections.Generic;
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
}