namespace Suni.Suni.NptEnvironment.Data.Types;

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

    public override string ToString() => "Detected an Error:" + ReferenceCode is not null? $"At [{ReferenceCode}] " : " " + $"{Diagnostic} - {Message}";
    public (Diagnostics, string) AsDIagnosticAndMessage() => (Diagnostic, Value.ToString());
}