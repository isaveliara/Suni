using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Core;

/// <summary>
/// Class for parser and execution
/// </summary>
public partial class NikoSharpSystem
{
    public EnvironmentDataContext ContextData { get; set; }
    public NikoSharpSystem(string script, EnvironmentDataContext contextData = null)
    {
        var tokenizedScript = Tokens.Tokenize(script);
        
        Console.WriteLine($"code:\n    {string.Join("\n    ", tokenizedScript)}");
        if (contextData is not null){
            ContextData = contextData;
            ContextData.Tokens = tokenizedScript;
        }
        else
            ContextData = new EnvironmentDataContext(tokenizedScript, null);
    }
}