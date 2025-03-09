//this class is directly in the ScriptParser, to interact with it.

//usages:
//std::outset() -> Hello World!
//std::out() -> Adding a Hello World!
//std::cls() -> null

using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;

namespace Suni.Suni.NikoSharp.Core;

public partial class NptSystem
{
    public static readonly List<string> MainControlerLibMethods = new List<string> { "out", "outset", "cls", "list_var", "list_libs" };

    /// <summary>
    /// Objects that interact with the main class itself (the parser)
    /// </summary>
    public Diagnostics STDController(string method, NptGroup args)
    {
        switch (method)
        {
            case "out": //std::out() -> hello world
                ContextData.Outputs.Add(args.Pointer().ToString());
                break;
            case "outset": //std::outset() -> hello world
                ContextData.Outputs = new List<string>{args.Pointer().ToString()};
                break;
            case "cls"://std::cls() -> nil
                ContextData.Outputs = new List<string>();
                break;
            case "list_var"://std::list_var() -> nil
                ContextData.Outputs.Add($">> Variables: {string.Join(", ", ContextData.Variables.Select(v => $"{v.Keys.First()}: {v.Values.First()}"))}");
                break;
            case "list_libs"://std::list_libs() -> nil
                ContextData.Outputs.Add($">> Includes: {string.Join("\n   ", ContextData.Includes.Keys)}");
                break;
            default:
                return Diagnostics.NotFoundIncludedObjectException;
        }
        return Diagnostics.Success;
    }
}
