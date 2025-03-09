namespace Suni.Suni.NikoSharp.Data.Types;

/// <summary>
/// Represents an Error in the NikoSharp environment.
/// </summary>
public class NikosError : SType
{
    public Diagnostics Diagnostic { get; }
    public string Message { get; }
    public string ReferenceCode { get; }

    public NikosError(Diagnostics diagnostic, string message, string referenceCode = null)
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