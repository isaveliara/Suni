namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Void type in NPT environment.
/// </summary>
public class NptVoid : SType
{
    public override STypes Type => STypes.Void;
    public override object Value => null;

    public override string ToString() => "void";

    [ExposedProperty("toStr")]
    public override NptStr ToNptStr() => new("void");
}
