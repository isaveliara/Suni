using System.Collections.Generic;
using Suni.Suni.NikoSharp.Core;
using Suni.Suni.NikoSharp.Data.Types;

namespace Suni.Suni.NikoSharp.Data;

public class EnvironmentDataContext
{
    public List<string> Lines { get; set; }
    public Stack<CodeBlock> BlockStack { get; set; } = new();
    public List<Dictionary<string, SType>> Variables { get; set; }

    //auto-set
    public List<string> ErrorMessages { get; set; }
    public List<string> Debugs { get; set; }
    public List<string> Outputs { get; set; }

    public EnvironmentDataContext(List<string> lines, List<Dictionary<string, SType>> variables)
    {
        Lines = lines;
        Variables = variables is not null? variables : new List<Dictionary<string, SType>> {
            new() { { "__version__", new NikosStr(SunClassBot.SuniV) } },
            new() { { "__time__", new NikosStr(DateTime.Now.ToString()) } },
            new() { { "__out__", new NikosFunction(new NikosGroup([new NikosVoid()]), "__out__", new NikosGroup([new NikosVoid()]), new NikosNil(), "std::out() -> s'hello there!'")} }
        };

        Debugs = new List<string>();
        Outputs = new List<string>();
        ErrorMessages = new List<string>();

        BlockStack.Push(new CodeBlock { CanExecute = true });
    }

    /// <summary>
    /// Logs a Diagnostic.
    /// If it's a Diagnostics.Success, nothing will be logged.
    /// If it's a Diagnostics.Anomaly, will be logged in output.
    /// Else, will be logged as an error.
    /// </summary>
    /// <param name="diagnosticType"></param>
    /// <param name="diagnosticMessage"></param>
    public void LogDiagnostic(Diagnostics diagnosticType, string diagnosticMessage)
    {
        string e;
        if (diagnosticType == Diagnostics.Success)
            return; //don't need to log this
        
        if (diagnosticType == Diagnostics.Anomaly)
            e = $"Warn: An Anomaly was found; {diagnosticMessage}";
        
        else{
            e = $"Error: An error was found; '{diagnosticType}' : {diagnosticMessage}";
            ErrorMessages.Add(e);
        }
        Outputs.Add(e);
    }

    public bool TryGetVariableValue(string identifier, out SType value)
    {
        foreach (var block in BlockStack.Reverse())
            if (block.LocalVariables.TryGetValue(identifier, out value))
                return true;

        value = null;
        return false;
    }
}