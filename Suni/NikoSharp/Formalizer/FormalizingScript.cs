using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Formalizer;

public partial class FormalizingScript
{
    public EnvironmentDataContext FormalizingDataContext { get; }
    internal CommandContext DiscordCtx { get; }
    public EnvironmentDataContext GetFormalized => FormalizingDataContext;

    public FormalizingScript(string code, CommandContext discordCtx)
    {
        var codeLines = SplitCode(code);
        DiscordCtx = discordCtx;
        FormalizingDataContext = new EnvironmentDataContext(codeLines, null);

        //uses the FormalizingDataContext
        SetPlaceHolders(); //for lines
    }
}