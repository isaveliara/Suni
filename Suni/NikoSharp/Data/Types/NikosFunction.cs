using System.Collections.Generic;
namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents a Function in NikoSharp environment.
/// </summary>
public class NikosFunction : SType
{
    private readonly Function _value;

    public NikosFunction(NikosGroup parametersTypes, string name, NikosGroup parameters, SType pointer, string code)
    {
        _value = new Function(parametersTypes, name, parameters, pointer, code);
    }

    public override STypes Type => STypes.Function;
    public override object Value => _value;
    
    public override string ToString()
    {
        var pointerStr = _value.Pointer?.ToString() ?? "nil";
        var paramTypesStr = _value.ParametersTypes?.ToString() ?? "";
        return $"[func \"{_value.Name}\"({paramTypesStr}) -> {pointerStr}]";
    }
}

public struct Function
{
    public Function(NikosGroup parametersTypes, string name, NikosGroup parameters, SType pointer, string code)
    {
        ParametersTypes = parametersTypes;
        Name = name;
        Parameters = parameters;
        Pointer = pointer;
        Code = code;
    }
    public NikosGroup ParametersTypes;
    public string Name;
    public NikosGroup Parameters;
    public SType Pointer;
    public string Code;
}