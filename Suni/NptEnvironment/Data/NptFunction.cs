using System.Collections.Generic;

namespace Suni.Suni.NptEnvironment.Data;

public class NptFunction
{
    public string Name { get; }
    public List<string> Parameters { get; }
    public string Code { get; }

    public NptFunction(string name, List<string> parameters, string code)
    {
        Name = name;
        Parameters = parameters;
        Code = code;
    }

    public override string ToString()
        => $"[func {Name}<{string.Join(", ", Parameters)}>] has \"{Code}\"";
}
