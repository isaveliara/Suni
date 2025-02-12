using System.Collections.Generic;
namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Function in NPT environment.
/// </summary>
public class NptFunction : SType
{
    private readonly Function _value;

    public NptFunction(NptGroup parametersTypes, string name, NptGroup parameters, SType pointer, string code)
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
    public Function(NptGroup parametersTypes, string name, NptGroup parameters, SType pointer, string code)
    {
        ParametersTypes = parametersTypes;
        Name = name;
        Parameters = parameters;
        Pointer = pointer;
        Code = code;
    }
    public NptGroup ParametersTypes;
    public string Name;
    public NptGroup Parameters;
    public SType Pointer;
    public string Code;
}