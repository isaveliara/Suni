namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a String in NPT environment.
/// </summary>
public class NptStr : SType
{
    private readonly string _value;
    public NptStr(string value) => _value = value;
    public override STypes Type => STypes.Str;
    public override object Value => _value;
    public NptStr ToUpper() => new NptStr(_value.ToUpper());
    public NptStr ToLower() => new NptStr(_value.ToLower());
    public override (Diagnostics, SType) ConvertTo(STypes targetType)
    {
        return targetType switch
        {
            STypes.Int => int.TryParse(_value, out var intVal)
                ? (Diagnostics.Success, new NptInt(intVal))
                : (Diagnostics.CannotConvertType, null),
            STypes.Float => float.TryParse(_value, out var floatVal)
                ? (Diagnostics.Success, new NptFloat(floatVal))
                : (Diagnostics.CannotConvertType, null),
            STypes.Bool => bool.TryParse(_value, out var boolVal)
                ? (Diagnostics.Success, new NptBool(boolVal))
                : (Diagnostics.CannotConvertType, null),
            STypes.Str => (Diagnostics.Success, this),
            _ => (Diagnostics.UnknowTypeException, null)
        };
    }
}
