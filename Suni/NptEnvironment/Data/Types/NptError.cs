namespace Suni.Suni.NptEnvironment.Data.Types;

/// <summary>
/// Represents an Error in the NPT environment.
/// </summary>
public class NptError : SType
{
    public Diagnostics Diagnostic { get; }
    public string Message { get; }

    public NptError(Diagnostics diagnostic, string message)
    {
        Diagnostic = diagnostic;
        Message = message;
    }

    public override STypes Type => STypes.Error;
    public override object Value => null;

    public override string ToString() => $"Error: {Diagnostic} - {Message}";
}