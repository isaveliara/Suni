using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Core;

public class ParseException : Exception
{
    public Diagnostics Diagnostic { get; }
    public ParseException(Diagnostics diagnostic, string message) : base(message)
    {
        Diagnostic = diagnostic;
    }
}
