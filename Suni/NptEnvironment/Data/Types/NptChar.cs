namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Character in NPT environment.
/// </summary>
public class NptChar : SType
{
    private readonly char _value;
    public NptChar(char value) => _value = value;
    public override STypes Type => STypes.Char;
    public override object Value => _value;
    public override string ToString() => _value.ToString();
}
