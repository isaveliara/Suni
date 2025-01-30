using System.Collections.Generic;
namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// All the Npt System Supported Types.
/// </summary>
public enum STypes
{
    Nil, Bool, Int, Float,
    Str, Function, Char, List, Dict
}

/// <summary>
/// System Type.
/// </summary>
public abstract class SType
{    
    public abstract STypes Type { get; }
    public abstract object Value { get; }
    public override string ToString() => Value?.ToString() ?? "nil";
    /// <summary>
    /// Convert to the specified SType. **Not implemented** yet.
    /// </summary>
    public virtual (Diagnostics, SType) ConvertTo(STypes targetType)
    {
        return (Diagnostics.Success, this);
    }

    public static SType Create(STypes type, object value)
    {
        return type switch
        {
            STypes.Nil => new NptNil(),
            STypes.Bool => new NptBool((bool)value),
            STypes.Int => new NptInt((int)value),
            STypes.Float => new NptFloat((float)value),
            STypes.Str => new NptStr((string)value),
            STypes.Char => new NptChar((char)value),
            STypes.Function => value is NptFunction fn ? fn : throw new ArgumentException("Invalid Function Value"),
            STypes.List => new NptList(value as List<SType> ?? throw new ArgumentException("Invalid List Value")),
            STypes.Dict => new NptDict(value as Dictionary<SType, SType> ?? throw new ArgumentException("Invalid Dictionary Value")),
            _ => throw new ArgumentException($"Unsupported type: {type}"),
        };
    }
}
