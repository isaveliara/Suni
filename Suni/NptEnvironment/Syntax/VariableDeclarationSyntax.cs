using Suni.Suni.NptEnvironment.Core.Evaluator;
using Suni.Suni.NptEnvironment.Data;
using Suni.Suni.NptEnvironment.Data.Types;

namespace Suni.Suni.NptEnvironment.Syntax;
public class VariableDeclarationSyntax : StatementSyntax
{
    //syntax: stype variable value
    //Int pi 3.14
    //Str helloWorld s'Hello World'
    
    public STypes Type { get; }
    public string Name { get; }
    public SType Value { get; }

    private VariableDeclarationSyntax(string originalLine, STypes type, string name, SType value)
        : base(originalLine)
    {
        Type = type;
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Attempts to interpret and create a variable declaration from one line.
    /// </summary>
    public static (Diagnostics, VariableDeclarationSyntax) TryParse(string variableName, STypes typedValue, SType variableValue, EnvironmentDataContext context)
    {
        context.Debugs.Add($"Interpretando variável: Tipo='{typedValue}', Nome='{variableName}', Valor='{variableValue}'");

        var variable = SType.Create(typedValue, variableValue.Value);
        if (variable is NptError error)
        {
            context.LogDiagnostic(error.Diagnostic, error.Message);
            return (error.Diagnostic, null);
        }
        
        context.Variables.Add(new Dictionary<string, SType> { { variableName, variable } });

        return (Diagnostics.Success, null);
    }
}
