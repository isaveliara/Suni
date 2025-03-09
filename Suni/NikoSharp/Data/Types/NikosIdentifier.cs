namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Identifier Value in NikoSharp environment.
/// </summary>
///
public class NikosIdentifier : SType
{
    private readonly string _value;
    /// <summary>
    /// Sets the '_value' field. Getting null, means that the value can't be set.
    /// </summary>
    /// <param name="value"></param>
    public NikosIdentifier(string value = null)
    {
        if (string.IsNullOrEmpty(value))
            _value = null;//nomenclature error

        else if (!char.IsLetter(value[0]) && value[0] != '_')
            _value = null; //nomenclature error
        
        else
            _value = value;
    }

    public override STypes Type => STypes.Identifier;
    public override object Value => _value;
}
