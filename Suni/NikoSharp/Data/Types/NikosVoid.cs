namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Void type in NikoSharp environment.
/// </summary>
public class NikosVoid : SType
{
    public override STypes Type => STypes.Void;
    public override object Value => null;

    public override string ToString() => "void";

    [ExposedProperty("toStr")]
    public override NikosStr ToNikosStr() => new("void");
}
