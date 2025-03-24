using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private Diagnostics ParseExitStatement()
    {
        ConsumeToken("exit");
        _context.Debugs.Add("exit command executed!");
        throw new ParseException(Diagnostics.EarlyTermination, "Exit requested.");
    }
}