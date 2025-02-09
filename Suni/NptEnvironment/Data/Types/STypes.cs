namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// All the Npt System Supported Types.
/// </summary>
public enum STypes
{
    Nil, Bool, Int, Float,
    Str, Function, Char, List, Dict,
    User,
    Error,
    Identifier,
}

/// <summary>
/// System Type.
/// </summary>
public abstract class SType
{    
    public abstract STypes Type { get; }
    public abstract object Value { get; }

    /// <summary>
    /// Get a string using an SType.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value?.ToString() ?? "nil";
    public virtual bool Contains(string value) => Value?.ToString().Contains(value) ?? false;

    /// <summary>
    /// Convert to the specified SType.
    /// </summary>
    public virtual (Diagnostics, SType) ConvertTo(STypes targetType)
    {
        if (Value is not string strValue)
            return (Diagnostics.UnknowTypeException, null);

        try
        {
            return targetType switch
            {
                STypes.Nil => strValue == "nil" ? (Diagnostics.Success, new NptNil()) : (Diagnostics.CannotConvertType, null),
                STypes.Bool => bool.TryParse(strValue, out var boolVal) ? (Diagnostics.Success, new NptBool(boolVal)) : (Diagnostics.CannotConvertType, null),
                STypes.Int => int.TryParse(strValue, out var intVal) ? (Diagnostics.Success, new NptInt(intVal)) : (Diagnostics.CannotConvertType, null),
                STypes.Float => float.TryParse(strValue, out var floatVal) ? (Diagnostics.Success, new NptFloat(floatVal)) : (Diagnostics.CannotConvertType, null),
                STypes.Char => strValue.Length == 1 ? (Diagnostics.Success, new NptChar(strValue[0])) : (Diagnostics.CannotConvertType, null),
                STypes.Str => (Diagnostics.Success, this),
                STypes.User => (Diagnostics.CannotConvertType, null),
                _ => (Diagnostics.UnknowTypeException, null)
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
                STypes.Nil => new NptNil(),
                STypes.Bool => new NptBool((bool)value),
                STypes.Int => new NptInt((int)value),
                STypes.Float => new NptFloat((double)value),
                STypes.Str => new NptStr((string)value),
                STypes.Char => new NptChar((char)value),
                STypes.Function => value is NptFunction fn ? fn : throw new ArgumentException("Invalid Function Value"),
                STypes.List => new NptList(value as List<SType> ?? throw new ArgumentException("Invalid List Value")),
                STypes.Dict => new NptDict(value as Dictionary<SType, SType> ?? throw new ArgumentException("Invalid Dictionary Value")),
                STypes.User => new NptUser(value as DiscordUser ?? throw new ArgumentException("Invalid User Value")),
                _ => new NptError(Diagnostics.UnknowTypeException, null),
            };
        }
        catch (Exception){
            return new NptError(Diagnostics.CannotConvertType, $"Cannot implicitly convert '{value}' to '{type}'. An explicit conversion exists.");
        }
    }
    
    
    [ExposedProperty("len")]
    public virtual NptInt Lenght() => new NptInt(Value?.ToString().Length ?? -1);

    [ExposedProperty("toStr")]
    public virtual NptStr ToNptStr() => new NptStr(Value?.ToString() ?? "nil");

    [ExposedProperty("typeof")]
    public NptStr TypeOf() => new NptStr(Type.ToString());
}
