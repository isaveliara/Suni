using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Formalizer;
namespace Suni.Suni.NikoSharp.Core;

/// <summary>
/// Class for parser and execution
/// </summary>
public partial class NikoSharpSystem
{
    public EnvironmentDataContext ContextData { get; set; }
    public CommandContext DiscordCtx { get; set; }
    public NikoSharpSystem(string script, CommandContext discordCtx, EnvironmentDataContext contextData = null)
    {
        if (contextData is not null){
            ContextData = contextData;
            ContextData.Lines = FormalizingScript.SplitCode(script);
        }
        else
            ContextData = new FormalizingScript(script, discordCtx).GetFormalized;
        
        DiscordCtx = discordCtx;
    }
}