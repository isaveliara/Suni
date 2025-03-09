namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Nil Value in NikoSharp environment.
/// </summary>
public class NikosNil : SType
{
    public override STypes Type => STypes.Nil;
    public override object Value => null;
    public override string ToString() => "nil";


    [ExposedProperty("toStr")]
    public override NikosStr ToNikosStr() => new("nil");
}