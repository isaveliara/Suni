using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Formalizer;

public partial class FormalizingScript
{
    public EnvironmentDataContext FormalizingDataContext { get; }
    internal CommandContext DiscordCtx { get; }
    public List<string> DefLines { get; }
    public EnvironmentDataContext GetFormalized => FormalizingDataContext;

    public FormalizingScript(string code, CommandContext discordCtx)
    {
        var codeLines = SplitCode(code);
        DefLines = codeLines.defLines;
        DiscordCtx = discordCtx;
        FormalizingDataContext = new EnvironmentDataContext(codeLines.lines, null, null);

        //uses the FormalizingDataContext
        SetPlaceHolders(); //for lines
        InterpretDefinitionsBlock(); //for values
    }
}