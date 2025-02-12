using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Formalizer;
namespace Suni.Suni.NptEnvironment.Core;

/// <summary>
/// Class for parser and execution
/// </summary>
public partial class NptSystem
{
    private Stack<CodeBlock> blockStack = new();
    public EnvironmentDataContext ContextData { get; set; }
    public CommandContext DiscordCtx { get; set; }
    public NptSystem(string script, CommandContext discordCtx, EnvironmentDataContext contextData = null)
    {
        if (contextData is not null){
            ContextData = contextData;
            ContextData.Lines = FormalizingScript.SplitCode(script).lines;
        }
        else
            ContextData = new FormalizingScript(script, discordCtx).GetFormalized;
        
        DiscordCtx = discordCtx;
    }
}