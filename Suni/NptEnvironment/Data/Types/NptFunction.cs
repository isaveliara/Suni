using System.Collections.Generic;
namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents a Function in NPT environment.
/// </summary>
public class NptFunction : SType
{
    public override STypes Type => STypes.Function;
    public override object Value => this;
    public string Name { get; }
    public List<string> Parameters { get; }
    public string Code { get; }

    public NptFunction(string name, List<string> parameters, string code)
    {
        Name = name;
        Parameters = parameters;
        Code = code;
    }

    public override string ToString() => $"[func {Name}<{string.Join(", ", Parameters)}>] has \"{Code}\"";
}
