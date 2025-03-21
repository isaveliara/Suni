namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// All the NikoSharp System Supported Types.
/// </summary>
public enum STypes
{
    Nil, Void, Bool, Int, Float,
    Str, List, Group, Dict,
    Error, Identifier, Class,
}

/// <summary>
/// System Type.
/// </summary>
public abstract class SType
{    
    public abstract STypes Type { get; }
    public virtual object Value { get; } = null;

    /// <summary>
    /// Gets a string using an SType.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value?.ToString() ?? "nil";
    /// <summary>
    /// Returns true if the value contains the other
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual bool Contains(string other) => Value?.ToString().Contains(other) ?? false;
    /// <summary>
    /// Convert to the specified SType.
    /// </summary>
    public virtual (Diagnostics, SType) ConvertTo(STypes targetType)
    {
        if (Value is not string strValue)
            return (Diagnostics.InvalidTypeException, null);

        try
        {
            return targetType switch
            {
                STypes.Nil => strValue == "nil" ? (Diagnostics.Success, new NikosNil()) : (Diagnostics.CannotConvertType, null),
                STypes.Void => strValue == "void" ? (Diagnostics.Success, new NikosVoid()) : (Diagnostics.CannotConvertType, null),
                STypes.Bool => bool.TryParse(strValue, out var boolVal) ? (Diagnostics.Success, new NikosBool(boolVal)) : (Diagnostics.CannotConvertType, null),
                STypes.Int => long.TryParse(strValue, out var intVal) ? (Diagnostics.Success, new NikosInt(intVal)) : (Diagnostics.CannotConvertType, null),
                STypes.Float => float.TryParse(strValue, out var floatVal) ? (Diagnostics.Success, new NikosFloat(floatVal)) : (Diagnostics.CannotConvertType, null),
                STypes.Str => (Diagnostics.Success, this),
                _ => (Diagnostics.CannotConvertType, null)
            };
        }
        catch{
            return (Diagnostics.UnknowException, null);
        }
    }

    public static SType Create(STypes type, object value)
    {
        try
        {
            return type switch
            {
                STypes.Nil => new NikosNil(),
                STypes.Void => new NikosVoid(),
                STypes.Bool => new NikosBool((bool)value),
                STypes.Int => new NikosInt((long)value),
                STypes.Float => new NikosFloat((double)value),
                STypes.Str => new NikosStr((string)value),
                STypes.List => new NikosList(value as List<SType> ?? throw new ArgumentException("Invalid List Value")),
                STypes.Group => new NikosGroup(value as List<SType> ?? throw new ArgumentException("Invalid Group Value")),
                STypes.Dict => new NikosDict(value as Dictionary<NikosStr, SType> ?? throw new ArgumentException("Invalid Dictionary Value")),
                _ => new NikosError(Diagnostics.InvalidTypeException, null),
            };
        }
        catch (Exception){
            return new NikosError(Diagnostics.CannotConvertType, $"Cannot implicitly convert '{value}' to '{type}'. An explicit conversion exists.");
        }
    }
    
    
    [ExposedProperty("len")]
    public virtual NikosInt Lenght() => new NikosInt(Value?.ToString().Length ?? -1);

    [ExposedProperty("toStr")]
    public virtual NikosStr ToNikosStr() => new NikosStr(Value?.ToString() ?? "nil");

    [ExposedProperty("typeof")]
    public virtual NikosStr TypeOf() => new NikosStr($"STypes.{Type.ToString()}");
}
