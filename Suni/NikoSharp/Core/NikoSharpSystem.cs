using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Core;

/// <summary>
/// Class for parser and execution
/// </summary>
public class NikoSharpSystem
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

    public async Task<(List<string> debugs, List<string> outputs, Diagnostics result)> ParseScriptAsync()
    {
        var parser = new NikoSharpParser(ContextData.Tokens, ContextData);
        Diagnostics result = Diagnostics.Success;
        while (parser.CurrentToken() != "EOF")
        {
            try
            {
                result = await parser.ParseStatementAsync();
                if (result != Diagnostics.Success)
                    return (ContextData.Debugs, ContextData.Outputs, result);
            }
            catch (ParseException ex)
            {
                ContextData.Outputs.Add($"{ex.Diagnostic}: {ex.Message}");
                return (ContextData.Debugs, ContextData.Outputs, ex.Diagnostic);
            }
        }
        return (ContextData.Debugs, ContextData.Outputs, Diagnostics.Success);
    }
}