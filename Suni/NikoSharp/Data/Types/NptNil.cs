namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Nil Value in NPT environment.
/// </summary>
public class NptNil : SType
{
    public override STypes Type => STypes.Nil;
    public override object Value => null;
    public override string ToString() => "nil";


    [ExposedProperty("toStr")]
    public override NptStr ToNptStr() => new("nil");
}