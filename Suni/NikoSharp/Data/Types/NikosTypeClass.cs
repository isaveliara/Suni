namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a TypeClass in NikoSharp environment.
/// </summary>
public class NikosTypeClass : SType
{
    private readonly NikosClass _value;
    
    public NikosTypeClass(string className)
    {
        _value = new NikosClass(className);
    }

    public override STypes Type => STypes.TypeClass;
    public override object Value => _value;
    
    public override string ToString()
    {
        return $"<TypeClass: {_value.NameClass} [{string.Join(", ", _value.Methods?.ConvertAll(m => m.NameMethod) ?? new List<string>())}]>";
    }
}

public class NikosClass
{
    public string NameClass { get; private set; }
    public List<NikosMethod> Methods { get; private set; }

    public NikosClass(string name)
    {
        NameClass = name;
        Methods = new List<NikosMethod>();
    }

    public void RegisterMethod(NikosMethod method)
    {
        Methods.Add(method);
    }

    public NikosMethod GetMethod(string name)
    {
        return Methods.Find(m => m.NameMethod == name);
    }
}

public class NikosMethod
{
    public string NameMethod { get; set; }
    
    public List<string> Code { get; set; }
    
    public NikosDict ArgsValues { get; set; } = new NikosDict(new Dictionary<NikosStr, SType>());
    public STypes ReturnType { get; set; } = STypes.Void;
    public SType ReturnResult { get; set; } = new NikosVoid();

    public override string ToString()
    {
        return $"<Method: {NameMethod} (Returns: {ReturnType})>";
    }
}
