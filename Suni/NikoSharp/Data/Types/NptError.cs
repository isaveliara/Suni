namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents an Error in the NPT environment.
/// </summary>
public class NptError : SType
{
    public Diagnostics Diagnostic { get; }
    public string Message { get; }
    public string ReferenceCode { get; }

    public NptError(Diagnostics diagnostic, string message, string referenceCode = null)
    {
        Diagnostic = diagnostic;
        Message = message;
        ReferenceCode = referenceCode;
    }

    public override STypes Type => STypes.Error;
    public override object Value => null;

    public override string ToString() => $"Detected an Error type '{Diagnostic}'" + (ReferenceCode is not null ? $" - At [{ReferenceCode}]: " : ": ") + $"{Message}";
    public (Diagnostics, string) AsDiagnosticAndMessage() => (Diagnostic, Value.ToString());
}