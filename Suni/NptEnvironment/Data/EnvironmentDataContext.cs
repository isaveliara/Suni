using System.Collections.Generic;
using Suni.Suni.NptEnvironment.Data.Types;

namespace Suni.Suni.NptEnvironment.Data;

public class EnvironmentDataContext
{
    public List<string> Lines { get; set; }    
    public Dictionary<string, List<string>> Includes { get; set; }
    public List<Dictionary<string, SType>> Variables { get; set; }

    //auto-set
    public List<string> ErrorMessages { get; set; }
    public List<string> Debugs { get; set; }
    public List<string> Outputs { get; set; }
    public string ActualLine { get; set; }
    public string Code { get; internal set; }

    public EnvironmentDataContext(List<string> lines, Dictionary<string, List<string>> includes, List<Dictionary<string, SType>> variables)
    {
        Lines = lines;
        Includes = includes;
        Variables = variables;

        Debugs = new List<string>();
        Outputs = new List<string>();
        ErrorMessages = new List<string>();
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
        
        else
        {
            e = $"Error: An error was found; '{diagnosticType}' : {diagnosticMessage}";
            ErrorMessages.Add(e);
        }
        Outputs.Add(e);
    }
}